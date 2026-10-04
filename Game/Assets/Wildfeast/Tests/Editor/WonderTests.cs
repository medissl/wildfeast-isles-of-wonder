using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEditor.SceneManagement;
namespace Wildfeast.Tests
{
    public class WonderTests
    {
        [Test] public void IslandsHaveDifferentActualDimensionsAndLandings()
        {
            Assert.AreEqual(5,Archipelago.Islands.Select(i=>i.size).Distinct().Count());
            Assert.GreaterOrEqual(Archipelago.Islands.Select(i=>i.dock).Distinct().Count(),4);
            foreach(var i in Archipelago.Islands)
            {Assert.IsFalse(WorldView.Water(i.arrival,i.id),i.key);Assert.IsTrue(Archipelago.Road(i.arrival,i.id,.3f),i.key);var art=WorldView.Art(i.key);Assert.AreEqual(i.size.x*32,art.rect.width);Assert.AreEqual(i.size.y*32,art.rect.height);}
        }
        [Test] public void WholeVisibleSoilFootprintCannotOverlapPavingOrWater()
        {
            foreach(var i in Archipelago.Islands)
            for(int x=-(int)i.size.x/2;x<(int)i.size.x/2;x++)for(int y=-(int)i.size.y/2;y<(int)i.size.y/2;y++)
            {
                Vector2 tile=new Vector2(x,y);if(!Archipelago.Tillable(tile,i.id))continue;
                for(int a=-2;a<=2;a++)for(int b=-2;b<=2;b++)
                {var sample=tile+new Vector2(a*.25f,b*.25f);Assert.IsFalse(Archipelago.Road(sample,i.id),i.key+sample);Assert.IsFalse(WorldView.Water(sample,i.id),i.key+sample);}
            }
        }
        [Test] public void WideIslandsPermitGardeningOutsideTheOldMapLimit()
        {var game=new GameModel(Content.Load(),Progress.New());bool found=false;for(int x=-24;x<=24;x++)for(int y=-9;y<8;y++)if(Mathf.Abs(x)>18&&Archipelago.Tillable(new Vector2(x,y),3)){Assert.IsTrue(ItemInventory.Till(game,3,new Vector2(x,y)));found=true;break;}Assert.IsTrue(found);}
        [Test] public void CreatureBodyHitTestsWorkFromOrdinaryApproachDistance()
        {
            EditorSceneManager.OpenScene("Assets/Wildfeast/Scenes/Wildfeast.unity");var world=Object.FindFirstObjectByType<WorldView>();
            foreach(var island in Archipelago.Islands)
            foreach(var point in world.points.Where(p=>p.transform.IsChildOf(world.IslandRoot(island.id))&&p.GetComponent<FoodEcology>()))
            {world.SetArea(island.id,(Vector2)point.transform.position+Vector2.down*2.1f);Assert.AreSame(point,world.Nearest(),point.label);Assert.AreSame(point,world.PointAt(point.artwork.bounds.center),point.label+" visible body");}
        }
        [Test] public void ReadyAndHarvestedEcologyCannotSpendWaterOrContactsAgain()
        {var g=new GameModel(Content.Load(),Progress.New());Assert.IsTrue(ItemInventory.EcologyAction(g,"crab-ember","crab",9));Assert.IsTrue(ItemInventory.EcologyAction(g,"crab-ember","crab",9));Assert.IsTrue(ItemInventory.EcologyAction(g,"crab-ember","crab",9));Assert.IsFalse(ItemInventory.EcologyAction(g,"crab-ember","crab",9));Assert.IsTrue(g.Gather("spiceclaw",1,"crab-ember"));Assert.IsFalse(g.Gather("spiceclaw",1,"crab-ember"));Assert.IsFalse(ItemInventory.EcologyAction(g,"crab-ember","crab",9));}
        [Test] public void FullSatchelDoesNotConsumeReadyCreatureReward()
        {var g=new GameModel(Content.Load(),Progress.New());Assert.IsTrue(ItemInventory.EcologyAction(g,"snail-tide","snail",2));Assert.IsTrue(g.Gather("grain",g.Capacity));Assert.IsFalse(g.Gather("kelpjelly",1,"snail-tide"));Assert.IsTrue(g.State.ecology.Single().ready);Assert.IsFalse(g.State.harvested.Contains("snail-tide"));g.Deposit();Assert.IsTrue(g.Gather("kelpjelly",1,"snail-tide"));Assert.AreEqual(1,g.Count("kelpjelly",true));}
    }
}
