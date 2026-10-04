using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEditor.SceneManagement;
namespace Wildfeast.Tests
{
    public class ArchipelagoTests
    {
        [Test] public void FiveDistinctIslandsAreFreeAndRestaurantIsNotADestination()
        {var game=new GameModel(Content.Load(),Progress.New());Assert.AreEqual(5,Archipelago.Islands.Length);Assert.AreEqual(5,Archipelago.Islands.Select(i=>i.key).Distinct().Count());foreach(var island in Archipelago.Islands){Assert.IsTrue(game.Travel(island.id));Assert.AreEqual(island.id,game.State.island);}Assert.IsFalse(game.Travel(2));Assert.IsFalse(game.Travel(6));Assert.AreEqual(0,game.State.coins);}
        [Test] public void RoadsAndWaterAreNeverTilledButGreenSoilWorksOnEveryIsland()
        {var game=new GameModel(Content.Load(),Progress.New());foreach(var island in Archipelago.Islands){Assert.IsFalse(ItemInventory.Till(game,island.id,island.roads[0].points[1]));Assert.IsFalse(ItemInventory.Till(game,island.id,island.pond));Vector2 clear=Vector2.zero;bool found=false;for(int x=-10;x<=10&&!found;x++)for(int y=-5;y<=5&&!found;y++){clear=new Vector2(x,y);found=Archipelago.Tillable(clear,island.id);}Assert.IsTrue(found);Assert.IsTrue(ItemInventory.Till(game,island.id,clear));}}
        [Test] public void NewSeedsHaveIdentityAndAtomicGrowthHarvest()
        {var game=new GameModel(Content.Load(),Progress.New());game.Gather("mooncap",1,"grove");Assert.AreEqual(1,game.Seeds("mooncap"));Assert.IsTrue(game.State.slots.Any(s=>s.id=="seed-mooncap"));Assert.IsTrue(ItemInventory.Till(game,4,new Vector2(-6,-7)));var plot=game.State.fields.Single();Assert.IsTrue(ItemInventory.Plant(game,plot,"mooncap"));Assert.AreEqual(0,game.Seeds("mooncap"));ItemInventory.Water(game,plot);game.NextDay();ItemInventory.Water(game,plot);game.NextDay();Assert.AreEqual(2,plot.crop.growth);Assert.IsTrue(ItemInventory.Harvest(game,plot));Assert.IsFalse(plot.crop.planted);Assert.AreEqual(4,game.Count("mooncap",true));}
        [Test] public void EveryNewRecipeUnlocksFromItsActualIngredientsAndCanBeCooked()
        {var game=new GameModel(Content.Load(),Progress.New());foreach(var ingredient in game.Data.ingredients){game.Gather(ingredient.id,2);game.Deposit();}Assert.AreEqual(19,game.Data.ingredients.Length);Assert.AreEqual(20,game.Data.recipes.Length);foreach(var recipe in game.Data.recipes){Assert.Contains(recipe.id,game.State.recipes);Assert.IsTrue(game.CanCook(recipe.id),recipe.id);Assert.IsNotNull(WorldView.Art(recipe.icon));Assert.IsNotNull(WorldView.Art("held-"+recipe.icon));}}
        [Test] public void OptionsSeedsAndEcologySurviveDiskRoundTrip()
        {string folder=Path.Combine(Path.GetTempPath(),"wildfeast-islands-"+Guid.NewGuid());try{var game=new GameModel(Content.Load(),Progress.New());game.Travel(5);game.Gather("pearlsprout",1,"bed");game.State.musicVolume=.11f;game.State.effectsVolume=.81f;game.State.zoom=2;game.State.reducedMotion=true;game.State.ecology.Add(new EcologyProgress{source="snail-tide",ready=true});var store=new SaveStore(Path.Combine(folder,"save.json"));store.Write(game.State);var saved=store.Read(game.Data);Assert.IsNull(store.Warning);Assert.AreEqual(5,saved.island);Assert.AreEqual(.11f,saved.musicVolume,.001f);Assert.AreEqual(.81f,saved.effectsVolume,.001f);Assert.AreEqual(2,saved.zoom);Assert.IsTrue(saved.reducedMotion);Assert.IsTrue(saved.ecology.Single().ready);Assert.AreEqual(1,saved.seeds.Single().count);}finally{if(Directory.Exists(folder))Directory.Delete(folder,true);}}
        [Test] public void OlderCombinedAudioSettingMigratesWithoutLosingProgress()
        {string folder=Path.Combine(Path.GetTempPath(),"wildfeast-migrate-"+Guid.NewGuid());try{Directory.CreateDirectory(folder);var p=new GameModel(Content.Load(),Progress.New()).State;p.coins=71;p.volume=.63f;var path=Path.Combine(folder,"save.json");string json=System.Text.RegularExpressions.Regex.Replace(JsonUtility.ToJson(p), "\"(musicVolume|effectsVolume)\":[^,]+,", "");File.WriteAllText(path,json);var store=new SaveStore(path);var saved=store.Read(Content.Load());Assert.IsNull(store.Warning);Assert.AreEqual(71,saved.coins);Assert.AreEqual(.63f,saved.musicVolume,.001f);Assert.AreEqual(.63f,saved.effectsVolume,.001f);Assert.AreEqual(1,saved.zoom);}finally{if(Directory.Exists(folder))Directory.Delete(folder,true);}}
        [Test] public void AuthoredFiveRootsContainRealLandmarksCreaturesAndResourceFootprints()
        {EditorSceneManager.OpenScene("Assets/Wildfeast/Scenes/Wildfeast.unity");var world=UnityEngine.Object.FindFirstObjectByType<WorldView>();Assert.AreEqual(5,world.islands.Length);foreach(var island in Archipelago.Islands){var root=world.IslandRoot(island.id);Assert.IsNotNull(root.GetComponentInChildren<UnityEngine.EdgeCollider2D>(true));Assert.IsTrue(world.points.Any(p=>p.transform.IsChildOf(root)&&p.action=="boat"));Assert.IsTrue(world.points.Any(p=>p.transform.IsChildOf(root)&&p.action=="fish"));Assert.IsNotNull(WorldView.Art("map-"+island.key));foreach(var node in root.GetComponentsInChildren<HarvestNode>(true).Where(n=>n.item!="fiber"))Assert.IsTrue(node.colliders.Length>0);}Assert.AreEqual(6,world.GetComponentsInChildren<FoodEcology>(true).Length);}
        [Test] public void EcologyContactsAndWaterApplyOnceAtAnAtomicCheckpoint()
        {var game=new GameModel(Content.Load(),Progress.New());int changes=0;game.Changed+=()=>{changes++;Assert.IsTrue(game.State.ecology.First(e=>e.source=="snail-tide").ready);Assert.AreEqual(19,game.State.water);};Assert.IsTrue(ItemInventory.EcologyAction(game,"snail-tide","snail",2));Assert.IsFalse(ItemInventory.EcologyAction(game,"snail-tide","snail",2));Assert.AreEqual(1,changes);}
        [Test] public void LegacyGardenOnANewRoadMovesWithoutLosingItsCrop()
        {string folder=Path.Combine(Path.GetTempPath(),"wildfeast-relocate-"+Guid.NewGuid());try{var game=new GameModel(Content.Load(),Progress.New());game.State.fields.Add(new FieldPlot{island=0,x=-11,y=-6,crop=new Crop{item="lanternroot",planted=true,growth=2,wateredDay=1}});var store=new SaveStore(Path.Combine(folder,"save.json"));store.Write(game.State);var loaded=store.Read(game.Data);Assert.IsNull(store.Warning);var field=loaded.fields.Single();Assert.IsTrue(Archipelago.Tillable(new Vector2(field.x,field.y),0));Assert.AreEqual(2,field.crop.growth);Assert.AreEqual("lanternroot",field.crop.item);Assert.AreEqual(1,field.crop.wateredDay);}finally{if(Directory.Exists(folder))Directory.Delete(folder,true);}}
        [Test] public void AllVegetationFramesStayOnNativePixelGrid()
        {foreach(var island in Archipelago.Islands){for(int f=0;f<4;f++){var tree=WorldView.Art("tree-"+island.tree+"-"+f);Assert.IsNotNull(tree);Assert.AreEqual(32,tree.pixelsPerUnit);Assert.AreEqual(64,tree.rect.width);Assert.AreEqual(96,tree.rect.height);}for(int f=0;f<3;f++){var grass=WorldView.Art("grass-"+island.key+"-"+f);Assert.IsNotNull(grass);Assert.AreEqual(32,grass.pixelsPerUnit);Assert.AreEqual(16,grass.rect.height);}}}
        [Test] public void EveryAuthoredRoadHasAWalkableApproach()
        {
            EditorSceneManager.OpenScene("Assets/Wildfeast/Scenes/Wildfeast.unity");var world=UnityEngine.Object.FindFirstObjectByType<WorldView>();world.player.gameObject.SetActive(false);
            foreach(var island in Archipelago.Islands)
            {
                foreach(var root in world.islands)root.gameObject.SetActive(root==world.IslandRoot(island.id));world.restaurant.gameObject.SetActive(false);Physics2D.SyncTransforms();
                foreach(var route in island.roads)for(int k=0;k<route.points.Length-1;k++)for(int n=0;n<=30;n++)
                {
                    Vector2 sample=Vector2.Lerp(route.points[k],route.points[k+1],n/30f);
                    Assert.IsFalse(WorldView.Water(sample,island.id),island.key+" road enters water at "+sample);
                    Assert.IsNull(Physics2D.OverlapCircle(sample+Vector2.up*.15f,.16f),island.key+" road intersects a solid footprint at "+sample);
                }
            }
        }
    }
}
