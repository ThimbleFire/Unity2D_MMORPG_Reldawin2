using UnityEngine;

namespace LowCloud.Reldawin
{
    public class MyMath
    {
        /// <summary>
        /// Gets the tile at the given world space position
        /// </summary>
        public static Vector2Int IsometricToCell( Vector2 position, bool offset = true )
        {
            if(offset)
                position.x += Tile.WorldSpaceHalfWidth;

            float height = ( position.x / Tile.WorldSpaceWidth ) + ( position.y / Tile.WorldSpaceHeight );
            float width = ( position.x / Tile.WorldSpaceWidth ) + ( -position.y / Tile.WorldSpaceHeight );

            int tileClickedX = Mathf.FloorToInt( (int)width / 2 );
            int tileClickedY = Mathf.FloorToInt( (int)height / 2 );

            return new Vector2Int( tileClickedX, tileClickedY );
        }

        /// <summary>
        /// Converts world space to isometric world space
        /// </summary>
        public static Vector2 CellToIsometric( int x, int y )
        {
            float ofsX = ( y * Tile.WorldSpaceHeight ) + ( x * Tile.WorldSpaceHeight );
            float ofsY = ( y * Tile.WorldSpaceHeight / 2 ) - ( x * Tile.WorldSpaceHeight / 2 );

            //Since we've removed scale, Tile.WorldSpaceHeight and Width have halved.
            //However our chunk meshes are still 2x scale
            return new Vector2( ofsX, ofsY ) * 2;
        }

        public static Vector2 CellToIsometric( Vector2Int chunkIndex )
        {
            return CellToIsometric( chunkIndex.x, chunkIndex.y );
        }
    }
}