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
            if( c == UnityEngine.Color.blue ) {
                return new Tile( XMLLoader.Tile[5].id );//water
            }
            if( c == new UnityEngine.Color( 160, 157, 0 ) ) {//sand
                return new Tile( XMLLoader.Tile[4].id );
            }
            if( c == new UnityEngine.Color( 102, 57, 49 ) ) {//dirt
                return new Tile( XMLLoader.Tile[3].id );
            }
            if( c == new UnityEngine.Color( 72, 160, 0 ) ) { //grass
                return new Tile( XMLLoader.Tile[2].id );
            }
            return new Tile( XMLLoader.Tile[0].id ); //void or empty
        }

        public Tile( byte type )
        {
            this.TileType = type;
        }
    }
}