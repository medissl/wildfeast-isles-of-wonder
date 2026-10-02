using UnityEngine;

namespace Wildfeast
{
    public class WorldMotion : MonoBehaviour
    {
        public int mode;
        public float speed=1, phase, radius=.35f;
        public Vector3 destination;
        Vector3 origin;
        SpriteRenderer art;
        float born;
        void OnEnable(){origin=transform.localPosition;art=GetComponent<SpriteRenderer>();born=Time.time;}
        void Update()
        {
            float t=Time.time*speed+phase;
            if(mode==0)transform.localPosition=origin+new Vector3(Mathf.Round(Mathf.Sin(t)*1)/32,0,0);
            if(mode==1||mode==4){transform.localPosition=origin+new Vector3(Mathf.Sin(t)*radius,Mathf.Sin(t*.7f)*radius*.3f,0);art.flipX=Mathf.Cos(t)<0;}
            if(mode==2)art.color=new Color(1,1,1,.7f+Mathf.Sin(t)*.15f);
            if(mode==3){transform.localPosition+=Vector3.up*Time.deltaTime*speed;var c=art.color;c.a=Mathf.Clamp01(1-(Time.time-born)/.8f);art.color=c;}
            if(mode==5){art.sprite=WorldView.Art("ripple-"+((int)(t*2)%4));}
            if(mode==6){float f=Mathf.Clamp01((Time.time-born)/.65f);transform.position=Vector3.Lerp(origin,destination,f)+Vector3.up*Mathf.Sin(f*Mathf.PI)*1.8f;}
        }
    }
}
