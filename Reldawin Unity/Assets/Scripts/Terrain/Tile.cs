using UnityEngine;

namespace LowCloud.Reldawin
{
    public class Tile
    {
        // dimensions
        public const float WorldSpaceWidth = 0.64f;
        public const float WorldSpaceHalfWidth = 0.32f;
        public const float WorldSpaceHeight = 0.32f;
        
        // transform
        public Vector2Int CellPositionInWorld { get; set; }
        public Vector2Int CellPositionInChunk { get; set; }
        public Vector2 WorldPosition
        { 
            get 
            { 
                return MyMath.CellToIsometric( CellPositionInWorld ); 
            } 
        }
        public Vector2Int GetChunkIndex
        {
            get
            {
                return new Vector2Int( Mathf.FloorToInt( CellPositionInWorld.x / Chunk.Size ),
                                       Mathf.FloorToInt( CellPositionInWorld.y / Chunk.Size ) );
            }
        }

        // type
        public int TileType { get; set; }

        public byte GetLayer { get { return (byte)XMLLoader.GetTile(TileType).layerIndex; } }

        public static Tile GetTileByIndex( char v )
        {
            return new Tile( XMLLoader.GetTile(v - '0').id );
        }

        public Tile( int type )
        {
            this.TileType = type;
        }
    }
}