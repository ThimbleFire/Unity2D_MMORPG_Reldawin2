using UnityEngine;

namespace AlwaysEast
{
    public class MyMath
    {
        /// <summary>
        /// Gets the tile at the given world space position
        /// </summary>
        public static Vector2Int IsometricToCell( Vector2 position, bool offset = true )
        {
            if(offset)
                position.x += Tile.Width / 2;

            float height =  position.x / Tile.Width  +  position.y / Tile.Height;
            float width =  position.x / Tile.Width  +  -position.y / Tile.Height;

            int tileClickedX = Mathf.FloorToInt( (int)width  );
            int tileClickedY = Mathf.FloorToInt( (int)height  );

            return new Vector2Int( tileClickedX, tileClickedY ) * 1;
        }

        /// <summary>
        /// Converts world space to isometric world space
        /// </summary>
        public static Vector2 CellToIsometric( int x, int y )
        {
            float ofsX = ( y * Tile.Height ) + ( x * Tile.Height );
            float ofsY = ( y * Tile.Height / 2 ) - ( x * Tile.Height / 2 );

            return new Vector2( ofsX, ofsY ) * 1;
        }
        public static Vector2 CellToIsometric( Vector3Int chunkIndex ) {
            return CellToIsometric( chunkIndex.x, chunkIndex.y );
        }

        public static Vector2Int ClampVector2Int( Vector2Int toClamp, int xMin, int xMax, int yMin, int yMax )
        {
            toClamp.x = Mathf.Clamp( toClamp.x, xMin, xMax );
            toClamp.y = Mathf.Clamp( toClamp.y, yMin, yMax );

            return toClamp;
        }
        public static Vector2 ClampVector2( Vector2 toClamp, float xMin, float xMax, float yMin, float yMax )
        {
            toClamp.x = Mathf.Clamp( toClamp.x, xMin, xMax );
            toClamp.y = Mathf.Clamp( toClamp.y, yMin, yMax );

            return toClamp;
        }
    }
}