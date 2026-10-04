using System.Collections;
using UnityEngine;
using UnityEngine.UI;
namespace Wildfeast
{
    public class SceneCurtain : MonoBehaviour
    {
        Image image;
        public static SceneCurtain Create(GameUI ui)
        {
            var rt=GameUI.Box(ui.transform,"Journey fade",Vector2.zero,Vector2.zero,new Vector2(.5f,.5f),new Color(.07f,.12f,.16f,0));rt.anchorMin=Vector2.zero;rt.anchorMax=Vector2.one;rt.offsetMin=rt.offsetMax=Vector2.zero;
            var curtain=rt.gameObject.AddComponent<SceneCurtain>();curtain.image=rt.GetComponent<Image>();curtain.image.raycastTarget=false;return curtain;
        }
        public IEnumerator Fade(bool dark)
        {
            transform.SetAsLastSibling();float start=image.color.a,end=dark?1:0;
            for(float t=0;t<.25f;t+=Time.unscaledDeltaTime){image.color=new Color(.07f,.12f,.16f,Mathf.Lerp(start,end,t/.25f));yield return null;}
            image.color=new Color(.07f,.12f,.16f,end);
        }
    }
}
