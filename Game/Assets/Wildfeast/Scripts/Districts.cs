using System.Linq;
using UnityEngine;
namespace Wildfeast
{
    public static class Districts
    {
        public static readonly int[] Ports={0,3,5};
        public static int Island(int district)=>district==1||district==7?0:district==4?3:district;
        public static string IslandName(int district)=>Island(district)==0?"Saltleaf Island":Island(district)==3?"Ember & Moon Island":"Pearltide Island";
        public static string RoomName(WorldView world)=>world.GetComponentsInChildren<ResidentRoom>(true).FirstOrDefault(r=>r.area==world.Area)?.label??"YOUR ROOM";
        public static bool Interior(int area)=>area==2||area==6||area>=8;
    }
}
