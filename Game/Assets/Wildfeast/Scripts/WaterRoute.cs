using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace Wildfeast
{
    public static class WaterRoute
    {
        public static bool Clear(Vector2 p,int area)
        {foreach(var offset in new[]{Vector2.zero,new Vector2(-.9f,0),new Vector2(.9f,0),new Vector2(0,.6f),new Vector2(0,-.6f)})if(!WorldView.Water(p+offset,area))return false;return true;}
        public static Vector2 Harbor(int area)
        {
            var dock=Archipelago.Get(area).dock;var candidates=new List<Vector2>();
            for(int x=-8;x<=8;x++)for(int y=-8;y<=8;y++){var p=(Vector2)TerrainGrid.Cell(dock)+new Vector2(x,y);if(Clear(p,area))candidates.Add(p);}
            return candidates.OrderBy(p=>(p-dock).sqrMagnitude).First();
        }
        public static Vector2[] Escape(int area,float seaY)
        {
            var start=TerrainGrid.Cell(Harbor(area));var goal=new Vector2Int(start.x,Mathf.FloorToInt(seaY));var size=Archipelago.Get(area).size;
            var queue=new Queue<Vector2Int>();var parents=new Dictionary<Vector2Int,Vector2Int>();queue.Enqueue(start);parents[start]=start;
            while(queue.Count>0)
            {
                var p=queue.Dequeue();if(p==goal)break;
                foreach(var d in new[]{Vector2Int.down,Vector2Int.left,Vector2Int.right,Vector2Int.up})
                {var q=p+d;if(Mathf.Abs(q.x)>size.x/2+12||q.y>size.y/2+8||q.y<seaY-4||parents.ContainsKey(q)||!Clear(q,area))continue;parents[q]=p;queue.Enqueue(q);}
            }
            if(!parents.ContainsKey(goal))throw new System.InvalidOperationException("No navigable harbor route: "+area);
            var result=new List<Vector2>();var current=goal;while(current!=start){result.Add(current);current=parents[current];}result.Add(start);result.Reverse();return result.ToArray();
        }
    }
}
