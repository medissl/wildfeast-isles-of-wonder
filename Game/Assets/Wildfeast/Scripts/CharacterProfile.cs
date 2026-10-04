using System;
using System.Collections.Generic;
using UnityEngine;
namespace Wildfeast
{
    [Serializable] public class CharacterProfile
    {
        public string name="Mira";
        public int skin,hairColor,hairStyle=3,face,shirt,pants,boots;
        public void Normalize()
        {
            name=string.IsNullOrWhiteSpace(name)?"Mira":name.Trim();if(name.Length>18)name=name.Substring(0,18);
            skin=Mathf.Clamp(skin,0,5);hairColor=Mathf.Clamp(hairColor,0,5);hairStyle=Mathf.Clamp(hairStyle,0,3);
            face=Mathf.Clamp(face,0,2);shirt=Mathf.Clamp(shirt,0,5);pants=Mathf.Clamp(pants,0,5);boots=Mathf.Clamp(boots,0,5);
        }
    }
    public sealed class CharacterLook : IDisposable
    {
        public static readonly string[][] Palettes={
            new[]{"dfad84","f3d7b4","c8906c","b78261","895c49","5c3f35"},
            new[]{"584344","d7b676","ae603c","303c52","9276a6","e2ded0"},
            new[]{"568f88","aa6158","6c80aa","8d739f","b38d4e","657d4e"},
            new[]{"566173","443d55","716348","7b5356","365d5f","ad986f"},
            new[]{"74543e","473b38","966740","704e64","4c6375","a79a78"}};
        readonly Dictionary<Sprite,Sprite> cache=new Dictionary<Sprite,Sprite>();readonly CharacterProfile profile;
        public CharacterLook(CharacterProfile p){profile=p??new CharacterProfile();profile.Normalize();}
        public Sprite Apply(Sprite source)
        {
            if(!source)return source;if(cache.TryGetValue(source,out var result))return result;
            var rect=source.rect;int w=(int)rect.width,h=(int)rect.height;var all=source.texture.GetPixels32();var pixels=new Color32[w*h];
            Color32[] from={GameUI.C("dfad84"),GameUI.C("584344"),GameUI.C("568f88"),GameUI.C("566173"),GameUI.C("74543e")};
            int[] choices={profile.skin,profile.hairColor,profile.shirt,profile.pants,profile.boots};
            for(int y=0;y<h;y++)for(int x=0;x<w;x++)
            {var c=all[((int)rect.y+y)*source.texture.width+(int)rect.x+x];for(int n=0;n<5;n++)if(c.Equals(from[n])){c=GameUI.C(Palettes[n][choices[n]]);break;}pixels[y*w+x]=c;}
            // Locate the head in each animated frame. The generated frames share 32×48 anatomy.
            int frame=0;int.TryParse(source.name.Substring(source.name.LastIndexOf('-')+1),out frame);frame=Mathf.Clamp(frame,0,4);
            int bend=0,bob=source.name.StartsWith("action-")?0:frame%2;
            if(source.name.StartsWith("action-")){var kind=source.name.Split('-')[1];int[] bends=kind=="plant"||kind=="pull"?new[]{0,2,4,2,0}:kind=="stir"?new[]{0,1,2,1,0}:kind=="pour"?new[]{0,1,2,2,0}:kind=="swing"?new[]{0,0,2,1,0}:new[]{0,0,1,1,0};bend=bends[frame];}
            int headTop=33-bob-bend;Color32 skin=GameUI.C(Palettes[0][profile.skin]);
            bool back=source.name.Contains("-up-");Color32 hair=GameUI.C(Palettes[1][profile.hairColor]);
            void Pixel(int x,int y,Color32 c){if(x>=0&&x<w&&y>=0&&y<h)pixels[y*w+x]=c;}
            Color32 ink=GameUI.C("273740");
            if(profile.hairStyle!=3)
            {
                for(int y=headTop+1;y<h;y++)for(int x=4;x<29;x++)Pixel(x,y,new Color32(0,0,0,0));
                // Rounded crown, side locks, bangs and a highlight, all on the native pixel grid.
                for(int row=0;row<8;row++){int inset=row>=6?3:row>=4?1:0;for(int x=8+inset;x<=24-inset;x++)Pixel(x,headTop+row,ink);for(int x=9+inset;x<=23-inset;x++)Pixel(x,headTop+row,hair);}
                Color32 highlight=Color.Lerp((Color)hair,GameUI.C("f3d7b4"),.25f);for(int x=12;x<=20;x++)Pixel(x,headTop+5,highlight);
                for(int y=headTop-2;y<=headTop;y++)for(int x=10;x<=13;x++)Pixel(x,y,hair);
                if(profile.hairStyle==1)for(int y=headTop-12;y<=headTop;y++){Pixel(7,y,ink);Pixel(8,y,hair);Pixel(9,y,hair);Pixel(23,y,hair);Pixel(24,y,hair);Pixel(25,y,ink);}
                if(profile.hairStyle==2)for(int row=0;row<5;row++){int inset=row==0||row==4?1:0;for(int x=14+inset;x<=21-inset;x++)Pixel(x,headTop+7+row,hair);}
                if(back)for(int y=headTop-10;y<headTop;y++)for(int x=10;x<23;x++)Pixel(x,y,hair);
            }
            if(!back&&profile.face!=0)
            {
                int y=headTop-2;bool left=source.name.Contains("-left-"),right=source.name.Contains("-right-");
                foreach(int center in left?new[]{12}:right?new[]{20}:new[]{12,20})
                {
                    for(int xx=center-2;xx<=center+2;xx++)for(int yy=y-2;yy<=y+1;yy++)Pixel(xx,yy,skin);
                    if(profile.face==1){Pixel(center-1,y-1,ink);Pixel(center,y,ink);Pixel(center+1,y-1,ink);}
                    else{for(int xx=center-2;xx<=center+2;xx++){Pixel(xx,y+1,ink);Pixel(xx,y-2,ink);}for(int yy=y-2;yy<=y+1;yy++){Pixel(center-2,yy,ink);Pixel(center+2,yy,ink);}Pixel(center,y-1,ink);}
                }
                if(profile.face==2&&!left&&!right)for(int x=15;x<=17;x++)Pixel(x,y,ink);
            }
            var texture=new Texture2D(w,h,TextureFormat.RGBA32,false){filterMode=FilterMode.Point,name="Avatar "+source.name};texture.SetPixels32(pixels);texture.Apply();
            result=Sprite.Create(texture,new Rect(0,0,w,h),source.pivot/rect.size,source.pixelsPerUnit);result.name=source.name;cache[source]=result;return result;
        }
        public void Dispose(){foreach(var sprite in cache.Values){if(Application.isPlaying){UnityEngine.Object.Destroy(sprite.texture);UnityEngine.Object.Destroy(sprite);}else{UnityEngine.Object.DestroyImmediate(sprite.texture);UnityEngine.Object.DestroyImmediate(sprite);}}cache.Clear();}
    }
}
