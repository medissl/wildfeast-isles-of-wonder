using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
namespace Wildfeast
{
    public class JourneyFrontEnd : MonoBehaviour
    {
        public bool Active {get;private set;}
        public string ScreenName {get;private set;}
        public JourneySlots Slots {get;private set;}
        public CharacterProfile Draft {get;private set;}=new CharacterProfile();
        public TMP_InputField NameField {get;private set;}
        GameController game;RectTransform root,content;Image portrait;CharacterLook look;TMP_Text caption;
        readonly Dictionary<GameObject,bool> hud=new Dictionary<GameObject,bool>();
        readonly List<RectTransform> clouds=new List<RectTransform>();
        readonly List<Image> glints=new List<Image>();
        readonly List<GameObject> introPlates=new List<GameObject>();bool servedIntro;Image fade;
        int newSlot,beat=-1;float introClock;Vector2 introStart;
        static readonly Color Cream=GameUI.C("f9e7be"),Ink=GameUI.C("3b353a");
        public void Init(GameController controller,string directory)
        {game=controller;Slots=new JourneySlots(directory);Title();}
        void CaptureHud()
        {
            if(Active)return;hud.Clear();foreach(Transform child in game.ui.transform){hud[child.gameObject]=child.gameObject.activeSelf;child.gameObject.SetActive(false);}Active=true;
        }
        void Page(string screen,bool landscape=true)
        {
            CaptureHud();if(root){root.gameObject.SetActive(false);Destroy(root.gameObject);}clouds.Clear();glints.Clear();
            root=GameUI.Rect(game.ui.transform,"Journey front end",Vector2.zero,Vector2.zero,new Vector2(.5f,.5f));
            root.anchorMin=Vector2.zero;root.anchorMax=Vector2.one;root.offsetMin=root.offsetMax=Vector2.zero;ScreenName=screen;
            if(landscape)
            {
                var backdrop=GameUI.Box(root,"Wonder horizon",Vector2.zero,Vector2.zero,new Vector2(.5f,.5f),Color.white);backdrop.anchorMin=Vector2.zero;backdrop.anchorMax=Vector2.one;backdrop.offsetMin=new Vector2(-70,-40);backdrop.offsetMax=new Vector2(70,40);
                backdrop.GetComponent<Image>().sprite=WorldView.Art("title-horizon");
                for(int i=0;i<5;i++){var cloud=GameUI.Box(root,"Drifting cloud",new Vector2(-460+i*240,205-i%2*75),new Vector2(144,64),new Vector2(.5f,.5f),new Color(1,1,1,.3f));cloud.GetComponent<Image>().sprite=WorldView.Art("title-cloud");cloud.GetComponent<Image>().raycastTarget=false;clouds.Add(cloud);}
            }
            if(landscape)for(int i=0;i<12;i++)
            {
                var glint=GameUI.Box(root,"Sea glint",new Vector2(-500+i*89,-130-i%3*65),new Vector2(12,8),new Vector2(.5f,.5f),Color.white).GetComponent<Image>();glint.sprite=WorldView.Art("spark");glint.raycastTarget=false;glints.Add(glint);
            }
            content=GameUI.Rect(root,"Front end content",Vector2.zero,new Vector2(1080,650),new Vector2(.5f,.5f));
        }
        TMP_Text Label(Transform parent,string text,Vector2 pos,Vector2 size,int font=22)
        {return GameUI.Text(parent,text,pos,size,font,Cream);}
        public void Title()
        {
            game.ui.Hide();Page("Title");game.SuspendJourney();game.world.SetArea(0,new Vector2(-6,-2.5f));
            var plaque=GameUI.Box(content,"Title sign",new Vector2(0,-75),new Vector2(650,174),new Vector2(.5f,1),Color.white);GameUI.Frame(plaque);
            var title=GameUI.Text(plaque,"WILDFEAST",new Vector2(24,-20),new Vector2(602,90),64,Ink);title.alignment=TextAlignmentOptions.Center;
            var sub=GameUI.Text(plaque,"ISLES OF WONDER",new Vector2(24,-113),new Vector2(602,40),28,GameUI.C("845c42"));sub.alignment=TextAlignmentOptions.Center;
            string[] names={"New","Load","Options","Exit"};Action[] actions={NewJourney,LoadPage,Options,()=>Application.Quit()};
            for(int i=0;i<4;i++)GameUI.Button(content,names[i],new Vector2(204+i*170,-470),new Vector2(158,64),actions[i],new Vector2(0,1));
            UnityEngine.EventSystems.EventSystem.current?.SetSelectedGameObject(content.GetComponentsInChildren<UnityEngine.UI.Button>().FirstOrDefault()?.gameObject);

        }
        public void NewJourney()
        {
            newSlot=Slots.Empty();if(newSlot<0){SlotPage(true);return;}Draft=new CharacterProfile();Creation();
        }
        void SlotPage(bool full)
        {
            Page("Load");var panel=Panel(full?"All five journeys are occupied":"Your journeys",full?"Load a journey, or back up and remove a slot file to free space. New never replaces a save.":"");
            for(int i=0;i<JourneySlots.Count;i++)
            {
                int slot=i;var p=Slots.Preview(i,game.Model.Data);bool exists=Slots.Exists(i);
                string name=p==null?exists?"Unreadable save · original preserved":"Empty journey":p.avatar.name+" · Day "+p.day+" · "+Archipelago.Get(p.island).name;
                var button=GameUI.Button(panel,(i+1)+"   "+name,new Vector2(35,-150-i*64),new Vector2(810,52),()=>{if(exists)Load(slot);else{newSlot=slot;Draft=new CharacterProfile();Creation();}},new Vector2(0,1));button.interactable=!exists||p!=null;
            }
            if(File.Exists(game.LegacySavePath))GameUI.Button(panel,"Continue original save",new Vector2(35,-484),new Vector2(394,46),()=>Begin(new SaveStore(game.LegacySavePath),false),new Vector2(0,1));
            GameUI.Button(panel,"Back",new Vector2(-35,-484),new Vector2(190,46),Title,new Vector2(1,1));
        }
        public void LoadPage()=>SlotPage(false);
        public void Load(int slot){if(Slots.Preview(slot,game.Model.Data)==null)return;Begin(new SaveStore(Slots.PathFor(slot)),false);}
        RectTransform Panel(string title,string detail)
        {
            var panel=GameUI.Box(content,"Journey panel",Vector2.zero,new Vector2(880,580),new Vector2(.5f,.5f),Color.white);GameUI.Frame(panel);
            GameUI.Text(panel,title,new Vector2(35,-26),new Vector2(780,40),30,Ink);
            GameUI.Text(panel,detail,new Vector2(35,-80),new Vector2(810,56),18,Ink);return panel;
        }
        public void Creation()
        {
            Page("Character");var panel=Panel("Your explorer","");
            var preview=GameUI.Box(panel,"Character preview",new Vector2(38,-161),new Vector2(222,244),new Vector2(0,1),Color.white);GameUI.Frame(preview);
            var image=GameUI.Box(preview,"Your explorer",new Vector2(47,-30),new Vector2(128,192),new Vector2(0,1),Color.white);portrait=image.GetComponent<Image>();portrait.preserveAspect=true;portrait.raycastTarget=false;
            Label(panel,"Name",new Vector2(295,-149),new Vector2(200,30),19).color=Ink;
            var field=GameUI.Box(panel,"Explorer name",new Vector2(408,-144),new Vector2(300,40),new Vector2(0,1),GameUI.C("f2dbaf"));GameUI.Frame(field);
            var text=GameUI.Text(field,Draft.name,new Vector2(12,-5),new Vector2(276,32),22,Ink);text.raycastTarget=true;
            NameField=field.gameObject.AddComponent<TMP_InputField>();NameField.textViewport=field;NameField.textComponent=(TextMeshProUGUI)text;NameField.fontAsset=text.font;NameField.characterLimit=18;NameField.contentType=TMP_InputField.ContentType.Standard;NameField.text=Draft.name;
            NameField.onValueChanged.AddListener(value=>Draft.name=value);
            string[] names={"Skin","Hair style","Hair color","Face","Shirt","Pants","Boots"};
            for(int i=0;i<names.Length;i++)
            {
                int index=i;var label=GameUI.Text(panel,names[i],new Vector2(295,-197-i*39),new Vector2(175,30),20,Ink);
                GameUI.Button(panel,"<",new Vector2(488,-193-i*39),new Vector2(40,33),()=>Change(index,-1),new Vector2(0,1));
                var value=GameUI.Text(panel,"",new Vector2(541,-198-i*39),new Vector2(145,28),18,Ink);value.name="Choice "+i;value.alignment=TextAlignmentOptions.Center;
                GameUI.Button(panel,">",new Vector2(699,-193-i*39),new Vector2(40,33),()=>Change(index,1),new Vector2(0,1));
            }
            GameUI.Button(panel,"Random",new Vector2(38,-424),new Vector2(222,42),()=>{Draft.skin=UnityEngine.Random.Range(0,6);Draft.hairColor=UnityEngine.Random.Range(0,6);Draft.hairStyle=UnityEngine.Random.Range(0,4);Draft.face=UnityEngine.Random.Range(0,3);Draft.shirt=UnityEngine.Random.Range(0,6);Draft.pants=UnityEngine.Random.Range(0,6);Draft.boots=UnityEngine.Random.Range(0,6);UpdatePreview();},new Vector2(0,1));
            GameUI.Button(panel,"Back",new Vector2(38,-507),new Vector2(200,46),Title,new Vector2(0,1));
            GameUI.Button(panel,"Begin journey",new Vector2(-35,-507),new Vector2(250,46),Confirm,new Vector2(1,1));UpdatePreview();
        }
        public void Change(int category,int delta)
        {
            int[] values={Draft.skin,Draft.hairStyle,Draft.hairColor,Draft.face,Draft.shirt,Draft.pants,Draft.boots};int count=category==1?4:category==3?3:6;values[category]=(values[category]+delta+count)%count;
            Draft.skin=values[0];Draft.hairStyle=values[1];Draft.hairColor=values[2];Draft.face=values[3];Draft.shirt=values[4];Draft.pants=values[5];Draft.boots=values[6];UpdatePreview();
        }
        void UpdatePreview()
        {
            look?.Dispose();look=new CharacterLook(Draft);portrait.sprite=look.Apply(WorldView.Art("chef-down-0"));
            int[] values={Draft.skin,Draft.hairStyle,Draft.hairColor,Draft.face,Draft.shirt,Draft.pants,Draft.boots};
            foreach(var t in content.GetComponentsInChildren<TMP_Text>())if(t.name.StartsWith("Choice ")){int n=int.Parse(t.name.Substring(7));t.text=n==1?new[]{"Short","Long","Top knot","Chef hat"}[values[n]]:n==3?new[]{"Bright eyes","Smile","Glasses"}[values[n]]:n==0?new[]{"Honey","Light","Golden","Brown","Deep","Warm dark"}[values[n]]:n==2?new[]{"Chestnut","Wheat","Copper","Ink","Lavender","Silver"}[values[n]]:n==4?new[]{"Sea glass","Rose","Blue","Plum","Ochre","Moss"}[values[n]]:n==5?new[]{"Slate","Night","Earth","Berry","Lagoon","Sand"}[values[n]]:new[]{"Leather","Charcoal","Amber","Wine","Storm","Stone"}[values[n]];}
        }
        public void Confirm()
        {
            Draft.Normalize();var state=Progress.New();state.avatar=Draft;state.stage=1;
            state.musicVolume=PlayerPrefs.GetFloat("journey-music",.35f);state.effectsVolume=PlayerPrefs.GetFloat("journey-sfx",.55f);
            state.zoom=PlayerPrefs.GetInt("journey-zoom",1);state.reducedMotion=PlayerPrefs.GetInt("journey-motion",0)==1;
            try{Begin(Slots.Create(newSlot,state),true);}catch(Exception ex){GameUI.Text(content,ex.Message,new Vector2(100,-600),new Vector2(880,35),18,Cream);}
        }
        void Begin(SaveStore store,bool fresh)
        {
            game.ActivateJourney(store);if(fresh||!game.Model.State.introSeen&&game.Model.State.stage==1){Intro();return;}Resume();
        }
        void Resume()
        {
            if(root){root.gameObject.SetActive(false);Destroy(root.gameObject);}look?.Dispose();look=null;
            foreach(var pair in hud)if(pair.Key)pair.Key.SetActive(pair.Value);Active=false;ScreenName="Playing";game.ui.Hide();game.ResumeJourney();
        }
        public void Intro()
        {
            Page("Introduction",false);introClock=0;beat=-1;servedIntro=false;
            var curtain=GameUI.Box(root,"Gentle scene transition",Vector2.zero,Vector2.zero,new Vector2(.5f,.5f),new Color(.08f,.12f,.16f,1));curtain.anchorMin=Vector2.zero;curtain.anchorMax=Vector2.one;curtain.offsetMin=curtain.offsetMax=Vector2.zero;fade=curtain.GetComponent<Image>();fade.raycastTarget=false;game.world.FollowSea=false;
            var lower=GameUI.Box(root,"Intro caption",new Vector2(0,0),new Vector2(1280,114),new Vector2(.5f,0),new Color(.12f,.16f,.18f,.91f));
            caption=GameUI.Text(lower,"",new Vector2(130,-24),new Vector2(1020,72),25,Cream);caption.alignment=TextAlignmentOptions.Center;
            GameUI.Button(root,"Skip introduction",new Vector2(-28,-26),new Vector2(205,40),FinishIntro,new Vector2(1,1));
            game.world.player.GetComponent<Rigidbody2D>().simulated=false;
        }
        public void FinishIntro()
        {
            foreach(var plate in introPlates)if(plate)Destroy(plate);introPlates.Clear();
            foreach(var guest in game.world.guests)guest.gameObject.SetActive(false);game.world.carriedDish.sprite=null;
            game.world.player.GetComponent<Rigidbody2D>().simulated=true;game.Model.State.introSeen=true;game.Model.Notify();game.SaveIntroduction();
            game.world.SetArea(game.Model.State.island,Archipelago.Get(game.Model.State.island).arrival);Resume();
        }
        public void Options()
        {
            Page("Options");var panel=Panel("Options","");
            Slider(panel,"Music",160,PlayerPrefs.GetFloat("journey-music",.35f),v=>{PlayerPrefs.SetFloat("journey-music",v);game.MenuAudio(v,PlayerPrefs.GetFloat("journey-sfx",.55f));});
            Slider(panel,"Sound effects",228,PlayerPrefs.GetFloat("journey-sfx",.55f),v=>{PlayerPrefs.SetFloat("journey-sfx",v);game.MenuAudio(PlayerPrefs.GetFloat("journey-music",.35f),v);});
            GameUI.Button(panel,"World zoom: "+new[]{"Wide","Comfort","Close"}[PlayerPrefs.GetInt("journey-zoom",1)],new Vector2(35,-305),new Vector2(394,44),()=>{PlayerPrefs.SetInt("journey-zoom",(PlayerPrefs.GetInt("journey-zoom",1)+1)%3);Options();},new Vector2(0,1));
            GameUI.Button(panel,"Motion: "+(PlayerPrefs.GetInt("journey-motion",0)==1?"Reduced":"Full"),new Vector2(-35,-305),new Vector2(394,44),()=>{PlayerPrefs.SetInt("journey-motion",1-PlayerPrefs.GetInt("journey-motion",0));Options();},new Vector2(1,1));
            GameUI.Button(panel,Screen.fullScreen?"Display: Fullscreen":"Display: Windowed",new Vector2(35,-367),new Vector2(394,44),()=>{Screen.fullScreen=!Screen.fullScreen;Options();},new Vector2(0,1));

            GameUI.Button(panel,"Back",new Vector2(-35,-507),new Vector2(200,46),()=>{PlayerPrefs.Save();Title();},new Vector2(1,1));
        }
        void Slider(Transform parent,string name,int y,float initial,Action<float> changed)
        {
            GameUI.Text(parent,name,new Vector2(35,-y),new Vector2(205,30),23,Ink);
            var label=GameUI.Text(parent,Mathf.RoundToInt(initial*100)+"%",new Vector2(748,-y),new Vector2(95,30),21,Ink);
            var rail=GameUI.Box(parent,name+" slider",new Vector2(270,-y-7),new Vector2(444,14),new Vector2(0,1),GameUI.C("8a6e53"));
            var handle=GameUI.Box(rail,"Handle",Vector2.zero,new Vector2(24,34),new Vector2(0,.5f),Color.white);GameUI.Skin(handle.GetComponent<Image>(),true);
            var slider=rail.gameObject.AddComponent<UnityEngine.UI.Slider>();slider.handleRect=handle;slider.targetGraphic=handle.GetComponent<Image>();slider.SetValueWithoutNotify(initial);slider.onValueChanged.AddListener(v=>{label.text=Mathf.RoundToInt(v*100)+"%";changed(v);});
        }
        void Update()
        {
            if(!Active)return;
            if(ScreenName=="Introduction")
            {
                introClock+=Time.unscaledDeltaTime;int next=Mathf.Min(5,(int)(introClock/4));
                if(next!=beat)
                {
                    beat=next;int[] areas={0,1,3,4,5,2};string[] text={"Beyond the familiar seas, ingredients grow into worlds…","Cloudfruit rises where gentle Custardrams dream.","Warm stone hides spice, stories, and stubborn little cooks.","Moonlight feeds flowers that remember your touch.","Here, even a quiet tide brings something extraordinary.","Bring your discoveries home. There is always room at the table."};
                    Vector2[] positions={new Vector2(-3,2),new Vector2(4,3),new Vector2(6,4),new Vector2(1,12),new Vector2(14,3),new Vector2(0,-2)};
                    introStart=positions[beat];game.world.SetArea(areas[beat],introStart);caption.text=text[beat];game.world.Burst(introStart+Vector2.up,"spark");
                    if(beat==5){var sign=game.world.points.First(p=>p.action=="service");if(sign.artwork)sign.artwork.sprite=WorldView.Art("sign-open");for(int i=0;i<3;i++){game.world.guests[i].gameObject.SetActive(true);game.world.guests[i].position=new Vector2(-5+i*5,-.9f);}game.world.carriedDish.sprite=WorldView.Art("held-dish-fish");}
                }
                float t=Mathf.Repeat(introClock,4);if(fade)fade.color=new Color(.08f,.12f,.16f,game.Model.State.reducedMotion?0:Mathf.Max(Mathf.Clamp01((.4f-t)/.4f),Mathf.Clamp01((t-3.6f)/.4f)));
                if(beat==5&&t>2&&!servedIntro){servedIntro=true;game.world.carriedDish.sprite=null;for(int i=0;i<3;i++){var plate=WorldView.Add(game.world.restaurant,"held-dish-fish",game.world.restaurant.InverseTransformPoint(game.world.guests[i].position+Vector3.up*.7f),1800);introPlates.Add(plate.gameObject);game.world.Burst(game.world.guests[i].position+Vector3.up,"spark");}}
                game.world.player.position=introStart+Vector2.right*(Mathf.Min(t,2)*.45f);game.world.Animate(t<2?Vector2.right:Vector2.zero);game.world.Tool(-1,Vector2.right,false,null,"");game.world.Roam();
                if(beat==5)for(int i=0;i<3;i++){game.world.guests[i].GetComponentInChildren<SpriteRenderer>().sprite=WorldView.Art("visitor-"+i+"-"+((int)(Time.time*3)%4));if(t>2&&t<2.04f)game.world.Burst(game.world.guests[i].position+Vector3.up,"spark");}
                if(introClock>=24||Keyboard.current?.escapeKey.wasPressedThisFrame==true)FinishIntro();return;
            }
            if(Keyboard.current?.escapeKey.wasPressedThisFrame==true&&ScreenName!="Title")Title();
            if(ScreenName=="Character"&&portrait&&look!=null){int f=(int)(Time.unscaledTime*5)%4;portrait.sprite=look.Apply(WorldView.Art("chef-down-"+f));}
            for(int i=0;i<glints.Count;i++)glints[i].color=new Color(1,.96f,.8f,PlayerPrefs.GetInt("journey-motion",0)==1?.18f:Mathf.Pow(Mathf.Max(0,Mathf.Sin(Time.unscaledTime*1.1f+i*1.7f)),6)*.6f);
            if(PlayerPrefs.GetInt("journey-motion",0)==0)for(int i=0;i<clouds.Count;i++){var p=clouds[i].anchoredPosition;p.x+=Time.unscaledDeltaTime*(4+i);if(p.x>740)p.x=-740;clouds[i].anchoredPosition=p;}
        }
        void OnDestroy(){look?.Dispose();}
    }
}
