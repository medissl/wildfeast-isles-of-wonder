using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
namespace Wildfeast.Tests
{
    public class IslandLifeTests
    {
        GameModel game;
        [SetUp] public void Init(){game=new GameModel(Content.Load(),Progress.New());}
        [Test] public void FoodFillsHotbarThenOverflowsIntoBackpack()
        {
            game.Gather("leafgill");game.Gather("pepperbell");game.Gather("lanternroot");
            Assert.AreEqual("leafgill",game.State.slots[0].id);Assert.AreEqual("pepperbell",game.State.slots[4].id);Assert.AreEqual("lanternroot",game.State.slots[10].id);
            Assert.AreEqual(1,game.State.slots[10].count);
        }
        [Test] public void RearrangedToolsKeepIdentityAndFoodDepositsLeaveTools()
        {
            Assert.IsTrue(ItemInventory.Swap(game.State,1,20));game.State.equipped=1;Assert.AreEqual(0,ItemInventory.EquippedTool(game.State));
            ItemInventory.Swap(game.State,20,0);game.State.equipped=0;Assert.AreEqual(1,ItemInventory.EquippedTool(game.State));
            game.Gather("leafgill",2);game.Deposit();Assert.AreEqual("tool-rod",game.State.slots[0].id);Assert.AreEqual(2,game.Count("leafgill"));Assert.IsFalse(game.State.slots.Any(a=>a.id=="leafgill"));
        }
        [Test] public void ResourcesAreUniquePerSourceAndPersistThroughDeposit()
        {
            Assert.IsTrue(ItemInventory.Resource(game,"wood",3,"tree-1"));Assert.IsFalse(ItemInventory.Resource(game,"wood",3,"tree-1"));Assert.IsFalse(ItemInventory.Resource(game,"leafgill",1,"bad"));game.Deposit();Assert.AreEqual(3,game.State.resources.Single().count);game.NextDay();Assert.IsTrue(ItemInventory.Resource(game,"wood",3,"tree-1"));Assert.AreEqual(6,game.State.resources.Single().count);
        }
        [Test] public void FreePlotsConsumeSeedAndWaterThenGrowOnlyOnWateredNights()
        {
            Assert.IsTrue(ItemInventory.Till(game,1,new Vector2(2,3)));Assert.IsFalse(ItemInventory.Till(game,1,new Vector2(2,3)));var plot=game.State.fields.Single();
            Assert.IsTrue(ItemInventory.Plant(game,plot,"pepperbell"));Assert.AreEqual(4,game.State.pepperSeeds);game.NextDay();Assert.AreEqual(0,plot.crop.growth);
            Assert.IsTrue(ItemInventory.Water(game,plot));Assert.IsFalse(ItemInventory.Water(game,plot));Assert.AreEqual(19,game.State.water);game.NextDay();Assert.AreEqual(1,plot.crop.growth);
            game.State.water=0;Assert.IsFalse(ItemInventory.Water(game,plot));game.NextDay();Assert.AreEqual(1,plot.crop.growth);
        }
        [Test] public void FieldHarvestIsAtomicAndPreservesMatureCropWhenFull()
        {
            ItemInventory.Till(game,0,Vector2.zero);var plot=game.State.fields.Single();ItemInventory.Plant(game,plot,"pepperbell");plot.crop.growth=2;
            game.Gather("leafgill",8);Assert.IsFalse(ItemInventory.Harvest(game,plot));Assert.AreEqual(2,plot.crop.growth);game.Deposit();int changes=0;game.Changed+=()=>{changes++;Assert.IsFalse(plot.crop.planted);Assert.AreEqual(3,game.Count("pepperbell",true));};Assert.IsTrue(ItemInventory.Harvest(game,plot));Assert.AreEqual(1,changes);
        }
        [Test] public void ForeignPlotsAndOutOfRangeSwapsCannotConsumeAnything()
        {
            Assert.IsFalse(ItemInventory.Plant(game,new FieldPlot(),"pepperbell"));Assert.IsFalse(ItemInventory.Swap(game.State,-1,2));Assert.IsFalse(ItemInventory.Swap(game.State,0,40));Assert.AreEqual(5,game.State.pepperSeeds);
        }
        [Test] public void NewInventoryAndFieldsRoundTripWithoutChangingLegacyEconomy()
        {
            string folder=Path.Combine(Path.GetTempPath(),"wildfeast-life-"+Guid.NewGuid());Directory.CreateDirectory(folder);
            try{game.State.coins=37;ItemInventory.Swap(game.State,9,25);ItemInventory.Till(game,1,new Vector2(1,3));ItemInventory.Plant(game,game.State.fields.Single(),"pepperbell");game.State.water=7;var store=new SaveStore(Path.Combine(folder,"save.json"));store.Write(game.State);var loaded=store.Read(game.Data);Assert.IsNull(store.Warning);Assert.AreEqual("tool-pickaxe",loaded.slots[25].id);Assert.AreEqual(7,loaded.water);Assert.AreEqual(37,loaded.coins);Assert.AreEqual(1,loaded.fields.Single().island);Assert.IsTrue(loaded.fields.Single().crop.planted);}finally{Directory.Delete(folder,true);}
        }
        [Test] public void IslandsHaveDifferentShorelines()
        {Assert.AreNotEqual(WorldView.Coast(0).Length,WorldView.Coast(1).Length);}
        [Test] public void WorkshopTradePaysExactlyOnceAndClearsMaterialSlots()
        {ItemInventory.Resource(game,"wood",3,"tree");ItemInventory.Resource(game,"stone",2,"rock");Assert.IsTrue(ItemInventory.SellResources(game));Assert.AreEqual(13,game.State.coins);Assert.IsFalse(ItemInventory.SellResources(game));Assert.IsFalse(game.State.slots.Any(s=>s.id=="wood"||s.id=="stone"));Assert.AreEqual(13,game.State.coins);}
        [Test] public void MalformedToolStackRestoresBackupInsteadOfCrashing()
        {
            string folder=Path.Combine(Path.GetTempPath(),"wildfeast-invalid-"+Guid.NewGuid());Directory.CreateDirectory(folder);
            try{var store=new SaveStore(Path.Combine(folder,"save.json"));game.State.coins=29;store.Write(game.State);game.State.slots[1].count=5;store.Write(game.State);var loaded=store.Read(game.Data);Assert.IsNotNull(store.Warning);Assert.AreEqual(29,loaded.coins);Assert.AreEqual(1,loaded.slots[1].count);}finally{Directory.Delete(folder,true);}
        }
    }
}
