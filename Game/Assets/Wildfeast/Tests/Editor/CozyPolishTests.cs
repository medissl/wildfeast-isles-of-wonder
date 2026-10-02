using NUnit.Framework;
using UnityEngine;
using TMPro;
using UnityEditor.SceneManagement;

namespace Wildfeast.Tests
{
    public class CozyPolishTests
    {
        [Test] public void RapidActionsCannotDuplicateContactOrSkipRecovery()
        {
            var clock=new ToolActionClock();int hits=0;
            Assert.IsTrue(clock.Begin(9,()=>hits++));
            for(int n=0;n<6;n++){Assert.IsFalse(clock.Begin(9,()=>hits+=100));clock.Tick(.05f);}
            Assert.AreEqual(0,hits,"Mining must wait for the contact pose.");
            clock.Tick(.1f);Assert.AreEqual(1,hits);Assert.IsTrue(clock.Busy);
            Assert.IsFalse(clock.Begin(7,()=>hits+=100));clock.Tick(.4f);
            Assert.IsFalse(clock.Busy);Assert.AreEqual(1,hits);
            Assert.IsTrue(clock.Begin(7,()=>hits++));clock.Tick(3);Assert.AreEqual(2,hits);Assert.IsFalse(clock.Busy);
        }
        [Test] public void InterruptedWindupCannotGrantAnIngredientLater()
        {var clock=new ToolActionClock();int hits=0;clock.Begin(2,()=>hits++);clock.Tick(.1f);clock.Cancel();clock.Tick(5);Assert.AreEqual(0,hits);Assert.IsFalse(clock.Busy);}
        [Test] public void EveryToolUsesABoundedAnimationWindow()
        {foreach(int tool in new[]{0,2,3,4,5,6,7,8,9}){var clock=new ToolActionClock();int hits=0;clock.Begin(tool,()=>hits++);clock.Tick(ToolActionClock.Duration(tool)*.45f);Assert.AreEqual(0,hits);clock.Tick(.03f);Assert.AreEqual(1,hits);Assert.IsTrue(clock.Busy);clock.Tick(1);Assert.IsFalse(clock.Busy);Assert.AreEqual(1,hits);}}
        [Test] public void AuthoredRoomHasOneKitchenFootprintAndConsistentFonts()
        {
            EditorSceneManager.OpenScene("Assets/Wildfeast/Scenes/Wildfeast.unity");var world=Object.FindFirstObjectByType<WorldView>();
            var kitchen=world.restaurant.GetComponentsInChildren<WorldPoint>(true);
            var point=System.Array.Find(kitchen,p=>p.action=="kitchen");
            Assert.AreEqual("kitchen-worktop",point.artwork.sprite.name);Assert.IsNotNull(point.artwork.GetComponent<PropDepth>());
            Assert.Greater(point.artwork.GetComponentInChildren<BoxCollider2D>().size.x,4);
            Assert.IsNull(world.restaurant.Find("prep-board"));Assert.IsNull(world.restaurant.Find("cook-pot"));Assert.IsNull(world.restaurant.Find("cook-pan"));
            var font=Resources.Load<TMP_FontAsset>("Fonts/Pixelify");Assert.IsNotNull(font);Assert.AreEqual(AtlasPopulationMode.Static,font.atlasPopulationMode);
            foreach(var text in Object.FindObjectsByType<TMP_Text>(FindObjectsInactive.Include,FindObjectsSortMode.None)){Assert.AreSame(font,text.font);Assert.IsFalse(text.enableAutoSizing);}
            foreach(char c in "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789·–—…×’")Assert.IsTrue(font.HasCharacter(c),"Missing "+c);
        }
        [Test] public void ChefActionFramesAndHeldToolsKeepTheWorldPixelDensity()
        {
            foreach(string kind in new[]{"swing","pour","plant","pull","cast","stir"})foreach(string direction in new[]{"down","up","left","right"})for(int f=0;f<5;f++)
            {var art=WorldView.Art("action-"+kind+"-"+direction+"-"+f);Assert.IsNotNull(art);Assert.AreEqual(new Vector2(32,48),art.rect.size);Assert.AreEqual(32,art.pixelsPerUnit);}
            Assert.AreNotEqual(WorldView.Art("tool-scythe"),WorldView.Art("tool-pickaxe"));
            foreach(string tool in new[]{"axe","scythe","pickaxe","can","shovel","knife","rod"}){var art=WorldView.Art("held-"+tool);Assert.IsNotNull(art);Assert.LessOrEqual(art.rect.height,32);Assert.AreEqual(32,art.pixelsPerUnit);}
        }
        [Test] public void RestaurantRoutesLeaveClearAislesAroundFurniture()
        {
            EditorSceneManager.OpenScene("Assets/Wildfeast/Scenes/Wildfeast.unity");var world=Object.FindFirstObjectByType<WorldView>();world.SetArea(2,Vector2.zero);world.terrace.SetActive(true);Physics2D.SyncTransforms();
            for(int table=0;table<5;table++)foreach(var route in new[]{world.GuestRoute(table),world.StaffRoute(table)})
            {
                for(int segment=1;segment<route.Length;segment++)for(int sample=0;sample<=20;sample++)
                {
                    Vector2 position=Vector2.Lerp(route[segment-1],route[segment],sample/20f)+Vector2.up*.15f;
                    Assert.IsFalse(System.Array.Exists(Physics2D.OverlapCircleAll(position,.14f),c=>c.transform.IsChildOf(world.restaurant)),"Furniture intersects route for table "+table+" at "+position);
                }
            }
            Assert.LessOrEqual(WorldView.Art("held-brothback").rect.height,24,"Stock is held in a small flask, not as an entire animal.");
        }
    }
}
