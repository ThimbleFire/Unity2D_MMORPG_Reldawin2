using UnityEngine;

namespace AlwaysEast
{
    public class AEMath
    {        
        /// <summary>
        /// Gets the tile at the given world space position
        /// offset returns the center of the cell, good for entity movement and scene object placement.
        /// THIS CODE IS EXPERIMENTAL AND MAY BE INACCURATE
        /// </summary>
        public static Vector3Int IsometricToCell( Vector2 position, bool offset = true )
        {
            if(offset) position.x += Tile.Width / 2;

            float height = ( position.x / Tile.Width ) + ( position.y / Tile.Height );
            float width = ( position.x / Tile.Width ) + ( -position.y / Tile.Height );

            int tileClickedX = Mathf.FloorToInt( (int)width  );
            int tileClickedY = Mathf.FloorToInt( (int)height  );

            return new Vector3Int( tileClickedX, tileClickedY );
        }

        /// <summary>
        /// Converts world space to isometric world space
        /// THIS CODE IS EXPERIMENTAL AND MAY BE INACCURATE
        /// </summary>
        public static Vector2 CellToIsometric( int x, int y )
        {
            float ofsX = ( y * Tile.Height ) + ( x * Tile.Height );
            float ofsY = ( y * Tile.Height / 2 ) - ( x * Tile.Height / 2 );

            return new Vector2( ofsX, ofsY );
        }
        /// <summary>
        /// Converts world space to isometric world space
        /// THIS CODE IS EXPERIMENTAL AND MAY BE INACCURATE
        /// </summary>
        public static Vector2 CellToIsometric( Vector2Int chunkIndex )
        {
            return CellToIsometric( chunkIndex.x, chunkIndex.y );
        }
    }
}
