using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

namespace Wildfeast.Tests
{
    public class GameRulesTests
    {
        GameModel game;
        string temporary;
        [SetUp] public void SetUp(){game=new GameModel(Content.Load(),Progress.New());temporary=Path.Combine(Path.GetTempPath(),"wildfeast-tests-"+Guid.NewGuid());Directory.CreateDirectory(temporary);}
        [TearDown] public void TearDown(){Directory.Delete(temporary,true);}
        void Open(string dish="seared",int quantity=3)
        {
            for(int i=0;i<quantity;i++)game.Gather("leafgill");game.Deposit();game.State.menu.Clear();game.State.menu.Add(dish);Assert.IsTrue(game.StartService());
        }
        [Test] public void BagCapacityRejectsWholeOverflowWithoutLosingItems()
        {Assert.IsTrue(game.Gather("leafgill",7));Assert.IsFalse(game.Gather("pepperbell",2));Assert.AreEqual(7,game.BagCount);Assert.AreEqual(0,game.Count("pepperbell",true));}
        [Test] public void DailyHarvestIsIdempotentAndRecoversNextDay()
        {Assert.IsTrue(game.Gather("pepperbell",2,"plant"));Assert.IsFalse(game.Gather("pepperbell",2,"plant"));game.NextDay();Assert.IsTrue(game.Gather("pepperbell",2,"plant"));}
        [Test] public void UnknownAndNegativeGatherCannotMutateInventory()
        {Assert.IsFalse(game.Gather("missing"));Assert.IsFalse(game.Gather("leafgill",-1));Assert.AreEqual(0,game.BagCount);}
        [Test] public void DepositMovesQuantitiesExactlyOnce()
        {game.Gather("leafgill",4);game.Deposit();game.Deposit();Assert.AreEqual(0,game.BagCount);Assert.AreEqual(4,game.Count("leafgill"));}
        [Test] public void CookingAndDeliveryCannotBeRepeated()
        {Open();Assert.IsTrue(game.Cook(0,2));Assert.IsFalse(game.Cook(0,2));Assert.AreEqual(2,game.Count("leafgill"));Assert.IsTrue(game.Serve(0));int earned=game.State.coins;Assert.IsFalse(game.Serve(0));Assert.AreEqual(earned,game.State.coins);}
        [Test] public void MissingIngredientsDoNotPartiallyConsumeRecipe()
        {game.Gather("leafgill");game.Gather("pepperbell");game.Deposit();game.State.menu.Clear();game.State.menu.Add("wrap");game.StartService();game.State.pantry.RemoveAll(a=>a.id=="pepperbell");Assert.IsFalse(game.Cook(0,2));Assert.AreEqual(1,game.Count("leafgill"));}
        [Test] public void OrdersNeverExceedStockAcrossMultipleRecipes()
        {game.Gather("leafgill",3);game.Gather("pepperbell",1);game.Deposit();Assert.IsTrue(game.StartService());Assert.AreEqual(3,game.State.orders.Count);foreach(var order in game.State.orders)Assert.IsTrue(game.Cook(order.number,1));Assert.AreEqual(0,game.Count("leafgill"));}
        [Test] public void UncookedOrderCannotPayAndServiceCannotBeAbandoned()
        {Open();Assert.IsFalse(game.Serve(0));Assert.IsFalse(game.NextDay());Assert.IsFalse(game.CloseService());Assert.IsFalse(game.Travel(1));Assert.IsFalse(game.Gather("leafgill"));}
        [Test] public void PurchasesChargeOnceAndRespectPrerequisites()
        {game.State.coins=500;Assert.IsFalse(game.Buy("staff"));Assert.IsFalse(game.Buy("boat"));Assert.IsTrue(game.Buy("satchel"));int coins=game.State.coins;Assert.IsFalse(game.Buy("satchel"));Assert.AreEqual(coins,game.State.coins);Assert.AreEqual(16,game.Capacity);Assert.IsTrue(game.Buy("boat"));Assert.IsTrue(game.Travel(1));}
        [Test] public void DiscoveryUnlocksRecipeAndCultivationNeedsWateredNights()
        {game.Gather("lanternroot");Assert.Contains("lantern",game.State.recipes);Assert.IsTrue(game.Tend(0,"lanternroot"));game.NextDay();game.NextDay();Assert.AreEqual(1,game.State.crops[0].growth);Assert.IsTrue(game.Tend(0));game.NextDay();Assert.AreEqual(2,game.State.crops[0].growth);Assert.IsTrue(game.Tend(0));Assert.AreEqual(4,game.Count("lanternroot",true));}
        [Test] public void FullBagDoesNotEraseMatureCrop()
        {game.Gather("pepperbell");game.Tend(0);game.NextDay();game.Tend(0);game.NextDay();game.Gather("leafgill",7);Assert.IsFalse(game.Tend(0));Assert.AreEqual(2,game.State.crops[0].growth);}
        [Test] public void HarvestCheckpointContainsBothRewardAndResetCrop()
        {game.Gather("pepperbell");game.Deposit();game.Tend(0);game.NextDay();game.Tend(0);game.NextDay();int checkpoints=0;game.Changed+=()=>{checkpoints++;Assert.AreEqual(0,game.State.crops[0].growth);Assert.AreEqual(3,game.Count("pepperbell",true));};Assert.IsTrue(game.Tend(0));Assert.AreEqual(1,checkpoints);}
        [Test] public void FreeGrainEnsuresRecoveryFromEmptyKitchen()
        {game.State.pantry.Clear();game.NextDay();game.State.menu.Clear();game.State.menu.Add("porridge");Assert.IsTrue(game.StartService());Assert.AreEqual(3,game.State.orders.Count);foreach(var o in game.State.orders){Assert.IsTrue(game.Cook(o.number,0));Assert.IsTrue(game.Serve(o.number));}Assert.Greater(game.State.coins,0);}
        [Test] public void CustomerPreferenceAndCookingQualityAffectTips()
        {Open();game.State.orders[0].preference="fresh";game.Cook(0,2);game.Serve(0);Assert.AreEqual(28,game.State.coins);}
        [Test] public void CompletedRequestRewardsOnlyOnce()
        {Open();game.Cook(0,0);game.Serve(0);Assert.IsTrue(game.ClaimRequest());int money=game.State.coins;Assert.IsFalse(game.ClaimRequest());Assert.AreEqual(money,game.State.coins);}
        [Test] public void InterruptedServiceRestoresCookedAndPaidOrdersWithoutDuplicateMoney()
        {Open();game.Cook(0,2);game.Serve(0);game.Cook(1,1);var saves=new SaveStore(Path.Combine(temporary,"save.json"));saves.Write(game.State);var resumed=new GameModel(game.Data,saves.Read(game.Data));Assert.IsFalse(resumed.Serve(0));Assert.IsFalse(resumed.Cook(1,2));Assert.IsTrue(resumed.Serve(1));Assert.AreEqual(1,resumed.Count("leafgill"));Assert.AreEqual("service",resumed.State.phase);}
        [Test] public void SaveRoundTripPreservesProgressionAndCropState()
        {game.State.coins=500;game.Buy("satchel");game.Buy("boat");game.Travel(1);game.Gather("lanternroot");game.Tend(0,"lanternroot");var saves=new SaveStore(Path.Combine(temporary,"save.json"));saves.Write(game.State);var loaded=saves.Read(game.Data);Assert.AreEqual(1,loaded.island);Assert.Contains("satchel",loaded.upgrades);Assert.AreEqual("lanternroot",loaded.crops[0].item);Assert.Contains("lantern",loaded.recipes);Assert.IsNull(saves.Warning);}
        [Test] public void BrokenSaveUsesBackupAndPreservesOriginal()
        {var saves=new SaveStore(Path.Combine(temporary,"save.json"));saves.Write(game.State);game.State.coins=19;saves.Write(game.State);File.WriteAllText(saves.Path,"broken{");var loaded=saves.Read(game.Data);Assert.AreEqual(0,loaded.coins);Assert.IsNotNull(saves.Warning);Assert.AreEqual("broken{",File.ReadAllText(saves.Path));}
        [Test] public void UnknownSaveVersionStartsSafelyWithoutDeletingOldData()
        {var saves=new SaveStore(Path.Combine(temporary,"save.json"));game.State.version=200;saves.Write(game.State);var loaded=saves.Read(game.Data);Assert.AreEqual(1,loaded.version);Assert.IsTrue(File.Exists(saves.Path));Assert.IsNotNull(saves.Warning);}
        [Test] public void ContentIdsAndIngredientReferencesAreValid()
        {Assert.AreEqual(game.Data.ingredients.Length,game.Data.ingredients.Select(i=>i.id).Distinct().Count());Assert.AreEqual(game.Data.recipes.Length,game.Data.recipes.Select(i=>i.id).Distinct().Count());foreach(var r in game.Data.recipes){Assert.Greater(r.price,0);foreach(var a in r.ingredients){Assert.IsTrue(game.Data.ingredients.Any(i=>i.id==a.id));Assert.Greater(a.count,0);}}}
        [Test] public void SerializedSceneRetainsInteractionAndRequiredFontAssets()
        {
            UnityEditor.SceneManagement.EditorSceneManager.OpenScene("Assets/Wildfeast/Scenes/Wildfeast.unity");
            var controller=UnityEngine.Object.FindFirstObjectByType<GameController>();Assert.IsNotNull(controller);Assert.IsNotNull(controller.world);Assert.IsNotNull(controller.ui);
            Assert.IsTrue(controller.world.points.All(p=>p&&UnityEditor.MonoScript.FromMonoBehaviour(p)!=null));
            Assert.AreEqual("enter",controller.world.Nearest().action);
            Assert.IsNotNull(Resources.Load("TMP Settings"));Assert.IsNotNull(Resources.Load("Fonts & Materials/LiberationSans SDF"));
        }
    }
}
