using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
namespace Wildfeast
{
    // Opt-in isolated real-player diagnostics; positioning is scripted, actions use pointer/keyboard events.
    public class LivingRunner:MonoBehaviour
    {
        GameController game;Mouse mouse;Keyboard keyboard;string output;int checks;readonly List<string> errors=new List<string>();RenderTexture target;
        IEnumerator Start()
        {
            game=GetComponent<GameController>();var args=Environment.GetCommandLineArgs();output=args[Array.IndexOf(args,"--test-output")+1];Directory.CreateDirectory(output);
            Application.logMessageReceived+=Log;InputSystem.settings.backgroundBehavior=InputSettings.BackgroundBehavior.IgnoreFocus;mouse=InputSystem.AddDevice<Mouse>();keyboard=InputSystem.AddDevice<Keyboard>();InputSystem.onAfterUpdate+=Current;
            yield return new WaitForSeconds(.5f);game.ui.Hide();game.Model.State.stage=1;game.Model.Notify();
            Check(game.world.islands.Length==6&&Districts.Ports.Length==3,"Six outdoor districts grouped into three islands");
            Check(!game.world.GetComponentsInChildren<SpriteRenderer>(true).Any(sr=>sr.name=="fountain"),"Unmotivated restaurant fountain removed");
            Check(WorldView.ItemArt("brothback").rect.width==32&&WorldView.ItemArt("brothback")!=WorldView.Art("brothback"),"Broth ingredient is a compact jar");
            game.Model.Gather("brothback");game.Equip(0);game.Model.Notify();int brothSlot=Array.FindIndex(game.Model.State.slots,s=>s.id=="brothback");Check(game.ui.toolIcons[brothSlot].sprite==WorldView.ItemArt("brothback"),"Actual hotbar uses the portion icon");game.Model.Deposit();
            // Empty-hand Space is not a jump. Air tool swings animate but consume nothing.
            Vector2 clear=ClearCell(0);game.world.SetArea(0,clear);yield return new WaitForSeconds(.4f);game.Equip(0);int energy=game.Model.State.energy;Vector3 before=game.world.player.position;
            InputSystem.QueueStateEvent(keyboard,new KeyboardState(Key.Space));yield return null;InputSystem.QueueStateEvent(keyboard,new KeyboardState());yield return new WaitForSeconds(.15f);
            Check(!game.ToolBusy&&game.world.PlayerPose=="idle"&&game.world.player.position==before,"Empty Space keeps the player grounded");
            game.Equip(7);yield return ClickWorld(clear+Vector2.right);yield return new WaitForSeconds(1);Check(game.Model.State.energy==energy,"Swinging at air costs no energy");
            var tree=game.world.saltleaf.GetComponentsInChildren<HarvestNode>(true).First(n=>n.item=="wood");game.world.SetArea(0,(Vector2)tree.transform.position+Vector2.down*.9f);yield return new WaitForSeconds(.4f);
            yield return ClickWorld(tree.transform.position);Check(game.ToolBusy&&game.Model.State.energy==energy,"Tree windup does not charge before contact");yield return new WaitForSeconds(1);
            Check(tree.damage==1&&game.Model.State.energy==energy-2,"Successful chop charges exactly once");
            for(int n=0;n<2;n++){yield return ClickWorld(tree.transform.position);yield return new WaitForSeconds(1);}
            yield return new WaitForSeconds(.5f);Check(tree.art.sprite==WorldView.Art("stump")&&!tree.falling,"Fallen tree leaves its stump");Capture("01-stump.png");
            for(int n=0;n<2;n++){yield return ClickWorld(tree.transform.position);yield return new WaitForSeconds(1);}
            Check(!tree.art.sprite&&tree.colliders.All(c=>!c.enabled)&&game.Model.State.harvested.Contains(tree.source+"-stump"),"Two further chops remove the trunk and collision");
            game.world.SetArea(0,clear+Vector2.down*.8f);yield return new WaitForSeconds(.4f);game.Equip(6);energy=game.Model.State.energy;yield return ClickWorld(clear);yield return new WaitForSeconds(.8f);
            var plot=ItemInventory.Plot(game.Model.State,0,clear);Check(plot!=null&&game.Model.State.energy==energy-2,"Actual tilling charges two energy");Capture("02-soil.png");
            energy=game.Model.State.energy;yield return ClickWorld(clear);yield return new WaitForSeconds(.8f);Check(game.Model.State.energy==energy,"Repeated tilling an existing plot is free");
            game.Equip(3);yield return ClickWorld(clear);yield return new WaitForSeconds(.8f);game.Equip(2);yield return ClickWorld(clear);yield return new WaitForSeconds(.8f);energy=game.Model.State.energy;yield return ClickWorld(clear);yield return new WaitForSeconds(.8f);Check(plot.crop.planted&&plot.crop.wateredDay==game.Model.State.day&&game.Model.State.energy==energy,"Already watered crops cannot spend energy again");Capture("03-wet-soil.png");
            foreach(var area in new[]{0,7,1,3,4})
            {
                var point=game.world.points.First(p=>p.action=="passage"&&p.transform.IsChildOf(game.world.IslandRoot(area)));game.world.SetArea(area,(Vector2)point.transform.position+Vector2.down);yield return game.ChangeDistrict(point);int destination=int.Parse(point.source.Split('|')[0]);
                Check(game.world.Area==destination&&!WorldView.Water(game.world.player.position,destination),"Walk passage arrives on dry district land: "+area+" to "+destination);yield return new WaitForSeconds(.7f);Check(game.world.Area==destination,"Passage does not immediately bounce back");Capture("04-district-"+destination+".png");
            }
            foreach(var resident in Residents.All)
            {
                var point=game.world.points.First(p=>p.action=="talk"&&p.source==resident.id&&p.GetComponent<ResidentActor>());var area=Archipelago.Islands.First(i=>point.transform.IsChildOf(game.world.IslandRoot(i.id))).id;game.world.SetArea(area,(Vector2)point.transform.position+Vector2.down);game.Interact(point);yield return null;
                Check(game.ui.PageOpen&&game.ui.GetComponentsInChildren<UnityEngine.UI.Image>().Any(i=>i.sprite&&i.sprite.name=="portrait-"+resident.id+"-neutral"),"Resident opens framed face dialogue: "+resident.name);Capture("05-talk-"+resident.id+".png");
                yield return ClickButton("Next");Check(game.ui.GetComponentsInChildren<UnityEngine.UI.Image>().Any(i=>i.sprite&&i.sprite.name=="portrait-"+resident.id+"-laugh"),"Dialogue changes expression: "+resident.name);yield return ClickButton("Next");
                game.Model.Gather(resident.item);yield return ClickButton("Give "+game.Model.Data.Item(resident.item).name);Check(Residents.State(game.Model,resident.id).eventDone,"Ingredient event completes: "+resident.name);yield return ClickButton("Goodbye");
            }
            game.world.SetArea(7,new Vector2(-8,0));game.Model.State.coins=20;var shop=game.world.points.First(p=>p.action=="shop"&&p.transform.IsChildOf(game.world.IslandRoot(7)));game.Interact(shop);yield return ClickButton("5 shells");Check(game.Model.State.coins==15,"Town seed shop accepts pointer purchases");game.ui.Hide();Capture("06-town.png");
            var home=game.world.points.First(p=>p.action=="home"&&p.transform.IsChildOf(game.world.IslandRoot(7)));game.world.SetArea(7,(Vector2)home.transform.position+Vector2.down);yield return game.ChangeDistrict(home);Check(game.world.Area==8,"Town home doorway opens its own room");Capture("07-home.png");var exit=game.world.points.First(p=>p.action=="passage"&&p.transform.IsChildOf(game.world.GetComponentsInChildren<ResidentRoom>(true).First(r=>r.area==8).transform));yield return game.ChangeDistrict(exit);Check(game.world.Area==7,"Home exit returns to its district");
            game.Model.State.island=0;game.world.SetArea(0,Archipelago.Get(0).arrival);var boat=game.world.points.First(p=>p.action=="boat"&&p.transform.IsChildOf(game.world.saltleaf));game.Interact(boat);Check(game.ui.rows.GetComponentsInChildren<UnityEngine.UI.Button>().Count(b=>b.GetComponentInChildren<TMPro.TMP_Text>()?.text=="Sail")==3,"Ferry menu has three island destinations");game.ui.Hide();
            game.Sail(3);yield return new WaitForSeconds(.4f);var skiff=game.world.transform.Find("Sailing skiff");Check(skiff&&WaterRoute.Clear(skiff.position,0),"Departing ferry occupies clear water");Capture("08-sailing.png");float deadline=Time.time+20;while(game.Sailing&&Time.time<deadline)yield return null;Check(game.world.Area==3&&!WorldView.Water(game.world.player.position,3),"Ferry docks with a dry landing");
            game.world.SetArea(6,new Vector2(1,0));yield return game.SleepRoutine();var saved=game.Saves.Read(game.Model.Data);Check(saved.residents.Count==5&&saved.residents.All(r=>r.eventDone)&&saved.energy==100,"All resident events persist in the morning sleep checkpoint");
            Check(errors.Count==0,"No game runtime errors");File.WriteAllText(Path.Combine(output,"living-result.txt"),"PASS: "+checks+" living district checks");Application.Quit(0);
        }
        Vector2 ClearCell(int area)
        {for(int x=-12;x<12;x++)for(int y=-7;y<8;y++){var p=new Vector2(x,y);if(Archipelago.Tillable(p,area)&&!Physics2D.OverlapBox(p+Vector2.up*.2f,new Vector2(3,2),0)&&!game.world.GetComponentsInChildren<HarvestNode>(true).Any(n=>n.gameObject.activeInHierarchy&&Vector2.Distance(n.transform.position,p)<2)&&!game.world.points.Any(n=>n.gameObject.activeInHierarchy&&Vector2.Distance(n.transform.position,p)<3))return p;}throw new Exception("No clear work cell");}
        IEnumerator ClickWorld(Vector2 p)=>Click(game.world.worldCamera.WorldToScreenPoint(p));
        IEnumerator ClickButton(string name)
        {var button=game.ui.GetComponentsInChildren<UnityEngine.UI.Button>().FirstOrDefault(b=>b.gameObject.activeInHierarchy&&b.name==name);Check(button,"Visible pointer action: "+name);if(!button)yield break;var rect=button.GetComponent<RectTransform>();yield return Click(RectTransformUtility.WorldToScreenPoint(null,rect.TransformPoint(rect.rect.center)));}
        IEnumerator Click(Vector2 p){InputSystem.QueueStateEvent(mouse,new MouseState{position=p});yield return null;yield return null;InputSystem.QueueStateEvent(mouse,new MouseState{position=p}.WithButton(MouseButton.Left));yield return null;yield return null;InputSystem.QueueStateEvent(mouse,new MouseState{position=p});yield return null;yield return null;}
        void Current(){mouse?.MakeCurrent();keyboard?.MakeCurrent();}
        void Check(bool pass,string message){if(!pass){File.WriteAllText(Path.Combine(output,"living-result.txt"),"FAIL: "+message);Debug.LogError(message);Application.Quit(1);throw new Exception(message);}checks++;Debug.Log("LIVING PASS: "+message);}
        void Log(string message,string stack,LogType type){if(type==LogType.Error||type==LogType.Exception){errors.Add(message);File.WriteAllLines(Path.Combine(output,"runtime-errors.txt"),errors);}}
        void Capture(string name)
        {
            if(!target){target=new RenderTexture(Screen.width,Screen.height,24,RenderTextureFormat.ARGB32);target.Create();}var canvas=game.ui.canvas;var camera=game.world.worldCamera;canvas.renderMode=RenderMode.ScreenSpaceCamera;canvas.worldCamera=camera;canvas.planeDistance=1;canvas.sortingOrder=32760;Canvas.ForceUpdateCanvases();
            UnityEngine.Rendering.RenderPipeline.SubmitRenderRequest(camera,new UnityEngine.Rendering.Universal.UniversalRenderPipeline.SingleCameraRequest{destination=target});canvas.renderMode=RenderMode.ScreenSpaceOverlay;canvas.worldCamera=null;canvas.sortingOrder=0;var previous=RenderTexture.active;RenderTexture.active=target;var image=new Texture2D(target.width,target.height,TextureFormat.RGB24,false);image.ReadPixels(new Rect(0,0,target.width,target.height),0,0);image.Apply();File.WriteAllBytes(Path.Combine(output,name),image.EncodeToPNG());Destroy(image);RenderTexture.active=previous;
        }
        void OnDestroy(){Application.logMessageReceived-=Log;InputSystem.onAfterUpdate-=Current;if(target){target.Release();Destroy(target);}}
    }
}
