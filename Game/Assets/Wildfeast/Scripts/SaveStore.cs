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
            if (p.phase != "explore" && p.phase != "service" && p.phase != "closing") throw new InvalidDataException();
            if(p.pepperSeeds<0||p.rootSeeds<0||p.equipped<0||p.equipped>7)throw new InvalidDataException();
            foreach (var list in new[] { p.bag, p.pantry })
                if (list.Any(a => a == null || a.count < 1 || !data.ingredients.Any(i => i.id == a.id)) || list.Select(a => a.id).Distinct().Count() != list.Count) throw new InvalidDataException();
            if (p.orders.Any(o => o == null || !data.recipes.Any(r => r.id == o.recipe) || (o.paid && !o.cooked)) || p.orders.Select(o => o.number).Distinct().Count() != p.orders.Count) throw new InvalidDataException();
            if (p.crops.Any(c => c == null || c.growth < 0 || (c.item != "pepperbell" && c.item != "lanternroot"))) throw new InvalidDataException();
            if (p.island < 0 || p.island > 1 || p.menu.Any(id => !data.recipes.Any(r => r.id == id)) || p.upgrades.Any(id => !data.upgrades.Any(u => u.id == id))) throw new InvalidDataException();
            return p;
        }
        public void ArchiveAndReset()
        {
            if (File.Exists(Path)) File.Copy(Path, Path + ".reset-" + DateTime.Now.ToString("yyyyMMddHHmmss"), true);
            Write(Progress.New());
        }
    }
}
