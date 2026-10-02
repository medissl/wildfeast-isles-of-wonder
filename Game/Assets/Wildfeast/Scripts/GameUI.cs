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
            var brand = Box(transform, "Brand", new Vector2(24,-20), new Vector2(310,65), new Vector2(0,1), Ink);
            Text(brand,"W I L D F E A S T",new Vector2(17,-6),new Vector2(275,34),24,Gold);
            Text(brand,"ISLES OF WONDER",new Vector2(18,-39),new Vector2(270,16),12,Dim);
            var status = Box(transform,"Status",new Vector2(-24,-20),new Vector2(275,65),new Vector2(1,1),Ink);
            wallet = Text(status,"Day 1 · 0 shells",new Vector2(16,-10),new Vector2(240,27),22,Gold);
            location = Text(status,"SALTLEAF SHORE",new Vector2(16,-39),new Vector2(240,18),13,Dim);
            var quest = Box(transform,"Objective",new Vector2(24,-97),new Vector2(310,99),new Vector2(0,1),new Color(Panel.r,Panel.g,Panel.b,.93f));
            Text(quest,"YOUR NEXT DISCOVERY",new Vector2(16,-11),new Vector2(275,20),12,Gold);
            objective = Text(quest,"",new Vector2(16,-36),new Vector2(278,54),18,Cream);
            var controls = Box(transform,"Controls",new Vector2(24,20),new Vector2(420,44),new Vector2(0,0),Ink);
            Text(controls,"WASD  move     E  interact     Tab  journal",new Vector2(13,-11),new Vector2(395,25),16,Dim);
            journalButton = Button(transform,"Journal",new Vector2(-290,20),new Vector2(120,44),()=>{},new Vector2(1,0));
            bagButton = Button(transform,"Satchel",new Vector2(-154,20),new Vector2(120,44),()=>{},new Vector2(1,0));
            pauseButton = Button(transform,"Pause",new Vector2(-24,20),new Vector2(112,44),()=>{},new Vector2(1,0));
            prompt = Text(transform,"",new Vector2(0,80),new Vector2(600,40),22,Cream,new Vector2(.5f,0)); prompt.alignment = TextAlignmentOptions.Center;
            var message = Box(transform,"Toast",new Vector2(0,-20),new Vector2(560,42),new Vector2(.5f,1),new Color(Ink.r,Ink.g,Ink.b,.92f));
            toast = Text(message,"",new Vector2(12,-8),new Vector2(536,30),18,Cream);toast.alignment = TextAlignmentOptions.Center;message.gameObject.SetActive(false);
            overlay = Box(transform,"Overlay",Vector2.zero,Vector2.zero,new Vector2(.5f,.5f),new Color(.04f,.10f,.12f,.76f));
            overlay.anchorMin = Vector2.zero; overlay.anchorMax = Vector2.one; overlay.offsetMin = Vector2.zero; overlay.offsetMax = Vector2.zero;
            page = Box(overlay,"Page",Vector2.zero,new Vector2(800,584),new Vector2(.5f,.5f),Panel);
            Box(page,"TopAccent",Vector2.zero,new Vector2(800,3),new Vector2(0,1),Gold);
            title = Text(page,"",new Vector2(28,-23),new Vector2(680,40),30,Cream);
            subtitle = Text(page,"",new Vector2(28,-70),new Vector2(720,65),17,Dim);
            rows = Rect(page,"Content",new Vector2(28,-143),new Vector2(744,405),new Vector2(0,1));
            closeButton = Button(page,"×",new Vector2(-20,-20),new Vector2(40,40),()=>{},new Vector2(1,1));
            overlay.gameObject.SetActive(false);
        }
        public void Show(string heading,string description)
        {
            ClearRows(); title.text=heading;subtitle.text=description;overlay.gameObject.SetActive(true);MiniOpen=false;
            if (EventSystem.current) EventSystem.current.SetSelectedGameObject(closeButton.gameObject);
        }
        public void Hide()
        {
            overlay.gameObject.SetActive(false);MiniOpen=false;Closed?.Invoke();
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
            cursor.anchoredPosition=new Vector2(Mathf.Clamp01(position)*695,4); progressFill.sizeDelta=new Vector2(Mathf.Clamp01(progress)*700,6);miniLabel.text=label;miniProgress.text=info;
        }
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
