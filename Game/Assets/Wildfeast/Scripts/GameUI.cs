using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Wildfeast
{
    public class GameUI : MonoBehaviour
    {
        public Canvas canvas;
        public RectTransform overlay, page, rows, meter, cursor, progressFill, targetZone;
        public TMP_Text wallet, location, objective, prompt, toast, title, subtitle, miniLabel, miniHelp, miniProgress;
        public UnityEngine.UI.Button journalButton, bagButton, pauseButton, closeButton;
        public bool PageOpen => overlay.gameObject.activeSelf;
        public bool MiniOpen { get; private set; }
        public RectTransform toolBelt, activityPanel, cookingBoard;
        public UnityEngine.UI.Image[] toolIcons = new UnityEngine.UI.Image[8];
        public TMP_Text[] toolCounts = new TMP_Text[8];
        public UnityEngine.UI.Image[] toolFrames = new UnityEngine.UI.Image[8];
        public TMP_Text equippedLabel, activityText, orderText;
        public int CookingAction = -1;
        public Action<int> SelectTool;
        public Action Closed;
        static readonly Color Ink = C("152f36"), Panel = C("203e43"), Gold = C("e8bc74"), Cream = C("f6e7bf"), Dim = C("abc0ad");
        public static Color C(string hex) { ColorUtility.TryParseHtmlString("#" + hex, out var c); return c; }
        public void Init(Action journal, Action bag, Action pause)
        {
            journalButton.onClick.AddListener(() => journal()); bagButton.onClick.AddListener(() => bag()); pauseButton.onClick.AddListener(() => pause());
            closeButton.onClick.AddListener(Hide); Hide();
        }
        public void AuthorUI()
        {
            canvas = gameObject.AddComponent<Canvas>(); canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = gameObject.AddComponent<CanvasScaler>(); scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize; scaler.referenceResolution = new Vector2(1280, 720); scaler.matchWidthOrHeight = .5f;
            gameObject.AddComponent<GraphicRaycaster>();
            var status = Box(transform,"Day & purse",new Vector2(-18,-18),new Vector2(248,65),new Vector2(1,1),Ink);
            Frame(status);
            wallet = Text(status,"",new Vector2(16,-8),new Vector2(188,28),20,Gold);
            location = Text(status,"",new Vector2(16,-38),new Vector2(220,18),13,Dim);
            objective = Text(transform,"",new Vector2(22,-22),new Vector2(530,32),18,Cream);
            // Keyboard shortcuts live in the belt; exploration has no permanent wall of buttons.
            journalButton = Button(transform,"",Vector2.zero,new Vector2(1,1),()=>{},new Vector2(0,0));journalButton.gameObject.SetActive(false);
            bagButton = Button(transform,"",Vector2.zero,new Vector2(1,1),()=>{},new Vector2(0,0));bagButton.gameObject.SetActive(false);
            pauseButton = Button(status,"II",new Vector2(-8,-10),new Vector2(30,26),()=>{},new Vector2(1,1));
            toolBelt = Box(transform,"Tool belt",new Vector2(0,18),new Vector2(560,66),new Vector2(.5f,0),Ink);Frame(toolBelt);
            string[] icons={"tool-hand","tool-rod","tool-can","seed-pepper","seed-root","tool-knife","tool-journal","tool-bag"};
            for(int i=0;i<8;i++)
            {
                int slot=i;var b=Button(toolBelt,"",new Vector2(7+i*69,-7),new Vector2(62,52),()=>SelectTool?.Invoke(slot),new Vector2(0,1));
                toolFrames[i]=b.GetComponent<UnityEngine.UI.Image>();
                var icon=Box(b.transform,"Tool",new Vector2(17,-5),new Vector2(30,34),new Vector2(0,1),Color.white).GetComponent<UnityEngine.UI.Image>();icon.sprite=WorldView.Art(icons[i]);icon.preserveAspect=true;icon.raycastTarget=false;toolIcons[i]=icon;
                Text(b.transform,(i+1).ToString(),new Vector2(5,-3),new Vector2(14,18),12,Dim);
                toolCounts[i]=Text(b.transform,"",new Vector2(-4,2),new Vector2(48,18),12,Cream,new Vector2(1,0));toolCounts[i].alignment=TextAlignmentOptions.Right;
            }
            equippedLabel=Text(transform,"",new Vector2(0,86),new Vector2(560,24),16,Gold,new Vector2(.5f,0));equippedLabel.alignment=TextAlignmentOptions.Center;
            prompt = Text(transform,"",new Vector2(0,122),new Vector2(690,38),22,Cream,new Vector2(.5f,0)); prompt.alignment = TextAlignmentOptions.Center;
            prompt.outlineWidth=.2f;prompt.outlineColor=Ink;
            orderText=Text(transform,"",new Vector2(22,-67),new Vector2(240,60),17,Gold);
            var message = Box(transform,"Toast",new Vector2(0,-20),new Vector2(560,42),new Vector2(.5f,1),new Color(Ink.r,Ink.g,Ink.b,.92f));
            toast = Text(message,"",new Vector2(12,-8),new Vector2(536,30),18,Cream);toast.alignment = TextAlignmentOptions.Center;message.gameObject.SetActive(false);
            overlay = Box(transform,"Overlay",Vector2.zero,Vector2.zero,new Vector2(.5f,.5f),new Color(.04f,.10f,.12f,.76f));
            overlay.anchorMin = Vector2.zero; overlay.anchorMax = Vector2.one; overlay.offsetMin = Vector2.zero; overlay.offsetMax = Vector2.zero;
            page = Box(overlay,"Page",Vector2.zero,new Vector2(800,584),new Vector2(.5f,.5f),Panel);
            Frame(page);
            Box(page,"TopAccent",Vector2.zero,new Vector2(800,3),new Vector2(0,1),Gold);
            title = Text(page,"",new Vector2(28,-23),new Vector2(680,40),30,Cream);
            subtitle = Text(page,"",new Vector2(28,-70),new Vector2(720,65),17,Dim);
            rows = Rect(page,"Content",new Vector2(28,-143),new Vector2(744,405),new Vector2(0,1));
            closeButton = Button(page,"×",new Vector2(-20,-20),new Vector2(40,40),()=>{},new Vector2(1,1));
            overlay.gameObject.SetActive(false);
            activityPanel=Box(transform,"Fishing",new Vector2(0,165),new Vector2(370,108),new Vector2(.5f,0),Ink);Frame(activityPanel);
            activityText=Text(activityPanel,"",new Vector2(14,-9),new Vector2(344,48),18,Cream);activityText.alignment=TextAlignmentOptions.Center;
            meter=Box(activityPanel,"Line tension",new Vector2(18,-65),new Vector2(334,15),new Vector2(0,1),Panel);
            targetZone=Box(meter,"Good tension",new Vector2(334*.25f,0),new Vector2(334*.47f,15),new Vector2(0,1),C("65985f"));
            cursor=Box(meter,"Float",Vector2.zero,new Vector2(4,19),new Vector2(0,1),Gold);
            var rail=Box(activityPanel,"Catch",new Vector2(18,-88),new Vector2(334,4),new Vector2(0,1),Panel);
            progressFill=Box(rail,"Fill",Vector2.zero,new Vector2(0,4),new Vector2(0,1),Gold);
            activityPanel.gameObject.SetActive(false);
        }
        public void Show(string heading,string description)
        {
            ClearRows(); title.text=heading;subtitle.text=description;overlay.gameObject.SetActive(true);MiniOpen=false;
            if (EventSystem.current) EventSystem.current.SetSelectedGameObject(closeButton.gameObject);
        }
        public void Hide()
        {
            overlay.gameObject.SetActive(false);activityPanel.gameObject.SetActive(false);MiniOpen=false;Closed?.Invoke();
        }
        void ClearRows()
        {
            for(int i=rows.childCount-1;i>=0;i--) { var child=rows.GetChild(i).gameObject;child.SetActive(false);Destroy(child); }
        }
        public void Row(int index,string heading,string detail,string icon,string button,Action action,bool enabled=true)
        {
            var row=Box(rows,"Row",new Vector2(0,-index*65),new Vector2(744,59),new Vector2(0,1),Ink);
            float x=18;
            if (!string.IsNullOrEmpty(icon))
            {
                var img=Box(row,"Icon",new Vector2(14,-8),new Vector2(43,43),new Vector2(0,1),Color.white).GetComponent<UnityEngine.UI.Image>();img.sprite=WorldView.Art(icon);img.preserveAspect=true;x=70;
            }
            Text(row,heading,new Vector2(x,-7),new Vector2(540-x,24),20,Cream);
            Text(row,detail,new Vector2(x,-33),new Vector2(540-x,20),14,Dim);
            if (action!=null)
            {
                var b=Button(row,button,new Vector2(-10,-10),new Vector2(180,39),action,new Vector2(1,1));b.interactable=enabled;
            }
        }
        public void Paragraph(string text) { Text(rows,text,new Vector2(8,-4),new Vector2(718,380),22,Cream); }
        public void FooterButton(string caption,Action action)
        { Button(rows,caption,new Vector2(0,-350),new Vector2(744,48),action,new Vector2(0,1)); }
        public void StartMini(string heading,string description,string instruction,bool tension)
        {
            Show(heading,description);MiniOpen=true;
            miniLabel=Text(rows,"",new Vector2(0,-15),new Vector2(744,44),30,Gold);miniLabel.alignment=TextAlignmentOptions.Center;
            meter=Box(rows,"Meter",new Vector2(22,-95),new Vector2(700,44),new Vector2(0,1),Ink);
            targetZone=Box(meter,"SweetSpot",new Vector2(tension?700*.25f:700*.43f,0),new Vector2(tension?700*.47f:700*.18f,44),new Vector2(0,1),C("4e876b"));
            cursor=Box(meter,"Needle",new Vector2(0,4),new Vector2(5,52),new Vector2(0,1),Gold);
            var progress=Box(rows,"Progress",new Vector2(22,-165),new Vector2(700,6),new Vector2(0,1),Ink);
            progressFill=Box(progress,"Fill",Vector2.zero,new Vector2(0,6),new Vector2(0,1),Gold);
            miniHelp=Text(rows,instruction,new Vector2(22,-195),new Vector2(700,76),22,Cream);miniHelp.alignment=TextAlignmentOptions.Center;
            miniProgress=Text(rows,"",new Vector2(22,-288),new Vector2(700,40),18,Dim);miniProgress.alignment=TextAlignmentOptions.Center;
            Button(rows,tension?"Hold to reel / release for slack":"Finish cooking",new Vector2(170,-345),new Vector2(404,48),()=> MiniPressed=true,new Vector2(0,1));
        }
        public bool MiniPressed;
        public bool MouseHolding => Mouse.current != null && Mouse.current.leftButton.isPressed && RectTransformUtility.RectangleContainsScreenPoint(page,Mouse.current.position.ReadValue());
        public void Mini(float position,float progress,string label,string info)
        {
            cursor.anchoredPosition=new Vector2(Mathf.Clamp01(position)*330,2); progressFill.sizeDelta=new Vector2(Mathf.Clamp01(progress)*334,4);activityText.text=label+"\n"+info;
        }
        public void Fishing()
        { overlay.gameObject.SetActive(false);activityPanel.gameObject.SetActive(true);MiniOpen=true; }
        public void Belt(GameModel model)
        {
            string[] names={"Hands","Fishing rod","Watering can","Pepperbell seeds","Lanternroot seeds","Field knife","Journal · Tab","Satchel · B"};
            for(int i=0;i<8;i++)toolFrames[i].color=i==model.State.equipped?C("ab7847"):C("355349");
            toolCounts[3].text=model.State.pepperSeeds.ToString();toolCounts[4].text=model.State.rootSeeds.ToString();toolCounts[7].text=$"{model.BagCount}/{model.Capacity}";
            equippedLabel.text=names[model.State.equipped]+"   ·   1–8 / scroll";
        }
        public void CookStage(int step,string dish,string icon,string[] ingredients,string cookingArt)
        {
            Show(dish,step==0?"1  PREPARE     ·     2  COOK     ·     3  PLATE":step==1?"PREPARED     ·     2  COOK     ·     3  PLATE":"PREPARED     ·     COOKED     ·     3  PLATE");
            cookingBoard=Box(rows,"Cooking bench",new Vector2(12,-10),new Vector2(720,240),new Vector2(0,1),C("9b704b"));Frame(cookingBoard);
            cookingBoard.GetComponent<UnityEngine.UI.Image>().sprite=WorldView.Art("ui-board");
            var food=Box(cookingBoard,"Ingredient",new Vector2(278,-42),new Vector2(164,136),new Vector2(0,1),Color.white).GetComponent<UnityEngine.UI.Image>();food.sprite=WorldView.Art(step==0?"prep-board":step==1?cookingArt:icon);food.preserveAspect=true;food.raycastTarget=false;
            miniLabel=Text(rows,"",new Vector2(10,-264),new Vector2(720,36),22,Gold);miniLabel.alignment=TextAlignmentOptions.Center;
            miniProgress=Text(rows,"",new Vector2(10,-304),new Vector2(720,32),18,Cream);miniProgress.alignment=TextAlignmentOptions.Center;
            if(step==0)
            {
                Button(cookingBoard,"Left cut · A",new Vector2(18,-92),new Vector2(175,54),()=>CookingAction=0,new Vector2(0,1));
                Button(cookingBoard,"Right cut · D",new Vector2(-18,-92),new Vector2(175,54),()=>CookingAction=1,new Vector2(1,1));
                miniLabel.text="Alternate left and right cuts";miniProgress.text="Six cuts on the chopping board";
            }
            else if(step==1)
            {
                Button(cookingBoard,"Lower heat · A",new Vector2(18,-92),new Vector2(175,54),()=>CookingAction=0,new Vector2(0,1));
                Button(cookingBoard,"Raise heat · D",new Vector2(-18,-92),new Vector2(175,54),()=>CookingAction=1,new Vector2(1,1));
                Button(rows,"Stir / turn · Space",new Vector2(178,-352),new Vector2(390,44),()=>CookingAction=2,new Vector2(0,1));
            }
            else
            {
                // Three ingredients are dragged onto the plate. Keyboard alternatives are the same placements.
                for(int i=0;i<3;i++)
                {
                    int garnish=i;var g=Box(cookingBoard,"Garnish "+i,new Vector2(40+i*218,-188),new Vector2(64,44),new Vector2(0,1),Color.white);
                    var img=g.GetComponent<UnityEngine.UI.Image>();img.sprite=WorldView.Art(i<ingredients.Length?ingredients[i]:i==2?"garnish-salt":"garnish-herb");img.preserveAspect=true;
                    var drag=g.gameObject.AddComponent<PlateGarnish>();drag.ui=this;drag.target=food.rectTransform;drag.index=garnish;
                    var key=Button(rows,(i+1)+" · Place",new Vector2(12+i*244,-352),new Vector2(230,44),()=>CookingAction=garnish,new Vector2(0,1));
                }
                miniLabel.text="Drag the garnishes onto your plate";miniProgress.text="Or place them with 1, 2, 3";
            }
        }
        public static void Frame(RectTransform r)
        {var image=r.GetComponent<UnityEngine.UI.Image>();image.sprite=WorldView.Art("ui-frame");image.color=Color.white;image.type=UnityEngine.UI.Image.Type.Sliced;image.pixelsPerUnitMultiplier=3.125f;}
        public void Message(string text) { toast.text=text;toast.transform.parent.gameObject.SetActive(!string.IsNullOrEmpty(text));if(!string.IsNullOrEmpty(text))toast.transform.parent.SetAsLastSibling(); }
        public static RectTransform Rect(Transform parent,string name,Vector2 pos,Vector2 size,Vector2 anchor)
        {
            var go=new GameObject(name,typeof(RectTransform));go.transform.SetParent(parent,false);var r=go.GetComponent<RectTransform>();r.anchorMin=r.anchorMax=anchor;
            r.pivot=new Vector2(anchor.x,anchor.y==0?0:anchor.y==.5f?.5f:1);r.anchoredPosition=pos;r.sizeDelta=size;return r;
        }
        static RectTransform Box(Transform parent,string name,Vector2 pos,Vector2 size,Vector2 anchor,Color color)
        {
            var r=Rect(parent,name,pos,size,anchor);var img=r.gameObject.AddComponent<UnityEngine.UI.Image>();img.color=color;return r;
        }
        static TMP_Text Text(Transform parent,string text,Vector2 pos,Vector2 size,int fontSize,Color color,Vector2? anchor=null)
        {
            var r=Rect(parent,"Text",pos,size,anchor??new Vector2(0,1));var t=r.gameObject.AddComponent<TextMeshProUGUI>();t.text=text;t.fontSize=fontSize;t.color=color;
            t.font=Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");t.textWrappingMode=TextWrappingModes.Normal;t.overflowMode=TextOverflowModes.Ellipsis;t.raycastTarget=false;return t;
        }
        static UnityEngine.UI.Button Button(Transform parent,string caption,Vector2 pos,Vector2 size,Action action,Vector2 anchor)
        {
            var r=Box(parent,caption,pos,size,anchor,C("35585a"));var b=r.gameObject.AddComponent<UnityEngine.UI.Button>();var c=b.colors;c.normalColor=Color.white;c.highlightedColor=Gold;c.selectedColor=Gold;c.pressedColor=C("adbc90");c.disabledColor=new Color(.45f,.5f,.48f,1);b.colors=c;
            var t=Text(r,caption,new Vector2(0,0),size,18,Cream);t.rectTransform.anchorMin=Vector2.zero;t.rectTransform.anchorMax=Vector2.one;t.rectTransform.offsetMin=Vector2.zero;t.rectTransform.offsetMax=Vector2.zero;t.alignment=TextAlignmentOptions.Center;
            b.onClick.AddListener(()=>action());return b;
        }
    }
}
