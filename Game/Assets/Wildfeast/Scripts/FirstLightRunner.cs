using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UI;
namespace Wildfeast
{
    // Opt-in end-to-end diagnostic, with isolated save directory and virtual input devices.
    public class FirstLightRunner : MonoBehaviour
    {
        GameController game;Keyboard keyboard;Mouse mouse;string output;int failed;readonly List<string> checks=new List<string>(),errors=new List<string>();
        IEnumerator Start()
        {
            game=GetComponent<GameController>();InputSystem.settings.backgroundBehavior=InputSettings.BackgroundBehavior.IgnoreFocus;
            keyboard=InputSystem.AddDevice<Keyboard>();mouse=InputSystem.AddDevice<Mouse>();Application.logMessageReceived+=Log;
            var args=Environment.GetCommandLineArgs();int a=Array.IndexOf(args,"--test-output");output=args[a+1];Directory.CreateDirectory(output);yield return new WaitForSeconds(.5f);
            Check(game.FrontEnd.ScreenName=="Title"&&game.FrontEnd.Active,"Normal startup opens title screen");Capture("01-title.png");
            yield return Click("Options");Check(game.FrontEnd.ScreenName=="Options","Options button opens front end preferences");Capture("02-options.png");yield return Click("Back");
            yield return Click("New");Check(game.FrontEnd.ScreenName=="Character","New opens character creation in an empty slot");
            game.FrontEnd.NameField.text="Juniper";game.FrontEnd.Change(0,3);game.FrontEnd.Change(1,2);game.FrontEnd.Change(2,4);game.FrontEnd.Change(3,2);game.FrontEnd.Change(4,2);game.FrontEnd.Change(5,4);game.FrontEnd.Change(6,1);yield return null;Capture("03-character.png");
            yield return Click("Begin journey");Check(game.FrontEnd.ScreenName=="Introduction","Confirm creates a save and begins the world introduction");
            Check(game.Model.State.avatar.name=="Juniper"&&game.Model.State.avatar.skin==3&&game.Model.State.avatar.hairStyle==5,"Name, skin and hair survive character creation");
            Check(game.FrontEnd.Slots.Exists(0)&&!game.Model.State.introSeen,"New journey is checkpointed before its first introduction");
            yield return new WaitForSeconds(.6f);Capture("04-intro-harbor.png");
            InputSystem.QueueStateEvent(keyboard,new KeyboardState(Key.Space));yield return null;InputSystem.QueueStateEvent(keyboard,new KeyboardState());yield return null;
            Check(game.ActiveActivity==null,"World tool input is blocked during introduction");
            yield return new WaitForSeconds(8);Capture("05-intro-creature.png");yield return new WaitForSeconds(9);Capture("05b-intro-fishing.png");yield return new WaitForSeconds(8);Capture("05c-intro-foraging.png");yield return new WaitForSeconds(8);Capture("05d-intro-cooking.png");yield return new WaitForSeconds(7);Capture("06-intro-supper.png");
            yield return new WaitForSeconds(9);Check(!game.FrontEnd.Active&&game.Model.State.introSeen,"Full introduction completes into a playable saved world");
            Check(game.world.player.GetComponent<Rigidbody2D>().simulated,"Player physics returns after the introduction");Capture("07-custom-player.png");
            Check(game.Model.State.avatar.face==2&&game.Model.State.avatar.shirt==2&&game.Model.State.avatar.pants==4&&game.Model.State.avatar.boots==1,"All clothing and face categories are saved");
            game.Model.State.coins=77;game.Model.Notify();game.world.SetArea(6,new Vector2(1,0));yield return game.SleepRoutine();string first=File.ReadAllText(game.FrontEnd.Slots.PathFor(0));
            game.FrontEnd.Title();yield return null;yield return Click("New");game.FrontEnd.NameField.text="Basil";yield return Click("Begin journey");yield return Click("Skip introduction");yield return new WaitForSeconds(1.3f);
            Check(game.FrontEnd.Slots.Exists(1)&&game.Model.State.avatar.name=="Basil"&&game.Model.State.coins==0,"Second new journey uses another slot and fresh progress");
            Check(File.ReadAllText(game.FrontEnd.Slots.PathFor(0))==first,"Creating another journey leaves the first save byte-identical");
            game.FrontEnd.Title();yield return null;yield return Click("Load");Capture("08-load-slots.png");
            yield return ClickPrefix("1   Juniper");Check(!game.FrontEnd.Active&&game.Model.State.avatar.name=="Juniper"&&game.Model.State.coins==77,"Load returns to the selected journey and skips a completed introduction");
            game.Model.State.equipped=1;game.Model.Notify();
            foreach(var island in Archipelago.Islands)
            {
                game.Model.Travel(island.id);game.world.SetArea(island.id,island.arrival);yield return null;var fish=game.world.points.First(p=>p.action=="fish"&&p.transform.IsChildOf(game.world.IslandRoot(island.id)));
                Vector2 shore=FindShore(fish.transform.position,island.id);game.world.SetArea(island.id,shore);game.Model.Notify();yield return null;int before=game.Model.Count(fish.item,true);game.Interact(fish);yield return null;
                Check(game.ActiveActivity=="fish",island.name+" supports a shoreline cast");
                yield return new WaitForSeconds(.22f);Check(game.world.PlayerPose=="cast",island.name+" uses an animated player cast");
                float deadline=Time.time+25;while(game.ActiveActivity=="fish"&&Time.time<deadline){InputSystem.QueueStateEvent(keyboard,game.CatchZone<game.FishPosition?new KeyboardState(Key.Space):new KeyboardState());if(game.FishingPhase=="Tracking"&&island.id==4)CaptureOnce("09-fishing-moonfen.png");if(game.FishingPhase=="Reel")CaptureOnce("10-retrieval.png");yield return null;}
                InputSystem.QueueStateEvent(keyboard,new KeyboardState());yield return null;Check(game.Model.Count(fish.item,true)==before+1,island.name+" lands exactly one native fish after retrieval");
            }
            Check(game.Model.State.bag.Select(b=>b.id).Distinct().Count()>=5,"Fishing across islands produces distinct ingredients");
            game.Model.Deposit();game.Model.Travel(1);var discovery=game.world.points.First(p=>p.action=="discovery"&&p.transform.IsChildOf(game.world.IslandRoot(1)));
            game.world.SetArea(1,(Vector2)discovery.transform.position+Vector2.down);game.Model.Notify();int water=game.Model.State.water;game.GetComponent<IslandDiscoveries>().Use(discovery,2);
            Check(game.Model.State.landmarks.Contains(discovery.source)&&game.Model.State.water==water-1,"Water awakens the remote rain kettle and records its discovery");
            Check(game.Model.State.recipes.Contains("custard"),"Discovery unlocks its restaurant recipe");game.ui.Hide();int count=game.Model.Count(discovery.item,true);game.GetComponent<IslandDiscoveries>().Use(discovery,2);Check(game.Model.Count(discovery.item,true)==count,"A remembered discovery cannot duplicate its reward");game.ui.Hide();yield return null;Capture("11-outer-orchard.png");
            Check(errors.Count==0,"No runtime errors occurred during title, creation, intro, slots and five-island fishing");File.WriteAllLines(Path.Combine(output,"checks.txt"),checks);File.WriteAllText(Path.Combine(output,"result.txt"),$"{checks.Count-failed}/{checks.Count} passed\n");if(errors.Count>0)File.WriteAllLines(Path.Combine(output,"runtime-errors.txt"),errors);
            Debug.Log("FIRST LIGHT "+(failed==0?"PASS":"FAIL")+" "+checks.Count+" checks");Application.Quit(failed==0?0:1);
        }
        Vector2 FindShore(Vector2 near,int area)
        {for(float radius=.5f;radius<8;radius+=.3f)for(int k=0;k<32;k++){var p=near+new Vector2(Mathf.Cos(k*Mathf.PI/16),Mathf.Sin(k*Mathf.PI/16))*radius;if(!WorldView.Water(p,area)&&game.world.WaterAt(p,(near-p).normalized).HasValue&&!Physics2D.OverlapCircle(p+Vector2.up*.15f,.17f))return p;}return Archipelago.Get(area).arrival;}
        IEnumerator Click(string name)=>ClickButton(Find(name,false));IEnumerator ClickPrefix(string name)=>ClickButton(Find(name,true));
        Button Find(string name,bool prefix)=>game.ui.GetComponentsInChildren<Button>().FirstOrDefault(b=>b.gameObject.activeInHierarchy&&(prefix?b.name.StartsWith(name):b.name==name));
        IEnumerator ClickButton(Button button)
        {
            if(!button){Check(false,"Expected a visible front end button");yield break;}
            var rect=button.GetComponent<RectTransform>();Vector2 p=RectTransformUtility.WorldToScreenPoint(null,rect.TransformPoint(rect.rect.center));
            InputSystem.QueueStateEvent(mouse,new MouseState{position=p});yield return null;yield return null;
            InputSystem.QueueStateEvent(mouse,new MouseState{position=p}.WithButton(MouseButton.Left));yield return null;yield return null;
            InputSystem.QueueStateEvent(mouse,new MouseState{position=p});yield return null;yield return null;yield return null;
        }
        void Check(bool pass,string message){checks.Add((pass?"PASS ":"FAIL ")+message);if(!pass)failed++;Debug.Log(checks.Last());}
        void Log(string message,string stack,LogType type){if(type==LogType.Exception||type==LogType.Error)errors.Add(message+"\n"+stack);}
        void CaptureOnce(string name){if(!File.Exists(Path.Combine(output,name)))Capture(name);}
        RenderTexture captureTarget;
        void Capture(string name)
        {
            if(!captureTarget){captureTarget=new RenderTexture(Screen.width,Screen.height,24,RenderTextureFormat.ARGB32);captureTarget.Create();}
            var camera=game.world.worldCamera;var canvas=game.ui.canvas;int sorting=canvas.sortingOrder;
            canvas.renderMode=RenderMode.ScreenSpaceCamera;canvas.worldCamera=camera;canvas.planeDistance=1;canvas.sortingOrder=32760;Canvas.ForceUpdateCanvases();
            UnityEngine.Rendering.RenderPipeline.SubmitRenderRequest(camera,new UnityEngine.Rendering.Universal.UniversalRenderPipeline.SingleCameraRequest{destination=captureTarget});
            canvas.renderMode=RenderMode.ScreenSpaceOverlay;canvas.worldCamera=null;canvas.sortingOrder=sorting;
            var previous=RenderTexture.active;RenderTexture.active=captureTarget;var image=new Texture2D(captureTarget.width,captureTarget.height,TextureFormat.RGB24,false);image.ReadPixels(new Rect(0,0,captureTarget.width,captureTarget.height),0,0);image.Apply();File.WriteAllBytes(Path.Combine(output,name),image.EncodeToPNG());Destroy(image);RenderTexture.active=previous;
        }
        void OnDestroy(){if(captureTarget){captureTarget.Release();Destroy(captureTarget);}Application.logMessageReceived-=Log;if(keyboard!=null)InputSystem.RemoveDevice(keyboard);if(mouse!=null)InputSystem.RemoveDevice(mouse);}
    }
}
