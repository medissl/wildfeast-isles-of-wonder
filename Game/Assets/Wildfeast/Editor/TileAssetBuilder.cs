using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
namespace Wildfeast.Editor
{
    public static class TileAssetBuilder
    {
        public static void Create()
        {
            const string folder="Assets/Wildfeast/Resources/Terrain";Directory.CreateDirectory(folder);AssetDatabase.Refresh();
            foreach(var file in Directory.GetFiles("Assets/Wildfeast/Resources/Art","tile-*.png"))
            {
                var name=Path.GetFileNameWithoutExtension(file);if(name.StartsWith("tile-water-")&&!name.EndsWith("-0"))continue;
                var key=name.StartsWith("tile-water-")?name.Substring(0,name.Length-2):name;string path=folder+"/"+key+".asset";
                var tile=AssetDatabase.LoadAssetAtPath<TileBase>(path);
                if(!tile){tile=key.StartsWith("tile-water-")?ScriptableObject.CreateInstance<TidalTile>():ScriptableObject.CreateInstance<Tile>();AssetDatabase.CreateAsset(tile,path);}
                if(tile is Tile ground){ground.sprite=Resources.Load<Sprite>("Art/"+name);ground.color=Color.white;ground.colliderType=Tile.ColliderType.None;}
                if(tile is TidalTile tide){tide.colliderType=Tile.ColliderType.Grid;tide.frames=new Sprite[6];for(int f=0;f<6;f++)tide.frames[f]=Resources.Load<Sprite>("Art/"+key+"-"+f);}
                EditorUtility.SetDirty(tile);
            }
            AssetDatabase.SaveAssets();
            // These former painted terrain canvases are no longer referenced by authored worlds.
            foreach(var island in Archipelago.Islands)AssetDatabase.DeleteAsset("Assets/Wildfeast/Resources/Art/"+island.key+".png");
            AssetDatabase.DeleteAsset("Assets/Wildfeast/Resources/Art/ocean.png");
        }
    }
}
