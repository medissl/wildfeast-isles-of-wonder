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
        bool huntPushed;
        int cookStep, cuts, expectedCut, turns;
        float heat, goodHeat, cookClock, actionUntil, staffClock;
        readonly bool[] plated=new bool[3];
        Vector2 facing=Vector2.down;
        Order staffOrder;
        public int CookingStep => cookStep;
        public float CookingHeat => heat;
        public int CookingCuts => cuts;
        public void Start()
        {
            if (initialized) return; initialized = true;
            var content = Content.Load();
            string saveName = Application.isEditor ? "wildfeast-editor.json" : "wildfeast.json";
            var args = Environment.GetCommandLineArgs();int arg = Array.IndexOf(args, "--save-path");
            if(!Application.isEditor&&!args.Contains("-screen-fullscreen")&&!PlayerPrefs.HasKey("living-world-display"))
            {
                Screen.SetResolution(Display.main.systemWidth,Display.main.systemHeight,FullScreenMode.FullScreenWindow);
                PlayerPrefs.SetInt("living-world-display",1);PlayerPrefs.Save();
            }
            Saves = new SaveStore(arg >= 0 && arg + 1 < args.Length ? args[arg + 1] : Path.Combine(Application.persistentDataPath, saveName));
            Model = new GameModel(content, Saves.Read(content)); body = world.player.GetComponent<Rigidbody2D>();world.Init();
            ui.Init(ShowJournal,ShowBag,ShowPause);ui.Closed=CancelActivity;
            ui.SelectTool=Equip;
            ambience=gameObject.AddComponent<AudioSource>();ambience.clip=Resources.Load<AudioClip>("Audio/harbor");ambience.loop=true;ambience.Play();
            effects=gameObject.AddComponent<AudioSource>();
            Model.Changed += OnChanged;
            world.SetArea(Model.State.phase=="service" || Model.State.phase=="closing" ? 2 : Model.State.island, Model.State.phase=="explore"?new Vector2(-6,-2.5f):new Vector2(0,-3));
            Refresh();
            if (Saves.Warning!=null) Say(Saves.Warning);
            if (Model.State.stage == 0) ShowWelcome();
            else if (Model.State.phase=="closing") ShowClosing();
            if (args.Contains("--smoke-test")) gameObject.AddComponent<SmokeRunner>();
        }
        void OnChanged()
        {
            Save();Refresh();
        }
        void Save()
        {
            try { Saves.Write(Model.State);SaveError=null; }
            catch (Exception ex) { SaveError=ex.Message;Debug.LogError("Save failed: "+ex.Message);ui.Message("Progress could not be saved. Check available disk space."); }
        }
        void Refresh()
        {
            ui.wallet.text=$"Day {Model.State.day}  ·  {Model.State.coins} shells";
            ui.location.text=world.Area==2 ? "THE HARBOR TABLE" : Model.State.island==1 ? "MISTWAKE ISLE" : "SALTLEAF SHORE";
            ui.objective.text=Objective();world.Refresh(Model);
            ui.Belt(Model);
            ambience.volume=Model.State.muted?0:Model.State.volume*.6f;effects.volume=Model.State.muted?0:Model.State.volume;
        }
        string Objective()
        {
            if(Model.State.phase=="service")return "Evening service  ·  Stove → guest";
            if(Model.State.phase=="closing")return "Service complete  ·  Rest at the bed";
            if(Model.State.served==0)return "First supper  ·  Catch a Leafgill, then open the door sign";
            return "Explore  ·  Gather  ·  Cook  ·  Grow";
        }
        void Update()
        {
            if(!initialized)return;
            if(Time.unscaledTime>toastUntil && SaveError==null)ui.Message("");
            if(GameInput.Back)
            {
                if(ui.PageOpen||activity!=null)ui.Hide();else ShowPause();
            }
            if(GameInput.Journal && activity==null && hunting==null) { if(ui.PageOpen)ui.Hide();else ShowJournal(); }
            if(GameInput.Bag && activity==null){if(ui.PageOpen)ui.Hide();else ShowBag();}
            if(!ui.PageOpen&&activity==null)
            {
                if(GameInput.Slot>=0)Equip(GameInput.Slot);
                if(GameInput.Scroll!=0)Equip((Model.State.equipped+GameInput.Scroll+8)%8);
            }
            movement=ui.PageOpen||activity!=null?Vector2.zero:GameInput.Move;
            if(movement.sqrMagnitude>.01f)facing=movement.normalized;
            world.Animate(movement);
            if(hunting==null)world.Roam();
            world.Tool(Model.State.equipped,facing,Time.time<actionUntil,activity);
            var point=world.Nearest();
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
            if(!ui.PageOpen && activity==null && !wasBusy)
            {
                if(GameInput.Use)UseTool(point);
                else if(GameInput.Interact && point!=null && point!=hunting)Interact(point);
            }
            UpdateStaff();
        }
        void FixedUpdate()
        {
            if(!initialized)return;
            body.MovePosition(body.position + movement*Model.Data.moveSpeed*Time.fixedDeltaTime);
        }
        public void Interact(WorldPoint p)
        {
            if(activity!=null)return;
            switch(p.action)
            {
                case "enter": Model.Deposit();world.SetArea(2,new Vector2(0,-3.5f));Refresh();Say("Ingredients stored in the pantry.");break;
                case "exit":
                    if(Model.State.phase=="service")Say("Finish tonight's orders before leaving.");
                    else {world.SetArea(Model.State.island,new Vector2(-6,-2.5f));Refresh();}break;
                case "fish": if(Model.State.equipped==1)StartFishing(p);else Say("Equip the rod [2], then cast with Space.");break;
                case "forage":Gather(p,2);break;
                case "fruit":Gather(p,Model.Has("reach")?3:2);break;
                case "hunt": if(hunting==null)Say("Brothback reacts when you approach. Watch its steam.");break;
                case "crop":UseGarden(p);break;
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
                case "bed":if(Model.State.phase=="service")Say("There are still guests waiting.");else ShowClosing();break;
                case "requests":ShowRequests();break;
            }
        }
        void Gather(WorldPoint p,int quantity)
        {
            if(Model.Gather(p.item,quantity,p.source)) { Sound("chime");actionUntil=Time.time+.45f;world.Burst(p.transform.position,p.item);Say("+"+quantity+" "+Model.Data.Item(p.item).name+(p.item=="pepperbell"||p.item=="lanternroot"?"  ·  +1 seed":"")); }
            else Say(Model.State.harvested.Contains(p.source)?"Let this source recover until tomorrow.":Model.State.phase!="explore"?"Gather ingredients before opening the restaurant.":"Your satchel is full. Store ingredients at home.");
        }
        void StartFishing(WorldPoint p)
        {
            if(Model.State.phase!="explore" || Model.BagCount>=Model.Capacity){Say("Make room in your satchel before casting.");return;}
            var water=world.WaterAt(world.player.position,facing);if(world.Area==2||!water.HasValue){Say("Cast beside water.");return;}
            if(!WorldView.Water(p.transform.position))p.transform.position=water.Value;
            fishingSpot=p;elapsed=0;tension=.4f;progress=0;danger=0;
            ui.Fishing();activity="fish";world.Cast(p.transform.position);Sound("splash");
        }
        void UpdateFishing()
        {
            elapsed+=Time.deltaTime;
            world.Fishing(elapsed,tension,progress);
            if(elapsed<1.5f){ui.Mini(.4f,0,"Waiting for a bite…","Watch the float · Esc cancels");return;}
            bool pulling=GameInput.Hold||Mouse.current?.leftButton.isPressed==true;
            tension=Mathf.Clamp01(tension+Time.deltaTime*(pulling?.43f:-.28f)+Time.deltaTime*Mathf.Sin(elapsed*2.5f)*.13f);
            if(tension>.25f && tension<.72f)progress+=Time.deltaTime/Model.Data.fishDuration;
            danger=tension>.95f?danger+Time.deltaTime:0;
            ui.Mini(tension,progress,pulling?"Reeling in":"Giving slack",$"{Mathf.RoundToInt(progress*100)}% landed · "+(tension>.72f?"Too tight — release!":tension<.25f?"Too loose — reel!":"Good tension"));
            if(danger>(Model.State.relaxed?2f:.65f) || elapsed>45)
            {Debug.Log($"Fishing escaped at {elapsed:F2}s tension={tension:F2} progress={progress:F2}");activity=null;ui.Hide();Say("The Leafgill slipped away. You can cast again.");return;}
            if(progress>=1)
            {var spot=fishingSpot;world.LandFish(spot.item);activity=null;ui.Hide();if(Model.Gather(spot.item)){Sound("chime");Say("+1 Leafgill");}}
        }
        public void StartCooking(int order)
        {
            var o=Model.State.orders.FirstOrDefault(x=>x.number==order);
            if(o==null || o.cooked || o.paid || !Model.CanCook(o.recipe))return;
            if(!Model.Has("staff")&&Model.State.orders.Any(x=>x.cooked&&!x.paid)){Say("Serve the dish in your hands first.");return;}
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
                if(action==expectedCut){cuts++;expectedCut=1-expectedCut;Sound("cook");ui.cookingBoard.localRotation=Quaternion.Euler(0,0,cuts%2==0?0:.7f);}
                ui.miniLabel.text=$"Chop {cuts}/6  ·  Next: {(expectedCut==0?"A / left":"D / right")}";
                if(cuts>=6){cookStep=1;ShowCookStep();}
            }
            else if(cookStep==1)
            {
                if(GameInput.LeftCut)action=0;if(GameInput.RightCut)action=1;if(GameInput.Press)action=2;
                if(action==0)heat-=.16f;if(action==1)heat+=.16f;if(action==2){turns++;Sound("cook");ui.cookingBoard.localRotation=Quaternion.Euler(0,0,turns%2==0?0:-1);}
                heat=Mathf.Clamp01(heat+Time.deltaTime*.055f);cookClock+=Time.deltaTime;
                if(heat>=.35f&&heat<=.7f)goodHeat+=Time.deltaTime;
                ui.miniLabel.text=heat>.7f?"Too hot · lower the heat":heat<.35f?"Too cold · raise the heat":"Gently sizzling";
                ui.miniProgress.text=$"Heat {Mathf.RoundToInt(heat*100)}%  ·  {Mathf.CeilToInt(Mathf.Max(0,5-cookClock))}s  ·  Stirred {turns} times";
                if(cookClock>=5){cookStep=2;ShowCookStep();}
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
        void ShowCookStep(){var o=Model.State.orders.First(x=>x.number==cookingOrder);var r=Model.Data.Dish(o.recipe);ui.CookStage(cookStep,r.name,r.icon,r.ingredients.Select(a=>Model.Data.Item(a.id).icon).ToArray(),r.id=="cloud"?"cook-whisk":r.id=="porridge"||r.id=="broth"||r.id=="lantern"?"cook-pot":"cook-pan");ui.CookingAction=-1;}
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
                if((GameInput.Interact||GameInput.Use) && Vector2.Distance(world.player.position,hunting.transform.position)<1.8f)
                {var p=hunting;EndHunt();Gather(p,2);return;}
                if(huntTimer>(Model.State.relaxed?5:3)){huntTimer=0;huntPhase=0;hunting.transform.position=huntOrigin;}
            }
            if(Vector2.Distance(world.player.position,huntOrigin)>9 || world.Area!=0)EndHunt();
        }
        void EndHunt(){if(hunting){hunting.transform.position=huntOrigin;hunting.artwork.color=Color.white;}hunting=null;world.Refresh(Model);}
        void CancelActivity(){activity=null;cookingOrder=-1;ui.MiniPressed=false;world.CancelCast();}
        void Say(string text){ui.Message(text);toastUntil=Time.unscaledTime+5;}
        void Sound(string name){effects.PlayOneShot(Resources.Load<AudioClip>("Audio/"+name));}
        void CheckClosing(){if(Model.CloseService())Say($"Service complete · {Model.State.earned} shells earned. Rest at the bed when ready.");}
        public void ShowWelcome()
        {
            ui.Show("Your little table. A world of wonder.","Welcome to Saltleaf. The restaurant is yours.");
            ui.Paragraph("WASD to walk. E to use doors, signs, and plants.\n\nChoose a tool with 1–8 or the mouse wheel.\nSpace uses the tool in your hands.\n\nCatch something delicious. Bring it home.\nFlip the OPEN sign beside your restaurant door.");
            ui.FooterButton("Step into Saltleaf",()=>{Model.State.stage=1;Model.Notify();ui.Hide();});
        }
        public void ShowBag()
        {
            ui.Show("Satchel & pantry",$"Satchel {Model.BagCount}/{Model.Capacity} · Pantry ingredients are available for cooking. Enter home to store your catch.");
            for(int i=0;i<Model.Data.ingredients.Length;i++)
            {
                var item=Model.Data.ingredients[i];ui.Row(i,item.name,$"Satchel: {Model.Count(item.id,true)}     Pantry: {Model.Count(item.id)}",item.icon,"Read journal",()=>ShowItem(item));
            }
        }
        public void ShowJournal()
        {
            ui.Show("The forager's journal","Every ingredient tells a story. Discover it in the wild to learn its behavior and recipes.");
            for(int i=0;i<Model.Data.ingredients.Length;i++)
            {
                var item=Model.Data.ingredients[i];bool known=Model.State.discovered.Contains(item.id);
                ui.Row(i,known?item.name:"Unrecorded discovery",known?item.habitat:"Explore the islands to fill this page.",known?item.icon:"spark",known?"Read notes":"Unknown",()=>ShowItem(item),known);
            }
        }
        void ShowItem(Ingredient item){ui.Show(item.name,item.habitat);ui.Paragraph(item.description);ui.FooterButton("Back to journal",ShowJournal);}
        public void ShowMenu()
        {
            ui.Show("Tonight's menu","Pick the dishes you want to offer. Guests order from stocked recipes; matching their taste earns a tip.");
            for(int i=0;i<Model.Data.recipes.Length;i++)
            {
                var recipe=Model.Data.recipes[i];bool known=Model.State.recipes.Contains(recipe.id),selected=Model.State.menu.Contains(recipe.id);
                string needs=string.Join(" · ",recipe.ingredients.Select(a=>$"{a.count} {Model.Data.Item(a.id).name}"));
                ui.Row(i,known?recipe.name:"Undiscovered recipe",known?needs+$"  /  {recipe.price} shells":"Find its main ingredient to learn this recipe.",recipe.icon,selected?"On the menu":"Add to menu",()=>{if(selected)Model.State.menu.Remove(recipe.id);else Model.State.menu.Add(recipe.id);Model.Notify();ShowMenu();},known&&Model.State.phase=="explore");
            }
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
            ui.Show("The evening kitchen","Cook an order, then serve its guest at the matching table. Nori delivers finished dishes when hired.");
            foreach(var o in Model.State.orders)
            {
                int number=o.number;var r=Model.Data.Dish(o.recipe);
                ui.Row(number,$"Table {number+1} · {r.name}",o.customer+$" · likes {o.preference}"+(o.paid?" · served":o.cooked?" · ready to deliver":""),r.icon,o.paid?"Served":o.cooked?"Dish ready":"Cook",()=>StartCooking(number),!o.cooked&&!o.paid);
            }
        }
        public void ShowUpgrades()
        {
            ui.Show("The harbor workshop","Restore your home and expedition tools. Everything here is bought with shells earned in your restaurant.");
            for(int i=0;i<Model.Data.upgrades.Length;i++)
            {
                var u=Model.Data.upgrades[i];bool owned=Model.Has(u.id);string detail=u.description;
                ui.Row(i,u.name,detail,null,owned?"Owned":$"{u.cost} shells",()=>{if(Model.Buy(u.id)){Sound("chime");ShowUpgrades();}else Say("Check your shells and the required improvements.");},!owned&&Model.State.phase!="service");
            }
        }
        void ShowCrop(int index)
        {
            var c=Model.State.crops[index];ui.Show("The kitchen garden",$"Plot {index+1} · Water once each morning. A harvest grows after {Model.Data.cropDays} watered nights; seeds are kept from discoveries.");
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
            ui.Show("Chart a course","The harbor ferry sails freely. Choose an island.");
            ui.Row(0,"Saltleaf Shore","Home · garden · Steamstone springs","restaurant","Sail",()=>Sail(0),Model.State.phase=="explore");
            ui.Row(1,"Mistwake Isle","Cloudfruit orchard · tidekeeper's garden","cloudfruit","Sail",()=>Sail(1),Model.State.phase=="explore");
        }
        void Sail(int island){if(Model.Travel(island)){ui.Hide();world.SetArea(island,new Vector2(-10,-6));world.Burst(world.player.position,"splash");Refresh();Say(island==0?"Saltleaf Harbor":"Mistwake Isle");}}
        public void Equip(int slot)
        {
            if(activity!=null||slot<0||slot>7)return;
            if(slot==6){ShowJournal();return;}if(slot==7){ShowBag();return;}
            Model.State.equipped=slot;Model.Notify();
        }
        string Prompt(WorldPoint p)
        {
            if(p.action=="fish")return Model.State.equipped==1?"Space · Cast into water":"2 · Equip fishing rod";
            if(p.action=="crop")
            {
                var c=Model.State.crops[p.index];
                if(c.growth>=Model.Data.cropDays)return "E · Harvest";
                if(!c.planted)return Model.State.equipped==3||Model.State.equipped==4?"Space · Plant seed":"4 / 5 · Hold seeds to plant";
                return c.wateredDay==Model.State.day?"Watered today":Model.State.equipped==2?"Space · Water":"3 · Equip watering can";
            }
            if(p.action=="hunt")return "Watch the steam · Dodge the charge";
            return "E · "+p.label;
        }
        void UseTool(WorldPoint p)
        {
            if(p!=null && p.action=="crop"){UseGarden(p);return;}
            if(Model.State.equipped==1)
            {
                var water=world.WaterAt(world.player.position,facing);
                if(water.HasValue)
                {
                    var spot=world.points.First(x=>x.action=="fish"&&x.transform.IsChildOf(world.Area==0?world.saltleaf:world.mistwake));
                    spot.transform.position=water.Value;StartFishing(spot);
                }
                else Say("Stand beside the shore or pond to cast.");
                return;
            }
            if(p!=null&&(p.action=="forage"||p.action=="fruit"))Interact(p);
            else {actionUntil=Time.time+.4f;if(Model.State.equipped==2){world.Burst(world.player.position+(Vector3)facing*.65f,"splash");Sound("splash");}}
        }
        void UseGarden(WorldPoint p)
        {
            var c=Model.State.crops[p.index];bool changed=false;
            if(c.planted&&c.growth>=Model.Data.cropDays)changed=Model.Harvest(p.index);
            else if(!c.planted&&(Model.State.equipped==3||Model.State.equipped==4))changed=Model.Plant(p.index,Model.State.equipped==3?"pepperbell":"lanternroot");
            else if(c.planted&&Model.State.equipped==2)changed=Model.Water(p.index);
            if(changed){actionUntil=Time.time+.65f;world.Burst(p.transform.position,Model.State.equipped==2?"splash":"spark");Sound(Model.State.equipped==2?"splash":"chime");}
            else Say(Prompt(p).Replace("Space · ",""));
        }
        void UpdateStaff()
        {
            if(!Model.Has("staff")||world.Area!=2)return;
            if(staffOrder==null)staffOrder=Model.State.orders.FirstOrDefault(o=>o.cooked&&!o.paid);
            var employee=world.employee.transform;
            Vector3 target=staffOrder!=null?world.GuestSeat(staffOrder.number)+new Vector2(.65f,0):new Vector2(2,3);
            employee.position=Vector3.MoveTowards(employee.position,target,Time.deltaTime*3.2f);
            world.StaffDish(staffOrder==null?null:Model.Data.Dish(staffOrder.recipe).icon);
            if(staffOrder!=null&&Vector2.Distance(employee.position,target)<.1f)
            {
                staffClock+=Time.deltaTime;
                if(staffClock>.35f){Model.Serve(staffOrder.number);staffOrder=null;staffClock=0;Sound("chime");CheckClosing();}
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
            ui.Show("Harbor requests","Fulfill a request during service, then collect its reward before beginning the next day.");
            if(index>=goals.Length){ui.Paragraph("All four harbor requests are complete. Your table has brought a little of every habitat home. There are still gardens to tend and menus to try.");return;}
            var r=Model.Data.Dish(goals[index]);ui.Row(0,names[index],$"Serve {r.name} · reward {18+6*index} shells",r.icon,"Claim reward",()=>{if(Model.ClaimRequest()){Sound("chime");ShowRequests();}else Say("Serve the requested dish before collecting this reward.");});
        }
        public void ShowClosing()
        {
            if(Model.State.phase=="service")return;
            ui.Show(Model.State.phase=="closing"?"The last guest has gone":"A quiet evening",$"Day {Model.State.day} · Today's service earned {Model.State.earned} shells. Ingredients keep overnight; tomorrow brings fresh forage.");
            ui.Row(0,"Your harbor requests","Collect a completed request reward before sleeping.","dish-fish","Requests",ShowRequests);
            ui.Row(1,"Invest in tomorrow","Restore tools, the terrace, and the skiff.","crate","Workshop",ShowUpgrades);
            ui.Row(2,"The kitchen garden","Watered plants grow overnight. Wild sources recover each morning.","pepperbell",null,null);
            ui.FooterButton("Begin the next day",()=>{if(Model.NextDay()){ui.Hide();world.SetArea(0,new Vector2(-6,-2.5f));Refresh();Say("A fresh morning. Your journal is waiting.");}});
        }
        public void ShowPause()
        {
            ui.Show("Take a breath","Progress saves after discoveries, cooking, serving, upgrades, and day changes. Relaxed timing is on by default.");
            ui.Row(0,"Challenge",Model.State.relaxed?"More time to recover a fishing line and finish cooking.":"Faster cooking and a less forgiving fishing line.",null,Model.State.relaxed?"Relaxed":"Standard",()=>{Model.State.relaxed=!Model.State.relaxed;Model.Notify();ShowPause();});
            ui.Row(1,"Sound","Original ambient loop and quiet interaction cues.",null,Model.State.muted?"Muted":"Sound on",()=>{Model.State.muted=!Model.State.muted;Model.Notify();ShowPause();});
            ui.Row(2,"Display",Screen.fullScreen?"Fills your display":"Resizable window",null,Screen.fullScreen?"Windowed":"Fullscreen",()=>{if(Screen.fullScreen)Screen.SetResolution(Mathf.Min(1280,Display.main.systemWidth-80),Mathf.Min(720,Display.main.systemHeight-80),FullScreenMode.Windowed);else Screen.SetResolution(Display.main.systemWidth,Display.main.systemHeight,FullScreenMode.FullScreenWindow);ShowPause();});
            ui.Row(3,"Start a new journey","Your previous save is archived before resetting.",null,"New journey",ShowReset);
            ui.Row(4,"Leave the table","Save and close the game.",null,"Quit",()=>{Save();Application.Quit();});
        }
        void ShowReset()
        {
            ui.Show("Start a new journey?","This resets the restaurant, discoveries, upgrades, and current service. A copy of the old save will be preserved.");
            ui.FooterButton("Archive old save and restart",()=>{Saves.ArchiveAndReset();UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);});
        }
        void OnApplicationQuit(){if(initialized)Save();}
        void OnApplicationPause(bool paused){if(paused&&initialized)Save();}
    }
}
