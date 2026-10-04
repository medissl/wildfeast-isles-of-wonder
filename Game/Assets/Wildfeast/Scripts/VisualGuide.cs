using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace Wildfeast
{
    // Optional picture cards, opened deliberately; nothing interrupts ordinary play.
    public static class VisualGuide
    {
        static readonly string[] titles={"A world to wander","Make a little garden","Cast into the blue","Bring it to the table"};
        static readonly string[] pictures={"guide-explore","guide-garden","guide-fishing","guide-kitchen"};
        static readonly string[] hints={"WASD to walk. E or right click to interact. M opens your island map.","Shovel a grass cell, plant a seed, then water it. Refill your can beside water.","Choose the rod beside water. Hold mouse or Space to keep the green zone under the fish.","Bring ingredients home. Choose tonight's menu, flip the OPEN sign, cook and serve."};
        public static void Show(GameUI ui,int page,Action back)
        {
            page=(page+titles.Length)%titles.Length;ui.Show(titles[page],"");
            var picture=GameUI.Box(ui.rows,"Guide screenshot",new Vector2(82,-8),new Vector2(580,270),new Vector2(0,1),Color.white).GetComponent<Image>();picture.sprite=WorldView.Art(pictures[page]);picture.preserveAspect=true;picture.raycastTarget=false;
            var text=GameUI.Text(ui.rows,hints[page],new Vector2(82,-289),new Vector2(580,54),19,GameUI.C("3b353a"));text.alignment=TextAlignmentOptions.Center;
            int current=page;GameUI.Button(ui.rows,"<",new Vector2(0,-113),new Vector2(48,48),()=>Show(ui,current-1,back),new Vector2(0,1));
            GameUI.Button(ui.rows,">",new Vector2(696,-113),new Vector2(48,48),()=>Show(ui,current+1,back),new Vector2(0,1));
            GameUI.Text(ui.rows,(page+1)+" / "+titles.Length,new Vector2(330,-350),new Vector2(120,28),18,GameUI.C("3b353a"));
            GameUI.Button(ui.rows,"Back",new Vector2(0,-350),new Vector2(180,44),back,new Vector2(0,1));
        }
    }
}
