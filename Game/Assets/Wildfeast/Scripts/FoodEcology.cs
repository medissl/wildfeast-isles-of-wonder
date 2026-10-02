using UnityEngine;
using System;
using System.Linq;
namespace Wildfeast
{
    [Serializable] public class EcologyProgress {public string source;public int contacts;public bool ready;}
    // Food creatures shed ingredients rather than being killed. Conditions are spatial and visible.
    public class FoodEcology : MonoBehaviour
    {
        public string kind,sprite; public WorldPoint point;
        public bool Ready {get;private set;} public int Contacts {get;private set;}
        GameController game; Vector3 home; Vector2 lastHero; float calm; int day;
        void Start(){game=FindFirstObjectByType<GameController>();home=transform.localPosition;lastHero=game.world.player.position;day=game.Model.State.day;var state=game.Model.State.ecology.FirstOrDefault(s=>s.source==point.source);if(state!=null){Contacts=state.contacts;Ready=state.ready;}}
        void ReadCondition()
        {var state=game.Model.State.ecology.FirstOrDefault(s=>s.source==point.source);if(state!=null){Contacts=state.contacts;Ready=state.ready;}}
        void Update()
        {
            if(!game||game.Model==null||game.Sailing)return;
            if(day!=game.Model.State.day){day=game.Model.State.day;Ready=false;Contacts=0;calm=0;transform.localPosition=home;}
            bool harvested=game.Model.State.harvested.Contains(point.source);
            Vector2 hero=game.world.player.position;float distance=Vector2.Distance(hero,transform.position);
            if(kind=="ram")
            {
                bool still=Vector2.Distance(hero,lastHero)<.01f;
                calm=still&&distance<2.5f?calm+Time.deltaTime:0;Ready=calm>=2;
                if(!still&&distance<2.1f)transform.localPosition=Vector3.MoveTowards(transform.localPosition,home+(transform.position-(Vector3)hero).normalized*.65f,Time.deltaTime*.6f);
                else transform.localPosition=Vector3.MoveTowards(transform.localPosition,home,Time.deltaTime*.35f);
            }
            if(kind=="moth")
            {
                bool lure=game.Model.State.slots[game.Model.State.equipped].id=="lanternroot";
                var target=lure&&distance<4?Vector3.Lerp(home,transform.parent.InverseTransformPoint(hero),.65f):home;
                transform.localPosition=Vector3.MoveTowards(transform.localPosition,target,Time.deltaTime*.5f);Ready=lure&&distance<1.8f;
            }
            if(kind=="crab"&&!Ready&&distance<2.5f)transform.localPosition=home+new Vector3(Mathf.Sin(Time.time)*.2f,0,0);
            lastHero=hero;
            if(point.artwork)
            {
                if(kind=="ram"||kind=="moth"||kind=="crab"||kind=="snail")point.artwork.sprite=WorldView.Art(sprite+"-"+((int)(Time.time*(Ready?1.5f:3))%4));
                if(kind=="bud")point.artwork.sprite=WorldView.Art(Ready?"dewblossom-open":"dewblossom-closed");
                if(kind=="crab"&&Ready&&!harvested)point.artwork.sprite=WorldView.Art("spicecrab-cracked");
                point.artwork.sortingOrder=1000-Mathf.RoundToInt(transform.position.y*32);
                point.artwork.color=harvested?new Color(.9f,.94f,.9f,1):Ready?new Color(1,1,.8f,1):Color.white;
            }
        }
        public string Hint=>game&&game.Model.State.harvested.Contains(point.source)?"Recovering Â· Fresh harvest tomorrow":kind=="ram"?(Ready?"E Â· Collect cream":"Stand still nearby Â· let the ram settle"):kind=="moth"?(Ready?"E Â· Collect pollen":"Hold Lanternroot to attract the moth"):kind=="crab"?(Ready?"E Â· Collect shed spice":$"Pickaxe Â· Crack shell {Contacts}/3"):kind=="snail"?(Ready?"E Â· Collect kelp jelly":"Watering can Â· Wake the Kelpsnail"):kind=="bud"?(Ready?"E Â· Harvest dew nectar":"Watering can Â· Open the Dewblossom"):"Field knife Â· Tap Cinnamon sap";
        public bool Tool(int tool)
        {
            if(game.Model.State.harvested.Contains(point.source)){game.Say("This source recovers tomorrow.");return true;}
            if(kind=="tap"&&tool==5){Collect(true);return true;}
            if(kind=="crab"&&tool==9&&!Ready){ItemInventory.EcologyAction(game.Model,point.source,kind,tool);ReadCondition();game.Sound("cook");game.world.Burst(transform.position,"stone");game.Say(Hint);return true;}
            if((kind=="snail"||kind=="bud")&&tool==2&&!Ready)
            {
                if(game.Model.State.water<1){game.Say("Refill your watering can first.");return true;}
                ItemInventory.EcologyAction(game.Model,point.source,kind,tool);ReadCondition();game.Sound("splash");game.world.Burst(transform.position,"splash");game.Say(Hint);return true;
            }
            return false;
        }
        public void Collect(bool fromTool=false)
        {
            if(game.Model.State.harvested.Contains(point.source)){game.Say("Return tomorrow for a fresh harvest.");return;}
            if(!Ready&&!(kind=="tap"&&fromTool)){game.Say(Hint);return;}
            game.HarvestEcology(point,kind=="tap"?2:1);
        }
    }
}
