using System.Drawing;
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
        public byte TileType { get; set; }

        public int GetLayer { get { return XMLLoader.Tile[TileType].layerIndex; } }

        public static Tile GetTileByIndex( char v )
        {
            return new Tile( XMLLoader.Tile[v - '0'].id );
        }

        public static Tile GetTileByColour(UnityEngine.Color c) {
            if( c == new UnityEngine.Color( 0.282353f, 0.627451f, 0.000f ) ) { //grass
                return new Tile( XMLLoader.Tile[2].id );
            }
            if( c == new UnityEngine.Color( 0.627451f, 0.6156863f, 0.0f ) ) {//sand
                return new Tile( XMLLoader.Tile[4].id );
            }
            if( c == new UnityEngine.Color( 0.4f, 0.2235294f, 0.1921569f ) ) {//dirt
                return new Tile( XMLLoader.Tile[3].id );
            }
            if( c == new UnityEngine.Color( 0.0f, 0.2980392f, 0.6196079f ) ) {
                return new Tile( XMLLoader.Tile[5].id );//water
            }
            return new Tile( XMLLoader.Tile[0].id ); //void or empty
        }

        public Tile( byte type )
        {
            this.TileType = type;
        }
    }
}
