using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Wildfeast.Editor
{
    public static class ArchipelagoBuilder
    {
        static void DetectPipeline()
        {
            var pipeline=GraphicsSettings.currentRenderPipeline;
            if(!pipeline||!pipeline.GetType().FullName.Contains("Universal"))throw new InvalidOperationException("The inspected pixel camera requires URP.");
            Debug.Log("ARCHIPELAGO PIPELINE: "+pipeline.GetType().FullName);
        }
        public static void Author()
        {
            DetectPipeline();AssetDatabase.Refresh();
            foreach(var file in Directory.GetFiles("Assets/Wildfeast/Resources/Art","*.png"))
            {
                var path=file.Replace('\\','/');var importer=(TextureImporter)AssetImporter.GetAtPath(path);
                if(!importer)continue;
                string name=Path.GetFileNameWithoutExtension(path);var settings=new TextureImporterSettings();importer.ReadTextureSettings(settings);
                var pivot=name.StartsWith("tile-")?new Vector2(.5f,.5f):name.StartsWith("held-")?CozyPolish.HeldPivot(name):new Vector2(.5f,0);
                int maximum=name.StartsWith("map-")?4096:2048;bool readable=name.StartsWith("chef-")||name.StartsWith("action-")||name.StartsWith("player-");
                bool changed=importer.textureType!=TextureImporterType.Sprite||importer.spriteImportMode!=SpriteImportMode.Single||importer.spritePixelsPerUnit!=32||importer.filterMode!=FilterMode.Point||importer.mipmapEnabled||importer.textureCompression!=TextureImporterCompression.Uncompressed||settings.spriteAlignment!=(int)SpriteAlignment.Custom||settings.spritePivot!=pivot||importer.maxTextureSize!=maximum||importer.isReadable!=readable;
                if(!changed)continue;
                importer.textureType=TextureImporterType.Sprite;importer.spriteImportMode=SpriteImportMode.Single;importer.spritePixelsPerUnit=32;importer.filterMode=FilterMode.Point;importer.mipmapEnabled=false;importer.textureCompression=TextureImporterCompression.Uncompressed;
                settings.spriteAlignment=(int)SpriteAlignment.Custom;settings.spritePivot=pivot;importer.SetTextureSettings(settings);importer.maxTextureSize=maximum;importer.isReadable=readable;importer.SaveAndReimport();

            }
            TileAssetBuilder.Create();
            var scene=EditorSceneManager.OpenScene("Assets/Wildfeast/Scenes/Wildfeast.unity");var world=UnityEngine.Object.FindFirstObjectByType<WorldView>();
            var oldRoots=world.islands!=null&&world.islands.Length>0?world.islands:new[]{world.saltleaf,world.mistwake};
            world.points.RemoveAll(p=>!p||oldRoots.Any(root=>p.transform.IsChildOf(root)));world.cropArt.Clear();
            foreach(var root in oldRoots)UnityEngine.Object.DestroyImmediate(root.gameObject);
            Archipelago.Author(world);TileWorld.AuthorRoom(world.restaurant);world.AuthorBedroom(true);OriginalPresentation.Author(world);world.SetArea(0,new Vector2(-6,-2.5f));
            ProjectBuilder.BakeIslandMaps(world,world.worldCamera,world.worldCamera.GetComponent<PixelPerfectCamera>());
            GuidePictureBuilder.Bake(world,true);
            EditorSceneManager.MarkSceneDirty(scene);EditorSceneManager.SaveScene(scene);AssetDatabase.SaveAssets();
            EditorSceneManager.OpenScene("Assets/Wildfeast/Scenes/Wildfeast.unity");world=UnityEngine.Object.FindFirstObjectByType<WorldView>();
            if(world.islands.Length!=Archipelago.Islands.Length||world.points.Any(p=>!p)||world.islands.Any(root=>!root))throw new InvalidOperationException("Island scene references did not round trip.");
            Debug.Log("ARCHIPELAGO AUTHOR SUCCESS: connected outdoor districts; restaurant and UI preserved.");
        }
        public static void AuthorAndBuild(){Author();ProjectBuilder.Build();}
    }
}
