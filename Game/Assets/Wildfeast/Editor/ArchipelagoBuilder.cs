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
                importer.textureType=TextureImporterType.Sprite;importer.spriteImportMode=SpriteImportMode.Single;importer.spritePixelsPerUnit=32;importer.filterMode=FilterMode.Point;importer.mipmapEnabled=false;importer.textureCompression=TextureImporterCompression.Uncompressed;
                var settings=new TextureImporterSettings();importer.ReadTextureSettings(settings);settings.spriteAlignment=(int)SpriteAlignment.Custom;string name=Path.GetFileNameWithoutExtension(path);settings.spritePivot=name.StartsWith("held-")?CozyPolish.HeldPivot(name):new Vector2(.5f,0);importer.SetTextureSettings(settings);importer.maxTextureSize=new[]{"emberfold","moonfen","pearltide","map-emberfold","map-moonfen","map-pearltide"}.Contains(name)?4096:2048;importer.isReadable=name.StartsWith("chef-")||name.StartsWith("action-")||name.StartsWith("player-");
                // Existing compact tool handle pivots and UI borders remain intact.
                importer.SaveAndReimport();
            }
            var scene=EditorSceneManager.OpenScene("Assets/Wildfeast/Scenes/Wildfeast.unity");var world=UnityEngine.Object.FindFirstObjectByType<WorldView>();
            var oldRoots=world.islands!=null&&world.islands.Length==5?world.islands:new[]{world.saltleaf,world.mistwake};
            world.points.RemoveAll(p=>!p||oldRoots.Any(root=>p.transform.IsChildOf(root)));world.cropArt.Clear();
            foreach(var root in oldRoots)UnityEngine.Object.DestroyImmediate(root.gameObject);
            Archipelago.Author(world);world.SetArea(0,new Vector2(-6,-2.5f));
            ProjectBuilder.BakeIslandMaps(world,world.worldCamera,world.worldCamera.GetComponent<PixelPerfectCamera>());
            EditorSceneManager.MarkSceneDirty(scene);EditorSceneManager.SaveScene(scene);AssetDatabase.SaveAssets();
            EditorSceneManager.OpenScene("Assets/Wildfeast/Scenes/Wildfeast.unity");world=UnityEngine.Object.FindFirstObjectByType<WorldView>();
            if(world.islands.Length!=5||world.points.Any(p=>!p)||world.islands.Any(root=>!root))throw new InvalidOperationException("Island scene references did not round trip.");
            Debug.Log("ARCHIPELAGO AUTHOR SUCCESS: 5 authored islands; restaurant and UI preserved.");
        }
        public static void AuthorAndBuild(){Author();ProjectBuilder.Build();}
    }
}
