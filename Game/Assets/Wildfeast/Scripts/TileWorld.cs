using UnityEngine;
using UnityEngine.Tilemaps;
namespace Wildfeast
{
    public class TileWorld : MonoBehaviour
    {
        public int area;
        public Tilemap ground,paths,water,soil;
        public void Soil(int x,int y,bool wet)=>soil.SetTile(new Vector3Int(x,y,0),Resources.Load<TileBase>("Terrain/tile-soil"+(wet?"-wet":"")));
        public void ClearSoil()=>soil.ClearAllTiles();
        public static GameObject Ocean(Transform parent,bool reducedMotion)
        {
            var go=new GameObject("Open sea grid",typeof(Grid));go.transform.SetParent(parent,false);go.transform.localPosition=new Vector3(-.5f,-.5f,0);
            var map=Layer(go.transform,"Open sea",-2100);map.animationFrameRate=reducedMotion?0:1;
            var variants=new[]{Load("tile-water-0-0"),Load("tile-water-0-1")};
            for(int x=-25;x<90;x++)for(int y=-80;y<20;y++)map.SetTile(new Vector3Int(x,y,0),variants[TerrainGrid.Variant(new Vector2Int(x,y),2)]);
            return go;
        }
        public static void AuthorRoom(Transform parent)
        {
            var previous=parent.GetComponentInChildren<TileWorld>(true);if(previous)Object.DestroyImmediate(previous.gameObject);
            var go=new GameObject("Restaurant floor grid",typeof(Grid),typeof(TileWorld));go.transform.SetParent(parent,false);go.transform.localPosition=new Vector3(-.5f,-.5f,0);
            var world=go.GetComponent<TileWorld>();world.area=2;world.ground=Layer(go.transform,"Timber floor",-1990);world.paths=Layer(go.transform,"Woven rugs and kitchen tiles",-1980);world.soil=Layer(go.transform,"Cultivated cells",-1700);
            for(int x=-9;x<=9;x++)for(int y=-5;y<=3;y++)
            {
                var p=new Vector3Int(x,y,0);world.ground.SetTile(p,Load("tile-timber-"+TerrainGrid.Variant(new Vector2Int(x,y),4)));
                if(y>=2&&x>=-3&&x<=3)world.paths.SetTile(p,Load("tile-kitchen"));
                if(y>=-1&&y<=1&&((x>=-8&&x<=-1)||(x>=1&&x<=8)))world.paths.SetTile(p,Load("tile-rug"));
            }
        }
        public static TileWorld Author(Transform parent,IslandDefinition island)
        {
            var go=new GameObject("Terrain grid",typeof(Grid),typeof(TileWorld));go.transform.SetParent(parent,false);go.transform.localPosition=new Vector3(-.5f,-.5f,0);
            var world=go.GetComponent<TileWorld>();world.area=island.id;
            world.water=Layer(go.transform,"Tidal water",-2000);world.ground=Layer(go.transform,"Textured land",-1900);world.paths=Layer(go.transform,"Connected trails",-1800);world.soil=Layer(go.transform,"Cultivated cells",-1700);
            var body=world.water.gameObject.AddComponent<Rigidbody2D>();body.bodyType=RigidbodyType2D.Static;
            var collider=world.water.gameObject.AddComponent<TilemapCollider2D>();collider.compositeOperation=Collider2D.CompositeOperation.Merge;
            var composite=world.water.gameObject.AddComponent<CompositeCollider2D>();composite.geometryType=CompositeCollider2D.GeometryType.Polygons;
            for(int x=-(int)island.size.x/2;x<(int)island.size.x/2;x++)for(int y=-(int)island.size.y/2;y<(int)island.size.y/2;y++)
            {
                var cell=new Vector2Int(x,y);var p=new Vector3Int(x,y,0);int mask=TerrainGrid.Neighbors(cell,island.id);
                if(!TerrainGrid.Land(cell,island.id))world.water.SetTile(p,Load("tile-water-"+mask+"-"+TerrainGrid.Variant(cell,2)));
                else
                {
                    world.ground.SetTile(p,Load("tile-"+island.key+(mask==15?"-grass-"+TerrainGrid.Variant(cell,6):"-bank-"+mask)));
                    if(TerrainGrid.Road(cell,island.id))world.paths.SetTile(p,Load(TerrainGrid.Deck(cell,island.id)?"tile-deck-"+TerrainGrid.Neighbors(cell,island.id,true):"tile-"+island.key+"-path-"+TerrainGrid.Neighbors(cell,island.id,true)+"-"+TerrainGrid.Variant(cell,3)));
                }
            }
            collider.ProcessTilemapChanges();world.water.CompressBounds();world.ground.CompressBounds();world.paths.CompressBounds();return world;
        }
        static TileBase Load(string name)=>Resources.Load<TileBase>("Terrain/"+name);
        static Tilemap Layer(Transform parent,string name,int order)
        {
            var go=new GameObject(name,typeof(Tilemap),typeof(TilemapRenderer));go.transform.SetParent(parent,false);var renderer=go.GetComponent<TilemapRenderer>();renderer.sortingOrder=order;return go.GetComponent<Tilemap>();
        }
    }
}
