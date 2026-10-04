using UnityEngine;
namespace Wildfeast
{
    public class HarvestNode : MonoBehaviour
    {
        public SpriteRenderer art;public Sprite original;public string item,source;public int required,hits,damage,lastDay;public Collider2D[] colliders;public bool falling;
    }
}
