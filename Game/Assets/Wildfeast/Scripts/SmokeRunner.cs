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
        public IEnumerator Start()
        {
            game=GetComponent<GameController>();
            InputSystem.settings.backgroundBehavior=InputSettings.BackgroundBehavior.IgnoreFocus;
            keyboard=InputSystem.AddDevice<Keyboard>("Wildfeast verification keyboard");
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
            for(int n=0;n<3;n++)
            {
                game.world.SetArea(0,(Vector2)fish.transform.position+Vector2.left*.5f);
                game.Interact(fish);
                float deadline=Time.time+25;
                while(game.ActiveActivity=="fish" && Time.time<deadline)
                {
                    InputSystem.QueueStateEvent(keyboard,game.FishingTension<.55f?new KeyboardState(Key.Space):new KeyboardState());
                    yield return null;
                }
                InputSystem.QueueStateEvent(keyboard,new KeyboardState());
                Check(game.Model.Count("leafgill",true)==n+1,"Fishing grants exactly one ingredient");
                yield return null;
            }
            var pepper=game.world.points.First(p=>p.source=="pepper-east");game.Interact(pepper);
            Check(game.Model.Count("pepperbell",true)==2,"Foraging grants two portions");game.Interact(pepper);
            Check(game.Model.Count("pepperbell",true)==2,"Harvest cannot be repeated the same day");
            game.Interact(game.world.points.First(p=>p.action=="enter"));
            Check(game.Model.BagCount==0&&game.Model.Count("leafgill")==3,"Entering home deposits the catch");
            game.Model.State.menu.Clear();game.Model.State.menu.Add("seared");
            Check(game.Model.StartService(),"Stocked menu opens service");
            yield return new WaitForSeconds(.2f);
            Capture("02-restaurant.png");
            for(int n=0;n<3;n++)
            {
                game.StartCooking(n);float deadline=Time.time+10;
                yield return new WaitForSeconds(1.05f);
                if(n==0)Capture("04-cooking.png");
                InputSystem.QueueStateEvent(keyboard,new KeyboardState(Key.Space));
                yield return null;yield return null;
                InputSystem.QueueStateEvent(keyboard,new KeyboardState());
                while(game.ActiveActivity=="cook"&&Time.time<deadline)yield return null;
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
            Check(game.Model.Tend(0),"Discovered Pepperbell can be planted");
            game.Model.NextDay();Check(game.Model.Tend(0),"Growing plant can be watered next day");
            game.Model.NextDay();Check(game.Model.Tend(0),"Two watered nights mature a crop");
            Check(game.Model.Count("pepperbell",true)==3,"Crop harvest yields three portions");
            // Domain progression is exercised in the player's real model; later content is not unlocked by editing the save.
            game.Model.Deposit();
            var hunt=game.world.points.First(p=>p.action=="hunt");
            game.world.SetArea(0,(Vector2)hunt.transform.position+new Vector2(-1,-1));
            yield return new WaitForFixedUpdate();yield return null;
            Debug.Log($"SMOKE HUNT START: chef={game.world.player.position} creature={hunt.transform.position} area={game.world.Area} phase={game.Model.State.phase} bag={game.Model.BagCount} page={game.ui.PageOpen}");
            game.Interact(hunt);float huntDeadline=Time.time+10;
            Debug.Log($"SMOKE HUNT ACTIVE: {game.HuntingPoint!=null}");
            while(game.HuntingPoint!=null&&game.HuntingPhase!=2&&Time.time<huntDeadline)yield return null;
            Debug.Log($"SMOKE HUNT END: active={game.HuntingPoint!=null} phase={game.HuntingPhase} chef={game.world.player.position}");
            Check(game.HuntingPoint!=null&&game.HuntingPhase==2,"Brothback telegraphs and charges before cooling");
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
            yield return new WaitForSeconds(1.6f);
            Check(game.Model.State.orders[0].paid,"Nori automatically delivers a finished dish");
            foreach(var order in game.Model.State.orders.Where(o=>!o.paid)){game.Model.Cook(order.number,1);game.Model.Serve(order.number);}
            game.Model.CloseService();game.Model.NextDay();game.ui.Hide();
            Check(game.Model.Travel(1),"Restored boat opens the second island");
            game.world.SetArea(1,new Vector2(-6,-2.5f));game.Interact(game.world.points.First(p=>p.action=="story"));game.ui.Hide();
            Check(game.Model.State.storySeen,"Mistwake has a persisted local story");
            game.Interact(game.world.points.First(p=>p.action=="fruit"));
            Check(game.Model.Count("cloudfruit",true)==2&&game.Model.State.recipes.Contains("cloud"),"Reach tool unlocks floating fruit and its recipe");
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
        void MakeKeyboardCurrent(){keyboard?.MakeCurrent();}
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
