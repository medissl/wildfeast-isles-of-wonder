using System.Collections.Generic;
using UnityEngine;
namespace Wildfeast
{
    // Persisted field coordinates are integer centres. Cell edges are at +/- .5.
    public static class TerrainGrid
    {
        static readonly Dictionary<int,HashSet<int>> land=new Dictionary<int,HashSet<int>>(),roads=new Dictionary<int,HashSet<int>>(),decks=new Dictionary<int,HashSet<int>>();
        public static Vector2Int Cell(Vector2 p)=>new Vector2Int(Mathf.FloorToInt(p.x+.5f),Mathf.FloorToInt(p.y+.5f));
        static int Index(Vector2Int p,IslandDefinition i)=> (p.y+Mathf.RoundToInt(i.size.y)/2)*Mathf.RoundToInt(i.size.x)+p.x+Mathf.RoundToInt(i.size.x)/2;
        static bool InBounds(Vector2Int p,IslandDefinition i)=>p.x>=-i.size.x/2&&p.x<i.size.x/2&&p.y>=-i.size.y/2&&p.y<i.size.y/2;
        public static bool Land(Vector2Int p,int area)
        {
            var i=Archipelago.Get(area);if(!InBounds(p,i))return false;
            if(!land.TryGetValue(area,out var cells)){cells=new HashSet<int>(i.gridLand);land.Add(area,cells);}return cells.Contains(Index(p,i));
        }
        public static bool Road(Vector2Int p,int area)
        {
            var i=Archipelago.Get(area);if(!InBounds(p,i))return false;
            if(!roads.TryGetValue(area,out var cells)){cells=new HashSet<int>(i.gridRoad);roads.Add(area,cells);}return cells.Contains(Index(p,i));
        }
        public static bool Deck(Vector2Int p,int area)
        {
            var i=Archipelago.Get(area);if(!InBounds(p,i))return false;
            if(!decks.TryGetValue(area,out var cells)){cells=new HashSet<int>(i.gridDeck);decks.Add(area,cells);}return cells.Contains(Index(p,i));
        }
        public static int Neighbors(Vector2Int p,int area,bool road=false)
        {
            var directions=new[]{Vector2Int.up,Vector2Int.right,Vector2Int.down,Vector2Int.left};int mask=0;
            for(int n=0;n<4;n++)if(road?Road(p+directions[n],area):Land(p+directions[n],area))mask|=1<<n;return mask;
        }
        public static bool Tillable(Vector2Int p,int area)=>Archipelago.Valid(area)&&Land(p,area)&&!Road(p,area)&&Neighbors(p,area)==15;
        public static int Variant(Vector2Int p,int count)=> (int)((uint)(p.x*73856093 ^ p.y*19349663)%count);
    }
}
