using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

namespace Wildfeast.Editor
{
    public static class ProjectBuilder
    {
        public static void ImportFontsAndExit()
        {
            var package=Directory.GetFiles("Library/PackageCache","TMP Essential Resources.unitypackage",SearchOption.AllDirectories).Single();
            double deadline=EditorApplication.timeSinceStartup+120;
            AssetDatabase.ImportPackage(package,false);
            EditorApplication.update+=Poll;
            void Poll()
            {
                if(AssetDatabase.LoadAssetAtPath<TMPro.TMP_FontAsset>("Assets/TextMesh Pro/Resources/Fonts & Materials/LiberationSans SDF.asset") && Resources.Load<TMPro.TMP_Settings>("TMP Settings"))
                {EditorApplication.update-=Poll;AssetDatabase.SaveAssets();Debug.Log("WILDFEAST FONT IMPORT COMPLETE");EditorApplication.Exit(0);}
                else if(EditorApplication.timeSinceStartup>deadline){EditorApplication.update-=Poll;Debug.LogError("TMP essential import timed out");EditorApplication.Exit(1);}
            }
        }
        [MenuItem("Wildfeast/Assemble playable scene")]
        public static void Assemble()
        {
            if(GraphicsSettings.currentRenderPipeline is not UniversalRenderPipelineAsset)throw new InvalidOperationException("Expected the inspected URP template.");
            if(!Resources.Load<TMPro.TMP_FontAsset>("Fonts & Materials/LiberationSans SDF") || !Resources.Load<TMPro.TMP_Settings>("TMP Settings"))
                throw new InvalidOperationException("Import TMP Essential Resources first using ImportFontsAndExit without -quit.");
            AssetDatabase.Refresh();
            foreach(string path in Directory.GetFiles("Assets/Wildfeast/Resources/Art","*.png"))
            {
                var importer=(TextureImporter)AssetImporter.GetAtPath(path.Replace('\\','/'));
                importer.textureType=TextureImporterType.Sprite;importer.spriteImportMode=SpriteImportMode.Single;importer.filterMode=FilterMode.Point;importer.mipmapEnabled=false;importer.textureCompression=TextureImporterCompression.Uncompressed;importer.spritePixelsPerUnit=32;
                var settings=new TextureImporterSettings();importer.ReadTextureSettings(settings);settings.spriteAlignment=(int)SpriteAlignment.Custom;settings.spritePivot=new Vector2(.5f,0);importer.SetTextureSettings(settings);if(path.EndsWith("ui-frame.png")||path.EndsWith("ui-board.png"))importer.spriteBorder=new Vector4(6,6,6,6);importer.maxTextureSize=2048;importer.SaveAndReimport();
            }
            var scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
            var cameraObject=new GameObject("WorldCamera",typeof(Camera),typeof(AudioListener),typeof(UniversalAdditionalCameraData));
            var camera=cameraObject.GetComponent<Camera>();cameraObject.tag="MainCamera";camera.orthographic=true;camera.orthographicSize=5.625f;camera.clearFlags=CameraClearFlags.SolidColor;camera.backgroundColor=GameUI.C("17383d");camera.allowHDR=camera.allowMSAA=camera.allowDynamicResolution=false;
            var pp=cameraObject.AddComponent<UnityEngine.Rendering.Universal.PixelPerfectCamera>();pp.assetsPPU=32;pp.refResolutionX=640;pp.refResolutionY=360;pp.gridSnapping=UnityEngine.Rendering.Universal.PixelPerfectCamera.GridSnapping.PixelSnapping;pp.cropFrame=UnityEngine.Rendering.Universal.PixelPerfectCamera.CropFrame.None;
            var light=new GameObject("Daylight",typeof(Light2D));light.GetComponent<Light2D>().lightType=Light2D.LightType.Global;light.GetComponent<Light2D>().intensity=1;
            var world=new GameObject("Archipelago",typeof(WorldView)).GetComponent<WorldView>();world.worldCamera=camera;world.AuthorWorlds();
            var ui=new GameObject("Interface",typeof(RectTransform),typeof(GameUI)).GetComponent<GameUI>();ui.AuthorUI();
            var events=new GameObject("EventSystem",typeof(EventSystem),typeof(InputSystemUIInputModule));events.GetComponent<InputSystemUIInputModule>().AssignDefaultActions();
            var controller=new GameObject("Wildfeast",typeof(GameController)).GetComponent<GameController>();controller.world=world;controller.ui=ui;
            QualitySettings.antiAliasing=0;QualitySettings.anisotropicFiltering=AnisotropicFiltering.Disable;QualitySettings.vSyncCount=1;
            PlayerSettings.companyName="Wildfeast";PlayerSettings.productName="Wildfeast - Isles of Wonder";PlayerSettings.defaultScreenWidth=1920;PlayerSettings.defaultScreenHeight=1080;PlayerSettings.fullScreenMode=FullScreenMode.FullScreenWindow;PlayerSettings.runInBackground=true;
            PlayerSettings.SetScriptingBackend(UnityEditor.Build.NamedBuildTarget.Standalone,ScriptingImplementation.Mono2x);
            PlayerSettings.resizableWindow=true;PlayerSettings.SplashScreen.show=false;
            EditorSettings.serializationMode=SerializationMode.ForceText;
            Directory.CreateDirectory("Assets/Wildfeast/Scenes");
            EditorSceneManager.SaveScene(scene,"Assets/Wildfeast/Scenes/Wildfeast.unity");
            EditorBuildSettings.scenes=new[]{new EditorBuildSettingsScene("Assets/Wildfeast/Scenes/Wildfeast.unity",true)};
            AssetDatabase.SaveAssets();
            EditorSceneManager.OpenScene("Assets/Wildfeast/Scenes/Wildfeast.unity");
            foreach(var mb in UnityEngine.Object.FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include,FindObjectsSortMode.None))
                if(mb && MonoScript.FromMonoBehaviour(mb)==null)throw new InvalidOperationException("Component has no serializable MonoScript: "+mb.GetType().FullName);
            var loaded=UnityEngine.Object.FindFirstObjectByType<GameController>();
            if(!loaded || !loaded.world || !loaded.ui || loaded.world.points.Any(p=>!p) || !loaded.ui.wallet.font)throw new InvalidOperationException("Scene round-trip lost a gameplay or font reference.");
            Debug.Log("WILDFEAST: playable scene assembled, serialized, reopened and verified.");
        }
        [MenuItem("Wildfeast/Build Windows player")]
        public static void Build()
        {
            string destination=Path.GetFullPath("../Builds/Windows/Wildfeast.exe");
            var args=Environment.GetCommandLineArgs();int index=Array.IndexOf(args,"-wildfeastBuild");if(index>=0)destination=args[index+1];
            Directory.CreateDirectory(Path.GetDirectoryName(destination));
            var report=BuildPipeline.BuildPlayer(new BuildPlayerOptions{scenes=new[]{"Assets/Wildfeast/Scenes/Wildfeast.unity"},locationPathName=destination,target=BuildTarget.StandaloneWindows64,options=BuildOptions.None});
            if(report.summary.result!=BuildResult.Succeeded)throw new Exception("Windows build failed: "+report.summary.result);
            Debug.Log("WILDFEAST BUILD SUCCESS: "+report.summary.totalSize+" bytes, "+report.summary.totalTime);
        }
        public static void AssembleAndBuild(){Assemble();Build();}
    }
}
