using System;
using System.IO;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;

namespace Wildfeast.Editor
{
    public static class CozyPolish
    {
        public const string FontPath="Assets/Wildfeast/Resources/Fonts/Pixelify.asset";
        [MenuItem("Wildfeast/Apply cozy typography")]
        public static void Typography()
        {
            AssetDatabase.Refresh();
            var font=AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontPath);
            if(!font)
            {
                Directory.CreateDirectory("Assets/Wildfeast/Resources/Fonts");AssetDatabase.Refresh();
                var source=AssetDatabase.LoadAssetAtPath<Font>("Assets/Wildfeast/Fonts/PixelifySans.ttf");
                font=TMP_FontAsset.CreateFontAsset(source,100,6,GlyphRenderMode.SDF16,1024,1024,AtlasPopulationMode.Dynamic,false);
                if(!font)throw new InvalidOperationException("Pixelify font import failed.");
                font.name="Pixelify Sans - Wildfeast";
                string characters=new string(Enumerable.Range(32,95).Select(i=>(char)i).ToArray())+"·–—…×’“”";
                if(!font.TryAddCharacters(characters,out string missing))throw new InvalidOperationException("Missing game glyphs: "+missing);
                font.atlasPopulationMode=AtlasPopulationMode.Static;font.atlasTextures[0].filterMode=FilterMode.Bilinear;
                font.fallbackFontAssetTable=new System.Collections.Generic.List<TMP_FontAsset>();font.material.SetFloat(ShaderUtilities.ID_WeightNormal,0);
                AssetDatabase.CreateAsset(font,FontPath);AssetDatabase.AddObjectToAsset(font.atlasTextures[0],font);AssetDatabase.AddObjectToAsset(font.material,font);
                EditorUtility.SetDirty(font);AssetDatabase.SaveAssets();
            }
            EditorSceneManager.OpenScene("Assets/Wildfeast/Scenes/Wildfeast.unity");
            foreach(var text in UnityEngine.Object.FindObjectsByType<TMP_Text>(FindObjectsInactive.Include,FindObjectsSortMode.None))
            {text.font=font;text.fontSharedMaterial=font.material;text.enableAutoSizing=false;EditorUtility.SetDirty(text);}
            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
            Debug.Log("WILDFEAST: all authored text uses the static Pixelify font.");
        }
        public static void FontAndBuild(){Typography();ProjectBuilder.Build();}
        public static void ApplyAndBuild(){Typography();ArtAndRoom();ProjectBuilder.Build();}
        [MenuItem("Wildfeast/Repair room and pixel widgets")]
        public static void ArtAndRoom()
        {
            if(UnityEngine.Rendering.GraphicsSettings.currentRenderPipeline is not UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset)throw new InvalidOperationException("Expected URP 2D.");
            AssetDatabase.Refresh();
            foreach(var path in Directory.GetFiles("Assets/Wildfeast/Resources/Art","*.png"))
            {
                var importer=(TextureImporter)AssetImporter.GetAtPath(path.Replace('\\','/'));
                importer.textureType=TextureImporterType.Sprite;importer.spriteImportMode=SpriteImportMode.Single;importer.filterMode=FilterMode.Point;importer.mipmapEnabled=false;importer.textureCompression=TextureImporterCompression.Uncompressed;importer.spritePixelsPerUnit=32;
                var settings=new TextureImporterSettings();importer.ReadTextureSettings(settings);settings.spriteAlignment=(int)SpriteAlignment.Custom;settings.spritePivot=new Vector2(.5f,0);
                string name=Path.GetFileNameWithoutExtension(path);
                if(name.StartsWith("held-"))settings.spritePivot=HeldPivot(name);
                importer.SetTextureSettings(settings);
                if(name=="ui-slot"||name=="ui-button")importer.spriteBorder=new Vector4(3,3,3,3);
                importer.SaveAndReimport();
            }
            EditorSceneManager.OpenScene("Assets/Wildfeast/Scenes/Wildfeast.unity");
            var world=UnityEngine.Object.FindFirstObjectByType<WorldView>();var room=world.restaurant;
            foreach(var sr in room.GetComponentsInChildren<SpriteRenderer>(true))
            {
                string name=sr.gameObject.name;
                if(new[]{"prep-board","cook-pot","cook-pan","flower-purple"}.Contains(name)){UnityEngine.Object.DestroyImmediate(sr.gameObject);continue;}
                if(name=="stove")
                {
                    var point=sr.GetComponentInParent<WorldPoint>(true);point.transform.localPosition=new Vector2(0,2.3f);sr.sprite=WorldView.Art("kitchen-worktop");sr.gameObject.name="kitchen-worktop";
                    foreach(var c in sr.GetComponentsInChildren<Collider2D>(true))UnityEngine.Object.DestroyImmediate(c.gameObject);
                    Foot(sr,new Vector2(4.65f,.85f),Vector2.up*.4f);
                }
                if(name=="table")
                {
                    if(sr.transform.localPosition.y>-1)sr.transform.localPosition=new Vector2(sr.transform.localPosition.x,.1f);
                    var collider=sr.GetComponentInChildren<BoxCollider2D>(true);collider.size=new Vector2(2.2f,.8f);
                }
                if(name=="bed"){var collider=sr.GetComponentInChildren<BoxCollider2D>(true);if(collider){collider.size=new Vector2(2.1f,1.35f);collider.transform.localPosition=Vector2.up*.7f;}}
                if(name.StartsWith("table-number-"))
                {
                    int index=int.Parse(name.Substring("table-number-".Length));var parent=sr.transform.parent;
                    var table=parent.name=="table"?parent.GetComponent<SpriteRenderer>():parent.GetComponentsInChildren<SpriteRenderer>(true).First(t=>t.gameObject.name=="table"&&Mathf.Abs(t.transform.localPosition.x-sr.transform.localPosition.x)<1);
                    sr.transform.SetParent(table.transform,true);sr.transform.localPosition=new Vector2(-.65f,.55f);
                    var depth=sr.GetComponent<PropDepth>()??sr.gameObject.AddComponent<PropDepth>();depth.groundOffset=-.3f;depth.bias=1;depth.Apply();
                }
                if(name.StartsWith("guest-")&&sr.GetComponentInParent<WorldPoint>(true).index<3)sr.transform.parent.localPosition=new Vector2(sr.transform.parent.localPosition.x,-.9f);
                if(name=="hanging-herbs"||name=="lantern")
                {
                    sr.transform.localPosition=new Vector2(sr.transform.localPosition.x,name=="lantern"?3.7f:3.75f);sr.sortingOrder=700;
                    foreach(var c in sr.GetComponentsInChildren<Collider2D>(true))UnityEngine.Object.DestroyImmediate(c.gameObject);
                }
                if(name=="menu-board"||name.StartsWith("sign-"))sr.sortingOrder=700;
                if(name=="steam"){sr.transform.localPosition=new Vector2(-.07f,3.95f);sr.sortingOrder=700;}
            }
            foreach(var sr in world.GetComponentsInChildren<SpriteRenderer>(true))
            {
                var collider=sr.GetComponentInChildren<Collider2D>(true);
                if(!collider||sr==world.playerArt||sr.transform.IsChildOf(world.player))continue;
                if(sr.gameObject.name=="saltstone"&&collider is BoxCollider2D stone){stone.size=new Vector2(1.45f,.65f);stone.transform.localPosition=Vector2.up*.3f;}
                var depth=sr.GetComponent<PropDepth>()??sr.gameObject.AddComponent<PropDepth>();depth.groundOffset=collider.transform.TransformPoint(collider.offset).y-sr.transform.position.y;depth.Apply();
            }
            if(!room.Find("Back wall footprint")){var wall=new GameObject("Back wall footprint",typeof(BoxCollider2D));wall.transform.SetParent(room,false);wall.transform.localPosition=new Vector2(0,4.6f);wall.GetComponent<BoxCollider2D>().size=new Vector2(19,.7f);}
            for(int i=0;i<2;i++)if(!room.Find("Floor planter footprint "+i)){var pot=new GameObject("Floor planter footprint "+i,typeof(BoxCollider2D));pot.transform.SetParent(room,false);pot.transform.localPosition=new Vector2(i==0?-8.75f:8.75f,-3.7f);pot.GetComponent<BoxCollider2D>().size=new Vector2(.95f,.75f);}
            for(int i=0;i<2;i++)if(!room.Find("Planter "+i)){var sr=WorldView.Add(room,"room-planter",new Vector2(-8.5f+i*17,1.3f),950);sr.gameObject.name="Planter "+i;Foot(sr,new Vector2(.65f,.4f),Vector2.up*.15f);sr.gameObject.AddComponent<PropDepth>().Apply();}
            world.carriedDish.transform.localPosition=new Vector2(.3f,.55f);
            world.employee.transform.position=WorldView.StaffHome;
            var ui=UnityEngine.Object.FindFirstObjectByType<GameUI>();
            foreach(var button in ui.GetComponentsInChildren<UnityEngine.UI.Button>(true))
            {GameUI.Skin(button.GetComponent<UnityEngine.UI.Image>(),ui.toolFrames.Contains(button.GetComponent<UnityEngine.UI.Image>()));var colors=button.colors;colors.normalColor=colors.selectedColor=Color.white;colors.highlightedColor=GameUI.C("ffe0b0");colors.pressedColor=GameUI.C("cfb383");button.colors=colors;}
            var font=Resources.Load<TMP_FontAsset>("Fonts/Pixelify");string materialPath="Assets/Wildfeast/Resources/Fonts/PixelifyHUD.mat";
            var hud=AssetDatabase.LoadAssetAtPath<Material>(materialPath);
            if(!hud){hud=new Material(font.material);hud.name="Pixelify HUD";hud.SetFloat(ShaderUtilities.ID_OutlineWidth,.16f);hud.SetColor(ShaderUtilities.ID_OutlineColor,GameUI.C("40372e"));AssetDatabase.CreateAsset(hud,materialPath);}
            foreach(var text in new[]{ui.prompt,ui.equippedLabel,ui.orderText}){text.fontSharedMaterial=hud;text.UpdateMeshPadding();}
            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());AssetDatabase.SaveAssets();
            Debug.Log("WILDFEAST: preserved scene, repaired restaurant furniture/depth/collision and themed existing buttons.");
        }
        static void Foot(SpriteRenderer sr,Vector2 size,Vector2 offset)
        {var go=new GameObject("Collision",typeof(BoxCollider2D));go.transform.SetParent(sr.transform,false);go.transform.localPosition=offset;go.GetComponent<BoxCollider2D>().size=size;}
        public static Vector2 HeldPivot(string name)=>name switch
        {
            "held-rod"=>new Vector2(4f/24,2f/32),"held-axe"=>new Vector2(7f/22,2f/26),"held-scythe"=>new Vector2(7f/26,3f/26),"held-pickaxe"=>new Vector2(9f/26,3f/26),"held-shovel"=>new Vector2(9f/20,21f/26),"held-can"=>new Vector2(.2f,.6f),"held-knife"=>new Vector2(3f/18,2f/24),_=>new Vector2(.5f,0)
        };
    }
}
