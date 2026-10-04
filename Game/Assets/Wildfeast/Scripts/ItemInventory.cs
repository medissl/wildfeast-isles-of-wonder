using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Wildfeast
{
    [Serializable] public class ItemSlot { public string id=""; public int count; }
    [Serializable] public class FieldPlot { public int island,x,y; public Crop crop=new Crop(); }
    [Serializable] public class ResourceStock { public string id; public int count; }
    public static class ItemInventory
    {
        public const int Hotbar=10, Size=40;
        public static readonly string[] Tools={"", "tool-rod","tool-can","seed-pepper","seed-root","tool-knife","tool-shovel","tool-axe","tool-scythe","tool-pickaxe"};
        public static bool Growable(string id)=>new[]{"pepperbell","lanternroot","custardpetal","emberbulb","mooncap","pearlsprout"}.Contains(id);
        public static string SeedItem(string id)=>id=="seed-pepper"?"pepperbell":id=="seed-root"?"lanternroot":id.StartsWith("seed-")?id.Substring(5):"";
        public static string Name(string id,Content data)
        {
            string seed=SeedItem(id);if(Growable(seed))return data.Item(seed).name+" seeds";
            var food=data.ingredients.FirstOrDefault(i=>i.id==id);if(food!=null)return food.name;
            switch(id){case "tool-rod":return "Fishing rod";case "tool-can":return "Watering can";case "tool-knife":return "Field knife";case "tool-shovel":return "Shovel";case "tool-axe":return "Axe";case "tool-scythe":return "Scythe";case "tool-pickaxe":return "Pickaxe";case "seed-pepper":return "Pepperbell seeds";case "seed-root":return "Lanternroot seeds";case "wood":return "Cinnamonwood";case "stone":return "Saltstone";case "fiber":return "Noodlegrass fiber";default:return "Empty slot";}
        }
        public static bool Valid(string id,Content data)=>string.IsNullOrEmpty(id)||Tools.Contains(id)||Growable(SeedItem(id))||data.ingredients.Any(i=>i.id==id)||id=="wood"||id=="stone"||id=="fiber";
        public static void Ensure(Progress p)
        {
            if(p.slots!=null&&p.slots.Length==Size)return;
            p.slots=Enumerable.Range(0,Size).Select(_=>new ItemSlot()).ToArray();
            for(int i=1;i<Hotbar;i++)if(i!=3&&i!=4)p.slots[i]=new ItemSlot{id=Tools[i],count=1};
            p.slots[3]=new ItemSlot{id="seed-pepper",count=p.pepperSeeds};p.slots[4]=new ItemSlot{id="seed-root",count=p.rootSeeds};
        }
        // The bag and seed counters remain the authoritative quantities for legacy saves.
        // Slots retain placement, including tools moved into the backpack.
        public static void Sync(Progress p)
        {
            Ensure(p);if(p.resources==null)p.resources=new List<ResourceStock>();if(p.fields==null)p.fields=new List<FieldPlot>();
            var quantities=p.bag.ToDictionary(a=>a.id,a=>a.count);
            quantities["seed-pepper"]=p.pepperSeeds;quantities["seed-root"]=p.rootSeeds;
            if(p.seeds==null)p.seeds=new List<ResourceStock>();if(p.ecology==null)p.ecology=new List<EcologyProgress>();
            foreach(var seed in p.seeds)quantities["seed-"+seed.id]=seed.count;
            foreach(var a in p.resources)quantities[a.id]=a.count;
            foreach(var slot in p.slots)
            {
                if(slot.id.StartsWith("tool-"))continue;
                if(!string.IsNullOrEmpty(slot.id)&&quantities.TryGetValue(slot.id,out int n)&&n>0){slot.count=n;quantities.Remove(slot.id);}
                else {slot.id="";slot.count=0;}
            }
            foreach(var pair in quantities.Where(a=>a.Value>0)){var free=p.slots.FirstOrDefault(a=>string.IsNullOrEmpty(a.id));if(free!=null){free.id=pair.Key;free.count=pair.Value;}}
        }
        public static bool Swap(Progress p,int from,int to)
        {
            Ensure(p);if(from<0||to<0||from>=Size||to>=Size||from==to)return false;
            var a=p.slots[from];p.slots[from]=p.slots[to];p.slots[to]=a;return true;
        }
        public static bool EcologyAction(GameModel model,string source,string kind,int tool)
        {
            var p=model.State;
            if(p.phase!="explore"||p.harvested.Contains(source)||!Archipelago.Islands.SelectMany(i=>i.points).Any(x=>x.source==source))return false;
            bool crack=kind=="crab"&&tool==9,water=(kind=="snail"||kind=="bud")&&tool==2;
            if(!crack&&!water)return false;
            var condition=p.ecology.FirstOrDefault(e=>e.source==source);
            if(condition!=null&&condition.ready||water&&p.water<1)return false;
            if(condition==null){condition=new EcologyProgress{source=source};p.ecology.Add(condition);}
            if(crack){condition.contacts++;condition.ready=condition.contacts>=3;}
            else {p.water--;condition.ready=true;}
            model.Notify();return true;
        }
        public static int EquippedTool(Progress p){Ensure(p);string id=p.slots[p.equipped].id;return Growable(SeedItem(id))&&id!="seed-root"?3:Array.IndexOf(Tools,id);}
        public static bool Resource(GameModel model,string id,int quantity,string source)
        {
            var p=model.State;Sync(p);if(p.phase!="explore"||quantity<1||string.IsNullOrEmpty(source)||p.harvested.Contains(source)||(id!="wood"&&id!="stone"&&id!="fiber")||(!p.slots.Any(s=>s.id==id)&&!p.slots.Any(s=>s.count==0)))return false;
            var stock=p.resources.FirstOrDefault(a=>a.id==id);if(stock==null){stock=new ResourceStock{id=id};p.resources.Add(stock);}stock.count+=quantity;p.harvested.Add(source);model.Notify();return true;
        }
        public static FieldPlot Plot(Progress p,int island,Vector2 target)=>p.fields.FirstOrDefault(f=>f.island==island&&f.x==TerrainGrid.Cell(target).x&&f.y==TerrainGrid.Cell(target).y);
        public static bool Till(GameModel model,int island,Vector2 target)
        {
            var p=model.State;if(p.phase!="explore"||!Archipelago.Valid(island)||float.IsNaN(target.x)||float.IsNaN(target.y)||float.IsInfinity(target.x)||float.IsInfinity(target.y)||!Archipelago.Tillable(TerrainGrid.Cell(target),island)||Plot(p,island,target)!=null||p.fields.Count>=256)return false;
            p.fields.Add(new FieldPlot{island=island,x=TerrainGrid.Cell(target).x,y=TerrainGrid.Cell(target).y});model.Notify();return true;
        }
        public static bool Plant(GameModel model,FieldPlot plot,string item)
        {
            if(plot==null||!model.State.fields.Contains(plot)||plot.crop.planted||model.State.phase!="explore"||model.Seeds(item)<1)return false;
            if(item=="pepperbell")model.State.pepperSeeds--;else if(item=="lanternroot")model.State.rootSeeds--;else if(Growable(item))model.State.seeds.First(s=>s.id==item).count--;else return false;
            plot.crop=new Crop{item=item,planted=true};model.Notify();return true;
        }
        public static bool Water(GameModel model,FieldPlot plot)
        {
            if(plot==null||!model.State.fields.Contains(plot)||!plot.crop.planted||plot.crop.wateredDay==model.State.day||model.State.water<1||model.State.phase!="explore")return false;
            model.State.water--;plot.crop.wateredDay=model.State.day;model.Notify();return true;
        }
        public static bool Harvest(GameModel model,FieldPlot plot)
        {
            if(plot==null||!model.State.fields.Contains(plot)||plot.crop.growth<model.Data.cropDays||!model.GatherInternal(plot.crop.item,3,null,false))return false;
            plot.crop=new Crop();model.Notify();return true;
        }
        public static int ResourceValue(Progress state)=>state.resources.Sum(a=>a.count*(a.id=="wood"?3:a.id=="stone"?2:1));
        public static bool SellResources(GameModel model)
        {
            int value=ResourceValue(model.State);if(model.State.phase!="explore"||value<1)return false;
            model.State.resources.Clear();model.State.coins+=value;model.Notify();return true;
        }
    }
}
