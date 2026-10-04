using System.Linq;
using UnityEngine;
namespace Wildfeast
{
    public class IslandDiscoveries : MonoBehaviour
    {
        GameController game;
        public static readonly string[] Stories={
            "The dinner bell has been silent for years. Its first note brings tiny Leafgills to the shallows. Someone once cooked for everyone on this shore.",
            "A forgotten rain kettle cradles a cloudfruit cutting. Water wakes its roots; sweet clouds puff from its spout. An orchard keeper left a custard recipe inside.",
            "Under the caramel crust, warm spice glows in the stone. The abandoned quarry was a kitchen long before it was a mine.",
            "The old moon harp is tangled in silvery reeds. Clear them and the wetland answers with soft pollen lights. Its tune belongs to the mooncake makers.",
            "A shell compass points toward home, whichever way you turn it. The tide leaves pearls in its bowl. A restaurant keeper has scratched a tempura recipe underneath."};
        public static readonly int[] Tools={0,2,9,8,0};
        public static readonly string[] Recipes={"broth","custard","claw","pollen","pearl"};
        public void Init(GameController controller)
        {
            game=controller;
            foreach(var i in Archipelago.Islands)
            {
                var root=game.world.IslandRoot(i.id);foreach(var marker in game.world.points.Where(p=>p.action=="fish"&&p.transform.IsChildOf(root)))if(marker.artwork)marker.artwork.enabled=false;
                // Schools are scenery in water, never land-based activation events.
                foreach(var pool in Archipelago.Pools(i.id))for(int k=0;k<4;k++)
                {
                    var school=WorldView.Add(root,"fish-shadow",pool.center+new Vector2((k-1.5f)*.5f,(k%2-.5f)*.6f),-1450,true);
                    var motion=school.gameObject.AddComponent<WorldMotion>();motion.mode=1;motion.phase=k;motion.radius=.18f;motion.speed=.5f;
                }
                var fish=i.points.First(p=>p.action=="fish");
                for(int k=0;k<3;k++){var pos=fish.position+new Vector2(k*.45f,0);if(!WorldView.Water(pos,i.id))continue;var sr=WorldView.Add(root,"fish-shadow",pos,-1450,true);var motion=sr.gameObject.AddComponent<WorldMotion>();motion.mode=1;motion.phase=k;motion.radius=.12f;motion.speed=.4f;}
            }
        }
        public void Visit(WorldPoint p)
        {
            int index=System.Array.FindIndex(Archipelago.Islands,i=>p.transform.IsChildOf(game.world.IslandRoot(i.id)));if(index<0)return;
            if(game.Model.State.landmarks.Contains(p.source)){Show(p,index);return;}
            if(Tools[index]==0){game.DiscoveryContact(p,0);return;}
            game.Say(index==1?"The kettle's seed is thirsty. Try your watering can.":index==2?"Spice glows beneath the crust. Try your pickaxe.":"Silver reeds muffle the harp. Clear them with your scythe.");
        }
        public bool Use(WorldPoint p,int tool)
        {
            if(p==null||p.action!="discovery"||Vector2.Distance(game.world.player.position,p.transform.position)>2)return false;
            int index=System.Array.FindIndex(Archipelago.Islands,i=>i.id==game.world.Area);if(index<0)return false;
            if(game.Model.State.landmarks.Contains(p.source)){Show(p,index);return true;}
            if(tool!=Tools[index]){Visit(p);return true;}
            if(tool==2&&game.Model.State.water<=0){game.Say("Refill your watering can first.");return true;}
            if(!game.Model.Discover(p.source,p.item,Recipes[index],tool==2)){game.Say("Make room for two portions before uncovering this discovery.");return true;}
            game.world.Burst(p.transform.position+Vector3.up*.6f,"spark");game.world.Burst(p.transform.position+Vector3.up*.4f,p.item);Show(p,index);return true;
        }
        void Show(WorldPoint p,int index)
        {game.ui.Show(p.label,"Island discovery · remembered in your journal");game.ui.Paragraph(Stories[index]+"\n\nRecipe: "+game.Model.Data.Dish(Recipes[index]).name);game.ui.FooterButton("Back to the island",game.ui.Hide);}
        void Update()
        {
            if(!game)return;foreach(var p in game.world.points.Where(p=>p.action=="discovery"&&p.gameObject.activeInHierarchy))
            {bool open=game.Model.State.landmarks.Contains(p.source);p.artwork.sprite=WorldView.Art(p.source+(open?"-awake":""));p.artwork.transform.localPosition=Vector3.up*(.02f*Mathf.Sin(Time.time*1.5f));}
        }
    }
}
