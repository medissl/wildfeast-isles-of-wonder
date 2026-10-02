using UnityEngine;
using UnityEngine.EventSystems;

namespace Wildfeast
{
    public class PlateGarnish : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public GameUI ui;
        public RectTransform target;
        public int index;
        Vector3 origin;
        public void OnBeginDrag(PointerEventData e){origin=transform.position;}
        public void OnDrag(PointerEventData e){transform.position=e.position;}
        public void OnEndDrag(PointerEventData e)
        {
            if(RectTransformUtility.RectangleContainsScreenPoint(target,e.position,e.pressEventCamera)){ui.CookingAction=index;gameObject.SetActive(false);}
            else transform.position=origin;
        }
    }
}
