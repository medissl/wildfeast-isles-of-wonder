using System;
using System.Linq;
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
        public UnityEngine.UI.Image[] toolIcons = new UnityEngine.UI.Image[10];
        public TMP_Text[] toolCounts = new TMP_Text[10];
        public UnityEngine.UI.Image[] toolFrames = new UnityEngine.UI.Image[10];
        public TMP_Text equippedLabel, activityText, orderText;
        public int CookingAction = -1;
        public Action<int> SelectTool;
        public Action Closed;
        public Action[] MenuActions;
        int inventorySelected=-1;
        RectTransform menuTabs;
        public void Tabs(string active,string[] labels,Action[] actions)
        {
            if(menuTabs){menuTabs.gameObject.SetActive(false);Destroy(menuTabs.gameObject);}
            var bar=Rect(page,"Menu tabs",new Vector2(0,42),new Vector2(800,38),new Vector2(0,1));
            menuTabs=bar;
            for(int i=0;i<labels.Length;i++){int index=i;var b=Button(bar,labels[i],new Vector2(i*133,0),new Vector2(128,38),()=>actions[index](),new Vector2(0,1));b.GetComponent<UnityEngine.UI.Image>().color=labels[i]==active?C("ffdb9d"):Color.white;}
        }
        public void Inventory(GameModel model,Action redraw)
        {
            ItemInventory.Sync(model.State);
            Show("Your satchel","");
            for(int i=0;i<ItemInventory.Size;i++)
            {
                int index=i;var item=model.State.slots[i];
                var b=Button(rows,"",new Vector2(12+(i%10)*72,-12-(i/10)*78),new Vector2(66,68),()=>{if(inventorySelected<0){inventorySelected=index;redraw();}else{ItemInventory.Swap(model.State,inventorySelected,index);inventorySelected=-1;redraw();}},new Vector2(0,1));
                b.gameObject.name="Inventory slot "+i;
                Skin(b.GetComponent<UnityEngine.UI.Image>(),true);
                b.GetComponent<UnityEngine.UI.Image>().color=inventorySelected==i?C("ffd583"):i<10?C("fff0c6"):Color.white;
                var drag=b.gameObject.AddComponent<InventoryDrag>();drag.index=index;drag.ui=this;drag.move=(from,to)=>{ItemInventory.Swap(model.State,from,to);inventorySelected=-1;redraw();};
                if(item.count>0){var img=Box(b.transform,"Item",new Vector2(11,-5),new Vector2(44,44),new Vector2(0,1),Color.white).GetComponent<UnityEngine.UI.Image>();img.sprite=WorldView.ItemArt(item.id);img.preserveAspect=true;img.raycastTarget=false;Text(b.transform,item.id=="tool-can"?model.State.water+"/20":item.count>1?item.count.ToString():"",new Vector2(6,-48),new Vector2(55,17),13,Ink);}
                Text(b.transform,i<10?((i+1)%10).ToString():"",new Vector2(4,-3),new Vector2(14,18),12,Ink);
            }
            string selected=inventorySelected<0?"Hotbar above · Backpack below":ItemInventory.Name(model.State.slots[inventorySelected].id,model.Data)+" · Choose a destination slot";
            Text(rows,selected,new Vector2(12,-326),new Vector2(720,28),18,Ink);
            Text(rows,$"Food: {model.BagCount}/{model.Capacity} portions",new Vector2(12,-365),new Vector2(720,36),16,Ink);
        }
        public void Map(WorldView world)
        {
            var room=world.GetComponentsInChildren<ResidentRoom>(true).FirstOrDefault(r=>r.area==world.Area);var island=Archipelago.Get(room?room.district:Districts.Interior(world.Area)?0:world.Area);
            Show("Isles of Wonder",island.name);
            float scale=Mathf.Min(520/island.size.x,338/island.size.y),mw=island.size.x*scale,mh=island.size.y*scale;
            var map=Box(rows,"Island chart",new Vector2((520-mw)/2,-12),new Vector2(mw,mh),new Vector2(0,1),Color.white);
            map.GetComponent<UnityEngine.UI.Image>().sprite=WorldView.Art("map-"+island.key);
            Vector2 position=room?room.exterior:Districts.Interior(world.Area)?new Vector2(-6,-1):world.player.position;
            var pin=Box(map,"You are here",new Vector2((position.x+island.size.x/2)*scale,-(island.size.y/2-position.y)*scale),new Vector2(16,16),new Vector2(0,1),Color.white);
            pin.GetComponent<UnityEngine.UI.Image>().sprite=WorldView.Art("map-player");
            Text(rows,"YOU · Gold diamond",new Vector2(12,-361),new Vector2(320,28),18,Ink);
            var locations=world.points.Where(p=>p.transform.IsChildOf(world.IslandRoot(island.id))&&new[]{"enter","boat","upgrades","story","hunt","forage","fruit","creature","bud","tap","fish","discovery","passage","home","shop","talk"}.Contains(p.action)).ToArray();
            for(int i=0;i<locations.Length;i++)
            {
                var p=locations[i];var marker=Box(map,"Landmark",new Vector2((p.transform.position.x+island.size.x/2)*scale,-(island.size.y/2-p.transform.position.y)*scale),new Vector2(12,12),new Vector2(0,1),Color.white);
                marker.GetComponent<UnityEngine.UI.Image>().sprite=WorldView.Art("map-landmark");
                if(i<9)Text(rows,p.label,new Vector2(538,-16-i*34),new Vector2(202,31),15,Ink);
            }
        }
        static readonly Color Ink = C("493326"), Panel = C("b68b5b"), Gold = C("7b302e"), Cream = C("493326"), Dim = C("705440");
        public static Color C(string hex) { ColorUtility.TryParseHtmlString("#" + hex, out var c); return c; }
        RectTransform energyFill;TMP_Text energyText;
        public void Energy(int value)
        {
            if(!energyFill)
            {
                var panel=Box(transform,"Energy",new Vector2(-18,-88),new Vector2(248,47),new Vector2(1,1),Color.white);Frame(panel);
                Text(panel,"Energy",new Vector2(12,-6),new Vector2(84,22),16,C("244e48"));
                var rail=Box(panel,"Energy track",new Vector2(96,-12),new Vector2(136,12),new Vector2(0,1),C("244e48"));
                energyFill=Box(rail,"Energy fill",Vector2.zero,new Vector2(136,12),new Vector2(0,1),C("8cbd68"));
                energyText=Text(panel,"",new Vector2(12,-27),new Vector2(215,16),12,C("49635a"));
            }
            energyFill.sizeDelta=new Vector2(136*Mathf.Clamp01(value/100f),12);energyFill.GetComponent<UnityEngine.UI.Image>().color=value<20?C("df9965"):C("85ba65");energyText.text=value+" / 100";
        }
        public void Init(Action journal, Action bag, Action pause)
        {
            journalButton.onClick.AddListener(() => journal()); bagButton.onClick.AddListener(() => bag()); pauseButton.onClick.AddListener(() => pause());
            closeButton.onClick.AddListener(Hide);
            for(int i=0;i<toolFrames.Length;i++){int slot=i;var b=toolFrames[i].GetComponent<UnityEngine.UI.Button>();b.onClick.RemoveAllListeners();b.onClick.AddListener(()=>SelectTool?.Invoke(slot));}
            Hide();
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
            toolBelt = Box(transform,"Tool belt",new Vector2(0,18),new Vector2(698,66),new Vector2(.5f,0),Ink);Frame(toolBelt);
            string[] icons=ItemInventory.Tools;
            for(int i=0;i<10;i++)
            {
                int slot=i;var b=Button(toolBelt,"",new Vector2(7+i*69,-7),new Vector2(62,52),()=>SelectTool?.Invoke(slot),new Vector2(0,1));
                toolFrames[i]=b.GetComponent<UnityEngine.UI.Image>();
                Skin(toolFrames[i],true);
                var icon=Box(b.transform,"Tool",new Vector2(17,-5),new Vector2(30,34),new Vector2(0,1),Color.white).GetComponent<UnityEngine.UI.Image>();icon.sprite=string.IsNullOrEmpty(icons[i])?null:WorldView.Art(icons[i]);icon.preserveAspect=true;icon.raycastTarget=false;toolIcons[i]=icon;
                Text(b.transform,((i+1)%10).ToString(),new Vector2(5,-3),new Vector2(14,18),12,Dim);
                toolCounts[i]=Text(b.transform,"",new Vector2(-4,2),new Vector2(48,18),12,Cream,new Vector2(1,0));toolCounts[i].alignment=TextAlignmentOptions.Right;
            }
            equippedLabel=Text(transform,"",new Vector2(0,86),new Vector2(560,24),16,Gold,new Vector2(.5f,0));equippedLabel.alignment=TextAlignmentOptions.Center;
            prompt = Text(transform,"",new Vector2(0,122),new Vector2(690,38),22,Cream,new Vector2(.5f,0)); prompt.alignment = TextAlignmentOptions.Center;
            prompt.outlineWidth=.2f;prompt.outlineColor=Ink;
            prompt.color=C("fff0c9");prompt.outlineColor=C("40372e");equippedLabel.color=C("fff0c9");equippedLabel.outlineWidth=.2f;equippedLabel.outlineColor=C("40372e");
            orderText=Text(transform,"",new Vector2(22,-67),new Vector2(240,60),17,Gold);
            orderText.color=C("fff0c9");orderText.outlineWidth=.2f;orderText.outlineColor=C("40372e");
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
            if(dialogue){dialogue.gameObject.SetActive(false);Destroy(dialogue.gameObject);dialogue=null;}page.gameObject.SetActive(true);
            ClearRows(); title.text=heading;subtitle.text=description;overlay.gameObject.SetActive(true);MiniOpen=false;
            toast.transform.parent.gameObject.SetActive(false);
            string active=heading=="Tonight's menu"?"Recipes":heading=="The forager's journal"?"Journal":heading=="Harbor requests"?"Requests":heading=="Take a breath"?"Options":heading;
            if(MenuActions!=null)Tabs(active,new[]{"Inventory","Map","Recipes","Journal","Requests","Options"},MenuActions);
            if (EventSystem.current) EventSystem.current.SetSelectedGameObject(closeButton.gameObject);
        }
        public void Hide()
        {
            overlay.gameObject.SetActive(false);activityPanel.gameObject.SetActive(false);MiniOpen=false;Closed?.Invoke();
        }
        RectTransform dialogue;
        public void Dialogue(string name,string role,string line,string portrait,string next,Action advance,string choice,Action choose)
        {
            Show(name,role);page.gameObject.SetActive(false);
            dialogue=Box(overlay,"Resident conversation",new Vector2(0,105),new Vector2(1030,260),new Vector2(.5f,0),Color.white);Frame(dialogue);
            var frame=Box(dialogue,"Portrait frame",new Vector2(22,-22),new Vector2(172,204),new Vector2(0,1),Color.white);Frame(frame);
            var image=Box(frame,"Expression",new Vector2(10,-12),new Vector2(152,152),new Vector2(0,1),Color.white).GetComponent<UnityEngine.UI.Image>();image.sprite=WorldView.Art(portrait);image.preserveAspect=true;image.raycastTarget=false;
            var who=Text(frame,name,new Vector2(8,-167),new Vector2(156,34),20,Ink);who.alignment=TextAlignmentOptions.Center;
            Text(dialogue,role,new Vector2(217,-25),new Vector2(765,28),20,Gold);
            Text(dialogue,line,new Vector2(217,-67),new Vector2(765,108),25,Ink);
            Button(dialogue,next,new Vector2(-28,-197),new Vector2(165,40),advance,new Vector2(1,1));
            if(choice!=null)Button(dialogue,choice,new Vector2(217,-197),new Vector2(540,40),choose,new Vector2(0,1));
        }
        void ClearRows()
        {
            for(int i=rows.childCount-1;i>=0;i--) { var child=rows.GetChild(i).gameObject;child.SetActive(false);Destroy(child); }
        }
        public void Row(int index,string heading,string detail,string icon,string button,Action action,bool enabled=true)
        {
            var row=Box(rows,"Row",new Vector2(0,-index*65),new Vector2(744,59),new Vector2(0,1),C("ead2a1"));
            float x=18;
            if (!string.IsNullOrEmpty(icon))
            {
                var img=Box(row,"Icon",new Vector2(14,-8),new Vector2(43,43),new Vector2(0,1),Color.white).GetComponent<UnityEngine.UI.Image>();img.sprite=WorldView.ItemArt(icon);img.preserveAspect=true;x=70;
            }
            Text(row,heading,new Vector2(x,-7),new Vector2(540-x,24),20,Cream);
            Text(row,detail,new Vector2(x,-33),new Vector2(540-x,20),14,Dim);
            if (action!=null)
            {
                var b=Button(row,button,new Vector2(-10,-10),new Vector2(180,39),action,new Vector2(1,1));b.interactable=enabled;
            }
        }
        public void Paragraph(string text) { Text(rows,text,new Vector2(8,-4),new Vector2(718,380),22,Ink); }
        public void RecipeCard(int index,Recipe recipe,GameModel model,bool known,bool selected,Action choose)
        {
            var row=Box(rows,"Recipe "+recipe.id,new Vector2(0,-index*65),new Vector2(744,59),new Vector2(0,1),C("ead2a1"));
            var image=Box(row,"Dish",new Vector2(12,-7),new Vector2(46,46),new Vector2(0,1),Color.white).GetComponent<UnityEngine.UI.Image>();image.sprite=WorldView.ItemArt(known?recipe.icon:"spark");image.preserveAspect=true;image.raycastTarget=false;
            Text(row,known?recipe.name+$" · {recipe.price} shells":"Undiscovered recipe",new Vector2(72,-5),new Vector2(470,25),20,Cream);
            if(known)for(int i=0;i<recipe.ingredients.Length;i++)
            {
                var ingredient=recipe.ingredients[i];var icon=Box(row,"Ingredient",new Vector2(72+i*145,-32),new Vector2(22,22),new Vector2(0,1),Color.white).GetComponent<UnityEngine.UI.Image>();icon.sprite=WorldView.ItemArt(model.Data.Item(ingredient.id).icon);icon.preserveAspect=true;icon.raycastTarget=false;
                Text(row,$"{model.Count(ingredient.id)}/{ingredient.count} in pantry",new Vector2(98+i*145,-34),new Vector2(118,22),13,model.Count(ingredient.id)>=ingredient.count?Cream:C("ae4337"));
            }
            else Text(row,"Discover its main ingredient",new Vector2(72,-33),new Vector2(430,22),14,Dim);
            var button=Button(row,selected?"On the menu":"Add to menu",new Vector2(-10,-10),new Vector2(180,39),choose,new Vector2(1,1));button.interactable=known&&model.State.phase=="explore";
        }
        public void Pagination(int current,int count,int size,Action<int> change)
        {
            int pages=Mathf.CeilToInt(count/(float)size);
            var previous=Button(rows,"< Previous",new Vector2(0,-350),new Vector2(210,44),()=>change(current-1),new Vector2(0,1));previous.interactable=current>0;
            var next=Button(rows,"Next >",new Vector2(534,-350),new Vector2(210,44),()=>change(current+1),new Vector2(0,1));next.interactable=current<pages-1;
            var text=Text(rows,$"{current+1} / {pages}",new Vector2(220,-357),new Vector2(304,30),18,Cream);text.alignment=TextAlignmentOptions.Center;
        }
        public void SliderRow(int index,string caption,float value,Action<float> changed)
        {
            var row=Box(rows,caption,new Vector2(0,-index*65),new Vector2(744,59),new Vector2(0,1),C("ead2a1"));
            Text(row,caption,new Vector2(18,-15),new Vector2(245,32),21,Cream);
            var label=Text(row,Mathf.RoundToInt(value*100)+"%",new Vector2(655,-17),new Vector2(74,28),18,Gold);
            var rail=Box(row,"Volume slider",new Vector2(295,-25),new Vector2(340,12),new Vector2(0,1),C("8e6949"));Skin(rail.GetComponent<UnityEngine.UI.Image>(),true);
            var handle=Box(rail,"Handle",new Vector2(0,0),new Vector2(22,30),new Vector2(0,.5f),C("f4d898"));Skin(handle.GetComponent<UnityEngine.UI.Image>(),true);
            var slider=rail.gameObject.AddComponent<UnityEngine.UI.Slider>();slider.minValue=0;slider.maxValue=1;slider.handleRect=handle;slider.targetGraphic=handle.GetComponent<UnityEngine.UI.Image>();slider.SetValueWithoutNotify(value);
            slider.onValueChanged.AddListener(v=>{label.text=Mathf.RoundToInt(v*100)+"%";changed(v);});
        }
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
        public void FishTrack(FishingChallenge challenge,string status,string item)
        {
            float width=challenge.Width*334;targetZone.sizeDelta=new Vector2(width,15);targetZone.anchoredPosition=new Vector2(challenge.Zone*334-width/2,0);
            cursor.sizeDelta=new Vector2(18,18);cursor.anchoredPosition=new Vector2(challenge.Fish*334-9,2);var icon=cursor.GetComponent<UnityEngine.UI.Image>();icon.sprite=WorldView.Art("held-"+item)??WorldView.Art(item);icon.color=Color.white;icon.preserveAspect=true;
            progressFill.sizeDelta=new Vector2(challenge.Progress*334,4);progressFill.GetComponent<UnityEngine.UI.Image>().color=C("65985f");activityText.text=status+"  ·  "+Mathf.RoundToInt(challenge.Progress*100)+"%";
            targetZone.GetComponent<UnityEngine.UI.Image>().color=challenge.Tracking?C("87b565"):C("cbaa70");
        }
        public void Fishing()
        { overlay.gameObject.SetActive(false);activityPanel.gameObject.SetActive(true);MiniOpen=true; }
        public void Belt(GameModel model)
        {
            ItemInventory.Sync(model.State);
            for(int i=0;i<10;i++)
            {
                var slot=model.State.slots[i];toolFrames[i].color=i==model.State.equipped?C("ffd583"):Color.white;
                toolIcons[i].sprite=string.IsNullOrEmpty(slot.id)?null:WorldView.ItemArt(slot.id);toolIcons[i].enabled=slot.count>0;
                toolCounts[i].text=slot.id=="tool-can"?model.State.water+"/20":slot.count>1?slot.count.ToString():"";
            }
            equippedLabel.text=ItemInventory.Name(model.State.slots[model.State.equipped].id,model.Data);

        }
        public void CookStage(int step,string dish,string icon,string[] ingredients,string cookingArt)
        {
            Show(dish,step==0?"1  PREPARE     ·     2  COOK     ·     3  PLATE":step==1?"PREPARED     ·     2  COOK     ·     3  PLATE":"PREPARED     ·     COOKED     ·     3  PLATE");
            if(menuTabs)menuTabs.gameObject.SetActive(false);
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
        {var image=r.GetComponent<UnityEngine.UI.Image>();image.sprite=WorldView.Art("ui-frame");image.color=Color.white;image.type=UnityEngine.UI.Image.Type.Sliced;image.pixelsPerUnitMultiplier=2f;}
        public static void Skin(UnityEngine.UI.Image image,bool slot=false)
        {image.sprite=WorldView.Art(slot?"ui-slot":"ui-button");image.type=UnityEngine.UI.Image.Type.Sliced;image.color=Color.white;image.pixelsPerUnitMultiplier=2;}
        public void Message(string text) { toast.text=text;toast.transform.parent.gameObject.SetActive(!string.IsNullOrEmpty(text));if(!string.IsNullOrEmpty(text))toast.transform.parent.SetAsLastSibling(); }
        public static RectTransform Rect(Transform parent,string name,Vector2 pos,Vector2 size,Vector2 anchor)
        {
            var go=new GameObject(name,typeof(RectTransform));go.transform.SetParent(parent,false);var r=go.GetComponent<RectTransform>();r.anchorMin=r.anchorMax=anchor;
            r.pivot=new Vector2(anchor.x,anchor.y==0?0:anchor.y==.5f?.5f:1);r.anchoredPosition=pos;r.sizeDelta=size;return r;
        }
        public static RectTransform Box(Transform parent,string name,Vector2 pos,Vector2 size,Vector2 anchor,Color color)
        {
            var r=Rect(parent,name,pos,size,anchor);var img=r.gameObject.AddComponent<UnityEngine.UI.Image>();img.color=color;return r;
        }
        public static TMP_Text Text(Transform parent,string text,Vector2 pos,Vector2 size,int fontSize,Color color,Vector2? anchor=null)
        {
            var r=Rect(parent,"Text",pos,size,anchor??new Vector2(0,1));var t=r.gameObject.AddComponent<TextMeshProUGUI>();t.text=text;t.fontSize=fontSize;t.color=color;
            t.font=Resources.Load<TMP_FontAsset>("Fonts/Pixelify");t.enableAutoSizing=false;t.textWrappingMode=TextWrappingModes.Normal;t.overflowMode=TextOverflowModes.Ellipsis;t.raycastTarget=false;return t;
        }
        public static UnityEngine.UI.Button Button(Transform parent,string caption,Vector2 pos,Vector2 size,Action action,Vector2 anchor)
        {
            var r=Box(parent,caption,pos,size,anchor,Color.white);Skin(r.GetComponent<UnityEngine.UI.Image>());var b=r.gameObject.AddComponent<UnityEngine.UI.Button>();var c=b.colors;c.normalColor=Color.white;c.highlightedColor=C("ffe0b0");c.selectedColor=Color.white;c.pressedColor=C("cfb383");c.disabledColor=new Color(.55f,.55f,.55f,1);b.colors=c;
            var t=Text(r,caption,new Vector2(0,0),size,18,C("f6e7bf"));t.rectTransform.anchorMin=Vector2.zero;t.rectTransform.anchorMax=Vector2.one;t.rectTransform.offsetMin=Vector2.zero;t.rectTransform.offsetMax=Vector2.zero;t.alignment=TextAlignmentOptions.Center;
            b.onClick.AddListener(()=>action());return b;
        }
    }
}
