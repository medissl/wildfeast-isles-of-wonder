using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEditor.SceneManagement;
namespace Wildfeast.Tests
{
    public class LivingDistrictTests
    {
        [Test] public void ThreeIslandsHaveConnectedDistrictsAndCompatibleOldIds()
        {Assert.AreEqual(3,Districts.Ports.Length);foreach(int old in new[]{0,1,3,4,5})Assert.IsTrue(Archipelago.Valid(old));Assert.IsTrue(Archipelago.Valid(7));Assert.AreEqual(Districts.Island(0),Districts.Island(1));Assert.AreEqual(Districts.Island(3),Districts.Island(4));foreach(var i in Archipelago.Islands)foreach(var p in i.points.Where(p=>p.action=="passage")){var parts=p.source.Split('|');int area=int.Parse(parts[0]);var culture=System.Globalization.CultureInfo.InvariantCulture;var pos=new Vector2(float.Parse(parts[1],culture),float.Parse(parts[2],culture));Assert.IsFalse(WorldView.Water(pos,area));Assert.IsTrue(Archipelago.Get(area).points.Any(q=>q.action=="passage"&&q.source.StartsWith(i.id+"|")));Assert.IsTrue(Archipelago.Get(area).points.Where(q=>q.action=="passage").All(q=>Vector2.Distance(q.position,pos)>.6f));}}
        [Test] public void EveryBoatRouteHasClearWaterAtEverySample()
        {foreach(var i in Archipelago.Islands){var path=WaterRoute.Escape(i.id,-50);Assert.Greater(path.Length,2);foreach(var p in path)Assert.IsTrue(WaterRoute.Clear(p,i.id),i.key+" "+p);}}
        [Test] public void ResidentConversationOnlyAddsFriendshipOncePerDay()
        {var m=new GameModel(Content.Load(),Progress.New());Assert.IsTrue(Residents.Talk(m,"nori"));Assert.IsFalse(Residents.Talk(m,"nori"));Assert.AreEqual(1,Residents.State(m,"nori").friendship);m.NextDay();Assert.IsTrue(Residents.Talk(m,"nori"));Assert.AreEqual(2,Residents.State(m,"nori").friendship);}
        [Test] public void IngredientEventsConsumeOnePortionAndNeverDuplicateReward()
        {foreach(var resident in Residents.All){var m=new GameModel(Content.Load(),Progress.New());Assert.IsFalse(Residents.Deliver(m,resident.id));Assert.IsEmpty(m.State.residents);m.Gather(resident.item,2);int changes=0;m.Changed+=()=>changes++;Assert.IsTrue(Residents.Deliver(m,resident.id));Assert.AreEqual(1,m.Count(resident.item,true));Assert.AreEqual(20,m.State.coins);Assert.AreEqual(1,changes);Assert.IsFalse(Residents.Deliver(m,resident.id));Assert.AreEqual(20,m.State.coins);}}
        [Test] public void ResidentAndExtraAvatarChoicesSurviveSleepSaveSerialization()
        {var m=new GameModel(Content.Load(),Progress.New());Residents.Talk(m,"luma");m.Gather("dewnectar");Residents.Deliver(m,"luma");m.State.avatar.hairStyle=5;m.State.avatar.face=4;m.State.avatar.accessory=3;var p=SaveStore.Parse(JsonUtility.ToJson(m.State),m.Data);Assert.IsTrue(p.residents.Single().eventDone);Assert.AreEqual(3,p.residents.Single().friendship);Assert.AreEqual(5,p.avatar.hairStyle);Assert.AreEqual(4,p.avatar.face);Assert.AreEqual(3,p.avatar.accessory);}
        [Test] public void ResidentPortraitsAreNativeAndExpressionsDiffer()
        {foreach(var resident in Residents.All){var a=WorldView.Art("portrait-"+resident.id+"-neutral");foreach(string expression in new[]{"neutral","smile","mad","love","laugh"}){var sprite=WorldView.Art("portrait-"+resident.id+"-"+expression);Assert.IsNotNull(sprite);Assert.AreEqual(64,sprite.rect.width);Assert.AreEqual(FilterMode.Point,sprite.texture.filterMode);}Assert.AreNotSame(a,WorldView.Art("portrait-"+resident.id+"-laugh"));}}
        [Test] public void InventoryBrothIsACompactPortionAndCachedAvatarsKeepNativeDirections()
        {Assert.AreEqual(32,WorldView.ItemArt("brothback").rect.width);Assert.AreNotSame(WorldView.Art("brothback"),WorldView.ItemArt("brothback"));foreach(int style in Enumerable.Range(0,6))using(var look=new CharacterLook(new CharacterProfile{hairStyle=style,face=2,accessory=2})){foreach(string direction in new[]{"up","down","left","right"}){var source=WorldView.Art("chef-"+direction+"-0");var sprite=look.Apply(source);Assert.AreEqual(32,sprite.rect.width);Assert.AreEqual(48,sprite.rect.height);Assert.AreSame(sprite,look.Apply(source));}}}
        [Test] public void SceneHasFiveResidentsThreeHomesAndReachableTownShops()
        {EditorSceneManager.OpenScene("Assets/Wildfeast/Scenes/Wildfeast.unity");var world=Object.FindFirstObjectByType<WorldView>();Assert.AreEqual(5,world.GetComponentsInChildren<ResidentActor>(true).Length);Assert.AreEqual(3,world.GetComponentsInChildren<ResidentRoom>(true).Length);Assert.IsTrue(world.points.Count(p=>p.action=="shop")>=4);Assert.IsFalse(world.GetComponentsInChildren<SpriteRenderer>(true).Any(sr=>sr.name=="fountain"));}
        [Test] public void EveryDistrictChartIsAWholeNativeMap()
        {foreach(var island in Archipelago.Islands){var chart=WorldView.Art("map-"+island.key);Assert.IsNotNull(chart);Assert.AreEqual("map-"+island.key,chart.name);Assert.AreEqual(island.size.x*32,chart.rect.width);Assert.AreEqual(island.size.y*32,chart.rect.height);}}
        [Test] public void SeedPurchaseIsAtomicAndRequiresActualFunds()
        {var m=new GameModel(Content.Load(),Progress.New());int seeds=m.Seeds("mooncap");Assert.IsFalse(Residents.BuySeed(m,"mooncap"));m.State.coins=5;Assert.IsTrue(Residents.BuySeed(m,"mooncap"));Assert.AreEqual(seeds+1,m.Seeds("mooncap"));Assert.AreEqual(0,m.State.coins);Assert.IsFalse(Residents.BuySeed(m,"mooncap"));}
    }
}
