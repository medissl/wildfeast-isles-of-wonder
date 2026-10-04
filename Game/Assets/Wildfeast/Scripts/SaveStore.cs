using System;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Wildfeast
{
    public sealed class SaveStore
    {
        public readonly string Path;
        public string Warning { get; private set; }
        public SaveStore(string path) { Path = path; }
        public void Write(Progress p)
        {
            Directory.CreateDirectory(System.IO.Path.GetDirectoryName(Path));
            string temp = Path + ".tmp";
            File.WriteAllText(temp, JsonUtility.ToJson(p, true));
            if (File.Exists(Path)) File.Replace(temp, Path, Path + ".bak", true);
            else File.Move(temp, Path);
        }
        public Progress Read(Content content)
        {
            Warning = null;
            if (!File.Exists(Path)) return Progress.New();
            try { return Validate(File.ReadAllText(Path), content); }
            catch (Exception ex) when (ex is IOException || ex is ArgumentException || ex is InvalidDataException)
            {
                try { File.Copy(Path, Path + ".unreadable-" + DateTime.Now.ToString("yyyyMMddHHmmss"), true); } catch (IOException) { }
                Warning = "The main save could not be read. Restored a backup if available.";
                try { return Validate(File.ReadAllText(Path + ".bak"), content); }
                catch { Warning = "Save data could not be read. Started a fresh day; original files are preserved."; return Progress.New(); }
            }
        }
        static Progress Validate(string text, Content data)
        {
            var p = JsonUtility.FromJson<Progress>(text);
            if(p!=null&&!text.Contains("\"pepperSeeds\"")){p.pepperSeeds=5;p.rootSeeds=0;p.equipped=0;}
            if (p == null || p.version != 1 || p.day < 1 || p.coins < 0 || p.bag == null || p.pantry == null || p.orders == null || p.crops == null || p.crops.Length != 3 || p.discovered == null || p.upgrades == null || p.recipes == null || p.menu == null || p.harvested == null) throw new InvalidDataException();
            if(!text.Contains("\"musicVolume\"")){p.musicVolume=p.volume;p.effectsVolume=p.volume;p.zoom=1;}
            if(float.IsNaN(p.musicVolume)||float.IsInfinity(p.musicVolume)||p.musicVolume<0||p.musicVolume>1||float.IsNaN(p.effectsVolume)||float.IsInfinity(p.effectsVolume)||p.effectsVolume<0||p.effectsVolume>1||p.zoom<0||p.zoom>2)throw new InvalidDataException();
            if (p.phase != "explore" && p.phase != "service" && p.phase != "closing") throw new InvalidDataException();
            if(p.pepperSeeds<0||p.rootSeeds<0||p.equipped<0||p.equipped>9)throw new InvalidDataException();
            foreach (var list in new[] { p.bag, p.pantry })
                if (list.Any(a => a == null || a.count < 1 || !data.ingredients.Any(i => i.id == a.id)) || list.Select(a => a.id).Distinct().Count() != list.Count) throw new InvalidDataException();
            if (p.orders.Any(o => o == null || !data.recipes.Any(r => r.id == o.recipe) || (o.paid && !o.cooked)) || p.orders.Select(o => o.number).Distinct().Count() != p.orders.Count) throw new InvalidDataException();
            if (p.crops.Any(c => c == null || c.growth < 0 || (c.item != "pepperbell" && c.item != "lanternroot"))) throw new InvalidDataException();
            if (!Archipelago.Valid(p.island) || p.menu.Any(id => !data.recipes.Any(r => r.id == id)) || p.upgrades.Any(id => !data.upgrades.Any(u => u.id == id))) throw new InvalidDataException();
            if(!text.Contains("\"water\""))p.water=20;
            if(p.water<0||p.water>20)throw new InvalidDataException();
            if(p.slots!=null&&(p.slots.Length!=ItemInventory.Size||p.slots.Any(a=>a==null||a.id==null||a.count<0||!ItemInventory.Valid(a.id,data)||(a.id.StartsWith("tool-")&&a.count!=1)||(a.id==""&&a.count!=0))||p.slots.Where(a=>!string.IsNullOrEmpty(a.id)).GroupBy(a=>a.id).Any(g=>g.Count()>1)))throw new InvalidDataException();
            if(p.fields==null)p.fields=new System.Collections.Generic.List<FieldPlot>();
            if(p.resources==null)p.resources=new System.Collections.Generic.List<ResourceStock>();
            if(p.fields.Count>256||p.fields.Any(f=>f==null||!Archipelago.Valid(f.island)||f.crop==null||f.crop.growth<0||!ItemInventory.Growable(f.crop.item))||p.fields.GroupBy(f=>$"{f.island}:{f.x}:{f.y}").Any(g=>g.Count()>1))throw new InvalidDataException();
            if(p.resources.Any(a=>a==null||a.count<0||(a.id!="wood"&&a.id!="stone"&&a.id!="fiber"))||p.resources.GroupBy(a=>a.id).Any(g=>g.Count()>1))throw new InvalidDataException();
            if(p.seeds==null)p.seeds=new System.Collections.Generic.List<ResourceStock>();
            if(p.ecology==null)p.ecology=new System.Collections.Generic.List<EcologyProgress>();
            if(p.seeds.Any(s=>s==null||s.count<0||!ItemInventory.Growable(s.id)||s.id=="pepperbell"||s.id=="lanternroot")||p.seeds.GroupBy(s=>s.id).Any(g=>g.Count()>1)||p.ecology.Any(s=>s==null||string.IsNullOrEmpty(s.source)||s.contacts<0||s.contacts>3)||p.ecology.GroupBy(s=>s.source).Any(g=>g.Count()>1))throw new InvalidDataException();
            // Existing garden crops survive a redesigned road/pool: relocate the whole plot,
            // retaining its item, growth and watering day, instead of leaving farmland on a road.
            foreach(var field in p.fields)
            {
                if(Archipelago.Tillable(new Vector2(field.x,field.y),field.island))continue;
                var island=Archipelago.Get(field.island);Vector2 origin=new Vector2(field.x,field.y);
                var candidates=Enumerable.Range(-(int)island.size.x/2,(int)island.size.x).SelectMany(x=>Enumerable.Range(-(int)island.size.y/2,(int)island.size.y).Select(y=>new Vector2(x,y))).Where(tile=>Archipelago.Tillable(tile,field.island)&&!p.fields.Any(f=>f!=field&&f.island==field.island&&f.x==tile.x&&f.y==tile.y)&&!island.props.Any(prop=>prop.role!="grass"&&Vector2.Distance(prop.position,tile)<1.2f)&&!island.points.Any(point=>point.action!="fish"&&Vector2.Distance(point.position,tile)<1.5f)).OrderBy(tile=>(tile-origin).sqrMagnitude).ToArray();
                if(candidates.Length==0)throw new InvalidDataException();field.x=(int)candidates[0].x;field.y=(int)candidates[0].y;
            }
            ItemInventory.Sync(p);return p;
        }
        public void ArchiveAndReset()
        {
            if (File.Exists(Path)) File.Copy(Path, Path + ".reset-" + DateTime.Now.ToString("yyyyMMddHHmmss"), true);
            Write(Progress.New());
        }
    }
}
