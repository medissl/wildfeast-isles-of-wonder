using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

namespace Wildfeast
{
    // Opt-in diagnostic journey in the actual Windows player. Never runs in ordinary play.
    public class SmokeRunner : MonoBehaviour
    {
        GameController game;
        string output;
        int checks;
        bool runtimeError;
        Keyboard keyboard;
        Mouse testMouse;
        public IEnumerator Start()
        {
            game=GetComponent<GameController>();
            InputSystem.settings.backgroundBehavior=InputSettings.BackgroundBehavior.IgnoreFocus;
            keyboard=InputSystem.AddDevice<Keyboard>("Wildfeast verification keyboard");
            testMouse=InputSystem.AddDevice<Mouse>("Wildfeast verification mouse");InputSystem.QueueStateEvent(testMouse,new MouseState{position=new Vector2(0,0)});
            InputSystem.onAfterUpdate+=MakeKeyboardCurrent;
            string[] args=Environment.GetCommandLineArgs();int i=Array.IndexOf(args,"--test-output");
            output=i>=0?args[i+1]:Application.persistentDataPath;
            Directory.CreateDirectory(output);
            Application.logMessageReceived+=Error;
            yield return new WaitForSeconds(.4f);
            game.ui.Hide();
            game.Model.State.stage=1;game.Model.Notify();
            var before=game.world.player.position;
            InputSystem.QueueStateEvent(keyboard,new KeyboardState(Key.D));
            yield return new WaitForSeconds(.35f);
            Capture("00-input.png");
            yield return new WaitForSeconds(.2f);
            Debug.Log($"SMOKE INPUT: key={keyboard.dKey.isPressed} motion={GameInput.Move} page={game.ui.PageOpen} simulated={game.world.player.GetComponent<Rigidbody2D>().simulated} pos={game.world.player.position}");
            InputSystem.QueueStateEvent(keyboard,new KeyboardState());
            Check(game.world.player.position.x>before.x+.4f,$"Input System movement ({before.x:F2} to {game.world.player.position.x:F2})");
            yield return new WaitForSeconds(.2f);
            Capture("01-saltleaf.png");
            var fish=game.world.points.First(p=>p.action=="fish"&&p.transform.IsChildOf(game.world.saltleaf));
            Check(game.Model.Travel(1)&&!game.Model.Has("boat"),"Island travel is available without purchases");game.Model.Travel(0);
            yield return Tap(Key.Digit2);
            Check(game.Model.State.equipped==1,"Tool belt selects the fishing rod with number input");
            InputSystem.QueueStateEvent(testMouse,new MouseState{scroll=new Vector2(0,-120)});yield return null;yield return null;
            Check(game.Model.State.equipped==2,"Mouse wheel scrolls to the watering can");
            InputSystem.QueueStateEvent(testMouse,new MouseState());yield return null;yield return Tap(Key.Digit2);
            Check(game.world.worldCamera.rect==new Rect(0,0,1,1),"Camera uses the full viewport instead of a windowboxed world");
            for(int n=0;n<3;n++)
            {
                game.world.SetArea(0,(Vector2)fish.transform.position+Vector2.left*.5f);
                if(n==0){yield return Tap(Key.Space);Check(game.ActiveActivity=="fish","Equipped rod casts into nearby water without a fish activation menu");Capture("06-fishing.png");}
                else game.Interact(fish);
                float deadline=Time.time+25;
                while(game.ActiveActivity=="fish" && Time.time<deadline)
                {
                    InputSystem.QueueStateEvent(keyboard,game.FishingTension<.55f?new KeyboardState(Key.Space):new KeyboardState());
                    yield return null;
                }
                InputSystem.QueueStateEvent(keyboard,new KeyboardState());
                Debug.Log($"FISH END activity={game.ActiveActivity} tension={game.FishingTension} bag={game.Model.Count("leafgill",true)} time={Time.time}");
                Check(game.Model.Count("leafgill",true)==n+1,"Fishing grants exactly one ingredient");
                yield return null;
            }
            var pepper=game.world.points.First(p=>p.source=="pepper-east");game.Interact(pepper);
            Check(game.Model.Count("pepperbell",true)==2,"Foraging grants two portions");game.Interact(pepper);
            Check(game.Model.Count("pepperbell",true)==2,"Harvest cannot be repeated the same day");
            game.Interact(game.world.points.First(p=>p.action=="enter"));
            Check(game.Model.BagCount==0&&game.Model.Count("leafgill")==3,"Entering home deposits the catch");
            game.Model.State.menu.Clear();game.Model.State.menu.Add("seared");
            game.Interact(game.world.points.First(p=>p.action=="service"&&p.transform.IsChildOf(game.world.saltleaf)));
            Check(game.ui.PageOpen&&game.ui.title.text=="Open the Harbor Table","Outdoor opening sign leads directly to the restaurant");
            var openButton=game.ui.rows.GetComponentInChildren<UnityEngine.UI.Button>();
            yield return Click(RectTransformUtility.WorldToScreenPoint(null,openButton.transform.position));
            Check(game.Model.State.phase=="service"&&!game.ui.PageOpen,"Clicking the OPEN button starts stocked service");
            yield return new WaitForSeconds(.2f);
            Capture("02-restaurant.png");
            for(int n=0;n<3;n++)
            {
                game.StartCooking(n);
                for(int cut=0;cut<6;cut++)yield return Tap(cut%2==0?Key.A:Key.D);
                Check(game.CookingStep==1,"Alternating chopping prepares the ingredients");
                if(n==0)Capture("04-cooking.png");
                float deadline=Time.time+10;float nextTurn=Time.time;
                while(game.CookingStep==1&&Time.time<deadline)
                {
                    if(game.CookingHeat>.6f)yield return Tap(Key.A);
                    else if(Time.time>nextTurn){yield return Tap(Key.Space);nextTurn=Time.time+1.2f;}
                    else yield return null;
                }
                Check(game.CookingStep==2,"Managing stove heat leads to plating");
                if(n==0)Capture("07-plating.png");
                if(n==0)
                {
                    for(int garnish=0;garnish<3;garnish++)
                    {
                        var source=game.ui.cookingBoard.Find("Garnish "+garnish).GetComponent<RectTransform>();
                        var target=game.ui.cookingBoard.Find("Ingredient").GetComponent<RectTransform>();
                        yield return Drag(RectTransformUtility.WorldToScreenPoint(null,source.position),RectTransformUtility.WorldToScreenPoint(null,target.position));
                    }
                    Check(game.ActiveActivity!="cook","Pointer drag places all garnishes onto the plate");
                }
                else {yield return Tap(Key.Digit1);yield return Tap(Key.Digit2);yield return Tap(Key.Digit3);}
                Check(game.Model.State.orders[n].cooked,"Cooking interaction resolves an order");
                game.Interact(game.world.points.First(p=>p.action=="serve"&&p.index==n));game.ui.Hide();
                Check(game.Model.State.orders[n].paid,"Delivering the dish pays its order");
            }
            Check(game.Model.State.phase=="closing","All guests served closes the service");
            Check(game.Model.Buy("satchel"),"First service funds a useful upgrade");
            Check(game.Model.Capacity==16,"Satchel upgrade doubles carrying capacity");
            Check(game.Model.ClaimRequest(),"First request reward is claimable");
            Check(!game.Model.ClaimRequest(),"Request rewards cannot be duplicated");
            game.Model.NextDay();game.world.SetArea(0,new Vector2(-6,-2.5f));
            var plot=game.world.points.First(p=>p.action=="crop"&&p.index==0);
            game.world.SetArea(0,(Vector2)plot.transform.position+Vector2.down*.45f);
            yield return Tap(Key.Digit4);yield return Tap(Key.Space);
            Check(game.Model.State.crops[0].planted&&game.Model.State.crops[0].wateredDay==-1,"Held seed plants visibly and leaves watering separate");
            yield return Tap(Key.Digit3);yield return Tap(Key.Space);
            Check(game.Model.State.crops[0].wateredDay==game.Model.State.day,"Watering can waters the planted bed through tool input");
            Capture("08-gardening.png");
            game.Model.NextDay();game.Model.Water(0);game.Model.NextDay();game.Model.Harvest(0);
            Check(game.Model.Count("pepperbell",true)==3,"Crop harvest yields three portions");
            // Domain progression is exercised in the player's real model; later content is not unlocked by editing the save.
            game.Model.Deposit();
            var hunt=game.world.points.First(p=>p.action=="hunt");
            game.world.SetArea(0,(Vector2)hunt.transform.position+new Vector2(-1,-1));
            yield return new WaitForFixedUpdate();yield return null;
            Debug.Log($"SMOKE HUNT START: chef={game.world.player.position} creature={hunt.transform.position} area={game.world.Area} phase={game.Model.State.phase} bag={game.Model.BagCount} page={game.ui.PageOpen}");
            yield return null;float huntDeadline=Time.time+10;
            Debug.Log($"SMOKE HUNT ACTIVE: {game.HuntingPoint!=null}");
            while(game.HuntingPoint!=null&&game.HuntingPhase!=2&&Time.time<huntDeadline)yield return null;
            Debug.Log($"SMOKE HUNT END: active={game.HuntingPoint!=null} phase={game.HuntingPhase} chef={game.world.player.position}");
            Check(game.HuntingPoint!=null&&game.HuntingPhase==2,"Brothback detects proximity, telegraphs and charges naturally");
            Check(Vector2.Distance(game.world.player.position,new Vector2(7,3))<2,"Charge contact applies one bounded push instead of repeated frame-based knockback");
            game.world.player.position=(Vector2)hunt.transform.position+Vector2.left*.5f;
            game.world.player.GetComponent<Rigidbody2D>().position=game.world.player.position;
            yield return new WaitForFixedUpdate();
            InputSystem.QueueStateEvent(keyboard,new KeyboardState(Key.E));yield return null;yield return null;
            InputSystem.QueueStateEvent(keyboard,new KeyboardState());
            Check(game.Model.Count("brothback",true)==2,"Cooling hunt rewards stock through real interaction input");
            game.Model.Gather("lanternroot",2,"smoke-grove");game.Model.Deposit();
            Check(game.Model.State.recipes.Contains("broth")&&game.Model.State.recipes.Contains("lantern"),"Discoveries unlock recipes");
            for(int day=0;day<8;day++)
            {
                for(int j=0;j<5;j++)game.Model.Gather("leafgill");game.Model.Deposit();
                game.Model.State.menu.Clear();game.Model.State.menu.Add("seared");game.Model.StartService();
                foreach(var o in game.Model.State.orders){game.Model.Cook(o.number,2);game.Model.Serve(o.number);}
                game.Model.CloseService();game.Model.NextDay();
            }
            Check(game.Model.Buy("room")&&game.Model.Buy("staff")&&game.Model.Buy("boat")&&game.Model.Buy("reach"),"Service earnings fund all restaurant and travel improvements");
            game.Model.Gather("leafgill",5);game.Model.Deposit();game.world.SetArea(2,new Vector2(0,-3));game.Model.StartService();
            Check(game.Model.State.orders.Count==5&&game.world.terrace.activeInHierarchy&&game.world.employee.activeInHierarchy,"Restored terrace adds two guests and a visible employee");
            Capture("05-expanded-restaurant.png");game.Model.Cook(0,2);
            float staffDeadline=Time.time+12;while(!game.Model.State.orders[0].paid&&Time.time<staffDeadline)yield return null;
            Check(game.Model.State.orders[0].paid,"Nori automatically delivers a finished dish");
            foreach(var order in game.Model.State.orders.Where(o=>!o.paid)){game.Model.Cook(order.number,1);game.Model.Serve(order.number);}
            game.Model.CloseService();game.Model.NextDay();game.ui.Hide();
            Check(game.Model.Travel(1),"Restored boat opens the second island");
            game.world.SetArea(1,new Vector2(-6,-2.5f));game.Interact(game.world.points.First(p=>p.action=="story"));game.ui.Hide();
            Check(game.Model.State.storySeen,"Mistwake has a persisted local story");
            game.Interact(game.world.points.First(p=>p.action=="fruit"));
            Check(game.Model.Count("cloudfruit",true)==3&&game.Model.State.recipes.Contains("cloud"),"Botanical gloves increase fruit yield and discovery unlocks its recipe");
            yield return new WaitForSeconds(.5f);Capture("03-mistwake.png");
            var loaded=game.Saves.Read(game.Model.Data);
            Check(loaded.upgrades.Count==5&&loaded.storySeen&&loaded.bag.Any(a=>a.id=="cloudfruit"),"Player saves survive a disk round trip");
            yield return new WaitForSeconds(.5f);
            var frameTimes=new List<float>();
            for(int frame=0;frame<180;frame++){RenderFrame();yield return null;frameTimes.Add(Time.unscaledDeltaTime*1000);}
            frameTimes.Sort();
            File.WriteAllText(Path.Combine(output,"performance.txt"),$"Resolution: {Screen.width} x {Screen.height}\nRenderer: {SystemInfo.graphicsDeviceName}\n180-frame offscreen URP render-request sample on Mistwake, including UI setup. Hidden-window diagnostics; not a display FPS claim.\nMean frame: {frameTimes.Average():F2} ms\nP95 frame: {frameTimes[(int)(frameTimes.Count*.95f)]:F2} ms\nUnity allocated: {UnityEngine.Profiling.Profiler.GetTotalAllocatedMemoryLong()/1048576f:F1} MiB\n");
            Check(!runtimeError,"No logged runtime errors or exceptions");
            File.WriteAllText(Path.Combine(output,"smoke-result.txt"),$"PASS: {checks} integrated checks in the Windows player. Virtual keyboard events through Unity Input System exercise movement, fishing, cooking and hunt collection. Service, crop growth, upgrades, visible expansion, staff delivery, second island, story and disk save/load are checked. Repeated earning cycles use domain calls and area positioning is scripted. Human game feel and a physical controller remain untested.\n");
            Application.logMessageReceived-=Error;
            InputSystem.onAfterUpdate-=MakeKeyboardCurrent;
            Application.Quit(0);
        }
        void MakeKeyboardCurrent(){keyboard?.MakeCurrent();testMouse?.MakeCurrent();}
        IEnumerator Tap(Key key)
        {InputSystem.QueueStateEvent(keyboard,new KeyboardState(key));yield return null;yield return null;InputSystem.QueueStateEvent(keyboard,new KeyboardState());yield return null;yield return null;}
        IEnumerator Drag(Vector2 start,Vector2 end)
        {
            InputSystem.QueueStateEvent(testMouse,new MouseState{position=start});yield return null;yield return null;
            InputSystem.QueueStateEvent(testMouse,new MouseState{position=start}.WithButton(MouseButton.Left));yield return null;yield return null;
            for(int i=1;i<=8;i++){InputSystem.QueueStateEvent(testMouse,new MouseState{position=Vector2.Lerp(start,end,i/8f)}.WithButton(MouseButton.Left));yield return null;}
            InputSystem.QueueStateEvent(testMouse,new MouseState{position=end});yield return null;yield return null;yield return null;
        }
        IEnumerator Click(Vector2 position)
        {
            InputSystem.QueueStateEvent(testMouse,new MouseState{position=position});yield return null;yield return null;
            InputSystem.QueueStateEvent(testMouse,new MouseState{position=position}.WithButton(MouseButton.Left));yield return null;yield return null;
            InputSystem.QueueStateEvent(testMouse,new MouseState{position=position});yield return null;yield return null;yield return null;
        }
        RenderTexture captureTarget;
        void RenderFrame()
        {
            if(!captureTarget){captureTarget=new RenderTexture(Screen.width,Screen.height,24,RenderTextureFormat.ARGB32);captureTarget.Create();}
            var camera=game.world.worldCamera;var canvas=game.ui.canvas;
            int oldSorting=canvas.sortingOrder;
            canvas.renderMode=RenderMode.ScreenSpaceCamera;canvas.worldCamera=camera;canvas.planeDistance=1;
            canvas.sortingOrder=32760;
            Canvas.ForceUpdateCanvases();
            UnityEngine.Rendering.RenderPipeline.SubmitRenderRequest(camera,new UnityEngine.Rendering.Universal.UniversalRenderPipeline.SingleCameraRequest{destination=captureTarget});
            canvas.renderMode=RenderMode.ScreenSpaceOverlay;canvas.worldCamera=null;canvas.sortingOrder=oldSorting;
        }
        void Capture(string name)
        {
            RenderFrame();var previous=RenderTexture.active;RenderTexture.active=captureTarget;
            var image=new Texture2D(captureTarget.width,captureTarget.height,TextureFormat.RGB24,false);image.ReadPixels(new Rect(0,0,captureTarget.width,captureTarget.height),0,0);image.Apply();
            File.WriteAllBytes(Path.Combine(output,name),image.EncodeToPNG());Destroy(image);RenderTexture.active=previous;
        }
        void OnDestroy(){if(captureTarget){captureTarget.Release();Destroy(captureTarget);}InputSystem.onAfterUpdate-=MakeKeyboardCurrent;}
        void Check(bool condition,string label)
        {
            if(!condition){File.WriteAllText(Path.Combine(output,"smoke-result.txt"),"FAIL: "+label);Debug.LogError("SMOKE FAILED: "+label);Application.Quit(1);throw new Exception(label);}
            checks++;Debug.Log("SMOKE PASS: "+label);
        }
        void Error(string text,string stack,LogType type)
        {
            if(type!=LogType.Exception&&type!=LogType.Error)return;
            runtimeError=true;
            File.AppendAllText(Path.Combine(output,"runtime-errors.txt"),text+"\n"+stack+"\n");
        }
    }
}
