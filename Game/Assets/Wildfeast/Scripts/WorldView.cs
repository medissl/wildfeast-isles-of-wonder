using System.Collections.Generic;
using System.Linq;
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
        Sprite[][] directional;
        SpriteRenderer heldTool, bobber, staffDish;
        LineRenderer fishingLine;
        Vector2 castPoint;
        Vector2 facing=Vector2.down;
        readonly Vector2[] seats={new Vector2(-6,-1.5f),new Vector2(-3,-1.5f),new Vector2(0,-1.5f),new Vector2(3,-1.5f),new Vector2(6,-1.5f)};
        readonly List<SpriteRenderer> orderIcons=new List<SpriteRenderer>();
        readonly List<WorldMotion> ambience=new List<WorldMotion>();
        bool[] guestLeaving=new bool[5];
        bool[] guestPresent=new bool[5];
        float[] guestDelay=new float[5];
        string previousPhase;
        float burstCooldown;
        WorldPoint beast;
        Vector2 beastHome;
        public static Sprite Art(string name) => Resources.Load<Sprite>("Art/" + name);
        public void Init()
        {
            chefFrames = new Sprite[4]; for (int i = 0; i < 4; i++) chefFrames[i] = Art("player-" + i);
            directional=new Sprite[4][];
            string[] directions={"down","up","left","right"};
            for(int d=0;d<4;d++){directional[d]=new Sprite[4];for(int f=0;f<4;f++)directional[d][f]=Art("chef-"+directions[d]+"-"+f);}
            heldTool=Add(player,"tool-rod",new Vector2(.38f,.35f),1500);heldTool.gameObject.name="Held tool";
            bobber=Add(transform,"bobber",Vector2.zero,1500);bobber.gameObject.SetActive(false);
            var lineObject=new GameObject("Fishing line",typeof(LineRenderer));lineObject.transform.SetParent(transform);
            fishingLine=lineObject.GetComponent<LineRenderer>();fishingLine.material=new Material(Shader.Find("Sprites/Default"));fishingLine.startColor=fishingLine.endColor=new Color(.95f,.93f,.72f,.85f);fishingLine.startWidth=fishingLine.endWidth=.025f;fishingLine.positionCount=3;fishingLine.sortingOrder=1600;fishingLine.enabled=false;
            staffDish=Add(employee.transform,"dish-fish",new Vector2(.4f,.45f),1500);staffDish.transform.localScale=Vector3.one*.6f;staffDish.sprite=null;
            for(int i=0;i<5;i++){var icon=Add(guests[i],"dish-fish",new Vector2(0,1.65f),1600);icon.transform.localScale=Vector3.one*.65f;orderIcons.Add(icon);}
            ambience.AddRange(GetComponentsInChildren<WorldMotion>(true));
            beast=points.First(p=>p.action=="hunt");beastHome=beast.transform.position;
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
            if(motion.sqrMagnitude>.01f)facing=motion.normalized;
            int direction=Mathf.Abs(facing.x)>Mathf.Abs(facing.y)?facing.x<0?2:3:facing.y>0?1:0;
            playerArt.sprite = directional[direction][motion.sqrMagnitude > .01f ? (int)(Time.time * 9) % 4 : 0]??chefFrames[0];playerArt.flipX=false;
            playerArt.sortingOrder = 1000 - Mathf.RoundToInt(player.position.y * 32);
            if(carriedDish)carriedDish.sortingOrder=playerArt.sortingOrder+2;
            foreach (var p in points)
            {
                if (p.artwork && p.action == "fruit") p.artwork.transform.localPosition = new Vector3(0, .5f + Mathf.Round(Mathf.Sin(Time.time * 1.5f) * 4) / 32f, 0);
            }
            for(int i=0;i<5;i++)if(guestPresent[i]&&guests[i].gameObject.activeSelf)
            {
                guestDelay[i]-=Time.deltaTime;
                Vector2 destination=guestLeaving[i]?new Vector2(0,-5.1f):seats[i];
                if(guestDelay[i]<=0)guests[i].position=Vector3.MoveTowards(guests[i].position,destination,2.4f*Time.deltaTime);
                var sr=guests[i].GetComponentInChildren<SpriteRenderer>();sr.sortingOrder=1000-Mathf.RoundToInt(guests[i].position.y*32);
                sr.sprite=Art("visitor-"+i+"-"+(Vector2.Distance(guests[i].position,destination)>.1f?((int)(Time.time*8)%4):0));
                if(guestLeaving[i]&&Vector2.Distance(guests[i].position,destination)<.08f){guests[i].gameObject.SetActive(false);guestPresent[i]=false;}
            }
            var staff=employee.GetComponent<SpriteRenderer>();staff.sortingOrder=1000-Mathf.RoundToInt(employee.transform.position.y*32);
        }
        void LateUpdate()
        {
            if(chefFrames==null)return;
            Vector3 target = new Vector3(player.position.x, player.position.y + 1, -10);
            if (Area == 2) target = new Vector3(0, 0, -10);
            else {float halfX=worldCamera.orthographicSize*worldCamera.aspect;target.x=Mathf.Clamp(target.x,-Mathf.Max(0,20-halfX),Mathf.Max(0,20-halfX));target.y=Mathf.Clamp(target.y,-Mathf.Max(0,13-worldCamera.orthographicSize),Mathf.Max(0,13-worldCamera.orthographicSize));}
            var cameraPosition = Vector3.Lerp(worldCamera.transform.position, target, 1 - Mathf.Exp(-8 * Time.deltaTime));
            cameraPosition.x = Mathf.Round(cameraPosition.x * 32) / 32f; cameraPosition.y = Mathf.Round(cameraPosition.y * 32) / 32f;
            worldCamera.transform.position = cameraPosition;
        }
        public void Refresh(GameModel model)
        {
            terrace.SetActive(model.Has("room")); employee.SetActive(model.Has("staff"));
            bool newService=model.State.phase=="service"&&previousPhase!="service";
            for(int i=0;i<5;i++)
            {
                var order=model.State.orders.Find(o=>o.number==i);
                if(model.State.phase=="service"&&order!=null&&!order.paid)
                {
                    if(!guestPresent[i]||newService){guests[i].position=new Vector2(0,-5);guestDelay[i]=i*.55f;guestPresent[i]=true;guestLeaving[i]=false;}
                    guests[i].gameObject.SetActive(true);
                }
                else if(guestPresent[i])guestLeaving[i]=true;
                else guests[i].gameObject.SetActive(false);
                if(orderIcons.Count>i){orderIcons[i].sprite=order==null||order.paid?null:Art(model.Data.Dish(order.recipe).icon);orderIcons[i].color=order!=null&&order.cooked?new Color(.8f,1,.7f):Color.white;}
            }
            for (int i = 0; i < cropArt.Count; i++)
            {
                var crop = model.State.crops[i]; cropArt[i].sprite = crop.planted ? Art(crop.growth>=model.Data.cropDays?crop.item:crop.growth>0?"sprout":"planted-seed") : null;
                cropArt[i].color=Color.white;cropArt[i].transform.localScale=Vector3.one;
                points.Find(p=>p.action=="crop"&&p.index==i).artwork.sprite=Art(crop.wateredDay==model.State.day?"plot-wet":"plot");
            }
            foreach (var p in points) if (p.artwork && (p.action == "forage" || p.action == "hunt" || p.action == "fruit"))
                p.artwork.color = model.State.harvested.Contains(p.source) ? new Color(.55f, .6f, .48f, .45f) : Color.white;
            foreach(var p in points) if(p.action=="serve")
            {
                var order=model.State.orders.Find(o=>o.number==p.index);
                p.label=order==null?$"Table {p.index+1}":$"Table {p.index+1} · {model.Data.Dish(order.recipe).name}"+(order.cooked?" — dish ready":" — cook at the stove");
            }
            var ready=model.State.orders.Find(o=>o.cooked&&!o.paid);
            carriedDish.sprite=ready!=null&&!model.Has("staff")?Art(model.Data.Dish(ready.recipe).icon):null;
            foreach(var p in points)if(p.action=="service"&&p.artwork)p.artwork.sprite=Art(model.State.phase=="service"?"sign-open":"sign-closed");
            previousPhase=model.State.phase;
        }
        public Vector2 GuestSeat(int index)=>seats[index];
        public void StaffDish(string icon){if(staffDish)staffDish.sprite=icon==null?null:Art(icon);}
        public void Tool(int slot,Vector2 direction,bool usingTool,string activity)
        {
            if(!heldTool)return;
            string[] icons={null,"tool-rod","tool-can","seed-pepper","seed-root","tool-knife"};
            heldTool.sprite=slot<6&&icons[slot]!=null?Art(icons[slot]):null;
            heldTool.transform.localPosition=new Vector3(direction.x>=0?.43f:-.43f,.38f,0);
            heldTool.flipX=direction.x<0;heldTool.sortingOrder=playerArt.sortingOrder+1;
            heldTool.transform.localRotation=Quaternion.Euler(0,0,activity=="fish"?-25:usingTool?Mathf.Sin(Time.time*20)*30:0);
        }
        public void Roam()
        {
            if(!beast||Area!=0)return;
            beast.transform.position=beastHome+new Vector2(Mathf.Sin(Time.time*.6f)*.55f,Mathf.Sin(Time.time*.4f)*.15f);
            beast.artwork.sprite=Art("brothback-"+((int)(Time.time*3)%4));beast.artwork.flipX=Mathf.Cos(Time.time*.6f)<0;
        }
        public void Cast(Vector2 target){castPoint=target;bobber.gameObject.SetActive(true);bobber.transform.position=target;fishingLine.enabled=true;Burst(target,"splash");}
        public void Fishing(float time,float tension,float progress)
        {
            var end=(Vector3)castPoint+new Vector3(Mathf.Sin(time*4)*.1f,Mathf.Sin(time*5)*.04f,0);
            end=Vector3.Lerp(player.position+Vector3.up*.8f,end,Mathf.Clamp01(time/.4f));
            bobber.transform.position=end;bobber.color=time>1.5f&&Mathf.Sin(time*9)>0?new Color(1,.66f,.34f):Color.white;
            var start=player.position+new Vector3(facing.x>=0?.65f:-.65f,1.28f,0);fishingLine.SetPositions(new[]{start,(start+end)*.5f+Vector3.up*(.12f+(1-tension)*.45f),end});
        }
        public void CancelCast(){if(bobber)bobber.gameObject.SetActive(false);if(fishingLine)fishingLine.enabled=false;}
        public void LandFish(string item){var fish=Add(transform,item,castPoint,1800);var motion=fish.gameObject.AddComponent<WorldMotion>();motion.mode=6;motion.destination=player.position+Vector3.up*.7f;Destroy(fish.gameObject,.7f);Burst(player.position+Vector3.up*.8f,"spark",0);}
        public void Burst(Vector3 position,string sprite,float cooldown=0)
        {
            if(cooldown>0&&Time.time<burstCooldown)return;if(cooldown>0)burstCooldown=Time.time+cooldown;
            for(int i=0;i<(sprite=="steam"?1:5);i++){var sr=Add(transform,sprite,position+(Vector3)new Vector2(Random.Range(-.25f,.25f),Random.Range(0,.3f)),1800);sr.transform.localScale=Vector3.one*(sprite=="steam"?1:.45f);var motion=sr.gameObject.AddComponent<WorldMotion>();motion.mode=3;motion.speed=.7f+i*.1f;Destroy(sr.gameObject,.8f);}
        }
        // Shoreline polygon is shared with the authored collision and terrain generator.
        static readonly Vector2[] pixels={new Vector2(90,500),new Vector2(80,300),new Vector2(140,185),new Vector2(330,130),new Vector2(470,72),new Vector2(650,78),new Vector2(780,110),new Vector2(990,100),new Vector2(1140,230),new Vector2(1180,370),new Vector2(1140,590),new Vector2(1050,655),new Vector2(850,706),new Vector2(590,735),new Vector2(410,735),new Vector2(280,690),new Vector2(175,630)};
        public static Vector2[] Coast()=>pixels.Select(v=>new Vector2((v.x+(640-v.x)/12-640)/32,(416-v.y-(420-v.y)/12)/32)).ToArray();
        public static bool Water(Vector2 position)
        {
            if(((position.x-10.3f)/3.35f)*((position.x-10.3f)/3.35f)+((position.y-5.95f)/1.9f)*((position.y-5.95f)/1.9f)<1)return true;
            var polygon=Coast();bool inside=false;
            for(int i=0,j=polygon.Length-1;i<polygon.Length;j=i++)if((polygon[i].y>position.y)!=(polygon[j].y>position.y)&&position.x<(polygon[j].x-polygon[i].x)*(position.y-polygon[i].y)/(polygon[j].y-polygon[i].y)+polygon[i].x)inside=!inside;
            return !inside;
        }
        public Vector2? WaterAt(Vector2 position,Vector2 direction)
        {
            if(Area==2)return null;
            // Aim towards nearby water; a shoreline cast also works without precise facing.
            for(int k=0;k<16;k++){var aim=k==0?direction:new Vector2(Mathf.Cos(k*Mathf.PI/8),Mathf.Sin(k*Mathf.PI/8));for(float d=.6f;d<=2.3f;d+=.3f){var test=position+aim*d;if(Water(test))return test+aim*.55f;}}
            return null;
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
                    var tree = Add(parent, second ? "tree-blue" : i%4==0?"tree-purple":i%3==0?"tree-gold":"tree", new Vector2(x, y), 1000 - Mathf.RoundToInt(y * 32));
                    Block(tree.transform, new Vector2(.38f, .4f), new Vector2(0, .15f));
                }
                for (int i = 0; i < 130; i++)
                {
                    float x = (float)random.NextDouble() * 28 - 14, y = (float)random.NextDouble() * 15 - 7;
                    if (Mathf.Abs(x + 6) < 3 && Mathf.Abs(y) < 4) continue;
                    if(Water(new Vector2(x,y)))continue;
                    var plant=Add(parent, i%7==0?"fern":i%4==0?"mushrooms":i%3==0?"flower-purple":"flower", new Vector2(x, y), 950-Mathf.RoundToInt(y*32));
                    plant.gameObject.AddComponent<WorldMotion>().phase=i;
                }
                var pond=new GameObject("Spring bank",typeof(PolygonCollider2D));pond.transform.SetParent(parent,false);var edge=pond.GetComponent<PolygonCollider2D>();var circle=new Vector2[24];for(int n=0;n<24;n++)circle[n]=new Vector2(10.3f+Mathf.Cos(n*Mathf.PI/12)*3.35f,5.95f+Mathf.Sin(n*Mathf.PI/12)*1.9f);edge.points=circle;
                var coast=new GameObject("Shoreline",typeof(EdgeCollider2D));coast.transform.SetParent(parent,false);coast.GetComponent<EdgeCollider2D>().points=Coast().Concat(new[]{Coast()[0]}).ToArray();
                for(int n=0;n<14;n++)
                {
                    Vector2 water=new Vector2(-4+n*1.5f,-10.5f+(n%3)*.25f);
                    var fish=Add(parent,"fish-shadow",water,-1500);var m=fish.gameObject.AddComponent<WorldMotion>();m.mode=1;m.radius=.6f;m.phase=n;m.speed=.7f;
                    var ripple=Add(parent,"ripple-0",water,-1499);ripple.gameObject.AddComponent<WorldMotion>().mode=5;
                }
                for(int n=0;n<12;n++){var firefly=Add(parent,n%2==0?"butterfly":"spark",new Vector2(-12+n*2,2+n%3),1450);var m=firefly.gameObject.AddComponent<WorldMotion>();m.mode=1;m.radius=.55f;m.phase=n;m.speed=1.1f;firefly.transform.localScale=Vector3.one*.65f;}
                Decorate(parent,second);
            }
            var home = Add(saltleaf, "restaurant", new Vector2(-6, -.9f), 1030);
            Block(home.transform, new Vector2(4.8f, 2.7f), new Vector2(0, 2));
            Point(saltleaf, "enter", "Enter the restaurant", new Vector2(-6, -1.15f));
            Point(saltleaf, "service", "Open the restaurant", new Vector2(-3.6f,-1.2f),"","","sign-closed");
            Point(saltleaf, "fish", "Cast at the shore", new Vector2(8, -8.6f), "leafgill");
            Point(saltleaf, "forage", "Harvest Pepperbell", new Vector2(4.5f, -1.3f), "pepperbell", "pepper-east", "pepperbell");
            Point(saltleaf, "forage", "Harvest Pepperbell", new Vector2(6, -3.4f), "pepperbell", "pepper-south", "pepperbell");
            Point(saltleaf, "forage", "Gather Lanternroot", new Vector2(-9, 5.3f), "lanternroot", "root-grove", "lanternroot");
            Point(saltleaf, "hunt", "Observe Brothback", new Vector2(8, 4), "brothback", "broth-spring", "brothback");
            Point(saltleaf, "boat", "Visit the skiff", new Vector2(-11, -7.2f), "", "", "boat");
            Point(saltleaf, "upgrades", "Visit the workshop", new Vector2(-2, -3.8f), "", "", "workshop");
            for (int i = 0; i < 3; i++)
            {
                var plot = Point(saltleaf, "crop", "Garden bed", new Vector2(-11 + i * 2, -4f), "", "", "plot"); plot.index = i;
                var spr = Add(plot.transform, "pepperbell", new Vector2(0, .3f), 1100); cropArt.Add(spr);
            }
            Add(mistwake, "boat", new Vector2(-11, -7.2f), 1300);
            Point(mistwake, "boat", "Sail home", new Vector2(-11, -6.5f));
            Add(mistwake,"iona-house",new Vector2(-6,-.9f),1030);
            Point(mistwake, "story", "Read Iona's letter", new Vector2(-6, -2), "", "", "mailbox");
            Point(mistwake, "fruit", "Reach the floating Cloudfruit", new Vector2(0, 2), "cloudfruit", "fruit-mist", "cloudfruit");
            Point(mistwake, "fruit", "Reach the floating Cloudfruit", new Vector2(4, -.5f), "cloudfruit", "fruit-east", "cloudfruit");
            Point(mistwake, "forage", "Gather Lanternroot", new Vector2(-9, 5), "lanternroot", "root-mist", "lanternroot");
            Point(mistwake, "fish", "Cast at the shore", new Vector2(8, -8.6f), "leafgill");
            Border(restaurant, 9.8f, 5.45f);
            Point(restaurant, "exit", "Return to the shore", new Vector2(0, -5));
            Point(restaurant, "pantry", "Pantry", new Vector2(-8, 2.7f), "", "", "pantry");
            Point(restaurant, "kitchen", "Use the kitchen", new Vector2(0, 3.5f), "", "", "stove");
            Point(restaurant, "menu", "Menu board", new Vector2(-4, 3.2f), "", "", "menu-board");
            Point(restaurant, "service", "Open the restaurant", new Vector2(5, 3.2f), "", "", "sign-closed");
            Point(restaurant, "bed", "Rest until morning", new Vector2(8, 3), "", "", "bed");
            Point(restaurant, "requests", "Harbor letters", new Vector2(-8, -3), "", "", "mailbox");
            for (int i = 0; i < 5; i++)
            {
                Transform parent = restaurant;
                if (i == 3) { terrace = new GameObject("RestoredTerrace"); terrace.transform.SetParent(restaurant); }
                if (i >= 3) parent = terrace.transform;
                var pos = new Vector2(-6 + i * 3, -.5f);
                Add(parent, "table", pos, 1000);
                Add(parent,"table-number-"+i,pos+new Vector2(-.65f,.55f),1050);
                var guest = Point(parent, "serve", "Serve this guest", pos + new Vector2(0, -1), "", "", "guest-"+i); guest.index = i; guests.Add(guest.transform);
            }
            employee = Add(restaurant, "nori", new Vector2(2, 3), 1050).gameObject;
            for(int n=0;n<4;n++){Add(restaurant,"hanging-herbs",new Vector2(-7+n*4,4.4f),900);var lamp=Add(restaurant,"lantern",new Vector2(-8+n*5,4.3f),1100);lamp.gameObject.AddComponent<WorldMotion>().mode=2;}
            var chef = new GameObject("Chef", typeof(SpriteRenderer), typeof(Rigidbody2D), typeof(CapsuleCollider2D)); player = chef.transform; player.SetParent(transform);
            playerArt = chef.GetComponent<SpriteRenderer>(); playerArt.sprite = Art("player-0");
            carriedDish=Add(player,"dish-fish",new Vector2(.4f,.4f),1500);carriedDish.transform.localScale=Vector3.one*.6f;carriedDish.sprite=null;
            var body = chef.GetComponent<Rigidbody2D>(); body.gravityScale = 0; body.freezeRotation = true; body.interpolation = RigidbodyInterpolation2D.None;
            var collider = chef.GetComponent<CapsuleCollider2D>(); collider.size = new Vector2(.45f, .35f); collider.offset = new Vector2(0, .15f);
            SetArea(0, new Vector2(-6, -2.5f));
        }
        void Decorate(Transform parent,bool second)
        {
            // Planted borders leave the restaurant doorway, garden beds and main paths legible.
            for(int row=0;row<3;row++)for(int n=0;n<9;n++)
            {
                Vector2 pos=new Vector2(-13+n*1.12f,2.8f+row*1.2f);
                if(Vector2.Distance(pos,new Vector2(-9,5.3f))<1.3f)continue;
                var p=Add(parent,second?"crystal-plant":row%2==0?"flower-purple":"fern",pos,1000-Mathf.RoundToInt(pos.y*32));p.gameObject.AddComponent<WorldMotion>().phase=n+row;
            }
            for(int n=0;n<15;n++){var pos=new Vector2(-13+n*1.8f,8.4f);Add(parent,n%3==0?"tree-purple":second?"tree-blue":"tree",pos,730);}
            Add(parent,"arch",new Vector2(-6.5f,3.6f),880);Add(parent,"fountain",new Vector2(1.3f,1.4f),955);
            Add(parent,"mushroom-house",new Vector2(4,-4.8f),1120);
            Add(parent,"bench",new Vector2(1,-3.7f),1120);
            for(int n=0;n<8;n++)
            {
                var pos=new Vector2(-12+n*3.3f,-5.8f);var lamp=Add(parent,"lantern",pos,1000-Mathf.RoundToInt(pos.y*32));lamp.gameObject.AddComponent<WorldMotion>().mode=2;
                Add(parent,"flower-purple",pos+Vector2.right*.45f,1000-Mathf.RoundToInt(pos.y*32));
            }
            for(int n=0;n<3;n++){var npc=Add(parent,"guest-"+n,new Vector2(1+n*2,-2+n*.8f),1100);var m=npc.gameObject.AddComponent<WorldMotion>();m.mode=4;m.phase=n;m.radius=.9f;m.speed=.4f;}
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
