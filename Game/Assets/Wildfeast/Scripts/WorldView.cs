using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Wildfeast
{
    public class WorldView : MonoBehaviour
    {
        public Transform saltleaf, mistwake, restaurant;
        public Transform[] islands;
        public Transform IslandRoot(int area)=>islands!=null&&islands.Length==5?islands[System.Array.FindIndex(Archipelago.Islands,i=>i.id==area)]:area==0?saltleaf:mistwake;
        public Transform player;
        public Camera worldCamera;
        public SpriteRenderer playerArt;
        public SpriteRenderer carriedDish;
        public GameObject terrace, employee;
        public List<WorldPoint> points = new List<WorldPoint>();
        public List<Transform> guests = new List<Transform>();
        public List<SpriteRenderer> cropArt = new List<SpriteRenderer>();
        public int Area { get; private set; }
        public bool FollowSea;
        CharacterLook characterLook;
        public void Appearance(CharacterProfile profile){characterLook?.Dispose();characterLook=new CharacterLook(profile);Animate(Vector2.zero);}
        void OnDestroy(){characterLook?.Dispose();}
        Sprite[] chefFrames;
        Sprite[][] directional;
        readonly Dictionary<string,Sprite[][]> actionFrames=new Dictionary<string,Sprite[][]>();
        public string PlayerPose {get;private set;}="idle";
        SpriteRenderer heldTool, bobber, staffDish;
        LineRenderer fishingLine;
        Vector2 castPoint;
        Vector2 facing=Vector2.down;
        readonly Vector2[] seats={new Vector2(-5,-.9f),new Vector2(0,-.9f),new Vector2(5,-.9f),new Vector2(-4,-3.3f),new Vector2(4,-3.3f)};
        readonly List<SpriteRenderer> orderIcons=new List<SpriteRenderer>();
        readonly List<WorldMotion> ambience=new List<WorldMotion>();
        bool[] guestLeaving=new bool[5];
        bool[] guestPresent=new bool[5];
        float[] guestDelay=new float[5];
        string previousPhase;
        float burstCooldown;
        Vector3 lastStaffPosition;
        readonly List<GameObject> transientEffects=new List<GameObject>();
        readonly int[] guestWaypoint=new int[5];
        Vector2[][] guestArrivals,guestDepartures;
        WorldPoint beast;
        Vector2 beastHome;
        public static Sprite Art(string name) => Resources.Load<Sprite>("Art/" + (name=="tool-scythe"?"icon-scythe":name=="tool-pickaxe"?"icon-pickaxe":name));
        public void Init()
        {
            chefFrames = new Sprite[4]; for (int i = 0; i < 4; i++) chefFrames[i] = Art("player-" + i);
            directional=new Sprite[4][];
            string[] directions={"down","up","left","right"};
            for(int d=0;d<4;d++){directional[d]=new Sprite[4];for(int f=0;f<4;f++)directional[d][f]=Art("chef-"+directions[d]+"-"+f);}
            foreach(string kind in new[]{"swing","pour","plant","pull","cast","stir"})
            {var frames=new Sprite[4][];for(int d=0;d<4;d++){frames[d]=new Sprite[5];for(int f=0;f<5;f++)frames[d][f]=Art("action-"+kind+"-"+directions[d]+"-"+f);}actionFrames.Add(kind,frames);}
            heldTool=Add(player,"tool-rod",new Vector2(.38f,.35f),1500);heldTool.gameObject.name="Held tool";
            bobber=Add(transform,"bobber",Vector2.zero,1500);bobber.gameObject.SetActive(false);
            var lineObject=new GameObject("Fishing line",typeof(LineRenderer));lineObject.transform.SetParent(transform);
            fishingLine=lineObject.GetComponent<LineRenderer>();fishingLine.material=new Material(Shader.Find("Sprites/Default"));fishingLine.startColor=fishingLine.endColor=new Color(.95f,.93f,.72f,.85f);fishingLine.startWidth=fishingLine.endWidth=.025f;fishingLine.positionCount=3;fishingLine.sortingOrder=1600;fishingLine.enabled=false;
            staffDish=Add(employee.transform,"held-dish-fish",new Vector2(.3f,.55f),1500);staffDish.transform.localScale=Vector3.one;staffDish.sprite=null;
            for(int i=0;i<5;i++){var icon=Add(guests[i],"held-dish-fish",new Vector2(0,1.5f),1600);icon.transform.localScale=Vector3.one;orderIcons.Add(icon);}
            ambience.AddRange(GetComponentsInChildren<WorldMotion>(true));
            guestArrivals=new Vector2[5][];guestDepartures=new Vector2[5][];for(int i=0;i<5;i++){guestArrivals[i]=GuestRoute(i);guestDepartures[i]=GuestRoute(i,true);}
            beast=points.First(p=>p.action=="hunt");beastHome=beast.transform.position;
        }
        public void SetArea(int area, Vector2 position)
        {
            foreach(var effect in transientEffects)if(effect)Destroy(effect);transientEffects.Clear();
            Area = area;
            if(islands!=null&&islands.Length==5)for(int i=0;i<islands.Length;i++)islands[i].gameObject.SetActive(Archipelago.Islands[i].id==area);
            else {saltleaf.gameObject.SetActive(area==0);mistwake.gameObject.SetActive(area==1);}
            restaurant.gameObject.SetActive(area==2);
            player.position = position; player.GetComponent<Rigidbody2D>().position = position;
            worldCamera.transform.position = area==2?new Vector3(0,0,-10):new Vector3(position.x,position.y+1,-10);
        }
        public WorldPoint Nearest()
        {
            WorldPoint result = null; float distance = 2.4f;
            foreach (var p in points) if (p && p.gameObject.activeInHierarchy)
            {
                if(p.artwork&&!p.artwork.enabled&&(p.action=="forage"||p.action=="fruit"))continue;
                float d = Vector2.Distance(player.position, p.transform.position);
                float reach=p.GetComponent<FoodEcology>()?2.4f:1.6f;
                if (d < distance&&d<reach) { distance = d; result = p; }
            }
            return result;
        }
        public WorldPoint PointAt(Vector2 cursor)
        {
            return points.Where(p=>p&&p.gameObject.activeInHierarchy&&Vector2.Distance(player.position,p.transform.position)<=(p.GetComponent<FoodEcology>()?2.4f:2)&&(Vector2.Distance(cursor,p.transform.position)<.9f||(p.artwork&&p.artwork.bounds.Contains(new Vector3(cursor.x,cursor.y,p.artwork.transform.position.z))))&&(!p.artwork||p.artwork.enabled)).OrderBy(p=>Vector2.Distance(cursor,p.transform.position)).FirstOrDefault();
        }
        public void Face(Vector2 direction){if(direction.sqrMagnitude>.01f)facing=direction.normalized;}
        public void Animate(Vector2 motion)
        {
            if(motion.sqrMagnitude>.01f)facing=motion.normalized;
            int direction=Mathf.Abs(facing.x)>Mathf.Abs(facing.y)?facing.x<0?2:3:facing.y>0?1:0;
            playerArt.sprite = directional[direction][motion.sqrMagnitude > .01f ? (int)(Time.time * 9) % 4 : 0]??chefFrames[0];if(characterLook!=null)playerArt.sprite=characterLook.Apply(playerArt.sprite);playerArt.flipX=false;
            playerArt.sortingOrder = 1000 - Mathf.RoundToInt(player.position.y * 32);
            if(carriedDish)carriedDish.sortingOrder=playerArt.sortingOrder+2;
            foreach (var p in points)
            {
                if (p.artwork && p.action == "fruit") p.artwork.transform.localPosition = new Vector3(0, .5f + Mathf.Round(Mathf.Sin(Time.time * 1.5f) * 4) / 32f, 0);
            }
            for(int i=0;i<5;i++)if(guestPresent[i]&&guests[i].gameObject.activeSelf)
            {
                guestDelay[i]-=Time.deltaTime;
                var route=GuestRoute(i,guestLeaving[i]);Vector2 destination=route[guestWaypoint[i]];
                if(guestDelay[i]<=0)guests[i].position=Vector3.MoveTowards(guests[i].position,destination,2.4f*Time.deltaTime);
                var sr=guests[i].GetComponentInChildren<SpriteRenderer>();sr.sortingOrder=1000-Mathf.RoundToInt(guests[i].position.y*32);
                sr.sprite=Art("visitor-"+i+"-"+(Vector2.Distance(guests[i].position,destination)>.1f?((int)(Time.time*8)%4):0));
                if(Vector2.Distance(guests[i].position,destination)<.08f){if(guestWaypoint[i]<route.Length-1)guestWaypoint[i]++;else if(guestLeaving[i]){guests[i].gameObject.SetActive(false);guestPresent[i]=false;}}
                if(orderIcons.Count>i){orderIcons[i].sortingOrder=sr.sortingOrder+3;orderIcons[i].enabled=guests[i].position.y>-4.5f&&guestDelay[i]<=0&&!guestLeaving[i];}
            }
            var staff=employee.GetComponent<SpriteRenderer>();staff.sortingOrder=1000-Mathf.RoundToInt(employee.transform.position.y*32);
            var staffMotion=employee.transform.position-lastStaffPosition;
            staff.sprite=Art("visitor-2-"+(staffMotion.sqrMagnitude>.00001f?(int)(Time.time*8)%4:0));
            if(Mathf.Abs(staffMotion.x)>.001f)staff.flipX=staffMotion.x<0;lastStaffPosition=employee.transform.position;
        }
        void LateUpdate()
        {
            if(chefFrames==null)return;
            Vector3 target = new Vector3(player.position.x, player.position.y + 1, -10);
            if (Area == 2) target = new Vector3(0, 0, -10);
            else if(!FollowSea) {var size=Archipelago.Get(Area).size;float halfX=worldCamera.orthographicSize*worldCamera.aspect;target.x=Mathf.Clamp(target.x,-Mathf.Max(0,size.x/2-halfX),Mathf.Max(0,size.x/2-halfX));target.y=Mathf.Clamp(target.y,-Mathf.Max(0,size.y/2-worldCamera.orthographicSize),Mathf.Max(0,size.y/2-worldCamera.orthographicSize));}
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
                    if(!guestPresent[i]||newService){guests[i].position=new Vector2(0,-5.1f-i*.65f);guestWaypoint[i]=0;guestDelay[i]=i*.55f;guestPresent[i]=true;guestLeaving[i]=false;}
                    guests[i].gameObject.SetActive(true);
                }
                else if(guestPresent[i]&&!guestLeaving[i]){guestLeaving[i]=true;guestWaypoint[i]=0;}
                else guests[i].gameObject.SetActive(false);
                if(orderIcons.Count>i){orderIcons[i].sprite=order==null||order.paid?null:Art("held-"+model.Data.Dish(order.recipe).icon);orderIcons[i].color=order!=null&&order.cooked?new Color(.8f,1,.7f):Color.white;orderIcons[i].enabled=guestPresent[i]&&guests[i].position.y>-4.5f&&guestDelay[i]<=0&&!guestLeaving[i];}
            }
            for (int i = 0; i < cropArt.Count; i++)
            {
                var crop = model.State.crops[i]; cropArt[i].sprite = crop.planted ? Art(crop.growth>=model.Data.cropDays?crop.item:crop.growth>0?"sprout":"planted-seed") : null;
                cropArt[i].color=Color.white;cropArt[i].transform.localScale=Vector3.one;
                points.Find(p=>p.action=="crop"&&p.index==i).artwork.sprite=Art(crop.wateredDay==model.State.day?"plot-wet":"plot");
            }
            foreach (var p in points) if (p.artwork && (p.action == "forage" || p.action == "hunt" || p.action == "fruit"))
                {p.artwork.enabled=p.action=="hunt"||!model.State.harvested.Contains(p.source);p.artwork.color=Color.white;}
            foreach(var p in points) if(p.action=="serve")
            {
                var order=model.State.orders.Find(o=>o.number==p.index);
                p.label=order==null?$"Table {p.index+1}":$"Table {p.index+1} · {model.Data.Dish(order.recipe).name}"+(order.cooked?" — dish ready":" — cook at the stove");
            }
            var ready=model.State.orders.Find(o=>o.cooked&&!o.paid);
            carriedDish.sprite=ready!=null&&!model.Has("staff")?Art("held-"+model.Data.Dish(ready.recipe).icon):null;
            foreach(var p in points)if(p.action=="service"&&p.artwork)p.artwork.sprite=Art(model.State.phase=="service"?"sign-open":"sign-closed");
            previousPhase=model.State.phase;
        }
        public Vector2 GuestSeat(int index)=>seats[index];
        public Vector2[] GuestRoute(int index,bool leaving=false)
        {
            var cached=leaving?guestDepartures:guestArrivals;if(cached!=null&&cached[index]!=null)return cached[index];
            var seat=seats[index];var path=index<3?new[]{new Vector2(0,-4.6f),new Vector2(0,-1.25f),new Vector2(seat.x,-1.25f),seat}:new[]{new Vector2(0,-4.6f),new Vector2(seat.x,-4.6f),seat};
            return leaving?path.Reverse().Concat(new[]{new Vector2(0,-5.6f)}).ToArray():path;
        }
        public static Vector2 StaffHome=>new Vector2(3.1f,2.3f);
        public Vector2[] StaffRoute(int index,bool returning=false)
        {
            var target=seats[index]+new Vector2(.65f,0);
            var path=index<3?new[]{StaffHome,new Vector2(3.1f,1.2f),new Vector2(2.2f,-1.25f),new Vector2(target.x,-1.25f),target}:new[]{StaffHome,new Vector2(3.1f,1.2f),new Vector2(2.2f,-1.25f),new Vector2(0,-1.25f),new Vector2(0,-4.3f),new Vector2(target.x,-4.3f),target};
            return returning?path.Reverse().ToArray():path;
        }
        public void StaffDish(string icon){if(staffDish){staffDish.sprite=icon==null?null:Art("held-"+icon);staffDish.sortingOrder=employee.GetComponent<SpriteRenderer>().sortingOrder+2;}}
        public void Tool(int slot,Vector2 direction,bool usingTool,string activity,string item=null,float poseProgress=0,int cookStep=0)
        {
            if(!heldTool)return;
            string[] icons=ItemInventory.Tools;
            string id=slot>=0&&slot<10?icons[slot]:item;
            heldTool.sprite=string.IsNullOrEmpty(id)?null:Art(id.StartsWith("tool-")?id.Replace("tool-","held-"):"held-"+id)??Art(id);
            if(carriedDish.sprite)heldTool.sprite=null;
            string kind=activity=="forage"?"pull":activity=="fish"?poseProgress<1?"cast":"pull":activity=="cook"?usingTool?cookStep==0?"swing":cookStep==1?"stir":"plant":null:usingTool?slot==2?"pour":slot==3||slot==4?"plant":slot==0||slot<0?"pull":"swing":null;
            int d=Mathf.Abs(direction.x)>Mathf.Abs(direction.y)?direction.x<0?2:3:direction.y>0?1:0;
            int frame=Mathf.Min(4,Mathf.FloorToInt(poseProgress*5));
            if(activity=="fish"&&poseProgress>=1)frame=1+(int)(Time.time*5)%3;
            PlayerPose=kind??"idle";
            if(kind!=null){playerArt.sprite=actionFrames[kind][d][frame];if(characterLook!=null)playerArt.sprite=characterLook.Apply(playerArt.sprite);if(activity=="forage")heldTool.sprite=null;if(activity=="cook")heldTool.sprite=cookStep==0?Art("held-knife"):null;}
            float side=d==2?-1:1;
            float handY=kind=="swing"||kind=="cast"?new[]{.6f,1.05f,.56f,.43f,.6f}[frame]:kind=="plant"||kind=="pull"?new[]{.56f,.4f,.28f,.64f,.56f}[frame]:.56f;
            heldTool.transform.localPosition=new Vector3(side*.38f,handY,0);
            heldTool.flipX=side<0;heldTool.sortingOrder=playerArt.sortingOrder+(d==1?-1:1);
            float angle=kind=="swing"?new[]{-10f,35f,-65f,-85f,-10f}[frame]:kind=="pour"?-55f:activity=="fish"?-20f:0;
            heldTool.transform.localRotation=Quaternion.Euler(0,0,angle*side);
        }
        public void Roam()
        {
            if(!beast||Area!=0)return;
            beast.transform.position=beastHome+new Vector2(Mathf.Sin(Time.time*.6f)*.55f,Mathf.Sin(Time.time*.4f)*.15f);
            beast.artwork.sprite=Art("brothback-"+((int)(Time.time*3)%4));beast.artwork.flipX=Mathf.Cos(Time.time*.6f)<0;
        }
        public void ApplyZoom(int level)
        {
            if(!(UnityEngine.Rendering.GraphicsSettings.currentRenderPipeline is UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset))return;
            var pixel=worldCamera.GetComponent<UnityEngine.Rendering.Universal.PixelPerfectCamera>();if(!pixel)return;
            int[] widths={800,640,426};int[] heights={450,360,240};
            pixel.refResolutionX=widths[level];pixel.refResolutionY=heights[level];
        }
        public void Cast(Vector2 target){castPoint=target;bobber.gameObject.SetActive(true);bobber.transform.position=target;fishingLine.enabled=true;Burst(target,"splash");}
        public void Fishing(float time,float tension,float progress)
        {
            var end=(Vector3)castPoint+new Vector3(Mathf.Sin(time*4)*.1f,Mathf.Sin(time*5)*.04f,0);
            end=Vector3.Lerp(player.position+Vector3.up*.8f,end,Mathf.Clamp01(time/.4f));
            bobber.transform.position=end;bobber.color=time>1.5f&&Mathf.Sin(time*9)>0?new Color(1,.66f,.34f):Color.white;
            var start=player.position+new Vector3(facing.x>=0?.65f:-.65f,1.28f,0);fishingLine.SetPositions(new[]{start,(start+end)*.5f+Vector3.up*(.12f+(1-tension)*.45f),end});
        }
        public void Retrieve(float amount){var start=player.position+new Vector3(facing.x>=0?.65f:-.65f,1.28f,0);var end=Vector3.Lerp(castPoint,player.position+Vector3.up*.6f,amount);bobber.transform.position=end;fishingLine.SetPositions(new[]{start,(start+end)*.5f+Vector3.up*.12f,end});}
        public void CancelCast(){if(bobber)bobber.gameObject.SetActive(false);if(fishingLine)fishingLine.enabled=false;}
        public void LandFish(string item){var fish=Add(transform,item,castPoint,1800);transientEffects.Add(fish.gameObject);var motion=fish.gameObject.AddComponent<WorldMotion>();motion.mode=6;motion.destination=player.position+Vector3.up*.7f;Destroy(fish.gameObject,.7f);Burst(player.position+Vector3.up*.8f,"spark",0);}
        public void Burst(Vector3 position,string sprite,float cooldown=0)
        {
            if(cooldown>0&&Time.time<burstCooldown)return;if(cooldown>0)burstCooldown=Time.time+cooldown;
            var compact=Art("held-"+sprite);transientEffects.RemoveAll(effect=>!effect);
            for(int i=0;i<(sprite=="steam"||compact?1:5);i++){var sr=Add(transform,sprite,position+(Vector3)new Vector2(Random.Range(-.25f,.25f),Random.Range(0,.3f)),1800);if(compact)sr.sprite=compact;transientEffects.Add(sr.gameObject);var motion=sr.gameObject.AddComponent<WorldMotion>();motion.mode=3;motion.speed=.7f+i*.1f;Destroy(sr.gameObject,.8f);}
        }
        // Shoreline polygon is shared with the authored collision and terrain generator.
        public static Vector2[] Coast(int area=0)=>Archipelago.Get(area).coast;
        public static bool Water(Vector2 position,int area=0)
        {
            var island=Archipelago.Get(area);
            foreach(var pool in Archipelago.Pools(area)){Vector2 p=position-pool.center;if(p.x*p.x/(pool.size.x*pool.size.x)+p.y*p.y/(pool.size.y*pool.size.y)<1)return true;}
            var polygon=island.coast;bool inside=false;
            for(int i=0,j=polygon.Length-1;i<polygon.Length;j=i++)if((polygon[i].y>position.y)!=(polygon[j].y>position.y)&&position.x<(polygon[j].x-polygon[i].x)*(position.y-polygon[i].y)/(polygon[j].y-polygon[i].y)+polygon[i].x)inside=!inside;
            return !inside;
        }
        public Vector2? WaterAt(Vector2 position,Vector2 direction)
        {
            if(Area==2)return null;
            // Aim towards nearby water; a shoreline cast also works without precise facing.
            for(int k=0;k<16;k++){var aim=k==0?direction:new Vector2(Mathf.Cos(k*Mathf.PI/8),Mathf.Sin(k*Mathf.PI/8));for(float d=.6f;d<=2.3f;d+=.3f){var test=position+aim*d;if(Water(test,Area))return test+aim*.55f;}}
            return null;
        }
        // Called by the Editor bootstrap. The authored results are serialized into the scene.
        public void AuthorWorlds()
        {
            Archipelago.Author(this);
            restaurant=new GameObject("HarborRestaurant").transform;restaurant.SetParent(transform,false);
            Add(restaurant,"interior",Vector2.zero,-2000,true);
            Border(restaurant, 9.8f, 5.45f);
            Point(restaurant, "exit", "Return to the shore", new Vector2(0, -5));
            Point(restaurant, "pantry", "Pantry", new Vector2(-8, 2.7f), "", "", "pantry");
            Point(restaurant, "kitchen", "Use the kitchen", new Vector2(0, 2.3f), "", "", "kitchen-worktop");
            Point(restaurant, "menu", "Menu board", new Vector2(-4, 3.2f), "", "", "menu-board");
            Point(restaurant, "service", "Open the restaurant", new Vector2(5, 3.2f), "", "", "sign-closed");
            Point(restaurant, "bed", "Rest until morning", new Vector2(8, 3), "", "", "bed");
            Point(restaurant, "requests", "Harbor letters", new Vector2(-8, -3), "", "", "mailbox");
            for (int i = 0; i < 5; i++)
            {
                Transform parent = restaurant;
                if (i == 3) { terrace = new GameObject("RestoredTerrace"); terrace.transform.SetParent(restaurant); }
                if (i >= 3) parent = terrace.transform;
                var pos = seats[i]+Vector2.up;
                var table=Add(parent,"table",pos,1000-Mathf.RoundToInt(pos.y*32));
                var depth=table.gameObject.AddComponent<PropDepth>();depth.groundOffset=.25f;depth.Apply();
                var number=Add(table.transform,"table-number-"+i,new Vector2(-.65f,.55f),table.sortingOrder+1);
                var numberDepth=number.gameObject.AddComponent<PropDepth>();numberDepth.groundOffset=-.3f;numberDepth.bias=1;numberDepth.Apply();
                var guest = Point(parent, "serve", "Serve this guest", pos + new Vector2(0, -1), "", "", "guest-"+i); guest.index = i; guests.Add(guest.transform);
            }
            employee = Add(restaurant, "nori", StaffHome, 1050).gameObject;
            for(int n=0;n<4;n++){Add(restaurant,"hanging-herbs",new Vector2(-7+n*4,3.75f),700);var lamp=Add(restaurant,"lantern",new Vector2(-8+n*5,3.7f),700);lamp.gameObject.AddComponent<WorldMotion>().mode=2;}
            var chef = new GameObject("Chef", typeof(SpriteRenderer), typeof(Rigidbody2D), typeof(CapsuleCollider2D)); player = chef.transform; player.SetParent(transform);
            playerArt = chef.GetComponent<SpriteRenderer>(); playerArt.sprite = Art("player-0");
            carriedDish=Add(player,"held-dish-fish",new Vector2(.3f,.55f),1500);carriedDish.transform.localScale=Vector3.one;carriedDish.sprite=null;
            var body = chef.GetComponent<Rigidbody2D>(); body.gravityScale = 0; body.freezeRotation = true; body.interpolation = RigidbodyInterpolation2D.None;
            var collider = chef.GetComponent<CapsuleCollider2D>(); collider.size = new Vector2(.45f, .35f); collider.offset = new Vector2(0, .15f);
            foreach(var sr in GetComponentsInChildren<SpriteRenderer>(true))
            {
                string name=sr.gameObject.name;
                if(name=="lantern"&&sr.transform.IsChildOf(restaurant))continue;
                if(sr.GetComponentsInChildren<Collider2D>().Length>0)continue;
                Vector2 size=Vector2.zero, offset=Vector2.up*.25f;
                if(name.StartsWith("tree"))size=new Vector2(.55f,.4f);
                else if(name=="fountain")size=new Vector2(1.5f,.9f);
                else if(name=="bench")size=new Vector2(1.8f,.5f);
                else if(name=="mushroom-house"||name=="iona-house"){size=new Vector2(3,1.8f);offset=Vector2.up*1.2f;}
                else if(name=="table")size=new Vector2(2.2f,.8f);
                else if(name=="kitchen-worktop"){size=new Vector2(4.65f,.85f);offset=Vector2.up*.4f;}
                else if(name=="bed"){size=new Vector2(2.1f,1.35f);offset=Vector2.up*.7f;}
                else if(name=="saltstone"){size=new Vector2(1.45f,.65f);offset=Vector2.up*.3f;}
                else if(name=="stove"||name=="pantry"||name=="bed"||name=="workshop")size=new Vector2(1.5f,.6f);
                else if(name=="lantern"||name=="mailbox"||name=="saltstone")size=new Vector2(.45f,.4f);
                if(size!=Vector2.zero){Block(sr.transform,size,offset);var depth=sr.GetComponent<PropDepth>()??sr.gameObject.AddComponent<PropDepth>();depth.groundOffset=offset.y;depth.Apply();}
            }
            for(int i=0;i<2;i++){var plant=Add(restaurant,"room-planter",new Vector2(-8.5f+i*17,1.3f),960);Block(plant.transform,new Vector2(.65f,.4f),Vector2.up*.15f);plant.gameObject.AddComponent<PropDepth>().Apply();}
            var kitchenSteam=Add(restaurant,"steam",new Vector2(-.07f,3.95f),700);kitchenSteam.gameObject.AddComponent<WorldMotion>();
            var backWall=new GameObject("Back wall footprint",typeof(BoxCollider2D));backWall.transform.SetParent(restaurant,false);backWall.transform.localPosition=new Vector2(0,4.6f);backWall.GetComponent<BoxCollider2D>().size=new Vector2(19,.7f);
            for(int i=0;i<2;i++){var pot=new GameObject("Floor planter footprint "+i,typeof(BoxCollider2D));pot.transform.SetParent(restaurant,false);pot.transform.localPosition=new Vector2(i==0?-8.75f:8.75f,-3.7f);pot.GetComponent<BoxCollider2D>().size=new Vector2(.95f,.75f);}
            foreach(var sr in GetComponentsInChildren<SpriteRenderer>(true))
            {var foot=sr.GetComponentInChildren<Collider2D>(true);if(!foot||sr==playerArt||sr.transform.IsChildOf(player))continue;var depth=sr.GetComponent<PropDepth>()??sr.gameObject.AddComponent<PropDepth>();depth.groundOffset=foot.transform.TransformPoint(foot.offset).y-sr.transform.position.y;depth.Apply();}
            foreach(var point in points)if(point.transform.IsChildOf(restaurant)&&point.artwork&&(point.action=="menu"||point.action=="service"))point.artwork.sortingOrder=700;
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
            for(int n=0;n<15;n++){var pos=new Vector2(-13+n*1.8f,8.4f);if(!Water(pos,second?1:0))Add(parent,n%3==0?"tree-purple":second?"tree-blue":"tree",pos,730);}
            if(second){Add(parent,"mushroom-house",new Vector2(-7,1),980);Add(parent,"crystal-plant",new Vector2(6,4),870);Add(parent,"bench",new Vector2(-1,-4),1128);return;}
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
        public WorldPoint Point(Transform parent, string action, string label, Vector2 pos, string item = "", string source = "", string art = "")
        {
            var go = new GameObject(label, typeof(WorldPoint)); go.transform.SetParent(parent, false); go.transform.localPosition = pos;
            var p = go.GetComponent<WorldPoint>(); p.action = action; p.label = label; p.item = item; p.source = source;
            if (art != "") p.artwork = Add(go.transform, art, Vector2.zero, 1000 - Mathf.RoundToInt(pos.y * 32));
            points.Add(p); return p;
        }
        public static void Block(Transform parent, Vector2 size, Vector2 offset)
        { var go = new GameObject("Collision", typeof(BoxCollider2D)); go.transform.SetParent(parent, false); go.transform.localPosition = offset; go.GetComponent<BoxCollider2D>().size = size; }
        static void Border(Transform parent, float x, float y)
        {
            Block(parent, new Vector2(x * 2, 1), new Vector2(0, y)); Block(parent, new Vector2(x * 2, 1), new Vector2(0, -y));
            Block(parent, new Vector2(1, y * 2), new Vector2(x, 0)); Block(parent, new Vector2(1, y * 2), new Vector2(-x, 0));
        }
    }
}
