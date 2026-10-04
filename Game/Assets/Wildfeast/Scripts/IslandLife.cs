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
        Coroutine pulling;Transform pullArt;Vector3 pullOrigin;SpriteRenderer pullDrop;
        public bool Sailing {get;private set;}
        public void Init(GameController controller)
        {
            game=controller;
            nodes.AddRange(game.world.GetComponentsInChildren<HarvestNode>(true));
            game.Model.Changed+=Refresh;Refresh();
        }
        public void Rebind(GameModel previous)
        {previous.Changed-=Refresh;foreach(var views in plots.Values)if(views[0])Destroy(views[0].gameObject);plots.Clear();foreach(var grid in game.world.GetComponentsInChildren<TileWorld>(true))grid.ClearSoil();foreach(var node in nodes){node.damage=0;node.lastDay=-1;}game.Model.Changed+=Refresh;Refresh();}
        void Refresh()
        {
            foreach(var f in game.Model.State.fields)
            {
                if(!plots.TryGetValue(f,out var views))
                {
                    Transform root=game.world.IslandRoot(f.island);
                    var soil=WorldView.Add(root,"soil",new Vector2(f.x,f.y),-500,true);
                    var crop=WorldView.Add(soil.transform,"planted-seed",new Vector2(0,.2f),1000-f.y*32);
                    views=new[]{soil,crop};plots.Add(f,views);
                }
                views[0].sprite=null;game.world.IslandRoot(f.island).GetComponentInChildren<TileWorld>(true).Soil(f.x,f.y,f.crop.wateredDay==game.Model.State.day,game.Model.State);
                views[1].sprite=!f.crop.planted?null:WorldView.Art(f.crop.growth>=game.Model.Data.cropDays?f.crop.item:f.crop.growth>0?"sprout":"planted-seed");
            }
            foreach(var node in nodes)
            {
                bool harvested=game.Model.State.harvested.Contains(node.source);
                bool stump=harvested&&node.item=="wood"&&!game.Model.State.harvested.Contains(node.source+"-stump");
                node.art.sprite=stump?WorldView.Art("stump"):harvested?null:node.original;
                foreach(var collider in node.colliders)collider.enabled=!harvested||stump;
                if(node.lastDay!=game.Model.State.day){node.damage=0;node.lastDay=game.Model.State.day;}
            }
        }
        public Vector2 Target(Vector2 facing)
        {
            Vector2 hero=game.world.player.position;var mouse=Mouse.current;
            if(mouse!=null&&mouse.leftButton.wasPressedThisFrame)
            {
                Vector2 cursor=game.world.worldCamera.ScreenToWorldPoint(mouse.position.ReadValue());
                var bodyPoint=game.world.PointAt(cursor);
                if(bodyPoint&&bodyPoint.GetComponent<FoodEcology>())return bodyPoint.transform.position;
                if(Vector2.Distance(hero,cursor)<=2)return cursor;
            }
            return hero+facing*.85f;
        }
        FoodEcology Ecology(Vector2 target)=>game.world.points.Where(p=>p&&p.gameObject.activeInHierarchy&&p.GetComponent<FoodEcology>()&&Vector2.Distance(p.transform.position,target)<1.25f).OrderBy(p=>Vector2.Distance(p.transform.position,target)).FirstOrDefault()?.GetComponent<FoodEcology>();
        HarvestNode[] Targets(int tool,Vector2 target)=>nodes.Where(n=>n.gameObject.activeInHierarchy&&!n.falling&&n.art.sprite&&n.required==tool&&(!game.Model.State.harvested.Contains(n.source)||n.item=="wood"&&!game.Model.State.harvested.Contains(n.source+"-stump"))&&Vector2.Distance(n.transform.position,target)<(tool==8?1.5f:1.2f)).OrderBy(n=>Vector2.Distance(n.transform.position,target)).Take(tool==8?20:1).ToArray();
        public bool CanWork(int tool,Vector2 target)
        {
            if(Districts.Interior(game.world.Area)||game.Model.State.phase!="explore")return false;
            if(Ecology(target)?.CanTool(tool)==true)return true;
            Vector2 tile=TerrainGrid.Cell(target);var m=game.Model;var f=ItemInventory.Plot(m.State,game.world.Area,tile);
            if(tool==6)return f==null&&m.State.fields.Count<256&&Archipelago.Tillable(tile,game.world.Area)&&!Physics2D.OverlapBoxAll(tile+Vector2.up*.2f,new Vector2(.8f,.5f),0).Any(c=>!c.transform.IsChildOf(game.world.player)&&!c.isTrigger);
            if(tool==2)return f!=null?f.crop.planted&&f.crop.wateredDay!=m.State.day&&m.State.water>0:m.State.water<20&&game.world.WaterAt(game.world.player.position,(target-(Vector2)game.world.player.position).normalized).HasValue;
            if((tool==3||tool==4)&&f!=null)return !f.crop.planted&&m.Seeds(ItemInventory.SeedItem(m.State.slots[m.State.equipped].id))>0;
            if(f!=null&&f.crop.planted&&f.crop.growth>=m.Data.cropDays)return m.BagCount+3<=m.Capacity;
            return (tool==7||tool==8||tool==9)&&Targets(tool,target).Length>0;
        }
        public bool Use(int tool,Vector2 facing,Vector2 target)
        {
            if(!CanWork(tool,target))return false;
            var ecology=Ecology(target);if(ecology&&ecology.CanTool(tool)&&ecology.Tool(tool))return true;
            Vector2 tile=TerrainGrid.Cell(target);var model=game.Model;var f=ItemInventory.Plot(model.State,game.world.Area,tile);
            if(tool==6){if(!ItemInventory.Till(model,game.world.Area,tile))return false;game.Sound("cook");game.world.Burst(tile,"dirt-puff");return true;}
            if(tool==2){bool changed=f!=null?ItemInventory.Water(model,f):model.State.water<20;if(changed){if(f==null){model.State.water=20;model.Notify();}game.Sound("splash");game.world.Burst(tile,"splash");}return changed;}
            if((tool==3||tool==4)&&f!=null){bool changed=ItemInventory.Plant(model,f,ItemInventory.SeedItem(model.State.slots[model.State.equipped].id));if(changed)game.world.Burst(tile,"dirt-puff");return changed;}
            if(f!=null&&f.crop.planted&&f.crop.growth>=model.Data.cropDays){string item=f.crop.item;bool changed=ItemInventory.Harvest(model,f);if(changed)game.world.Burst(tile,item);return changed;}
            var selected=Targets(tool,target);
            foreach(var node in selected)
            {
                game.Sound("cook");node.damage++;game.world.Burst(node.transform.position,node.item);
                bool stump=model.State.harvested.Contains(node.source);int needed=stump?2:node.hits;
                if(node.damage>=needed)
                {
                    if(ItemInventory.Resource(model,node.item,stump?1:node.item=="fiber"?1:3,node.source+(stump?"-stump":"")))
                    {node.damage=0;if(node.item=="wood"&&!stump)StartCoroutine(Fall(node));}
                    else Notice("Make room for gathered materials.");
                }
                else StartCoroutine(Shake(node));
            }
            return selected.Length>0;
        }
        IEnumerator Fall(HarvestNode node)
        {
            node.falling=true;var tree=WorldView.Add(node.transform.parent,node.original.name,node.transform.localPosition,node.art.sortingOrder+1);tree.sprite=node.original;float direction=game.world.player.position.x<node.transform.position.x?-1:1;
            for(float t=0;t<.8f;t+=Time.deltaTime){float u=t/.8f;tree.transform.rotation=Quaternion.Euler(0,0,direction*u*u*88);if(t>.55f)tree.color=new Color(1,1,1,Mathf.Clamp01((.8f-t)/.25f));yield return null;}
            Destroy(tree.gameObject);node.falling=false;game.world.Burst(node.transform.position,"dirt-puff");
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
            bool shedding=point.action=="creature"||point.action=="hunt"||point.action=="bud"||point.action=="tap";
            if(shedding){pullDrop=WorldView.Add(point.transform,"held-"+point.item,Vector2.up*.65f,1800);pullArt=pullDrop.transform;}
            else pullArt=point.artwork?point.artwork.transform:null;
            if(!pullArt){complete();yield break;}
            pullOrigin=pullArt.localPosition;float t=0;
            while(t<.55f){t+=Time.deltaTime;pullArt.localPosition=pullOrigin+new Vector3(Mathf.Round(Mathf.Sin(t*35)*2)/32,Mathf.Round(t*4)/32,0);yield return null;}
            game.world.Burst(point.transform.position,"dirt-puff");
            Vector3 start=pullArt.position,end=game.world.player.position+Vector3.up*.6f;t=0;
            while(t<.35f){t+=Time.deltaTime;float u=Mathf.Clamp01(t/.35f);pullArt.position=Vector3.Lerp(start,end,u)+Vector3.up*Mathf.Sin(u*Mathf.PI)*.8f;yield return null;}
            if(pullDrop){Destroy(pullDrop.gameObject);pullDrop=null;}else pullArt.localPosition=pullOrigin;pullArt=null;pulling=null;complete();
        }
        public void CancelPull(){if(pulling!=null)StopCoroutine(pulling);if(pullDrop){Destroy(pullDrop.gameObject);pullDrop=null;}else if(pullArt)pullArt.localPosition=pullOrigin;pulling=null;pullArt=null;}
        public void Sail(int destination,Action complete){StartCoroutine(SailRoutine(destination,complete));}
        IEnumerator SailRoutine(int destination,Action complete)
        {
            Sailing=true;var curtain=SceneCurtain.Create(game.ui);var world=game.world;var body=world.player.GetComponent<Rigidbody2D>();body.simulated=false;
            var ships=world.GetComponentsInChildren<SpriteRenderer>(true).Where(sr=>sr.gameObject.name=="boat").ToArray();foreach(var sr in ships)sr.enabled=false;
            int origin=world.Area;Vector2 departure=WaterRoute.Harbor(origin),landing=WaterRoute.Harbor(destination);
            var ship=WorldView.Add(world.transform,"boat",departure,1800);ship.gameObject.name="Sailing skiff";
            world.FollowSea=true;world.playerArt.sortingOrder=1802;
            Transform arrival=world.IslandRoot(destination);
            Vector3 offset=new Vector3(Archipelago.Get(origin).size.x/2+Archipelago.Get(destination).size.x/2+24,0,0);arrival.localPosition=offset;arrival.gameObject.SetActive(true);
            var oceanTiles=new List<GameObject>{TileWorld.Ocean(world.transform,game.Model.State.reducedMotion)};
            float seaY=-Mathf.Max(Archipelago.Get(origin).size.y,Archipelago.Get(destination).size.y)/2-8;
            var departurePath=WaterRoute.Escape(origin,seaY);var arrivalPath=WaterRoute.Escape(destination,seaY).Reverse().Select(p=>p+(Vector2)offset).ToArray();
            yield return FollowShip(ship,departurePath,3.2f);
            yield return MoveShip(ship,departurePath.Last(),arrivalPath.First(),3.8f);
            yield return FollowShip(ship,arrivalPath,3.2f);
            yield return curtain.Fade(true);arrival.localPosition=Vector3.zero;foreach(var tile in oceanTiles)Destroy(tile);
            foreach(var sr in ships)sr.enabled=true;Destroy(ship.gameObject);body.simulated=true;world.FollowSea=false;
            world.SetArea(destination,Archipelago.Get(destination).arrival);yield return curtain.Fade(false);Destroy(curtain.gameObject);Sailing=false;complete();
        }
        IEnumerator FollowShip(SpriteRenderer ship,Vector2[] path,float duration)
        {for(int i=1;i<path.Length;i++)yield return MoveShip(ship,path[i-1],path[i],duration/Mathf.Max(1,path.Length-1));}
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
