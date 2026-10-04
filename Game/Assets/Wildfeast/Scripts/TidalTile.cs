using UnityEngine;
using UnityEngine.Tilemaps;
namespace Wildfeast
{
    public class TidalTile : TileBase
    {
        public Sprite[] frames;
        public Tile.ColliderType colliderType=Tile.ColliderType.Grid;
        public override void GetTileData(Vector3Int position,ITilemap tilemap,ref TileData data)
        {data.sprite=frames!=null&&frames.Length>0?frames[0]:null;data.color=Color.white;data.transform=Matrix4x4.identity;data.colliderType=colliderType;data.flags=TileFlags.None;}
        public override bool GetTileAnimationData(Vector3Int position,ITilemap tilemap,ref TileAnimationData data)
        {data.animatedSprites=frames;data.animationSpeed=5;data.animationStartTime=TerrainGrid.Variant(new Vector2Int(position.x,position.y),6)*.2f;return frames!=null&&frames.Length>0;}
    }
}
