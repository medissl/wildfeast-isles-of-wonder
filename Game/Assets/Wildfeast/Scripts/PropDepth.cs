using UnityEngine;

namespace Wildfeast
{
    // Ground furniture sorts at its footprint, independently of image height.
    public class PropDepth : MonoBehaviour
    {
        public float groundOffset;
        public int bias;
        public SpriteRenderer artwork;
        public int Order=>1000-Mathf.RoundToInt((transform.position.y+groundOffset)*32)+bias;
        public void Apply(){if(!artwork)artwork=GetComponent<SpriteRenderer>();if(artwork)artwork.sortingOrder=Order;}
        void OnEnable(){Apply();}
        void LateUpdate(){Apply();}
    }
}
