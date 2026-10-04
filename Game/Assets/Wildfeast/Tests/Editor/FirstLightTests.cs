using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
namespace Wildfeast.Tests
{
    public class FirstLightTests
    {
        [Test] public void TrackingFishGainsAndMissingFishLosesProgress()
        {
            var challenge=new FishingChallenge(2,true);float before=challenge.Progress;
            challenge.Tick(.02f,false);Assert.Greater(challenge.Progress,before);
            for(int i=0;i<250;i++)challenge.Tick(.02f,false);
            Assert.IsFalse(challenge.Tracking);before=challenge.Progress;challenge.Tick(.02f,false);Assert.Less(challenge.Progress,before);
        }
        [Test] public void GoodTrackingCanLandEverySpecies()
        {
            for(int species=0;species<5;species++)
            {var fish=new FishingChallenge(species,false);for(int i=0;i<1500&&fish.Progress<1;i++)fish.Tick(.02f,fish.Zone<fish.Fish);Assert.AreEqual(1,fish.Progress,"Species "+species);}
        }
        [Test] public void CatchZoneStaysInsideTheVisibleBar()
        {
            var fish=new FishingChallenge(0,true);for(int i=0;i<2000;i++){fish.Tick(.02f,i<1000);Assert.GreaterOrEqual(fish.Zone,fish.Width/2);Assert.LessOrEqual(fish.Zone,1-fish.Width/2);Assert.That(fish.Progress,Is.InRange(0f,1f));}
        }
        [Test] public void SaveSlotsNeverOverwriteAnExistingJourney()
        {
            string dir=Path.Combine(Path.GetTempPath(),"wildfeast-slots-"+Guid.NewGuid());var slots=new JourneySlots(dir);
            try{var first=Progress.New();first.avatar.name="Juniper";first.coins=91;slots.Create(0,first);string bytes=File.ReadAllText(slots.PathFor(0));Assert.AreEqual(1,slots.Empty());Assert.Throws<IOException>(()=>slots.Create(0,Progress.New()));slots.Create(1,Progress.New());Assert.AreEqual(bytes,File.ReadAllText(slots.PathFor(0)));Assert.AreEqual("Juniper",slots.Preview(0,Content.Load()).avatar.name);}finally{if(Directory.Exists(dir))Directory.Delete(dir,true);}
        }
        [Test] public void AllFiveSlotsReportFullWithoutReplacingAnything()
        {
            string dir=Path.Combine(Path.GetTempPath(),"wildfeast-slots-"+Guid.NewGuid());var slots=new JourneySlots(dir);
            try{for(int i=0;i<JourneySlots.Count;i++)slots.Create(i,Progress.New());Assert.AreEqual(-1,slots.Empty());Assert.Throws<ArgumentOutOfRangeException>(()=>slots.PathFor(-1));}finally{if(Directory.Exists(dir))Directory.Delete(dir,true);}
        }
        [Test] public void OldSavesReceiveAnAvatarAndDiscoveryDefaults()
        {
            var p=Progress.New();string text=JsonUtility.ToJson(p);text=text.Replace("\"avatar\":"+JsonUtility.ToJson(p.avatar)+",","").Replace("\"landmarks\":[],","");var loaded=SaveStore.Parse(text,Content.Load());Assert.NotNull(loaded.avatar);Assert.NotNull(loaded.landmarks);Assert.AreEqual("Mira",loaded.avatar.name);
        }
        [Test] public void AvatarAndIntroRoundTripAllChoices()
        {
            var p=Progress.New();p.avatar=new CharacterProfile{name="Nori",skin=5,hairStyle=2,hairColor=4,face=2,shirt=3,pants=2,boots=4};p.introSeen=true;
            var loaded=SaveStore.Parse(JsonUtility.ToJson(p),Content.Load());Assert.AreEqual(5,loaded.avatar.skin);Assert.AreEqual(2,loaded.avatar.hairStyle);Assert.AreEqual(4,loaded.avatar.hairColor);Assert.AreEqual(2,loaded.avatar.face);Assert.AreEqual(3,loaded.avatar.shirt);Assert.AreEqual(2,loaded.avatar.pants);Assert.AreEqual(4,loaded.avatar.boots);Assert.IsTrue(loaded.introSeen);
        }
        [Test] public void InvalidAvatarChoicesNormalizeSafely()
        {var p=new CharacterProfile{name="  ",skin=98,hairStyle=-2,face=7,boots=42};p.Normalize();Assert.AreEqual("Mira",p.name);Assert.AreEqual(5,p.skin);Assert.AreEqual(0,p.hairStyle);Assert.AreEqual(2,p.face);Assert.AreEqual(5,p.boots);}
        [Test] public void DiscoveryAwardsIngredientRecipeAndWaterAsOneTransaction()
        {
            var game=new GameModel(Content.Load(),Progress.New());int events=0;game.Changed+=()=>events++;
            Assert.IsTrue(game.Discover("kettle","cloudfruit","custard",true));Assert.AreEqual(1,events);Assert.AreEqual(19,game.State.water);Assert.AreEqual(2,game.Count("cloudfruit",true));Assert.Contains("custard",game.State.recipes);Assert.IsFalse(game.Discover("kettle","cloudfruit","custard",true));Assert.AreEqual(1,events);
        }
        [Test] public void FullSatchelCannotConsumeDiscoveryOrWater()
        {
            var game=new GameModel(Content.Load(),Progress.New());game.Gather("leafgill",game.Capacity);int water=game.State.water;
            Assert.IsFalse(game.Discover("kettle","cloudfruit","custard",true));Assert.AreEqual(water,game.State.water);Assert.IsEmpty(game.State.landmarks);Assert.IsFalse(game.State.recipes.Contains("custard"));
        }
        [Test] public void EveryIslandHasAReachableOuterDiscoveryAndFish()
        {
            foreach(var island in Archipelago.Islands)
            {var p=island.points.Single(p=>p.action=="discovery");Assert.IsFalse(WorldView.Water(p.position,island.id));Assert.IsTrue(Archipelago.Road(p.position,island.id,2.5f));Assert.IsTrue(island.points.Any(p=>p.action=="fish"));Assert.Greater(island.size.x*island.size.y,1500);Assert.NotNull(WorldView.Art(p.art));Assert.NotNull(WorldView.Art(p.source+"-awake"));}
        }
        [Test] public void GlassesSitOnTheEyesAndPaletteAppliesToActionFrames()
        {
            using(var look=new CharacterLook(new CharacterProfile{hairStyle=1,hairColor=4,skin=3,shirt=2,face=2}))
            {
                var walk=look.Apply(WorldView.Art("chef-down-0"));Assert.AreEqual((Color32)GameUI.C("273740"),walk.texture.GetPixels32()[30*32+10]);
                var action=look.Apply(WorldView.Art("action-cast-right-2"));Assert.IsTrue(action.texture.GetPixels32().Contains((Color32)GameUI.C("b78261")));Assert.IsTrue(action.texture.GetPixels32().Contains((Color32)GameUI.C("6c80aa")));
            }
        }
        [Test] public void CustomizationUsesReadableNativeFrames()
        {foreach(string name in new[]{"chef-down-0","action-cast-right-2","action-pour-up-2"}){var sprite=WorldView.Art(name);Assert.AreEqual(32,sprite.rect.width);Assert.AreEqual(48,sprite.rect.height);Assert.IsTrue(sprite.texture.isReadable);Assert.AreEqual(FilterMode.Point,sprite.texture.filterMode);}}
    }
}
