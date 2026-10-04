using UnityEngine;
namespace Wildfeast
{
    public class HabitatMotion:MonoBehaviour
    {
        public string kind="butterfly";public int area;public float radius=4;Vector2 home,target;SpriteRenderer art;float wait,phase;string frames;GameController game;
        void Start(){home=transform.localPosition;target=home;art=GetComponent<SpriteRenderer>();frames=art.sprite?art.sprite.name:null;if(frames!=null&&frames.EndsWith("-0"))frames=frames.Substring(0,frames.Length-2);phase=Mathf.Abs(home.x*3.1f+home.y*5.7f);wait=phase%4;game=FindFirstObjectByType<GameController>();}
        void Update()
        {
            if(!game||game.Model.State.reducedMotion)return;
            float t=Time.time+phase;
            if(kind=="smoke"){float u=Mathf.Repeat(t*.27f,1);transform.localPosition=home+new Vector2(Mathf.Sin(t*.5f)*.13f,u*.8f);art.color=new Color(1,.94f,.81f,(1-u)*.38f);return;}
            if(!game.JourneyActive||game.ui.PageOpen||game.Sailing)return;
            wait-=Time.deltaTime;
            if(wait<=0&&Vector2.Distance(transform.localPosition,target)<.2f)
            {
                for(int attempt=0;attempt<10;attempt++)
                {var candidate=home+Random.insideUnitCircle*radius;if(!WorldView.Water(candidate,area)&&!Physics2D.OverlapCircle(candidate+Vector2.up*.2f,.2f)){target=candidate;break;}}
                wait=kind=="animal"?Random.Range(1.5f,4):Random.Range(.5f,2);
            }
            Vector2 position=transform.localPosition;
            if(kind=="animal"&&Vector2.Distance(game.world.player.position,transform.position)<2)
            {var away=(position-(Vector2)game.world.player.position).normalized;var next=position+away*Time.deltaTime*1.6f;if(!WorldView.Water(next,area)&&!Physics2D.OverlapCircle(next+Vector2.up*.2f,.2f))position=next;}
            else if(wait<=0){var next=Vector2.MoveTowards(position,target,Time.deltaTime*(kind=="animal"?.65f:.85f));if(kind!="animal"||!WorldView.Water(next,area)&&!Physics2D.OverlapCircle(next+Vector2.up*.2f,.2f))position=next;else{target=position;wait=0;}}
            if(kind=="animal"&&frames!=null){var frame=WorldView.Art(frames+"-"+((int)(t*5)%4));if(frame)art.sprite=frame;}
            var delta=position-(Vector2)transform.localPosition;art.flipX=delta.x<-.0001f;transform.localPosition=new Vector2(Mathf.Round(position.x*64)/64,Mathf.Round(position.y*64)/64);
            if(kind=="butterfly")transform.localPosition+=Vector3.up*Mathf.Sin(t*7)*.0015f;
            art.sortingOrder=1000-Mathf.RoundToInt(transform.position.y*32)+(kind=="animal"?0:90);
        }
    }
}
