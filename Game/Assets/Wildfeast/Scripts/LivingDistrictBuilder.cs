using System.Linq;
using UnityEngine;
namespace Wildfeast
{
    // Supported scene authoring: useful homes, accessible doorways and purposeful garden borders.
    public static class LivingDistrictBuilder
    {
        public static void Author(WorldView world)
        {
            foreach(var previous in world.GetComponentsInChildren<ResidentRoom>(true))
            {world.points.RemoveAll(p=>!p||p.transform.IsChildOf(previous.transform));Object.DestroyImmediate(previous.gameObject);}
            foreach(var p in world.points.Where(p=>p&&p.action=="talk").ToArray())
            {
                var actor=p.gameObject.AddComponent<ResidentActor>();actor.id=p.source;actor.point=p;
                int area=Archipelago.Islands.First(i=>p.transform.IsChildOf(world.IslandRoot(i.id))).id;
                Vector2 origin=p.transform.localPosition;var cells=Archipelago.Get(area).roads.SelectMany(r=>r.points).Where(c=>Vector2.Distance(c,origin)<4).Distinct().OrderBy(c=>Vector2.Distance(c,origin)).ToArray();
                actor.route=cells.Length>1?new[]{origin,cells[0],cells[Mathf.Min(cells.Length-1,5)],origin}:new[]{origin,origin};
            }
            foreach(var p in world.points.Where(p=>p&&new[]{"home","shop"}.Contains(p.action)&&p.artwork))
            {WorldView.Block(p.artwork.transform,new Vector2(3.6f,2.7f),Vector2.up*1.7f);var depth=p.artwork.gameObject.AddComponent<PropDepth>();depth.groundOffset=.5f;depth.Apply();var chimney=WorldView.Add(p.transform,"steam",new Vector2(1.1f,3.6f),1800);chimney.gameObject.AddComponent<HabitatMotion>().kind="smoke";}
            Room(world,8,"Nori’s home",7,"portrait-nori-smile","nori",new Vector2(13,-9));
            Room(world,9,"Iona’s orchard home",1,"portrait-iona-smile","iona",new Vector2(3,1));
            Room(world,10,"Moon tea room",4,"portrait-luma-smile","luma",new Vector2(3,5));
        }
        static void Room(WorldView world,int id,string label,int district,string portrait,string resident,Vector2 outside)
        {
            var room=new GameObject(label).transform;room.SetParent(world.transform,false);var metadata=room.gameObject.AddComponent<ResidentRoom>();metadata.area=id;metadata.label=label;metadata.district=district;metadata.exterior=outside;
            WorldView.Add(room,"bedroom-interior",Vector2.zero,-2000,true);WorldView.Border(room,6.4f,4.4f);WorldView.Block(room,new Vector2(12.8f,1.6f),new Vector2(0,3.7f));
            var table=WorldView.Add(room,"table",new Vector2(-2,0),1000);WorldView.Block(table.transform,new Vector2(2,1.1f),Vector2.up*.55f);table.gameObject.AddComponent<PropDepth>().groundOffset=.55f;
            WorldView.Add(room,"held-dish-cloud",new Vector2(-2,1.2f),1200);var planter=WorldView.Add(room,"room-planter",new Vector2(4,2),900);WorldView.Block(planter.transform,new Vector2(.8f,.6f),Vector2.up*.3f);planter.gameObject.AddComponent<PropDepth>().groundOffset=.3f;var bed=WorldView.Add(room,"bed",new Vector2(3,0),1000);WorldView.Block(bed.transform,new Vector2(1.4f,1.5f),Vector2.up*.75f);bed.gameObject.AddComponent<PropDepth>().groundOffset=.75f;
            var guest=world.Point(room,"talk",Residents.Get(resident).name,new Vector2(-3,2),"",resident,"resident-"+resident+"-down-0");
            var exit=world.Point(room,"passage","Outside",new Vector2(0,-3.7f),"",district+"|"+outside.x.ToString(System.Globalization.CultureInfo.InvariantCulture)+"|"+outside.y.ToString(System.Globalization.CultureInfo.InvariantCulture),"room-stairs");
            if(id==10)world.Point(room,"shop","Moon tea seed shelf",new Vector2(3,2),"","","crate");
            room.gameObject.SetActive(false);
        }
    }
}
