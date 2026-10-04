using UnityEngine;
namespace Wildfeast
{
    public class ResidentActor:MonoBehaviour
    {
        public string id;public WorldPoint point;public Vector2[] route;
        int waypoint;float rest=2;Vector2 facing=Vector2.down;GameController game;
        void Start(){game=FindFirstObjectByType<GameController>();}
        void Update()
        {
            if(!game||!game.JourneyActive||game.ui.PageOpen||game.ActiveActivity!=null||game.Sailing||route==null||route.Length<2)return;
            Vector2 movement=Vector2.zero;rest-=Time.deltaTime;
            if(rest<=0){var delta=route[waypoint]-(Vector2)transform.localPosition;if(delta.magnitude<.05f){waypoint=(waypoint+1)%route.Length;rest=3+(waypoint%3)*1.5f;}else{movement=delta.normalized;var next=(Vector2)transform.localPosition+movement*Time.deltaTime*.6f;if(!WorldView.Water(next,game.world.Area))transform.localPosition=next;}}
            if(movement.sqrMagnitude>.01f)facing=movement;
            string direction=Mathf.Abs(facing.x)>Mathf.Abs(facing.y)?facing.x<0?"left":"right":facing.y>0?"up":"down";
            point.artwork.sprite=WorldView.Art("resident-"+id+"-"+direction+"-"+(movement.sqrMagnitude>.01f?(int)(Time.time*6)%4:0));point.artwork.sortingOrder=1000-Mathf.RoundToInt(transform.position.y*32);
        }
    }
}
