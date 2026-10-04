using NUnit.Framework;
using UnityEngine;
namespace Wildfeast.Tests
{
    public class RestTests
    {
        [Test] public void EnergyTransactionCannotOverdrawOrChangeOnFailure()
        {var m=new GameModel(Content.Load(),Progress.New());Assert.IsTrue(m.SpendEnergy(98));Assert.AreEqual(2,m.State.energy);Assert.IsFalse(m.SpendEnergy(3));Assert.AreEqual(2,m.State.energy);Assert.IsFalse(m.SpendEnergy(-1));Assert.AreEqual(2,m.State.energy);}
        [Test] public void MorningRestoresEnergyAndAdvancesWateredCropsOnce()
        {var m=new GameModel(Content.Load(),Progress.New());m.State.energy=0;m.State.fields.Add(new FieldPlot{island=0,x=-11,y=-4,crop=new Crop{planted=true,wateredDay=1,item="pepperbell"}});Assert.IsTrue(m.NextDay());Assert.AreEqual(2,m.State.day);Assert.AreEqual(100,m.State.energy);Assert.IsTrue(m.State.wokeAtHome);Assert.AreEqual(1,m.State.fields[0].crop.growth);}
        [Test] public void UnfinishedServiceCannotAdvanceDayOrRestoreEnergy()
        {var m=new GameModel(Content.Load(),Progress.New());m.State.energy=7;m.State.phase="service";m.State.orders.Add(new Order{recipe="wrap",number=0});Assert.IsFalse(m.NextDay());Assert.AreEqual(1,m.State.day);Assert.AreEqual(7,m.State.energy);}
        [Test] public void OldSaveWithoutEnergyLoadsWithFullEnergy()
        {var data=Content.Load();string json=JsonUtility.ToJson(Progress.New()).Replace("\"energy\":100,","");Assert.AreEqual(100,SaveStore.Parse(json,data).energy);}
        [Test] public void OldOccupiedBedMovesToSpatialGardenWithoutLosingGrowth()
        {var p=Progress.New();p.crops[0]=new Crop{planted=true,item="pepperbell",growth=1,wateredDay=1};var loaded=SaveStore.Parse(JsonUtility.ToJson(p),Content.Load());Assert.IsFalse(loaded.crops[0].planted);Assert.AreEqual(1,loaded.fields.Count);Assert.AreEqual(1,loaded.fields[0].crop.growth);Assert.AreEqual(1,loaded.fields[0].crop.wateredDay);Assert.IsTrue(Archipelago.Tillable(new Vector2(loaded.fields[0].x,loaded.fields[0].y),0));}
    }
}
