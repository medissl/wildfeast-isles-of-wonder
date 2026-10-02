using System;
using System.Linq;
using UnityEngine;

namespace Wildfeast
{
    [Serializable] public class IslandRoute { public Vector2[] points; }
    [Serializable] public class IslandRegion { public Vector2 center,size; public string label; }
    [Serializable] public class IslandPlacement { public string art,role; public Vector2 position; }
    [Serializable] public class IslandPool {public Vector2 center,size;}
    [Serializable] public class IslandPoint { public string action,label,item,source,art,kind; public Vector2 position; }
    [Serializable] public class IslandDefinition
    {
        public int id; public string key,name,subtitle,ground,path,tree;
        public Vector2[] coast; public Vector2 pond,pondSize;
        public IslandPool[] pools;
        public IslandRoute[] roads; public IslandRegion[] regions;
        public IslandPlacement[] props; public IslandPoint[] points;
    }
    [Serializable] public class IslandCatalog { public IslandDefinition[] islands; }
    // Area 2 remains the restaurant so existing scene references and saves keep their meaning.
    public static class Archipelago
    {
        static IslandDefinition[] definitions;
        public static IslandDefinition[] Islands => definitions??(definitions=JsonUtility.FromJson<IslandCatalog>(Resources.Load<TextAsset>("Archipelago").text).islands);
        public static bool Valid(int area)=>Islands.Any(i=>i.id==area);
        public static IslandDefinition Get(int area)=>Islands.FirstOrDefault(i=>i.id==area)??Islands[0];
        public static float SegmentDistance(Vector2 p,Vector2 a,Vector2 b)
        {Vector2 d=b-a;float t=d.sqrMagnitude>0?Mathf.Clamp01(Vector2.Dot(p-a,d)/d.sqrMagnitude):0;return Vector2.Distance(p,a+d*t);}
        public static bool Road(Vector2 p,int area,float margin=0)
        {return Get(area).roads.Any(r=>Enumerable.Range(0,r.points.Length-1).Any(n=>SegmentDistance(p,r.points[n],r.points[n+1])<=.65f+margin));}
        public static bool Tillable(Vector2 p,int area)=>Valid(area)&&!WorldView.Water(p,area)&&!Road(p,area,.42f);
        public static IslandPool[] Pools(int area)
        {var i=Get(area);return new[]{new IslandPool{center=i.pond,size=i.pondSize}}.Concat(i.pools??Array.Empty<IslandPool>()).ToArray();}
        public static void Author(WorldView world)
        {
            world.islands=new Transform[Islands.Length];
            for(int n=0;n<Islands.Length;n++)
            {
                var definition=Islands[n];var root=new GameObject(definition.name).transform;root.SetParent(world.transform,false);world.islands[n]=root;
                WorldView.Add(root,definition.key,Vector2.zero,-2000,true);
                var coast=new GameObject("Shoreline",typeof(EdgeCollider2D));coast.transform.SetParent(root,false);coast.GetComponent<EdgeCollider2D>().points=definition.coast.Concat(new[]{definition.coast[0]}).ToArray();
                foreach(var spring in Pools(definition.id))
                {
                    var pool=new GameObject("Spring bank",typeof(PolygonCollider2D));pool.transform.SetParent(root,false);
                    pool.GetComponent<PolygonCollider2D>().points=Enumerable.Range(0,32).Select(k=>spring.center+new Vector2(Mathf.Cos(k*Mathf.PI/16)*spring.size.x,Mathf.Sin(k*Mathf.PI/16)*spring.size.y)).ToArray();
                }
                foreach(var placement in definition.props)
                {
                    var sr=WorldView.Add(root,placement.art,placement.position,1000-Mathf.RoundToInt(placement.position.y*32));
                    if(placement.role=="landmark")
                    {
                        WorldView.Block(sr.transform,placement.art=="trail-post"?new Vector2(.2f,.2f):new Vector2(1.45f,.45f),Vector2.up*.15f);
                        var anchor=sr.gameObject.AddComponent<PropDepth>();anchor.groundOffset=.15f;anchor.Apply();continue;
                    }
                    var node=sr.gameObject.AddComponent<HarvestNode>();node.art=sr;node.original=sr.sprite;
                    node.item=placement.role=="tree"?"wood":placement.role=="stone"?"stone":"fiber";
                    node.required=node.item=="wood"?7:node.item=="stone"?9:8;node.hits=node.item=="fiber"?1:3;
                    node.source=$"archipelago-{definition.id}-{placement.position.x:F2}-{placement.position.y:F2}";
                    if(node.item!="fiber")WorldView.Block(sr.transform,node.item=="wood"?new Vector2(.5f,.4f):new Vector2(1.1f,.5f),Vector2.up*.15f);
                    node.colliders=sr.GetComponentsInChildren<Collider2D>();
                    var depth=sr.gameObject.AddComponent<PropDepth>();depth.groundOffset=.15f;depth.Apply();
                    if(node.item!="stone")
                    {
                        var motion=sr.gameObject.AddComponent<VegetationMotion>();motion.prefix=placement.art;motion.frameCount=node.item=="wood"?4:3;motion.node=node;motion.phase=placement.position.x+placement.position.y;
                    }
                }
                foreach(var p in definition.points)
                {
                    var point=world.Point(root,p.action,p.label,p.position,p.item,p.source,p.art);
                    if(p.action=="creature"||p.action=="bud"||p.action=="tap")
                    {
                        var behavior=point.gameObject.AddComponent<FoodEcology>();behavior.kind=string.IsNullOrEmpty(p.kind)?p.action:p.kind;behavior.point=point;behavior.sprite=p.art;
                        if(p.kind=="ram"||p.kind=="crab"||p.kind=="snail")WorldView.Block(point.transform,new Vector2(.9f,.55f),Vector2.up*.25f);
                        if(p.action=="tap")WorldView.Block(point.transform,new Vector2(.4f,.4f),Vector2.up*.15f);
                    }
                }
                // Tiny ripples and drifting motes inhabit water and groves, keeping roads quiet.
                for(int k=0;k<8;k++)
                {
                    Vector2 pos=definition.pond+new Vector2(Mathf.Cos(k)*2,Mathf.Sin(k)*.9f);
                    WorldView.Add(root,"ripple-0",pos,-1499).gameObject.AddComponent<WorldMotion>().mode=5;
                }
                foreach(var region in definition.regions.Take(2))
                for(int k=0;k<3;k++){var light=WorldView.Add(root,n==3?"spark":"butterfly",region.center+new Vector2(k-1,.5f),1400);var motion=light.gameObject.AddComponent<WorldMotion>();motion.mode=1;motion.phase=k;motion.radius=.25f;motion.speed=.5f;}
            }
            world.saltleaf=world.islands[0];world.mistwake=world.islands[1];
            var home=WorldView.Add(world.saltleaf,"restaurant",new Vector2(-6,-.9f),1030);WorldView.Block(home.transform,new Vector2(4.8f,2.7f),new Vector2(0,2));home.gameObject.AddComponent<PropDepth>().groundOffset=2;
            // Village center: one anchored fountain, workshop and garden, with open approaches.
            var fountain=WorldView.Add(world.saltleaf,"fountain",new Vector2(1.3f,1.4f),955);WorldView.Block(fountain.transform,new Vector2(1.5f,.9f),Vector2.up*.25f);fountain.gameObject.AddComponent<PropDepth>().groundOffset=.25f;
            for(int k=0;k<3;k++)
            {var plot=world.Point(world.saltleaf,"crop","Kitchen garden bed",new Vector2(-11+k*2,-4),"","","plot");plot.index=k;world.cropArt.Add(WorldView.Add(plot.transform,"pepperbell",Vector2.up*.3f,1100));}
            var cottage=WorldView.Add(world.mistwake,"iona-house",new Vector2(3,3),880);WorldView.Block(cottage.transform,new Vector2(3,1.8f),Vector2.up*1.2f);cottage.gameObject.AddComponent<PropDepth>().groundOffset=1.2f;
            // Workshop footprint, independent of the interaction prompt.
            foreach(var p in world.points.Where(p=>p.action=="upgrades"&&p.artwork))WorldView.Block(p.artwork.transform,new Vector2(1.5f,.6f),Vector2.up*.25f);
            var keeper=WorldView.Add(world.saltleaf,"guest-2",new Vector2(-.65f,-3.2f),1100);var stroll=keeper.gameObject.AddComponent<WorldMotion>();stroll.mode=4;stroll.radius=.25f;stroll.speed=.35f;
        }
    }
}
