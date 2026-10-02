using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Wildfeast
{
    // Spatial actions belong to the world. Rewards and persistent crops stay in the model.
    public class IslandLife : MonoBehaviour
    {
        GameController game;
        readonly Dictionary<FieldPlot,SpriteRenderer[]> plots=new Dictionary<FieldPlot,SpriteRenderer[]>();
        readonly List<HarvestNode> nodes=new List<HarvestNode>();
        Coroutine pulling;Transform pullArt;Vector3 pullOrigin;
        public bool Sailing {get;private set;}
        public void Init(GameController controller)
        {
            game=controller;
            foreach(var root in new[]{game.world.saltleaf,game.world.mistwake})
            {
                foreach(var sr in root.GetComponentsInChildren<SpriteRenderer>(true))
                {
                    string id=sr.gameObject.name;
                    if(id.StartsWith("tree")||id=="noodlegrass"||id=="saltstone")
                    {
                        var node=sr.gameObject.AddComponent<HarvestNode>();node.art=sr;node.original=sr.sprite;node.item=id.StartsWith("tree")?"wood":id=="saltstone"?"stone":"fiber";
                        node.required=node.item=="wood"?7:node.item=="stone"?9:8;node.hits=node.item=="fiber"?1:3;
                        node.source="resource-"+(root==game.world.saltleaf?0:1)+"-"+Mathf.RoundToInt(sr.transform.localPosition.x*32)+"-"+Mathf.RoundToInt(sr.transform.localPosition.y*32);
                        node.colliders=sr.GetComponentsInChildren<Collider2D>(true);nodes.Add(node);
                    }
                }
            }
            game.Model.Changed+=Refresh;Refresh();
        }
        void Refresh()
        {
            foreach(var f in game.Model.State.fields)
            {
                if(!plots.TryGetValue(f,out var views))
                {
                    Transform root=f.island==0?game.world.saltleaf:game.world.mistwake;
                    var soil=WorldView.Add(root,"soil",new Vector2(f.x,f.y),-500);
                    var crop=WorldView.Add(soil.transform,"planted-seed",new Vector2(0,.2f),1000-f.y*32);
                    views=new[]{soil,crop};plots.Add(f,views);
                }
                views[0].sprite=WorldView.Art(f.crop.wateredDay==game.Model.State.day?"soil-wet":"soil");
                views[1].sprite=!f.crop.planted?null:WorldView.Art(f.crop.growth>=game.Model.Data.cropDays?f.crop.item:f.crop.growth>0?"sprout":"planted-seed");
            }
            foreach(var node in nodes)
            {
                bool harvested=game.Model.State.harvested.Contains(node.source);
                node.art.sprite=harvested?(node.item=="wood"?WorldView.Art("stump"):null):node.original;
                foreach(var collider in node.colliders)collider.enabled=!harvested||node.item=="wood";
                if(!harvested&&node.lastDay!=game.Model.State.day){node.damage=0;node.lastDay=game.Model.State.day;}
            }
        }
        public Vector2 Target(Vector2 facing)
        {
            Vector2 hero=game.world.player.position;var mouse=Mouse.current;
            if(mouse!=null&&mouse.leftButton.wasPressedThisFrame)
            {
                Vector2 cursor=game.world.worldCamera.ScreenToWorldPoint(mouse.position.ReadValue());
                if(Vector2.Distance(hero,cursor)<=2)return cursor;
            }
            return hero+facing*.85f;
        }
        public bool Use(int tool,Vector2 facing,Vector2 target)
        {
            if(game.world.Area==2||game.Model.State.phase!="explore")return false;
            Vector2 tile=new Vector2(Mathf.Round(target.x),Mathf.Round(target.y));
            var model=game.Model;var f=ItemInventory.Plot(model.State,game.world.Area,tile);
            if(tool==6)
            {
                game.Sound("cook");
                var blockers=Physics2D.OverlapBoxAll(tile+Vector2.up*.2f,new Vector2(.8f,.5f),0);
                bool solid=blockers.Any(c=>!c.transform.IsChildOf(game.world.player));
                if(!WorldView.Water(tile,game.world.Area)&&!solid&&ItemInventory.Till(model,game.world.Area,tile)){game.world.Burst(tile,"dirt-puff");Notice("Soil tilled · Select seeds to plant");}
                else Notice(f!=null?"This soil is already tilled.":"Choose clear ground away from water and solid objects.");return true;
            }
            if(tool==2)
            {
                game.Sound("splash");
                if(game.world.WaterAt(game.world.player.position,facing).HasValue&&f==null){model.State.water=20;model.Notify();game.world.Burst(target,"splash");Notice("Watering can refilled · 20/20");return true;}
                if(f!=null){if(ItemInventory.Water(model,f))game.world.Burst(tile,"splash");else Notice(model.State.water<1?"Your can is empty. Refill beside water.":"Plant seeds here first, or water tomorrow.");return true;}
                // Legacy authored beds continue to use their own interaction.
                return false;
            }
            if((tool==3||tool==4)&&f!=null)
            {if(ItemInventory.Plant(model,f,tool==3?"pepperbell":"lanternroot"))game.world.Burst(tile,"dirt-puff");else Notice("Use an empty tilled plot and a seed packet.");return true;}
            if(f!=null&&f.crop.growth>=model.Data.cropDays)
            {if(ItemInventory.Harvest(model,f))game.world.Burst(tile,f.crop.item);return true;}
            if(tool==7||tool==8||tool==9)
            {
                var candidates=nodes.Where(n=>n.gameObject.activeInHierarchy&&!model.State.harvested.Contains(n.source)&&n.required==tool&&Vector2.Distance(n.transform.position,target)<(tool==8?1.5f:1.2f)).OrderBy(n=>Vector2.Distance(n.transform.position,target));
                var selected=tool==8?candidates.ToArray():candidates.Take(1).ToArray();
                foreach(var node in selected)
                {
                    game.Sound("cook");
                    node.damage++;StartCoroutine(Shake(node));game.world.Burst(node.transform.position,node.item=="wood"?"wood":node.item=="stone"?"stone":"fiber");
                    if(node.damage>=node.hits){if(ItemInventory.Resource(model,node.item,node.item=="fiber"?1:3,node.source))Notice("Gathered "+ItemInventory.Name(node.item,model.Data));}
                }
                if(selected.Length==0)Notice(tool==7?"Aim at a Cinnamonwood tree.":tool==9?"Aim at a Saltstone outcrop.":"Sweep the Noodlegrass around you.");return true;
            }
            return false;
        }
        void Notice(string text){game.Say(text);}
        void ClearNotice(){game.ui.Message("");}
        IEnumerator Shake(HarvestNode node)
        {
            Vector3 origin=node.transform.localPosition;float t=0;
            while(t<.3f){t+=Time.deltaTime;node.transform.localPosition=origin+Vector3.right*(Mathf.Round(Mathf.Sin(t*50)*2)/32);yield return null;}node.transform.localPosition=origin;
        }
        public void Pull(WorldPoint point,int quantity,Action complete)
        {
            CancelPull();pulling=StartCoroutine(PullRoutine(point,complete));
        }
        IEnumerator PullRoutine(WorldPoint point,Action complete)
        {
            pullArt=point.artwork?point.artwork.transform:null;if(!pullArt){complete();yield break;}
            pullOrigin=pullArt.localPosition;float t=0;
            while(t<.55f){t+=Time.deltaTime;pullArt.localPosition=pullOrigin+new Vector3(Mathf.Round(Mathf.Sin(t*35)*2)/32,Mathf.Round(t*4)/32,0);yield return null;}
            game.world.Burst(point.transform.position,"dirt-puff");
            Vector3 start=pullArt.position,end=game.world.player.position+Vector3.up*.6f;t=0;
            while(t<.35f){t+=Time.deltaTime;float u=Mathf.Clamp01(t/.35f);pullArt.position=Vector3.Lerp(start,end,u)+Vector3.up*Mathf.Sin(u*Mathf.PI)*.8f;yield return null;}
            pullArt.localPosition=pullOrigin;pullArt=null;pulling=null;complete();
        }
        public void CancelPull(){if(pulling!=null)StopCoroutine(pulling);if(pullArt)pullArt.localPosition=pullOrigin;pulling=null;pullArt=null;}
        public void Sail(int destination,Action complete){StartCoroutine(SailRoutine(destination,complete));}
        IEnumerator SailRoutine(int destination,Action complete)
        {
            Sailing=true;var world=game.world;var body=world.player.GetComponent<Rigidbody2D>();body.simulated=false;
            var ships=world.GetComponentsInChildren<SpriteRenderer>(true).Where(sr=>sr.gameObject.name=="boat").ToArray();foreach(var sr in ships)sr.enabled=false;
            var ship=WorldView.Add(world.transform,"boat",new Vector2(-11,-7.2f),1800);ship.gameObject.name="Sailing skiff";
            world.FollowSea=true;world.playerArt.sortingOrder=1802;
            // Temporarily position both authored islands in a continuous sea route.
            // Rebase to the destination's local coordinates only after physically docking.
            Transform arrival=destination==0?world.saltleaf:world.mistwake;
            Vector3 offset=new Vector3(40,-35,0);arrival.localPosition=offset;arrival.gameObject.SetActive(true);
            var oceanTiles=new List<GameObject>();
            for(int x=0;x<2;x++)for(int y=0;y<3;y++)oceanTiles.Add(WorldView.Add(world.transform,"ocean",new Vector2(x*40,-y*26),-2100,true).gameObject);
            yield return MoveShip(ship,new Vector2(-11,-7.2f),new Vector2(-11,-11.5f),2.2f);
            yield return MoveShip(ship,new Vector2(-11,-11.5f),new Vector2(12,-27),2.5f);
            yield return MoveShip(ship,new Vector2(12,-27),new Vector2(29,-46.5f),2.5f);
            yield return MoveShip(ship,new Vector2(29,-46.5f),new Vector2(29,-42.2f),2.2f);
            arrival.localPosition=Vector3.zero;foreach(var tile in oceanTiles)Destroy(tile);
            foreach(var sr in ships)sr.enabled=true;Destroy(ship.gameObject);body.simulated=true;world.FollowSea=false;
            world.SetArea(destination,new Vector2(-10,-6));Sailing=false;complete();
        }
        IEnumerator MoveShip(SpriteRenderer ship,Vector2 start,Vector2 end,float duration)
        {
            float t=0;while(t<duration)
            {
                t+=Time.deltaTime;float u=Mathf.SmoothStep(0,1,t/duration);Vector2 p=Vector2.Lerp(start,end,u);
                ship.transform.position=new Vector2(Mathf.Round(p.x*32)/32,Mathf.Round(p.y*32)/32);ship.flipX=end.x<start.x;
                game.world.player.position=ship.transform.position+Vector3.up*.7f;game.world.playerArt.sortingOrder=1802;
                game.world.Burst(ship.transform.position-Vector3.up*.25f,"ripple-0",.3f);yield return null;
            }
        }
    }
}
