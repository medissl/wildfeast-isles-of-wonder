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
        public void Start()
        {
            if (initialized) return; initialized = true;
            var content = Content.Load();
            string saveName = Application.isEditor ? "wildfeast-editor.json" : "wildfeast.json";
            var args = Environment.GetCommandLineArgs();int arg = Array.IndexOf(args, "--save-path");
            Saves = new SaveStore(arg >= 0 && arg + 1 < args.Length ? args[arg + 1] : Path.Combine(Application.persistentDataPath, saveName));
            Model = new GameModel(content, Saves.Read(content)); body = world.player.GetComponent<Rigidbody2D>();world.Init();
            ui.Init(ShowJournal,ShowBag,ShowPause);ui.Closed=CancelActivity;
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
            ambience.volume=Model.State.muted?0:Model.State.volume*.6f;effects.volume=Model.State.muted?0:Model.State.volume;
        }
        string Objective()
        {
            if(Model.State.phase=="service")return "Cook at the stove. Carry finished dishes to your guests.";
            if(Model.State.phase=="closing")return "Collect your request reward, then begin a new day.";
            if(Model.State.served<3)return "Fish on the southeast shore. Bring three catches home to cook.";
            if(!Model.Has("satchel"))return "Visit the workshop and buy a forager's satchel.";
            if(!Model.State.discovered.Contains("brothback"))return "Find the steam-venting Brothback at the northeast springs.";
            if(!Model.State.discovered.Contains("lanternroot"))return "Search Glowgrove in the northwest for a luminous root.";
            if(!Model.State.crops.Any(c=>c.planted))return "Plant Pepperbell beside the restaurant. Water it each morning.";
            if(!Model.Has("room"))return "Restore the terrace to welcome more guests.";
            if(!Model.Has("staff"))return "Hire Nori at the workshop to deliver your cooked dishes.";
            if(!Model.Has("boat"))return "Restore the skiff and discover Mistwake Isle.";
            if(!Model.Has("reach"))return "Buy a reach tool to harvest floating Cloudfruit.";
            if(!Model.State.storySeen)return "Read the tidekeeper's letter on Mistwake Isle.";
            return "Fill your journal, fulfill harbor requests, and build tomorrow's menu.";
        }
        void Update()
        {
            if(!initialized)return;
            if(Time.unscaledTime>toastUntil && SaveError==null)ui.Message("");
            if(GameInput.Back)
            {
                if(ui.PageOpen)ui.Hide();else if(hunting!=null)EndHunt();else ShowPause();
            }
            if(GameInput.Journal && activity==null && hunting==null) { if(ui.PageOpen)ui.Hide();else ShowJournal(); }
            movement=ui.PageOpen?Vector2.zero:GameInput.Move;
            world.Animate(movement);
            var point=world.Nearest();
            ui.prompt.text=!ui.PageOpen && point!=null ? "E  ·  "+point.label : "";
            if(activity=="fish")UpdateFishing();
            else if(activity=="cook")UpdateCooking();
            if(hunting!=null&&!ui.PageOpen)UpdateHunt(point);
            else if(!ui.PageOpen && GameInput.Interact && point!=null)Interact(point);
            if(Model.State.phase=="service" && Model.Has("staff") && !ui.PageOpen)
            {
                staffTimer+=Time.deltaTime;
                var ready=Model.State.orders.FirstOrDefault(o=>o.cooked&&!o.paid);
                if(ready!=null && staffTimer>1.2f) { staffTimer=0;Model.Serve(ready.number);Say("Nori delivered "+Model.Data.Dish(ready.recipe).name+".");CheckClosing(); }
            }
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
                case "fish": StartFishing(p);break;
                case "forage":Gather(p,2);break;
                case "fruit":if(!Model.Has("reach"))Say("Cloudfruit floats beyond your reach. Visit the harbor workshop for a reach tool.");else Gather(p,2);break;
                case "hunt":StartHunt(p);break;
                case "crop":ShowCrop(p.index);break;
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
                    if(Model.Serve(o.number)){Sound("chime");Say(o.customer.Split('·')[0]+" enjoyed the dish.");CheckClosing();}
                    else Say("Cook this guest's order at the stove first.");break;
                case "bed":if(Model.State.phase=="service")Say("There are still guests waiting.");else ShowClosing();break;
                case "requests":ShowRequests();break;
            }
        }
        void Gather(WorldPoint p,int quantity)
        {
            if(Model.Gather(p.item,quantity,p.source)) { Sound("chime");Say("Gathered "+quantity+" "+Model.Data.Item(p.item).name+". Your journal has the details."); }
            else Say(Model.State.harvested.Contains(p.source)?"Let this source recover until tomorrow.":Model.State.phase!="explore"?"Gather ingredients before opening the restaurant.":"Your satchel is full. Store ingredients at home.");
        }
        void StartFishing(WorldPoint p)
        {
            if(Model.State.phase!="explore" || Model.BagCount>=Model.Capacity){Say("Make room in your satchel before casting.");return;}
            fishingSpot=p;elapsed=0;tension=.4f;progress=0;danger=0;
            ui.StartMini("Saltleaf shallows","Leafgill folds its leaves under pressure. Reel gently and let it rest.","Wait for the bite. Then hold Space to reel, release to give slack.\nKeep tension inside the green band.",true);activity="fish";Sound("splash");
        }
        void UpdateFishing()
        {
            elapsed+=Time.deltaTime;
            if(elapsed<1.5f){ui.Mini(.4f,0,"Waiting for a bite…","Watch the float.");return;}
            bool pulling=GameInput.Hold||ui.MouseHolding;
            tension=Mathf.Clamp01(tension+Time.deltaTime*(pulling?.43f:-.28f)+Time.deltaTime*Mathf.Sin(elapsed*2.5f)*.13f);
            if(tension>.25f && tension<.72f)progress+=Time.deltaTime/Model.Data.fishDuration;
            danger=tension>.95f?danger+Time.deltaTime:0;
            ui.Mini(tension,progress,pulling?"Reeling in":"Giving slack",$"{Mathf.RoundToInt(progress*100)}% landed · "+(tension>.72f?"Too tight — release!":tension<.25f?"Too loose — reel!":"Good tension"));
            if(danger>(Model.State.relaxed?2f:.65f) || elapsed>45)
            {activity=null;ui.Hide();Say("The Leafgill slipped away. You can cast again.");return;}
            if(progress>=1)
            {var spot=fishingSpot;activity=null;ui.Hide();if(Model.Gather(spot.item)){Sound("chime");Say("Caught a Leafgill! Bring it home or cast again.");}}
        }
        public void StartCooking(int order)
        {
            var o=Model.State.orders.FirstOrDefault(x=>x.number==order);
            if(o==null || o.cooked || o.paid || !Model.CanCook(o.recipe))return;
            cookingOrder=order;elapsed=0;ui.MiniPressed=false;
            ui.StartMini(Model.Data.Dish(o.recipe).name,"Listen for the sizzle. Finish while the needle crosses the green band.","Press Space or Finish cooking inside the green band.\nA mistimed dish is still edible, but earns a smaller tip.",false);activity="cook";Sound("cook");
        }
        void UpdateCooking()
        {
            elapsed+=Time.deltaTime;
            float needle=Mathf.PingPong(elapsed*(Model.State.relaxed?.48f:.8f),1);
            ui.Mini(needle,elapsed/Model.Data.cookDuration,"On the stove",$"{Mathf.Max(0,Mathf.CeilToInt(Model.Data.cookDuration-elapsed))} seconds remaining");
            if((elapsed>.2f && (GameInput.Press||ui.MiniPressed)) || elapsed>=Model.Data.cookDuration)
            {
                ui.MiniPressed=false;
                int quality=elapsed>=Model.Data.cookDuration?0:needle>.43f && needle<.61f?2:needle>.25f&&needle<.8f?1:0;
                int number=cookingOrder;activity=null;ui.Hide();
                if(Model.Cook(number,quality)){Say(quality==2?"Beautifully cooked. Carry the dish to its guest.":"Dish ready. Carry it to the waiting guest.");Sound("chime");}
            }
        }
        void StartHunt(WorldPoint p)
        {
            if(Model.State.phase!="explore" || Model.State.harvested.Contains(p.source)){Say("The spring is quiet. Return tomorrow.");return;}
            if(Model.Capacity-Model.BagCount<2){Say("Leave room for two portions of stock.");return;}
            hunting=p;huntOrigin=p.transform.position;huntTimer=0;huntPhase=0;
            Say("Brothback is venting! Step aside before its charge, then collect stock while it cools.");
        }
        void UpdateHunt(WorldPoint nearest)
        {
            huntTimer+=Time.deltaTime;
            if(huntPhase==0)
            {
                ui.prompt.text="STEAM BUILDING  ·  Move away from the charge";
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
                ui.prompt.text="BROTHBACK COOLING  ·  Approach and press E to collect stock";
                if(GameInput.Interact && Vector2.Distance(world.player.position,hunting.transform.position)<1.8f)
                {var p=hunting;EndHunt();Gather(p,2);return;}
                if(huntTimer>(Model.State.relaxed?5:3)){huntTimer=0;huntPhase=0;hunting.transform.position=huntOrigin;}
            }
            if(Vector2.Distance(world.player.position,huntOrigin)>9 || world.Area!=0)EndHunt();
        }
        void EndHunt(){if(hunting){hunting.transform.position=huntOrigin;hunting.artwork.color=Color.white;}hunting=null;world.Refresh(Model);}
        void CancelActivity(){activity=null;cookingOrder=-1;ui.MiniPressed=false;}
        void Say(string text){ui.Message(text);toastUntil=Time.unscaledTime+5;}
        void Sound(string name){effects.PlayOneShot(Resources.Load<AudioClip>("Audio/"+name));}
        void CheckClosing(){if(Model.CloseService())ShowClosing();}
        public void ShowWelcome()
        {
            ui.Show("A table at the edge of wonder","Your restaurant has three tables, a tired stove, and a whole archipelago on its doorstep.");
            ui.Row(0,"Explore Saltleaf Shore","Fish in the southeast; gather Pepperbell along the path.","leafgill",null,null);
            ui.Row(1,"Cook what you discover","Return through the restaurant door to store your ingredients.","dish-wrap",null,null);
            ui.Row(2,"Open your restaurant","Choose a menu, cook guest orders, then carry each dish to its table.","table",null,null);
            ui.Row(3,"Let tomorrow grow","Spend shells at the workshop. Tend plants and discover other habitats.","pepperbell",null,null);
            ui.FooterButton("Begin your first day",()=>{Model.State.stage=1;Model.Notify();ui.Hide();});
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
            ui.Show("Welcome the evening","Your guests will choose dishes from tonight's menu. Each order has ingredients set aside in the pantry.");
            int row=0;foreach(var id in Model.State.menu)
            {
                var r=Model.Data.Dish(id);ui.Row(row++,r.name,Model.CanCook(id)?"Ingredients ready":"Missing ingredients",r.icon,null,null);
            }
            if(row>4){ui.Show("Welcome the evening","Your selected menu is ready. Guests will order only dishes you have stock to prepare.");}
            ui.FooterButton("Open the restaurant",()=>{if(Model.StartService()){ui.Hide();Say("Your guests have arrived. Head to the stove.");}else Say("Choose a stocked recipe on the menu. Harbor porridge uses your free grain.");});
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
            if(Model.State.island==1){Model.Travel(0);world.SetArea(0,new Vector2(-10,-6));Refresh();Say("Back at Saltleaf Harbor.");return;}
            ui.Show("Beyond the harbor","Mistwake Isle hides floating fruit and a letter left by its tidekeeper. Restore the skiff at the workshop first.");
            ui.Paragraph("The tidekeeper once supplied the harbor with cloud-light desserts. Her abandoned garden still grows above the mist. Bring a reach tool to harvest it.");
            ui.FooterButton(Model.Has("boat")?"Sail to Mistwake Isle":"Visit the workshop",()=>{if(!Model.Has("boat")){ShowUpgrades();return;}if(Model.Travel(1)){ui.Hide();world.SetArea(1,new Vector2(-10,-6));Refresh();Say("Mistwake Isle · Listen to the wind in the floating fruit.");}});
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
            ui.Row(2,"Save progress",SaveError==null?"Local save ready.":"Save failed: "+SaveError,null,"Save now",()=>{Save();Say(SaveError==null?"Progress saved.":"Save failed. Check disk space.");});
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
