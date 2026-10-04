using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
namespace Wildfeast.Editor
{
    public static class GuidePictureBuilder
    {
        public static void Bake(WorldView world,bool force=false)
        {
            if(!force&&File.Exists("Assets/Wildfeast/Resources/GuidePictures.json"))return;
            var camera=world.worldCamera;var pixel=camera.GetComponent<PixelPerfectCamera>();bool enabled=pixel.enabled;pixel.enabled=false;
            var names=new[]{"explore","garden","fishing","kitchen"};var positions=new[]{new Vector2(-4,0),new Vector2(-9,-4),Archipelago.Get(0).pond,new Vector2(0,0)};
            for(int n=0;n<4;n++)
            {
                world.SetArea(n==3?2:0,positions[n]);camera.transform.position=new Vector3(positions[n].x,positions[n].y,-10);camera.orthographicSize=n==1?3:5.625f;
                var target=RenderTexture.GetTemporary(640,360,24,RenderTextureFormat.ARGB32);target.filterMode=FilterMode.Point;
                RenderPipeline.SubmitRenderRequest(camera,new UniversalRenderPipeline.SingleCameraRequest{destination=target});
                var previous=RenderTexture.active;RenderTexture.active=target;var texture=new Texture2D(640,360,TextureFormat.RGBA32,false);texture.ReadPixels(new Rect(0,0,640,360),0,0);texture.Apply();
                var path="Assets/Wildfeast/Resources/Art/guide-"+names[n]+".png";File.WriteAllBytes(path,texture.EncodeToPNG());RenderTexture.active=previous;RenderTexture.ReleaseTemporary(target);Object.DestroyImmediate(texture);
                AssetDatabase.ImportAsset(path);var importer=(TextureImporter)AssetImporter.GetAtPath(path);importer.textureType=TextureImporterType.Sprite;importer.spriteImportMode=SpriteImportMode.Single;importer.filterMode=FilterMode.Point;importer.textureCompression=TextureImporterCompression.Uncompressed;importer.mipmapEnabled=false;importer.spritePixelsPerUnit=32;importer.SaveAndReimport();
            }
            pixel.enabled=enabled;world.SetArea(0,new Vector2(-6,-2.5f));
        }
    }
}
