using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Tilemaps;
namespace Wildfeast.Tests
{
    public class TileWorldTests
    {
        [Test] public void ToolAndPlotLookupAgreeAtExactCellBoundaries()
        {
            var game=new GameModel(Content.Load(),Progress.New());
            for(int x=-10;x<=10;x+=2)for(int y=-6;y<=6;y++)
            {
                if(!Archipelago.Tillable(new Vector2(x,y),3)||!Archipelago.Tillable(new Vector2(x+1,y),3))continue;
                var edge=new Vector2(x+.5f,y);Assert.IsTrue(ItemInventory.Till(game,3,edge));var plot=game.State.fields.Single();
                Assert.AreEqual(x+1,plot.x);Assert.AreSame(plot,ItemInventory.Plot(game.State,3,edge));Assert.AreSame(plot,ItemInventory.Plot(game.State,3,new Vector2(x+1,y)));Assert.IsNull(ItemInventory.Plot(game.State,3,new Vector2(x,y)));return;
            }
            Assert.Fail("No neighboring clear cells in Emberfold");
        }
        [Test] public void RestaurantForecourtHasNoRaisedGardenBoxes()
        {
            EditorSceneManager.OpenScene("Assets/Wildfeast/Scenes/Wildfeast.unity");var world=Object.FindFirstObjectByType<WorldView>();
            Assert.IsFalse(world.points.Any(p=>p.action=="crop"));Assert.NotNull(world.bedroom);
            Assert.IsTrue(world.points.Any(p=>p.action=="bed"&&p.transform.IsChildOf(world.bedroom)));Assert.IsTrue(world.points.Any(p=>p.action=="bedroom"&&p.transform.IsChildOf(world.restaurant)));
        }
        [Test] public void FieldCoordinatesMatchNativeTileCenters()
        {
            EditorSceneManager.OpenScene("Assets/Wildfeast/Scenes/Wildfeast.unity");var world=Object.FindFirstObjectByType<WorldView>();
            foreach(var i in Archipelago.Islands)
            {world.SetArea(i.id,i.arrival);var tiles=world.IslandRoot(i.id).GetComponentInChildren<TileWorld>(true);Assert.NotNull(tiles);Assert.AreEqual(new Vector3(3,-4,0),tiles.ground.GetCellCenterWorld(new Vector3Int(3,-4,0)));Assert.IsFalse(world.IslandRoot(i.id).GetComponentsInChildren<SpriteRenderer>(true).Any(s=>s.name==i.key));}
        }
        [Test] public void EveryIslandTrailIsOneConnectedNetwork()
        {
            foreach(var i in Archipelago.Islands)
            {
                var cells=new HashSet<Vector2Int>();for(int x=-(int)i.size.x/2;x<i.size.x/2;x++)for(int y=-(int)i.size.y/2;y<i.size.y/2;y++)if(TerrainGrid.Road(new Vector2Int(x,y),i.id))cells.Add(new Vector2Int(x,y));
                var visited=new HashSet<Vector2Int>();var queue=new Queue<Vector2Int>();queue.Enqueue(TerrainGrid.Cell(i.arrival));
                while(queue.Count>0){var cell=queue.Dequeue();if(!cells.Contains(cell)||!visited.Add(cell))continue;foreach(var d in new[]{Vector2Int.up,Vector2Int.right,Vector2Int.down,Vector2Int.left})queue.Enqueue(cell+d);}
                Assert.AreEqual(cells.Count,visited.Count,i.key);foreach(var cell in cells){Assert.IsTrue(TerrainGrid.Land(cell,i.id));Assert.IsFalse(Archipelago.Tillable(cell,i.id));}
            }
        }
        [Test] public void NeighboringSoilNeverModifiesAPathCell()
        {
            EditorSceneManager.OpenScene("Assets/Wildfeast/Scenes/Wildfeast.unity");var world=Object.FindFirstObjectByType<WorldView>();
            foreach(var i in Archipelago.Islands)
            {
                var tiles=world.IslandRoot(i.id).GetComponentInChildren<TileWorld>(true);bool found=false;
                for(int x=-(int)i.size.x/2;x<i.size.x/2&&!found;x++)for(int y=-(int)i.size.y/2;y<i.size.y/2&&!found;y++)
                {
                    var p=new Vector2Int(x,y);if(!TerrainGrid.Tillable(p,i.id))continue;var neighbor=new[]{Vector2Int.up,Vector2Int.right,Vector2Int.down,Vector2Int.left}.Select(d=>p+d).FirstOrDefault(q=>TerrainGrid.Road(q,i.id));
                    if(!TerrainGrid.Road(neighbor,i.id))continue;found=true;var path=tiles.paths.GetTile((Vector3Int)neighbor);tiles.Soil(x,y,false);Assert.AreSame(path,tiles.paths.GetTile((Vector3Int)neighbor));Assert.IsNull(tiles.soil.GetTile((Vector3Int)neighbor));Assert.NotNull(tiles.soil.GetTile((Vector3Int)p));tiles.ClearSoil();
                }
                Assert.IsTrue(found,"There should be plantable ground beside trails on "+i.key);
            }
        }
        [Test] public void WaterTilesAnimateAndBlockTheSameCellsUsedForFishing()
        {
            EditorSceneManager.OpenScene("Assets/Wildfeast/Scenes/Wildfeast.unity");var world=Object.FindFirstObjectByType<WorldView>();
            foreach(var i in Archipelago.Islands)
            {
                var tiles=world.IslandRoot(i.id).GetComponentInChildren<TileWorld>(true);Assert.NotNull(tiles.water.GetComponent<TilemapCollider2D>());
                foreach(var pos in tiles.water.cellBounds.allPositionsWithin)
                {
                    var tide=tiles.water.GetTile<TidalTile>(pos);if(!tide)continue;Assert.IsTrue(WorldView.Water(new Vector2(pos.x,pos.y),i.id));Assert.AreEqual(Tile.ColliderType.Grid,tide.colliderType);Assert.AreEqual(6,tide.frames.Length);Assert.IsTrue(tide.frames.All(f=>f&&f.rect.width==32&&f.pixelsPerUnit==32));
                }
            }
        }
    }
}
