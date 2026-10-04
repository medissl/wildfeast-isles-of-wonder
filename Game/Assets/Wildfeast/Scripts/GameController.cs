using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Wildfeast
{
    public class GameController : MonoBehaviour
    {
        public WorldView world;
        public GameUI ui;
        public GameModel Model { get; private set; }
        public SaveStore Saves { get; private set; }
        public string SaveError { get; private set; }
        public string ActiveActivity => activity;
        public float FishingTension => tension;
        public float FishPosition=>fishChallenge?.Fish??.5f;
        public float CatchZone=>fishChallenge?.Zone??.5f;
        public string FishingPhase=>activity!="fish"?"None":elapsed<.7f?"Cast":elapsed<1.7f?"Waiting":reeling>=0?"Reel":"Tracking";
        FishingChallenge fishChallenge;float reeling=-1;
        public JourneyFrontEnd FrontEnd {get;private set;}
        public string LegacySavePath {get;private set;}
        bool journeyActive;
        public void SuspendJourney(){movement=Vector2.zero;toolAction.Cancel();CancelActivity();journeyActive=false;}
        public void MenuAudio(float music,float sfx){ambience.volume=music*.6f;effects.volume=sfx;}
        public void ActivateJourney(SaveStore store)
        {var previous=Model;previous.Changed-=OnChanged;Saves=store;Model=new GameModel(Content.Load(),store.Read(Content.Load()));Model.Changed+=OnChanged;life.Rebind(previous);world.Appearance(Model.State.avatar);hunting=null;staffOrder=null;staffTimer=0;staffClock=0;toolAction.Cancel();world.StaffDish(null);SaveError=null;movement=Vector2.zero;Refresh();}
        public void ResumeJourney()
        {journeyActive=true;var pos=Model.State.island==0?new Vector2(-6,-2.5f):Archipelago.Get(Model.State.island).arrival;world.SetArea(Model.State.phase=="explore"?Model.State.island:2,Model.State.phase=="explore"?pos:new Vector2(0,-3));if(Model.State.wokeAtHome)world.SetArea(6,new Vector2(1,0));Refresh();if(Saves.Warning!=null)Say(Saves.Warning);}
        public int HuntingPhase => huntPhase;
        public WorldPoint HuntingPoint => hunting;
        Rigidbody2D body;
        AudioSource ambience, effects;
        float toastUntil, elapsed, tension, progress, danger, staffTimer, huntTimer;
        int cookingOrder = -1, huntPhase;
        string activity;
        WorldPoint fishingSpot, hunting;
        Vector2 huntOrigin, charge;
        Vector2 movement;
        bool initialized;
        IslandLife life;
        readonly ToolActionClock toolAction=new ToolActionClock();
        public bool ToolBusy=>toolAction.Busy;
        public float ToolProgress=>toolAction.Progress;
        float activityStarted;
        public bool Sailing => life!=null&&life.Sailing;
        bool huntPushed;
        int cookStep, cuts, expectedCut, turns;
        float heat, goodHeat, cookClock, staffClock;
        readonly bool[] plated=new bool[3];
        Vector2 facing=Vector2.down;
        Order staffOrder;
        Vector2[] staffRoute=Array.Empty<Vector2>();
        int staffWaypoint;
        public int CookingStep => cookStep;
        public float CookingHeat => heat;
        public int CookingCuts => cuts;
        public void Start()
        {
            if (initialized) return; initialized = true;
            var content = Content.Load();
            string saveName = Application.isEditor ? "wildfeast-editor.json" : "wildfeast.json";
            var args = Environment.GetCommandLineArgs();int arg = Array.IndexOf(args, "--save-path");
            if(!Application.isEditor&&!args.Contains("-screen-fullscreen")&&!PlayerPrefs.HasKey("island-life-display"))
            {
                Screen.SetResolution(Display.main.systemWidth,Display.main.systemHeight,FullScreenMode.FullScreenWindow);
                PlayerPrefs.SetInt("island-life-display",1);PlayerPrefs.Save();
            }
            Saves = new SaveStore(arg >= 0 && arg + 1 < args.Length ? args[arg + 1] : Path.Combine(Application.persistentDataPath, saveName));
            Model = new GameModel(content, Saves.Read(content)); body = world.player.GetComponent<Rigidbody2D>();world.Init();life=gameObject.AddComponent<IslandLife>();life.Init(this);gameObject.AddComponent<IslandDiscoveries>().Init(this);
            ui.Init(ShowJournal,ShowBag,ShowPause);ui.Closed=CancelActivity;
            ui.SelectTool=Equip;ui.MenuActions=new Action[]{ShowBag,ShowMap,ShowMenu,ShowJournal,ShowRequests,ShowPause};
            ambience=gameObject.AddComponent<AudioSource>();ambience.clip=Resources.Load<AudioClip>("Audio/music-saltleaf");ambience.loop=true;ambience.Play();
            effects=gameObject.AddComponent<AudioSource>();
            Model.Changed += OnChanged;
            Vector2 startingPoint=Model.State.island==0?new Vector2(-6,-2.5f):Archipelago.Get(Model.State.island).arrival;
            world.SetArea(Model.State.phase=="service" || Model.State.phase=="closing" ? 2 : Model.State.island, Model.State.phase=="explore"?startingPoint:new Vector2(0,-3));
            if(Model.State.wokeAtHome)world.SetArea(6,new Vector2(1,0));
            Refresh();
            if (Saves.Warning!=null) Say(Saves.Warning);
            journeyActive=args.Contains("--smoke-test");world.Appearance(Model.State.avatar);LegacySavePath=Saves.Path;
            if(!journeyActive){FrontEnd=gameObject.AddComponent<JourneyFrontEnd>();FrontEnd.Init(this,Path.Combine(Path.GetDirectoryName(Saves.Path),"journeys"));if(args.Contains("--frontend-test"))gameObject.AddComponent<FirstLightRunner>();}
            if (journeyActive&&Model.State.stage == 0) ShowWelcome();
            else if (journeyActive&&Model.State.phase=="closing") ShowClosing();
            if (args.Contains("--smoke-test")) gameObject.AddComponent<SmokeRunner>();
        }
        void OnChanged()
        {
            Refresh();
        }
        void SavePreferences()
        {
            try{var checkpoint=Saves.Read(Model.Data);checkpoint.musicVolume=Model.State.musicVolume;checkpoint.effectsVolume=Model.State.effectsVolume;checkpoint.zoom=Model.State.zoom;checkpoint.muted=Model.State.muted;checkpoint.reducedMotion=Model.State.reducedMotion;checkpoint.relaxed=Model.State.relaxed;Saves.Write(checkpoint);}catch(Exception ex){SaveError=ex.Message;Say("Could not save options.");}
        }
        public void SaveIntroduction()
        {try{var checkpoint=Saves.Read(Model.Data);checkpoint.introSeen=true;Saves.Write(checkpoint);}catch(Exception ex){SaveError=ex.Message;Say("Could not record the introduction.");}}
        void Refresh()
        {
            ui.wallet.text=$"Day {Model.State.day}  ·  {Model.State.coins} shells";
            ui.location.text=world.Area==6?"YOUR ROOM":world.Area==2?"THE HARBOR TABLE":Archipelago.Get(world.Area).name.ToUpperInvariant();
            ui.objective.text="";world.Refresh(Model);
            ui.Belt(Model);ui.Energy(Model.State.energy);
            var music=Resources.Load<AudioClip>("Audio/music-"+(world.Area==2||world.Area==6?"restaurant":Archipelago.Get(world.Area).key));if(ambience.clip!=music){ambience.clip=music;ambience.Play();}
            ambience.volume=Model.State.muted?0:Model.State.musicVolume*.6f;effects.volume=Model.State.muted?0:Model.State.effectsVolume;
            world.ApplyZoom(Model.State.zoom);
            foreach(var tiles in world.GetComponentsInChildren<TileWorld>(true))if(tiles.water)tiles.water.animationFrameRate=Model.State.reducedMotion?0:1;
            foreach(var motion in world.GetComponentsInChildren<VegetationMotion>(true))motion.enabled=!Model.State.reducedMotion;
            foreach(var motion in world.GetComponentsInChildren<WorldMotion>(true))if(motion.mode==0||motion.mode==1||motion.mode==5)motion.enabled=!Model.State.reducedMotion;
        }
        string Objective()
        {
            if(Model.State.phase=="service")return "Evening service  ·  Stove > guest";
            if(Model.State.phase=="closing")return "Service complete  ·  Rest at the bed";
            if(Model.State.served==0)return "First supper  ·  Catch a Leafgill, then open the door sign";
            return "Explore  ·  Gather  ·  Cook  ·  Grow";
        }
        void Update()
        {
            if(!initialized||!journeyActive||(FrontEnd!=null&&FrontEnd.Active))return;
            if(activity=="sleep"){movement=Vector2.zero;world.Animate(Vector2.zero);return;}
            toolAction.Tick(Time.deltaTime);
            if(Time.unscaledTime>toastUntil && SaveError==null)ui.Message("");
            if(GameInput.Back && !Sailing && !ToolBusy)
            {
                if(ui.PageOpen||activity!=null)ui.Hide();else ShowPause();
            }
            if(GameInput.Map && activity==null&&!ToolBusy){if(ui.PageOpen)ui.Hide();else ShowMap();}
            if(GameInput.Journal && activity==null && hunting==null&&!ToolBusy) { if(ui.PageOpen)ui.Hide();else ShowBag(); }
            if(GameInput.Bag && activity==null&&!ToolBusy){if(ui.PageOpen)ui.Hide();else ShowBag();}
            if(!ui.PageOpen&&activity==null)
            {
                if(GameInput.Slot>=0)Equip(GameInput.Slot);
                if(GameInput.Scroll!=0)Equip((Model.State.equipped+GameInput.Scroll+10)%10);
            }
            movement=ui.PageOpen||activity!=null||ToolBusy?Vector2.zero:GameInput.Move;
            if(movement.sqrMagnitude>.01f)facing=movement.normalized;
            world.Animate(movement);
            if(hunting==null)world.Roam();
            float poseProgress=ToolBusy?toolAction.Progress:activity=="forage"?Mathf.Clamp01((Time.time-activityStarted)/.9f):activity=="fish"?elapsed<.7f?Mathf.Clamp01(elapsed/.7f):1:Mathf.Repeat(elapsed/.75f,1);
            world.Tool(ToolBusy?toolAction.Tool:ItemInventory.EquippedTool(Model.State),facing,ToolBusy,activity,Model.State.slots[Model.State.equipped].id,poseProgress,cookStep);
            var point=world.Nearest();
            if(Mouse.current?.rightButton.wasPressedThisFrame==true)point=world.PointAt(world.worldCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue()));
            ui.prompt.text=!ui.PageOpen && activity==null && point!=null ? Prompt(point) : "";
            var carried=Model.State.orders.FirstOrDefault(o=>o.cooked&&!o.paid);
            ui.orderText.text=Model.State.phase=="service" ? carried!=null?$"Carrying: {Model.Data.Dish(carried.recipe).name}\nTable {carried.number+1}":$"{Model.State.orders.Count(o=>o.paid)}/{Model.State.orders.Count} guests served":"";
            bool wasBusy=activity!=null;
            if(activity=="fish")UpdateFishing();
            else if(activity=="cook")UpdateCooking();
            if(hunting==null && !ui.PageOpen && world.Area==0 && activity==null)
            {
                var creature=world.points.First(p=>p.action=="hunt");
                if(Vector2.Distance(world.player.position,creature.transform.position)<3.8f&&!Model.State.harvested.Contains(creature.source))StartHunt(creature);
            }
            if(hunting!=null&&!ui.PageOpen)UpdateHunt(point);
            if(!ui.PageOpen && activity==null && !wasBusy && !ToolBusy)
            {
                if(GameInput.Use)UseTool(point);
                else if(GameInput.Interact && point!=null && point!=hunting)Interact(point);
            }
            UpdateStaff();
        }
        void FixedUpdate()
        {
            if(!initialized||!journeyActive||(FrontEnd!=null&&FrontEnd.Active))return;
            body.MovePosition(body.position + movement*Model.Data.moveSpeed*Time.fixedDeltaTime);
        }
        public void Interact(WorldPoint p)
        {
            if(activity!=null||ToolBusy)return;
            switch(p.action)
            {
                case "creature": case "bud": case "tap": p.GetComponent<FoodEcology>().Collect();break;
                case "enter": Model.Deposit();world.SetArea(2,new Vector2(0,-3.5f));Refresh();Say("Ingredients stored in the pantry.");break;
                case "exit":
                    if(Model.State.phase=="service")Say("Finish tonight's orders before leaving.");
                    else {world.SetArea(Model.State.island,new Vector2(-6,-2.5f));Refresh();}break;
                case "fish": if(ItemInventory.EquippedTool(Model.State)==1)StartFishing(p);else Say("Fishing rod needed");break;
                case "forage":Gather(p,2);break;
                case "fruit":Gather(p,Model.Has("reach")?3:2);break;
                case "hunt": if(hunting==null)Say("Brothback reacts when you approach. Watch its steam.");break;
                case "crop":UseTool(p);break;
                case "boat":ShowTravel();break;
                case "story":ShowStory();break;
                case "upgrades":ShowUpgrades();break;
                case "pantry":Model.Deposit();ShowBag();break;
                case "menu":ShowMenu();break;
                case "kitchen":ShowKitchen();break;
                case "service":
                    if(Model.State.phase=="closing")ShowClosing();
                    else if(Model.State.phase=="service")ShowKitchen();else ShowOpening();break;
                case "serve":
                    var o=Model.State.orders.FirstOrDefault(x=>x.number==p.index);
                    if(o==null)break;
                    var held=Model.State.orders.FirstOrDefault(x=>x.cooked&&!x.paid);
                    if(held!=null&&held.number!=o.number){Say($"This dish belongs at table {held.number+1}.");break;}
                    if(Model.Serve(o.number)){Sound("chime");world.Burst(p.transform.position,"spark");Say("+"+(Model.Data.Dish(o.recipe).price+o.quality*3+(Model.Data.Dish(o.recipe).flavor==o.preference?4:0))+" shells");CheckClosing();}
                    else Say("Cook this guest's order at the stove first.");break;
                case "bedroom":world.SetArea(6,new Vector2(0,-2.7f));Refresh();break;
                case "downstairs":world.SetArea(2,new Vector2(7,2));Refresh();break;
                case "bed":if(Model.State.phase=="service")Say("There are still guests waiting.");else ShowClosing();break;
                case "requests":ShowRequests();break;
                case "discovery":GetComponent<IslandDiscoveries>().Visit(p);break;
            }
        }
        void Gather(WorldPoint p,int quantity)
        {
            if(activity==null&& !Model.State.harvested.Contains(p.source) && Model.State.phase=="explore" && Model.BagCount+quantity<=Model.Capacity){if(!Exert(3))return;activity="forage";activityStarted=Time.time;Vector2 aim=(Vector2)p.transform.position-(Vector2)world.player.position;if(aim.sqrMagnitude>.01f){facing=aim.normalized;world.Face(facing);}life.Pull(p,quantity,()=>{activity=null;AwardGather(p,quantity);});return;}
            AwardGather(p,quantity);
        }
        public void DiscoveryContact(WorldPoint point,int tool)
        {if(ToolBusy||activity!=null||!Exert(2))return;facing=((Vector2)point.transform.position-(Vector2)world.player.position).normalized;world.Face(facing);toolAction.Begin(tool,()=>GetComponent<IslandDiscoveries>().Use(point,tool));}
        public void HarvestEcology(WorldPoint p,int quantity)=>Gather(p,quantity);
        void AwardGather(WorldPoint p,int quantity)
        {
            if(Model.Gather(p.item,quantity,p.source)) { Sound("chime");world.Burst(p.transform.position,p.item);Say("+"+quantity+" "+Model.Data.Item(p.item).name+(p.item=="pepperbell"||p.item=="lanternroot"?"  ·  +1 seed":"")); }
            else Say(Model.State.harvested.Contains(p.source)?"Let this source recover until tomorrow.":Model.State.phase!="explore"?"Gather ingredients before opening the restaurant.":"Your satchel is full. Store ingredients at home.");
        }
        void StartFishing(WorldPoint p)
        {
            if(Model.State.phase!="explore" || Model.BagCount>=Model.Capacity){Say("Make room in your satchel before casting.");return;}
            var water=world.WaterAt(world.player.position,facing);if(world.Area==2||world.Area==6||!water.HasValue){Say("Cast beside water.");return;}
            if(!WorldView.Water(p.transform.position,world.Area))p.transform.position=water.Value;
            if(!Exert(6))return;
            fishingSpot=p;elapsed=0;tension=.5f;progress=0;danger=0;reeling=-1;fishChallenge=new FishingChallenge(Array.FindIndex(Archipelago.Islands,i=>i.id==world.Area),Model.State.relaxed,Model.Data.fishDuration);facing=((Vector2)p.transform.position-(Vector2)world.player.position).normalized;world.Face(facing);
            ui.Fishing();activity="fish";world.Cast(p.transform.position);Sound("splash");
        }
        void UpdateFishing()
        {
            elapsed+=Time.deltaTime;
            if(reeling>=0)
            {
                reeling+=Time.deltaTime;world.Retrieve(Mathf.Clamp01(reeling/.7f));ui.FishTrack(fishChallenge,"Reeling in…",fishingSpot.item);
                if(reeling>=.7f){var item=fishingSpot.item;world.LandFish(item);world.CancelCast();activity=null;ui.Hide();if(Model.Gather(item)){Sound("chime");Say("+1 "+Model.Data.Item(item).name);}}return;
            }
            world.Fishing(elapsed,fishChallenge.Zone,fishChallenge.Progress);
            if(elapsed<1.7f){ui.FishTrack(fishChallenge,elapsed<.7f?"Casting…":"Waiting for a bite…",fishingSpot.item);return;}
            if(elapsed-Time.deltaTime<1.7f){world.Burst(fishingSpot.transform.position,"splash");Sound("splash");}
            fishChallenge.Tick(Time.deltaTime,GameInput.Hold||Mouse.current?.leftButton.isPressed==true);tension=fishChallenge.Zone;progress=fishChallenge.Progress;
            ui.FishTrack(fishChallenge,fishChallenge.Tracking?"Keep it in the net!":"Follow the fish",fishingSpot.item);
            if(elapsed>60){CancelActivity();ui.Hide();Say("The fish slipped away. Cast again whenever you like.");return;}
            if(progress>=1){reeling=0;Sound("splash");}
        }
        public void StartCooking(int order)
        {
            var o=Model.State.orders.FirstOrDefault(x=>x.number==order);
            if(ToolBusy||o==null || o.cooked || o.paid || !Model.CanCook(o.recipe))return;
            if(!Model.Has("staff")&&Model.State.orders.Any(x=>x.cooked&&!x.paid)){Say("Serve the dish in your hands first.");return;}
            Model.SpendEnergy(Mathf.Min(5,Model.State.energy));
            cookingOrder=order;elapsed=0;cookStep=0;cuts=0;expectedCut=0;turns=0;goodHeat=0;heat=.48f;cookClock=0;
            for(int i=0;i<3;i++)plated[i]=false;
            ui.CookingAction=-1;activity="cook";ShowCookStep();Sound("cook");
        }
        void UpdateCooking()
        {
            elapsed+=Time.deltaTime;
            int action=ui.CookingAction;ui.CookingAction=-1;
            if(cookStep==0)
            {
                if(GameInput.LeftCut)action=0;if(GameInput.RightCut)action=1;
                if(action==expectedCut&&!ToolBusy)toolAction.Begin(5,()=>{if(activity!="cook"||cookStep!=0)return;cuts++;expectedCut=1-expectedCut;Sound("cook");ui.cookingBoard.localRotation=Quaternion.Euler(0,0,cuts%2==0?0:.7f);});
                ui.miniLabel.text=ToolBusy?$"Chopping · {cuts}/6":$"Chop {cuts}/6  ·  Next: {(expectedCut==0?"A / left":"D / right")}";
                if(cuts>=6&&!ToolBusy){cookStep=1;ShowCookStep();}
            }
            else if(cookStep==1)
            {
                if(GameInput.LeftCut)action=0;if(GameInput.RightCut)action=1;if(GameInput.Press)action=2;
                if(action==0)heat-=.16f;if(action==1)heat+=.16f;if(action==2&&!ToolBusy)toolAction.Begin(2,()=>{if(activity!="cook"||cookStep!=1)return;turns++;Sound("cook");ui.cookingBoard.localRotation=Quaternion.Euler(0,0,turns%2==0?0:-1);});
                heat=Mathf.Clamp01(heat+Time.deltaTime*.055f);cookClock+=Time.deltaTime;
                if(heat>=.35f&&heat<=.7f)goodHeat+=Time.deltaTime;
                ui.miniLabel.text=heat>.7f?"Too hot · lower the heat":heat<.35f?"Too cold · raise the heat":"Gently sizzling";
                ui.miniProgress.text=$"Heat {Mathf.RoundToInt(heat*100)}%  ·  {Mathf.CeilToInt(Mathf.Max(0,5-cookClock))}s  ·  Stirred {turns} times";
                if(cookClock>=5&&!ToolBusy){cookStep=2;ShowCookStep();}
            }
            else
            {
                if(GameInput.Slot>=0&&GameInput.Slot<3)action=GameInput.Slot;
                if(action>=0&&action<3&&!plated[action]){plated[action]=true;Sound("chime");var g=ui.cookingBoard.Find("Garnish "+action);if(g)g.gameObject.SetActive(false);}
                ui.miniProgress.text=$"{plated.Count(x=>x)}/3 garnishes placed";
                if(plated.All(x=>x))
                {
                    int quality=goodHeat>=3.7f&&turns>=2?2:goodHeat>=2?1:0;
                    int number=cookingOrder;activity=null;ui.Hide();
                    if(Model.Cook(number,quality)){Say($"Table {number+1} · Dish ready");Sound("chime");world.Burst(world.player.position,"spark");}
                }
            }
        }
        void ShowCookStep(){var o=Model.State.orders.First(x=>x.number==cookingOrder);var r=Model.Data.Dish(o.recipe);ui.CookStage(cookStep,r.name,r.icon,r.ingredients.Select(a=>Model.Data.Item(a.id).icon).ToArray(),r.id=="cloud"?"cook-whisk":r.id=="porridge"||r.id=="broth"||r.id=="lantern"||r.id=="pearlchowder"?"cook-pot":"cook-pan");ui.CookingAction=-1;}
        void StartHunt(WorldPoint p)
        {
            if(Model.State.phase!="explore" || Model.State.harvested.Contains(p.source)){Say("The spring is quiet. Return tomorrow.");return;}
            hunting=p;huntOrigin=p.transform.position;huntTimer=0;huntPhase=0;
            Say("Brothback is watching you…");
        }
        void UpdateHunt(WorldPoint nearest)
        {
            huntTimer+=Time.deltaTime;
            hunting.artwork.sprite=WorldView.Art("brothback-"+((int)(Time.time*(huntPhase==1?14:5))%4));
            hunting.artwork.sortingOrder=1000-Mathf.RoundToInt(hunting.transform.position.y*32);
            if(huntPhase==0)
            {
                ui.prompt.text="Steam rising!  ·  Dodge";world.Burst(hunting.transform.position+Vector3.up*.8f,"steam",.7f);
                hunting.artwork.color=Color.Lerp(Color.white,GameUI.C("eea15b"),Mathf.PingPong(huntTimer*3,1));
                if(huntTimer>1.7f){huntTimer=0;huntPhase=1;huntPushed=false;charge=((Vector2)world.player.position-(Vector2)hunting.transform.position).normalized;Sound("splash");}
            }
            else if(huntPhase==1)
            {
                hunting.transform.position+=(Vector3)(charge*4.2f*Time.deltaTime);
                if(!huntPushed&&Vector2.Distance(world.player.position,hunting.transform.position)<.7f)
                {
                    huntPushed=true;
                    Vector2 destination=(Vector2)world.player.position+charge*.7f;
                    destination.x=Mathf.Clamp(destination.x,-15,15);destination.y=Mathf.Clamp(destination.y,-8.6f,8.6f);
                    world.player.position=destination;body.position=destination;
                    Say("A steam puff pushed you back. Keep clear and try again.");
                }
                if(huntTimer>.75f){huntTimer=0;huntPhase=2;hunting.artwork.color=GameUI.C("abc0ad");}
            }
            else
            {
                ui.prompt.text="Cooling  ·  E collect stock";
                if(!ToolBusy&&(GameInput.Interact||GameInput.Use) && Vector2.Distance(world.player.position,hunting.transform.position)<2.4f)
                {var p=hunting;EndHunt();Gather(p,2);return;}
                // The stock stays ready while approaching; a short timer previously stole the reward.
            }
            if(Vector2.Distance(world.player.position,huntOrigin)>9 || world.Area!=0)EndHunt();
        }
        void EndHunt(){if(hunting){hunting.transform.position=huntOrigin;hunting.artwork.color=Color.white;}hunting=null;world.Refresh(Model);}
        void CancelActivity(){if(activity=="sail")return;if(activity=="forage")life.CancelPull();if(activity=="cook")toolAction.Cancel();activity=null;cookingOrder=-1;ui.MiniPressed=false;world.CancelCast();}
        public void Say(string text){ui.Message(text);toastUntil=Time.unscaledTime+5;}
        public void Sound(string name){effects.PlayOneShot(Resources.Load<AudioClip>("Audio/"+name));}
        void CheckClosing(){if(Model.CloseService())Say($"Service complete · {Model.State.earned} shells earned. Rest at the bed when ready.");}
        public void ShowWelcome()
        {
            VisualGuide.Show(ui,0,()=>{Model.State.stage=1;Model.Notify();ui.Hide();});
        }
        public void ShowBag(){ui.Inventory(Model,()=>{Model.Notify();ShowBag();});MenuTabs("Inventory");}
        public void ShowMap(){ui.Map(world);MenuTabs("Map");}
        void MenuTabs(string active){ui.Tabs(active,new[]{"Inventory","Map","Recipes","Journal","Requests","Options"},new Action[]{ShowBag,ShowMap,ShowMenu,ShowJournal,ShowRequests,ShowPause});}
        int journalPage,recipePage;
        public void ShowJournal()
        {
            ui.Show("The forager's journal","");
            for(int i=journalPage*5;i<Mathf.Min(Model.Data.ingredients.Length+5,journalPage*5+5);i++)
            {
                if(i>=Model.Data.ingredients.Length){var definition=Archipelago.Islands[i-Model.Data.ingredients.Length];var landmark=world.points.First(p=>p.action=="discovery"&&p.transform.IsChildOf(world.IslandRoot(definition.id)));bool visited=Model.State.landmarks.Contains(landmark.source);ui.Row(i-journalPage*5,visited?landmark.label:"Unexplored landmark",definition.name,landmark.source,visited?"Read":"Unknown",()=>GetComponent<IslandDiscoveries>().Visit(landmark),visited);continue;}
                var item=Model.Data.ingredients[i];bool known=Model.State.discovered.Contains(item.id);
                ui.Row(i-journalPage*5,known?item.name:"Unrecorded discovery",known?item.habitat:"Explore the islands to fill this page.",known?item.icon:"spark",known?"Read notes":"Unknown",()=>ShowItem(item),known);
            }
            ui.Pagination(journalPage,Model.Data.ingredients.Length+5,5,p=>{journalPage=p;ShowJournal();});
        }
        void ShowItem(Ingredient item){ui.Show(item.name,item.habitat);ui.Paragraph(item.description);ui.FooterButton("Back to journal",ShowJournal);}
        public void ShowMenu()
        {
            ui.Show("Tonight's menu","");
            for(int i=recipePage*5;i<Mathf.Min(Model.Data.recipes.Length,recipePage*5+5);i++)
            {
                var recipe=Model.Data.recipes[i];bool known=Model.State.recipes.Contains(recipe.id),selected=Model.State.menu.Contains(recipe.id);
                ui.RecipeCard(i-recipePage*5,recipe,Model,known,selected,()=>{if(selected)Model.State.menu.Remove(recipe.id);else Model.State.menu.Add(recipe.id);Model.Notify();ShowMenu();});
            }
            ui.Pagination(recipePage,Model.Data.recipes.Length,5,p=>{recipePage=p;ShowMenu();});
        }
        void ShowOpening()
        {
            if(world.Area!=2){Model.Deposit();world.SetArea(2,new Vector2(0,-3.5f));Refresh();}
            if(!Model.State.menu.Any(Model.CanCook)&&Model.CanCook("porridge")){Model.State.menu.Add("porridge");Model.Notify();}
            ui.Show("Open the Harbor Table","Tonight's menu · stocked dishes only");
            int row=0;foreach(var id in Model.State.menu)
            {
                var r=Model.Data.Dish(id);ui.Row(row++,r.name,Model.CanCook(id)?"Ingredients ready":"Missing ingredients",r.icon,null,null);
            }
            if(row>4){ui.Show("Welcome the evening","Your selected menu is ready. Guests will order only dishes you have stock to prepare.");}
            ui.FooterButton("Flip the OPEN sign",()=>{if(Model.StartService()){ui.Hide();Say("Guests are arriving · E at the stove to cook");}else Say("Add a stocked dish on the menu board.");});
        }
        public void ShowKitchen()
        {
            if(Model.State.phase!="service"){ShowMenu();return;}
            ui.Show("The evening kitchen","");
            foreach(var o in Model.State.orders)
            {
                int number=o.number;var r=Model.Data.Dish(o.recipe);
                ui.Row(number,$"Table {number+1} · {r.name}",o.customer+$" · likes {o.preference}"+(o.paid?" · served":o.cooked?" · ready to deliver":""),r.icon,o.paid?"Served":o.cooked?"Dish ready":"Cook",()=>StartCooking(number),!o.cooked&&!o.paid);
            }
        }
        public void ShowUpgrades()
        {
            ui.Show("The harbor workshop","");
            for(int i=0;i<Model.Data.upgrades.Length;i++)
            {
                var u=Model.Data.upgrades[i];bool owned=Model.Has(u.id);string detail=u.description;
                ui.Row(i,u.name,detail,null,owned?"Owned":$"{u.cost} shells",()=>{if(Model.Buy(u.id)){Sound("chime");ShowUpgrades();}else Say("Check your shells and the required improvements.");},!owned&&Model.State.phase!="service");
            }
            int value=ItemInventory.ResourceValue(Model.State);
            ui.Row(5,"Trade gathered materials","Cinnamonwood · Saltstone · Noodlegrass fiber","wood",$"Trade · {value} shells",()=>{if(ItemInventory.SellResources(Model)){Sound("chime");ShowUpgrades();}},value>0&&Model.State.phase=="explore");
        }
        void ShowCrop(int index)
        {
            var c=Model.State.crops[index];ui.Show("The kitchen garden",$"Plot {index+1}");
            if(!c.planted)
            {
                int row=0;foreach(string id in new[]{"pepperbell","lanternroot"})
                {string item=id;ui.Row(row++,Model.Data.Item(id).name,"Plant a seed from your journal.",id,"Plant",()=>{if(Model.Tend(index,item)){ui.Hide();Say("Seed planted and watered.");}},Model.State.discovered.Contains(id));}
            }
            else
            {
                bool ready=c.growth>=Model.Data.cropDays;
                ui.Row(0,Model.Data.Item(c.item).name,ready?"Ready to harvest three portions.":$"Growth {c.growth}/{Model.Data.cropDays} · "+(c.wateredDay==Model.State.day?"Watered today":"Needs water"),c.item,ready?"Harvest":"Water",()=>{if(Model.Tend(index)){ui.Hide();Say(ready?"Garden harvest added to your satchel.":"Watered. Growth happens overnight.");}else Say(ready?"Make room in your satchel.":"Already watered today.");});
            }
        }
        void ShowTravel()
        {
            ui.Show("Chart a course","");
            for(int n=0;n<Archipelago.Islands.Length;n++)
            {var i=Archipelago.Islands[n];int destination=i.id;ui.Row(n,i.name,i.subtitle,i.id==0?"restaurant":i.points.First(p=>p.item!="").art,"Sail",()=>Sail(destination),Model.State.phase=="explore"&&world.Area!=i.id);}
        }

        public void Sail(int island){if(activity!=null||ToolBusy||Model.State.phase!="explore"||!Archipelago.Valid(island)||!Archipelago.Valid(world.Area))return;if(island==Model.State.island){ui.Hide();Say("Already docked at this island.");return;}ui.Hide();EndHunt();activity="sail";life.Sail(island,()=>{activity=null;Model.Travel(island);Refresh();Say("Docked at "+Archipelago.Get(island).name);});}
        public void Equip(int slot)
        {
            if(activity!=null||ToolBusy||slot<0||slot>9)return;

            Model.State.equipped=slot;Model.Notify();
        }
        string Prompt(WorldPoint p)
        {
            var ecology=p.GetComponent<FoodEcology>();if(ecology)return ecology.Hint;
            if(p.action=="fish")return ItemInventory.EquippedTool(Model.State)==1?"Space · Cast into water":"Fishing rod";
            if(p.action=="crop")
            {
                var c=Model.State.crops[p.index];
                if(c.growth>=Model.Data.cropDays)return "E · Harvest";
                if(!c.planted)return ItemInventory.EquippedTool(Model.State)==3||ItemInventory.EquippedTool(Model.State)==4?"Space · Plant seed":"Seeds needed";
                return c.wateredDay==Model.State.day?"Watered today":ItemInventory.EquippedTool(Model.State)==2?"Space · Water":"Watering can";
            }
            if(p.action=="hunt")return "Watch the steam · Dodge the charge";
            return "E · "+p.label;
        }
        void UseTool(WorldPoint p)
        {
            if(ToolBusy)return;
            if(Mouse.current?.leftButton.wasPressedThisFrame==true){Vector2 aim=(Vector2)world.worldCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue())-(Vector2)world.player.position;if(aim.sqrMagnitude>.01f&&aim.sqrMagnitude<=4){facing=aim.normalized;world.Face(facing);}}
            int tool=ItemInventory.EquippedTool(Model.State);
            if(tool==1)
            {
                var water=world.WaterAt(world.player.position,facing);
                if(water.HasValue)
                {
                    var spot=world.points.First(x=>x.action=="fish"&&x.transform.IsChildOf(world.IslandRoot(world.Area)));
                    spot.transform.position=water.Value;StartFishing(spot);
                }
                else Say("Stand beside the shore or pond to cast.");
                return;
            }
            if((tool==0||tool<0)&&p!=null&&(p.action=="forage"||p.action=="fruit")){Interact(p);return;}
            if(tool<0||world.Area==6)return;
            if(!Exert(2))return;
            Vector2 target=life.Target(facing),direction=facing;
            toolAction.Begin(tool,()=>
            {
                if(GetComponent<IslandDiscoveries>().Use(p,tool))return;
                if(life.Use(tool,direction,target))return;
                if(p!=null&&p.action=="crop"){UseGarden(p);return;}
                if(p!=null&&(p.action=="forage"||p.action=="fruit"))Gather(p,p.action=="fruit"&&Model.Has("reach")?3:2);
                else if(tool==2){world.Burst(world.player.position+(Vector3)direction*.65f,"splash");Sound("splash");}
                else if(tool==5){world.Burst(world.player.position+(Vector3)direction*.65f,"spark");Sound("cook");}
            });
            movement=Vector2.zero;
        }
        void UseGarden(WorldPoint p)
        {
            var c=Model.State.crops[p.index];bool changed=false;
            if(c.planted&&c.growth>=Model.Data.cropDays)changed=Model.Harvest(p.index);
            else if(!c.planted&&(Model.State.slots[Model.State.equipped].id=="seed-pepper"||Model.State.slots[Model.State.equipped].id=="seed-root"))changed=Model.Plant(p.index,ItemInventory.EquippedTool(Model.State)==3?"pepperbell":"lanternroot");
            else if(c.planted&&ItemInventory.EquippedTool(Model.State)==2&&Model.State.water>0){changed=Model.Water(p.index);if(changed){Model.State.water--;Model.Notify();}}
            if(changed){world.Burst(p.transform.position,ItemInventory.EquippedTool(Model.State)==2?"splash":"spark");Sound(ItemInventory.EquippedTool(Model.State)==2?"splash":"chime");}
            else Say(Prompt(p).Replace("Space · ",""));
        }
        void UpdateStaff()
        {
            if(!Model.Has("staff")||world.Area!=2)return;
            var employee=world.employee.transform;
            if(staffOrder!=null&&(staffOrder.paid||!Model.State.orders.Contains(staffOrder))){staffOrder=null;staffRoute=Array.Empty<Vector2>();employee.position=WorldView.StaffHome;}
            if(staffRoute.Length==0)
            {
                staffOrder=Model.State.orders.FirstOrDefault(o=>o.cooked&&!o.paid);
                if(staffOrder==null){world.StaffDish(null);return;}
                staffRoute=world.StaffRoute(staffOrder.number);staffWaypoint=1;
            }
            Vector3 target=staffRoute[staffWaypoint];
            employee.position=Vector3.MoveTowards(employee.position,target,Time.deltaTime*3.2f);
            world.StaffDish(staffOrder==null?null:Model.Data.Dish(staffOrder.recipe).icon);
            if(Vector2.Distance(employee.position,target)<.08f)
            {
                if(staffWaypoint<staffRoute.Length-1){staffWaypoint++;return;}
                if(staffOrder==null){staffRoute=Array.Empty<Vector2>();return;}
                staffClock+=Time.deltaTime;
                if(staffClock>.35f){int index=staffOrder.number;Model.Serve(index);staffOrder=null;staffClock=0;staffRoute=world.StaffRoute(index,true);staffWaypoint=1;Sound("chime");CheckClosing();}
            }
        }
        void ShowStory()
        {
            ui.Show("A letter from the tidekeeper","Mistwake Isle · A recipe carried home");
            ui.Paragraph("To whoever finds this garden,\n\nI planted these trees for the people who never saw the sky beyond our harbor. The fruit rises when the mist rolls in. Reach gently; never pull a whole branch.\n\nIf your stove still burns, make something light enough to remind them of this place. A restaurant can be a little window onto the world.\n\n— Tidekeeper Iona");
            Model.State.storySeen=true;Model.Notify();
        }
        public void ShowRequests()
        {
            string[] goals={"seared","broth","lantern","cloud"};string[] names={"A first taste of the shore","Something for the ferryman","Light for the seed keeper","A taste of Iona's island"};
            int index=Model.State.requestIndex;
            ui.Show("Harbor requests","");
            if(index>=goals.Length){ui.Paragraph("The first four harbor requests are complete. The new islands still have food to discover. There are still gardens to tend and menus to try.");return;}
            var r=Model.Data.Dish(goals[index]);ui.Row(0,names[index],$"Serve {r.name} · reward {18+6*index} shells",r.icon,"Claim reward",()=>{if(Model.ClaimRequest()){Sound("chime");ShowRequests();}else Say("Serve the requested dish before collecting this reward.");});
        }
        bool Exert(int amount)
        {if(Model.SpendEnergy(amount))return true;Say("Time to rest. Sleep in your room to recover.");return false;}
        public void ShowClosing()
        {
            if(Model.State.phase=="service")return;
            if(world.Area!=6){ui.Show("Service complete","");ui.FooterButton("Return to the restaurant",()=>ui.Hide());return;}
            ui.Show("Rest until morning?","Your progress saves when you sleep.");
            ui.FooterButton("Sleep and save",()=>StartCoroutine(SleepRoutine()));
        }
        public System.Collections.IEnumerator SleepRoutine()
        {
            if(world.Area!=6||Model.State.phase=="service"||activity!=null||ToolBusy)yield break;
            ui.Hide();activity="sleep";movement=Vector2.zero;var curtain=SceneCurtain.Create(ui);
            yield return curtain.Fade(true);
            // Stage the next morning separately. A failed disk write leaves this day intact.
            var next=SaveStore.Parse(JsonUtility.ToJson(Model.State),Model.Data);var morning=new GameModel(Model.Data,next);
            if(morning.NextDay())
            {
                try{Saves.Write(next);ActivateJourney(Saves);journeyActive=true;world.SetArea(6,new Vector2(1,0));}
                catch(Exception ex){SaveError=ex.Message;Debug.LogError("Sleep save failed: "+ex.Message);}
            }
            activity="sleep";Refresh();yield return curtain.Fade(false);Destroy(curtain.gameObject);activity=null;
            Say(SaveError==null?"Morning · Progress saved":"Could not save. Your day has been preserved.");
        }
        public float MusicLevel=>ambience.volume;
        public float EffectsLevel=>effects.volume;
        public void ShowPause()
        {
            if(ToolBusy||Sailing)return;
            ui.Show("Take a breath","");
            ui.SliderRow(0,"Music volume",Model.State.musicVolume,v=>{Model.State.musicVolume=v;Model.Notify();SavePreferences();});
            ui.SliderRow(1,"Sound effects",Model.State.effectsVolume,v=>{Model.State.effectsVolume=v;Model.Notify();SavePreferences();});
            ui.Row(2,"Sound", "",null,Model.State.muted?"Muted":"Sound on",()=>{Model.State.muted=!Model.State.muted;Model.Notify();SavePreferences();ShowPause();});
            ui.Row(3,"World zoom","",null,new[]{"Wide","Comfort","Close"}[Model.State.zoom],()=>{Model.State.zoom=(Model.State.zoom+1)%3;Model.Notify();SavePreferences();ShowPause();});
            ui.Row(4,"Ambient motion","",null,Model.State.reducedMotion?"Reduced":"Full motion",()=>{Model.State.reducedMotion=!Model.State.reducedMotion;Model.Notify();SavePreferences();ShowPause();});
            ui.FooterButton("Display and field guide",ShowMoreOptions);
        }
        void ShowMoreOptions()
        {
            ui.Show("Take a breath","");
            ui.Row(0,"Challenge","",null,Model.State.relaxed?"Relaxed":"Standard",()=>{Model.State.relaxed=!Model.State.relaxed;Model.Notify();SavePreferences();ShowMoreOptions();});
            ui.Row(1,"Display",Screen.fullScreen?"Fills your display":"Resizable window",null,Screen.fullScreen?"Windowed":"Fullscreen",()=>{if(Screen.fullScreen)Screen.SetResolution(Mathf.Min(1280,Display.main.systemWidth-80),Mathf.Min(720,Display.main.systemHeight-80),FullScreenMode.Windowed);else Screen.SetResolution(Display.main.systemWidth,Display.main.systemHeight,FullScreenMode.FullScreenWindow);ShowMoreOptions();});
            ui.Row(2,"Field guide","",null,"View",()=>VisualGuide.Show(ui,0,ShowMoreOptions));
            ui.Row(3,"Your journeys","Continue from your last sleep.",null,"Title screen",()=>{if(FrontEnd==null){FrontEnd=gameObject.AddComponent<JourneyFrontEnd>();FrontEnd.Init(this,Path.Combine(Path.GetDirectoryName(Saves.Path),"journeys"));}else FrontEnd.Title();});
            ui.Row(4,"Leave the table","Continue from your last sleep.",null,"Quit",()=>Application.Quit());
            ui.FooterButton("Back to sound and camera",ShowPause);
        }
        void ShowReset()
        {
            ui.Show("Start a new journey?","This resets the restaurant, discoveries, upgrades, and current service. A copy of the old save will be preserved.");
            ui.FooterButton("Archive old save and restart",()=>{Saves.ArchiveAndReset();UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);});
        }
        void OnApplicationQuit(){}
        void OnApplicationPause(bool paused){}
    }
}
