using UnityEngine;
namespace Wildfeast
{
    // Native frame swaps move foliage one pixel; trunks, roots and collision never drift.
    public class VegetationMotion : MonoBehaviour
    {
        public string prefix; public int frameCount=4; public float phase;
        public HarvestNode node; SpriteRenderer artwork; Sprite[] frames; Sprite stump;
        void Start(){artwork=GetComponent<SpriteRenderer>();stump=WorldView.Art("stump");frames=new Sprite[frameCount];for(int i=0;i<frameCount;i++)frames[i]=WorldView.Art(prefix+"-"+i);}
        void Update()
        {if(!artwork||frames==null||!node||node.falling||artwork.sprite==null||artwork.sprite==stump)return;var f=frames[(int)(Time.time*.8f+Mathf.Abs(phase))%frameCount];if(f)artwork.sprite=f;}
    }
}
