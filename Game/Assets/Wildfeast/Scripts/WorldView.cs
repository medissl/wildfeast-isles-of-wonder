using System.Collections.Generic;
using UnityEngine;

namespace Wildfeast
{
    public class WorldView : MonoBehaviour
    {
        public Transform saltleaf, mistwake, restaurant;
        public Transform player;
        public Camera worldCamera;
        public SpriteRenderer playerArt;
        public SpriteRenderer carriedDish;
        public GameObject terrace, employee;
        public List<WorldPoint> points = new List<WorldPoint>();
        public List<Transform> guests = new List<Transform>();
        public List<SpriteRenderer> cropArt = new List<SpriteRenderer>();
        public int Area { get; private set; }
        Sprite[] chefFrames;
        public static Sprite Art(string name) => Resources.Load<Sprite>("Art/" + name);
        public void Init()
        {
            chefFrames = new Sprite[4]; for (int i = 0; i < 4; i++) chefFrames[i] = Art("player-" + i);
        }
        public void SetArea(int area, Vector2 position)
        {
            Area = area; saltleaf.gameObject.SetActive(area == 0); mistwake.gameObject.SetActive(area == 1); restaurant.gameObject.SetActive(area == 2);
            player.position = position; player.GetComponent<Rigidbody2D>().position = position;
            worldCamera.transform.position = new Vector3(position.x, position.y + 1, -10);
        }
        public WorldPoint Nearest()
        {
            WorldPoint result = null; float distance = 1.6f;
            foreach (var p in points) if (p && p.gameObject.activeInHierarchy)
            {
                float d = Vector2.Distance(player.position, p.transform.position);
                if (d < distance) { distance = d; result = p; }
            }
            return result;
        }
        public void Animate(Vector2 motion)
        {
            playerArt.sprite = chefFrames[motion.sqrMagnitude > .01f ? (int)(Time.time * 8) % 4 : 0];
            if (Mathf.Abs(motion.x) > .05f) playerArt.flipX = motion.x < 0;
            playerArt.sortingOrder = 1000 - Mathf.RoundToInt(player.position.y * 32);
            if(carriedDish)carriedDish.sortingOrder=playerArt.sortingOrder+2;
            foreach (var p in points)
            {
                if (p.artwork && p.action == "fruit") p.artwork.transform.localPosition = new Vector3(0, .5f + Mathf.Round(Mathf.Sin(Time.time * 1.5f) * 4) / 32f, 0);
            }
            Vector3 target = new Vector3(player.position.x, player.position.y + 1, -10);
            if (Area == 2) target = new Vector3(0, 0, -10);
            else { target.x = Mathf.Clamp(target.x, -8.9f, 8.9f); target.y = Mathf.Clamp(target.y, -5.9f, 5.9f); }
            var cameraPosition = Vector3.Lerp(worldCamera.transform.position, target, 1 - Mathf.Exp(-8 * Time.deltaTime));
            cameraPosition.x = Mathf.Round(cameraPosition.x * 32) / 32f; cameraPosition.y = Mathf.Round(cameraPosition.y * 32) / 32f;
            worldCamera.transform.position = cameraPosition;
        }
        public void Refresh(GameModel model)
        {
            terrace.SetActive(model.Has("room")); employee.SetActive(model.Has("staff"));
            for (int i = 0; i < guests.Count; i++) guests[i].gameObject.SetActive(model.State.phase == "service" && i < model.State.orders.Count && !model.State.orders[i].paid);
            for (int i = 0; i < cropArt.Count; i++)
            {
                var crop = model.State.crops[i]; cropArt[i].sprite = crop.planted ? Art(crop.item) : null;
                cropArt[i].color = crop.growth >= model.Data.cropDays ? Color.white : new Color(.65f, .78f, .65f, 1);
                cropArt[i].transform.localScale = Vector3.one * (crop.growth >= model.Data.cropDays ? 1 : .6f);
            }
            foreach (var p in points) if (p.artwork && (p.action == "forage" || p.action == "hunt" || p.action == "fruit"))
                p.artwork.color = model.State.harvested.Contains(p.source) ? new Color(.45f, .55f, .48f, .6f) : Color.white;
            foreach(var p in points) if(p.action=="serve")
            {
                var order=model.State.orders.Find(o=>o.number==p.index);
                p.label=order==null?$"Table {p.index+1}":$"Table {p.index+1} · {model.Data.Dish(order.recipe).name}"+(order.cooked?" — dish ready":" — cook at the stove");
            }
            var ready=model.State.orders.Find(o=>o.cooked&&!o.paid);
            carriedDish.sprite=ready!=null&&!model.Has("staff")?Art(model.Data.Dish(ready.recipe).icon):null;
        }
        // Called by the Editor bootstrap. The authored results are serialized into the scene.
        public void AuthorWorlds()
        {
            saltleaf = new GameObject("SaltleafShore").transform; saltleaf.SetParent(transform);
            mistwake = new GameObject("MistwakeIsle").transform; mistwake.SetParent(transform);
            restaurant = new GameObject("HarborRestaurant").transform; restaurant.SetParent(transform);
            Add(saltleaf, "saltleaf", Vector2.zero, -2000, true); Add(mistwake, "mistwake", Vector2.zero, -2000, true); Add(restaurant, "interior", Vector2.zero, -2000, true);
            var random = new System.Random(43);
            foreach (var parent in new[] { saltleaf, mistwake })
            {
                bool second = parent == mistwake;
                for (int i = 0; i < 82; i++)
                {
                    float x = (float)random.NextDouble() * 28 - 14, y = (float)random.NextDouble() * 13 + -2;
                    if ((Mathf.Abs(x + 6) < 4 && y < 4) || (x > 5 && y > 3) || (x > -3 && x < 7 && y < 3) || (x < -6 && y > 3 && y < 8)) continue;
                    var tree = Add(parent, second ? "pine" : "tree", new Vector2(x, y), 1000 - Mathf.RoundToInt(y * 32));
                    Block(tree.transform, new Vector2(.38f, .4f), new Vector2(0, .15f));
                }
                for (int i = 0; i < 130; i++)
                {
                    float x = (float)random.NextDouble() * 28 - 14, y = (float)random.NextDouble() * 15 - 7;
                    if (Mathf.Abs(x + 6) < 3 && Mathf.Abs(y) < 4) continue;
                    Add(parent, i % 4 == 0 ? "flower" : "reed", new Vector2(x, y), -1000);
                }
                Block(parent, new Vector2(5.9f, 3.6f), new Vector2(10.4f, 5.9f));
                Border(parent, 16, 9.4f);
            }
            var home = Add(saltleaf, "restaurant", new Vector2(-6, -.9f), 1030);
            Block(home.transform, new Vector2(4.8f, 2.7f), new Vector2(0, 2));
            Point(saltleaf, "enter", "Enter the restaurant", new Vector2(-6, -1.15f));
            Point(saltleaf, "fish", "Fish the Saltleaf shallows", new Vector2(8, -8.6f), "leafgill", "", "leafgill");
            Point(saltleaf, "forage", "Harvest Pepperbell", new Vector2(4.5f, -1.3f), "pepperbell", "pepper-east", "pepperbell");
            Point(saltleaf, "forage", "Harvest Pepperbell", new Vector2(6, -3.4f), "pepperbell", "pepper-south", "pepperbell");
            Point(saltleaf, "forage", "Gather Lanternroot", new Vector2(-9, 5.3f), "lanternroot", "root-grove", "lanternroot");
            Point(saltleaf, "hunt", "Observe Brothback", new Vector2(8, 4), "brothback", "broth-spring", "brothback");
            Point(saltleaf, "boat", "Visit the skiff", new Vector2(-11, -7.2f), "", "", "boat");
            Point(saltleaf, "upgrades", "Harbor workshop", new Vector2(-3, -2), "", "", "crate");
            for (int i = 0; i < 3; i++)
            {
                var plot = Point(saltleaf, "crop", "Tend the garden", new Vector2(-10 + i * 2, -.1f), "", "", "plot"); plot.index = i;
                var spr = Add(plot.transform, "pepperbell", new Vector2(0, .3f), 1100); cropArt.Add(spr);
            }
            Add(mistwake, "boat", new Vector2(-11, -7.2f), 1300);
            Point(mistwake, "boat", "Sail home", new Vector2(-11, -6.5f));
            Point(mistwake, "story", "Read the tidekeeper's letter", new Vector2(-6, -2), "", "", "crate");
            Point(mistwake, "fruit", "Reach the floating Cloudfruit", new Vector2(0, 2), "cloudfruit", "fruit-mist", "cloudfruit");
            Point(mistwake, "fruit", "Reach the floating Cloudfruit", new Vector2(4, -.5f), "cloudfruit", "fruit-east", "cloudfruit");
            Point(mistwake, "forage", "Gather Lanternroot", new Vector2(-9, 5), "lanternroot", "root-mist", "lanternroot");
            Point(mistwake, "fish", "Fish the mist shallows", new Vector2(8, -8.6f), "leafgill", "", "leafgill");
            Border(restaurant, 9.8f, 5.45f);
            Point(restaurant, "exit", "Return to the shore", new Vector2(0, -5));
            Point(restaurant, "pantry", "Store ingredients", new Vector2(-8, 2.7f), "", "", "crate");
            Point(restaurant, "kitchen", "Use the kitchen", new Vector2(0, 3.5f), "", "", "stove");
            Point(restaurant, "menu", "Plan tonight's menu", new Vector2(-4, 3.2f), "", "", "table");
            Point(restaurant, "service", "Open the restaurant", new Vector2(5, 3.2f), "", "", "table");
            Point(restaurant, "bed", "Close the day", new Vector2(8, 3), "", "", "crate");
            Point(restaurant, "requests", "Harbor requests", new Vector2(-8, -3), "", "", "crate");
            for (int i = 0; i < 5; i++)
            {
                Transform parent = restaurant;
                if (i == 3) { terrace = new GameObject("RestoredTerrace"); terrace.transform.SetParent(restaurant); }
                if (i >= 3) parent = terrace.transform;
                var pos = new Vector2(-6 + i * 3, -.5f);
                Add(parent, "table", pos, 1000);
                var guest = Point(parent, "serve", "Serve this guest", pos + new Vector2(0, -1), "", "", "guest-"+i); guest.index = i; guests.Add(guest.transform);
            }
            employee = Add(restaurant, "nori", new Vector2(2, 3), 1050).gameObject;
            var chef = new GameObject("Chef", typeof(SpriteRenderer), typeof(Rigidbody2D), typeof(CapsuleCollider2D)); player = chef.transform; player.SetParent(transform);
            playerArt = chef.GetComponent<SpriteRenderer>(); playerArt.sprite = Art("player-0");
            carriedDish=Add(player,"dish-fish",new Vector2(.4f,.4f),1500);carriedDish.transform.localScale=Vector3.one*.6f;carriedDish.sprite=null;
            var body = chef.GetComponent<Rigidbody2D>(); body.gravityScale = 0; body.freezeRotation = true; body.interpolation = RigidbodyInterpolation2D.None;
            var collider = chef.GetComponent<CapsuleCollider2D>(); collider.size = new Vector2(.45f, .35f); collider.offset = new Vector2(0, .15f);
            SetArea(0, new Vector2(-6, -2.5f));
        }
        public static SpriteRenderer Add(Transform parent, string sprite, Vector2 position, int order, bool center = false)
        {
            var go = new GameObject(sprite, typeof(SpriteRenderer)); go.transform.SetParent(parent, false); go.transform.localPosition = position;
            var sr = go.GetComponent<SpriteRenderer>(); sr.sprite = Art(sprite); sr.sortingOrder = order;
            if (center && sr.sprite) go.transform.localPosition -= new Vector3(0, sr.sprite.bounds.size.y / 2, 0);
            return sr;
        }
        WorldPoint Point(Transform parent, string action, string label, Vector2 pos, string item = "", string source = "", string art = "")
        {
            var go = new GameObject(label, typeof(WorldPoint)); go.transform.SetParent(parent, false); go.transform.localPosition = pos;
            var p = go.GetComponent<WorldPoint>(); p.action = action; p.label = label; p.item = item; p.source = source;
            if (art != "") p.artwork = Add(go.transform, art, Vector2.zero, 1000 - Mathf.RoundToInt(pos.y * 32));
            points.Add(p); return p;
        }
        static void Block(Transform parent, Vector2 size, Vector2 offset)
        { var go = new GameObject("Collision", typeof(BoxCollider2D)); go.transform.SetParent(parent, false); go.transform.localPosition = offset; go.GetComponent<BoxCollider2D>().size = size; }
        static void Border(Transform parent, float x, float y)
        {
            Block(parent, new Vector2(x * 2, 1), new Vector2(0, y)); Block(parent, new Vector2(x * 2, 1), new Vector2(0, -y));
            Block(parent, new Vector2(1, y * 2), new Vector2(x, 0)); Block(parent, new Vector2(1, y * 2), new Vector2(-x, 0));
        }
    }
}
