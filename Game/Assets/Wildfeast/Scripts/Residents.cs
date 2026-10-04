using System;
using System.Linq;
using UnityEngine;
namespace Wildfeast
{
    [Serializable] public class ResidentProgress { public string id;public int friendship,talkDay;public bool eventDone; }
    public sealed class ResidentDefinition
    {
        public string id,name,title,item,eventName;public string[] lines,daily;
        public ResidentDefinition(string id,string name,string title,string item,string eventName,string[] lines,string[] daily)
        {this.id=id;this.name=name;this.title=title;this.item=item;this.eventName=eventName;this.lines=lines;this.daily=daily;}
    }
    public static class Residents
    {
        public static readonly ResidentDefinition[] All={
            new ResidentDefinition("nori","Nori","Seed keeper of Bramblewick","pepperbell","The sneezing seed stall",new[]{"Welcome to Bramblewick! The bread trees wake before the bakers here.","My Pepperbell seeds sneezed all over the shop this morning. I told them: indoors voices!", "Bring me one Pepperbell. Its scent will coax the seedlings back into their pots."},new[]{"The noodlegrass is growing curly today. Rain must be close.","I saved a sunny shelf for your first harvest.","Don't worry about a crooked garden. My best cabbage-fish hatched upside down."}),
            new ResidentDefinition("iona","Iona","Keeper of Cloudfruit Heights","cloudfruit","A fruit that won't land",new[]{"You found my orchard. Walk softly; the fruit listens through its roots.","One cloudfruit floated into my chimney. My tea has tasted like the sky ever since!", "Could you bring one Cloudfruit? We'll weigh it down with a spoonful of honey."},new[]{"Custardrams nap beneath the cloud trees. Give them a little quiet.","Every fruit needs a different kind of patience.","I used to sail for days looking for a flavor. Now I grow little journeys here."}),
            new ResidentDefinition("saff","Saff","Spice smith of Emberfold","spiceclaw","The gentle spice hammer",new[]{"Mind the warm stone. The ridge is a sleeping spice oven.","A pangolin ate my last cinnamon hammer. Yes, the handle too. That's the trouble with edible tools.", "Bring a shed Pangolin spice plate. I'll make a hammer it can't resist sharing."},new[]{"Three light taps loosen a spice plate. No need to hurt the animal.","The moonfen trail is cool after the ridge. Good place to steep a thought.","I'm testing pepper-powered wind chimes. Stand back when they sneeze."}),
            new ResidentDefinition("luma","Luma","Moonfen tea keeper","dewnectar","Tea for the shy blossom",new[]{"Welcome. Even our lanterns bloom before they glow.","This blossom refuses to open unless someone compliments its leaves. Fair enough, honestly.", "Bring one Dew nectar. We can make a tea that warms the shyest flower."},new[]{"The silver reeds ring when the moths pass. Listen between your footsteps.","I brew moonlight slowly. Hurrying makes it taste like yesterday.","Your restaurant smells travel farther than your boat. The moths told me."}),
            new ResidentDefinition("pico","Pico","Pilot of the Pearltide skiff","kelpjelly","A wobbly little lighthouse",new[]{"Pearltide ahead! The reefs grow their own supper.","The lighthouse snail is sulking. Apparently I called it a pudding. An understandable mistake.", "Bring one Kelp jelly. A little snack will get our lighthouse smiling again."},new[]{"The ferry follows the water, chef. Roads are for people with dry boots.","Bubblecarp dance when the tide turns. They have terrible rhythm.","I sail three islands, but I never eat the same breakfast twice."})
        };
        public static ResidentDefinition Get(string id)=>All.FirstOrDefault(r=>r.id==id);
        public static ResidentProgress State(GameModel model,string id)
        {var state=model.State.residents.FirstOrDefault(r=>r.id==id);if(state==null){state=new ResidentProgress{id=id};model.State.residents.Add(state);}return state;}
        public static bool Talk(GameModel model,string id)
        {if(Get(id)==null)return false;var s=State(model,id);if(s.talkDay==model.State.day)return false;s.talkDay=model.State.day;s.friendship=Mathf.Min(10,s.friendship+1);model.Notify();return true;}
        public static bool Deliver(GameModel model,string id)
        {
            var definition=Get(id);if(definition==null||model.State.phase!="explore")return false;var s=model.State.residents.FirstOrDefault(a=>a.id==id);
            if((s!=null&&s.eventDone)||model.Count(definition.item,true)+model.Count(definition.item)<1)return false;
            if(s==null)s=State(model,id);
            var list=model.Count(definition.item,true)>0?model.State.bag:model.State.pantry;var portion=list.First(a=>a.id==definition.item);portion.count--;if(portion.count==0)list.Remove(portion);
            s.eventDone=true;s.friendship=Mathf.Min(10,s.friendship+2);model.State.coins+=20;model.Notify();return true;
        }
        public static bool BuySeed(GameModel model,string item)
        {
            if(model.State.phase!="explore"||model.State.coins<5||!ItemInventory.Growable(item))return false;
            model.State.coins-=5;if(item=="pepperbell")model.State.pepperSeeds++;else if(item=="lanternroot")model.State.rootSeeds++;
            else {var s=model.State.seeds.FirstOrDefault(a=>a.id==item);if(s==null){s=new ResourceStock{id=item};model.State.seeds.Add(s);}s.count++;}model.Notify();return true;
        }
        public static void Open(GameController game,string id,int page=0)
        {
            var d=Get(id);if(d==null)return;Talk(game.Model,id);var s=State(game.Model,id);
            string line=s.eventDone?d.daily[(game.Model.State.day+page)%d.daily.Length]:d.lines[Mathf.Min(page,2)];string expression=s.eventDone?s.friendship>=5?"love":"smile":page==0?"neutral":page==1?"laugh":"smile";
            game.ui.Dialogue(d.name,d.title,line,"portrait-"+id+"-"+expression,
                page<2?"Next":"Goodbye",()=>{if(page<2)Open(game,id,page+1);else game.ui.Hide();},
                !s.eventDone&&page==2?"Give "+game.Model.Data.Item(d.item).name:null,()=>
                {if(Deliver(game.Model,id))game.ui.Dialogue(d.name,d.eventName,"You brought it! We'll make a new flavor together. Here are 20 shells for your trouble.","portrait-"+id+"-love","Goodbye",game.ui.Hide,null,null);
                else game.ui.Dialogue(d.name,d.eventName,"An empty pocket? You teased me! All right, I forgive you. Come back with a portion; I’ll keep the kettle warm.","portrait-"+id+"-mad","Goodbye",game.ui.Hide,null,null);});
        }
    }
}
