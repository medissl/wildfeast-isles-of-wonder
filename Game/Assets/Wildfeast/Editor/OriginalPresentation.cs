using UnityEditor;
using UnityEngine;
namespace Wildfeast.Editor
{
    public static class OriginalPresentation
    {
        static void Frame(UnityEngine.UI.Image image,string name)
        {image.sprite=AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Wildfeast/Resources/Art/"+name+".png");image.color=Color.white;image.type=UnityEngine.UI.Image.Type.Sliced;image.pixelsPerUnitMultiplier=2;}
        public static void Author(WorldView world)
        {
            var ui=Object.FindFirstObjectByType<GameUI>();
            Frame(ui.wallet.transform.parent.GetComponent<UnityEngine.UI.Image>(),"ui-frame");Frame(ui.toolBelt.GetComponent<UnityEngine.UI.Image>(),"ui-frame");Frame(ui.page.GetComponent<UnityEngine.UI.Image>(),"ui-frame");
            foreach(var image in ui.toolFrames)Frame(image,"ui-slot");Frame(ui.pauseButton.GetComponent<UnityEngine.UI.Image>(),"ui-button");Frame(ui.closeButton.GetComponent<UnityEngine.UI.Image>(),"ui-button");
            ui.wallet.color=GameUI.C("7b302e");ui.location.color=GameUI.C("705440");ui.title.color=GameUI.C("493326");ui.subtitle.color=GameUI.C("705440");ui.prompt.color=GameUI.C("f9e7be");ui.equippedLabel.color=GameUI.C("f9e7be");ui.toast.color=GameUI.C("493326");
            foreach(string name in new[]{"sea-frame","sea-slot","sea-button","title-nameboard","title-horizon-study"})AssetDatabase.DeleteAsset("Assets/Wildfeast/Resources/Art/"+name+".png");
        }
    }
}
