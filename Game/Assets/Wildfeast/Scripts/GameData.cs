using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Wildfeast
{
    [Serializable] public class Ingredient { public string id, name, habitat, description, icon; }
    [Serializable] public class Amount { public string id; public int count; public Amount(string id, int count) { this.id = id; this.count = count; } }
    [Serializable] public class Recipe { public string id, name, description, flavor, icon; public int price; public Amount[] ingredients; }
    [Serializable] public class Upgrade { public string id, name, description; public int cost; }
    [Serializable] public class Content
    {
        public Ingredient[] ingredients; public Recipe[] recipes; public Upgrade[] upgrades;
        public int bagCapacity = 8, cropDays = 2, guests = 3;
        public float moveSpeed = 3.8f, fishDuration = 5f, cookDuration = 7f;
        public static Content Load() => JsonUtility.FromJson<Content>(Resources.Load<TextAsset>("Content").text);
        public Ingredient Item(string id) => ingredients.First(i => i.id == id);
        public Recipe Dish(string id) => recipes.First(r => r.id == id);
        public Upgrade Improvement(string id) => upgrades.First(u => u.id == id);
    }
    [Serializable] public class Crop { public string item = "pepperbell"; public int growth, wateredDay = -1; public bool planted; }
    [Serializable] public class Order { public int number; public string recipe, customer, preference; public bool cooked, paid; public int quality; }
    [Serializable] public class Progress
    {
        public int version = 1, day = 1, coins, earned, served, stage, island, requestIndex;
        public string phase = "explore";
        public List<Amount> bag = new List<Amount>(), pantry = new List<Amount>();
        public List<string> discovered = new List<string>(), recipes = new List<string>(), upgrades = new List<string>(), harvested = new List<string>();
        public List<Order> orders = new List<Order>();
        public List<string> menu = new List<string>();
        public Crop[] crops = { new Crop(), new Crop(), new Crop() };
        public bool relaxed = true, muted, storySeen;
        public float volume = 0.35f;
        public int pepperSeeds = 5, rootSeeds, equipped;
        public ItemSlot[] slots;
        public List<FieldPlot> fields = new List<FieldPlot>();
        public List<ResourceStock> resources = new List<ResourceStock>();
        public int water=20;
        public static Progress New()
        {
            var p = new Progress(); p.pantry.Add(new Amount("grain", 6)); p.discovered.Add("grain");
            p.recipes.Add("seared"); p.recipes.Add("wrap"); p.recipes.Add("porridge"); p.menu.Add("seared"); p.menu.Add("wrap");
            return p;
        }
    }

    // All quantities, payouts and unlocks change through this model. Views never award money.
    public sealed class GameModel
    {
        public Content Data { get; }
        public Progress State { get; private set; }
        public event Action Changed;
        public GameModel(Content data, Progress state) { Data = data; State = state; ItemInventory.Sync(State); }
        public int Capacity => Data.bagCapacity + (Has("satchel") ? 8 : 0) + (Has("boat") ? 4 : 0);
        public int BagCount => State.bag.Sum(a => a.count);
        public bool Has(string id) => State.upgrades.Contains(id);
        public int Count(string id, bool bag = false) => (bag ? State.bag : State.pantry).Where(a => a.id == id).Sum(a => a.count);
        public void Notify() { ItemInventory.Sync(State); Changed?.Invoke(); }
        static void Adjust(List<Amount> list, string id, int value)
        {
            var a = list.FirstOrDefault(x => x.id == id);
            if (a == null) { if (value > 0) list.Add(new Amount(id, value)); }
            else { a.count += value; if (a.count == 0) list.Remove(a); }
        }
        public bool Gather(string id, int count = 1, string source = null)
            => GatherInternal(id, count, source, true);
        internal bool GatherInternal(string id, int count, string source, bool notify)
        {
            if (State.phase != "explore" || count < 1 || BagCount + count > Capacity || !Data.ingredients.Any(i => i.id == id)) return false;
            if (source != null && State.harvested.Contains(source)) return false;
            Adjust(State.bag, id, count);
            if (source != null) State.harvested.Add(source);
            if (source != null && id == "pepperbell") State.pepperSeeds++;
            if (source != null && id == "lanternroot") State.rootSeeds++;
            if (!State.discovered.Contains(id)) State.discovered.Add(id);
            if (id == "brothback") Learn("broth");
            if (id == "lanternroot") Learn("lantern");
            if (id == "cloudfruit") Learn("cloud");
            if (notify) Notify(); return true;
        }
        void Learn(string id) { if (!State.recipes.Contains(id)) State.recipes.Add(id); }
        public void Deposit()
        {
            foreach (var a in State.bag) Adjust(State.pantry, a.id, a.count);
            State.bag.Clear(); Notify();
        }
        public bool Buy(string id)
        {
            var u = Data.upgrades.FirstOrDefault(x => x.id == id);
            if (u == null || Has(id) || State.coins < u.cost || State.phase == "service") return false;
            if (id == "staff" && !Has("room")) return false;
            State.coins -= u.cost; State.upgrades.Add(id); Notify(); return true;
        }
        public bool CanCook(string id)
        {
            var recipe = Data.recipes.FirstOrDefault(r => r.id == id);
            return recipe != null && State.recipes.Contains(id) && recipe.ingredients.All(a => Count(a.id) >= a.count);
        }
        public bool StartService()
        {
            if (State.phase != "explore") return false;
            Deposit();
            var offered = State.menu.Where(CanCook).Distinct().ToArray();
            if (offered.Length == 0) return false;
            State.orders.Clear(); State.earned = 0;
            var names = new[] { "Mira · cartographer", "Orrin · ferryman", "Saff · seed keeper", "Tala · traveler", "Jun · astronomer" };
            var flavors = new[] { "fresh", "warm", "spiced", "sweet", "sweet" };
            // Reserve against a temporary pantry so every offered order can actually be fulfilled.
            var remaining = State.pantry.ToDictionary(a => a.id, a => a.count);
            for (int i = 0; i < Data.guests + (Has("room") ? 2 : 0); i++)
            {
                int person = (i + State.day - 1) % names.Length;
                var available = offered.Where(id => Data.Dish(id).ingredients.All(a => remaining.GetValueOrDefault(a.id) >= a.count)).ToArray();
                if (available.Length == 0) break;
                string choice = available.FirstOrDefault(id => Data.Dish(id).flavor == flavors[person]) ?? available[i % available.Length];
                foreach (var a in Data.Dish(choice).ingredients) remaining[a.id] -= a.count;
                State.orders.Add(new Order { number = i, customer = names[person], preference = flavors[person], recipe = choice });
            }
            if (State.orders.Count == 0) return false;
            State.phase = "service"; Notify(); return true;
        }
        public bool Cook(int number, int quality)
        {
            var o = State.orders.FirstOrDefault(x => x.number == number);
            if (State.phase != "service" || o == null || o.cooked || o.paid || !CanCook(o.recipe)) return false;
            foreach (var a in Data.Dish(o.recipe).ingredients) Adjust(State.pantry, a.id, -a.count);
            o.cooked = true; o.quality = Math.Clamp(quality, 0, 2); Notify(); return true;
        }
        public bool Serve(int number)
        {
            var o = State.orders.FirstOrDefault(x => x.number == number);
            if (State.phase != "service" || o == null || !o.cooked || o.paid) return false;
            var recipe = Data.Dish(o.recipe);
            int payment = recipe.price + o.quality * 3 + (recipe.flavor == o.preference ? 4 : 0);
            o.paid = true; State.coins += payment; State.earned += payment; State.served++;
            Notify(); return true;
        }
        public bool CloseService()
        {
            if (State.phase != "service" || State.orders.Any(o => !o.paid)) return false;
            State.phase = "closing"; Notify(); return true;
        }
        public bool NextDay()
        {
            if (State.phase == "service" && State.orders.Any(o => !o.paid)) return false;
            foreach(var f in State.fields)if(f.crop.planted&&f.crop.wateredDay==State.day)f.crop.growth++;
            State.day++; State.phase = "explore"; State.island = 0; State.harvested.Clear(); State.orders.Clear();
            for (int i = 0; i < State.crops.Length; i++) if (State.crops[i].planted && State.crops[i].wateredDay == State.day - 1) State.crops[i].growth++;
            if (Count("grain") < 3) Adjust(State.pantry, "grain", 3 - Count("grain"));
            Notify(); return true;
        }
        public bool Tend(int index, string item = "pepperbell")
        {
            if (index < 0 || index >= State.crops.Length || State.phase != "explore") return false;
            var crop = State.crops[index];
            if (!crop.planted)
            {
                if (!State.discovered.Contains(item) || (item != "pepperbell" && item != "lanternroot")) return false;
                crop.planted = true; crop.item = item; crop.growth = 0; crop.wateredDay = State.day; Notify(); return true;
            }
            if (crop.growth >= Data.cropDays)
            {
                if (!GatherInternal(crop.item, 3, null, false)) return false;
                crop.growth = 0; crop.wateredDay = State.day; Notify(); return true;
            }
            if (crop.wateredDay == State.day) return false;
            crop.wateredDay = State.day; Notify(); return true;
        }
        public bool Travel(int island)
        {
            if (State.phase != "explore" || island < 0 || island > 1) return false;
            State.island = island; Notify(); return true;
        }
        public int Seeds(string item) => item == "pepperbell" ? State.pepperSeeds : item == "lanternroot" ? State.rootSeeds : 0;
        public bool Plant(int index, string item)
        {
            if (State.phase != "explore" || index < 0 || index >= State.crops.Length || State.crops[index].planted || Seeds(item) < 1) return false;
            if(item == "pepperbell") State.pepperSeeds--; else State.rootSeeds--;
            var crop=State.crops[index]; crop.planted=true;crop.item=item;crop.growth=0;crop.wateredDay=-1;
            Notify();return true;
        }
        public bool Water(int index)
        {
            if(State.phase != "explore" || index<0 || index>=State.crops.Length)return false;
            var crop=State.crops[index];if(!crop.planted || crop.wateredDay==State.day || crop.growth>=Data.cropDays)return false;
            crop.wateredDay=State.day;Notify();return true;
        }
        public bool Harvest(int index)
        {
            if(State.phase != "explore" || index<0 || index>=State.crops.Length)return false;
            var crop=State.crops[index];if(!crop.planted || crop.growth<Data.cropDays || !GatherInternal(crop.item,3,null,false))return false;
            crop.growth=0;crop.wateredDay=-1;Notify();return true;
        }
        public bool ClaimRequest()
        {
            // Each request is claimed once and advances its persisted index atomically.
            string[] goals = { "seared", "broth", "lantern", "cloud" };
            if (State.requestIndex >= goals.Length) return false;
            if (!State.orders.Any(o => o.paid && o.recipe == goals[State.requestIndex])) return false;
            State.coins += 18 + 6 * State.requestIndex; State.requestIndex++; Notify(); return true;
        }
    }
}
