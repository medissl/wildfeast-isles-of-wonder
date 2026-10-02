using System;
using UnityEngine;
using UnityEngine.EventSystems;
namespace Wildfeast
{
    public class InventoryDrag : MonoBehaviour,IBeginDragHandler,IDragHandler,IEndDragHandler
    {
        public int index;public GameUI ui;public Action<int,int> move;
        RectTransform ghost;
        public void OnBeginDrag(PointerEventData e)
        {
            var item=transform.Find("Item");if(!item)return;
            ghost=Instantiate(item.gameObject,ui.overlay).GetComponent<RectTransform>();ghost.position=e.position;ghost.SetAsLastSibling();
        }
        public void OnDrag(PointerEventData e){if(ghost)ghost.position=e.position;}
        public void OnEndDrag(PointerEventData e)
        {
            if(ghost)Destroy(ghost.gameObject);
            var destination=e.pointerCurrentRaycast.gameObject?.GetComponentInParent<InventoryDrag>();
            if(destination&&destination.index!=index)move(index,destination.index);
        }
    }
}
