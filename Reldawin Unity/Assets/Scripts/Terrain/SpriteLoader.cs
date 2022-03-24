using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace LowCloud.Reldawin
{
    public class SpriteLoader
    {
        private Enum Due { N, E, S, W, NE, SE, SW, NW };
        
        public static Dictionary<string, Vector2[]> TileUVDictionary;
        public static Dictionary<string, Sprite> doodadDictionary;
        public static Dictionary<string, Sprite> itemDictionary;

        public static void Setup()
        {
            SetupItems();
            SetupDoodads();
            SetupTileUVs();
        }
        
        private static void SetupItems()
        {
            itemDictionary = new Dictionary<string, Sprite>();
            sprites = Resources.LoadAll<Sprite>( "Sprites/Interface/Items/items_32x32" );

            foreach ( Sprite s in sprites )
            {
                itemDictionary.Add( s.name, s );
            }            
        }
        
        private static void SetupDoodads()
        {
            doodadDictionary = new Dictionary<string, Sprite>();
            sprites = Resources.LoadAll<Sprite>( "Sprites/Enviroment/Terrain/terrainDetails" );

            foreach ( Sprite s in sprites )
            {
                doodadDictionary.Add( s.name, s );
            }

            sprites = Resources.LoadAll<Sprite>( "Sprites/Interface/Buttons/icon_selected" );

            foreach ( Sprite s in sprites )
            {
                doodadDictionary.Add( s.name, s );
            }            
        }
        
        private static void SetupTileUVs()
        {
            TileUVDictionary = new Dictionary<string, Vector2[]>();
            int spriteMapWidth = 1024;
            int spriteMapHeight = 1024;
            
            Sprite[] sprites = Resources.LoadAll<Sprite>( "Sprites/Enviroment/Terrain/Tile" );

            foreach ( Sprite s in sprites )
            {
                float left = s.rect.x / spriteMapWidth;
                float right = ( s.rect.x + s.rect.width ) / spriteMapWidth;

                float top = s.rect.y / spriteMapHeight;
                float bot = ( s.rect.y + s.rect.height ) / spriteMapHeight;

                float middleX = ( s.rect.x + ( s.rect.width / 2 ) ) / spriteMapWidth;
                float middleY = ( s.rect.y + ( s.rect.height / 2 ) ) / spriteMapHeight;

                Vector2 topMiddle = new Vector2( middleX, top );
                Vector2 botMiddle = new Vector2( middleX, bot );

                Vector2 rightMiddle = new Vector2( right, middleY );
                Vector2 leftMiddle = new Vector2( left, middleY );

                Vector2[] uvs = new Vector2[]
                {
                    topMiddle,
                    rightMiddle,
                    leftMiddle,
                    botMiddle
                };

                TileUVDictionary.Add( s.name, uvs );
            }
        }

        ///<summary>Get the appropriate tile UV. If neighbours are provided it'll take them into consideration. Type is the tile type and nType is an int array of neighbour types</summary>
        public static Vector2[] GetTileUVs( int type, int[] nType = null )
        {
            string key = XMLLoader.Tile[type].name;
                        
            if ( type != nType[Due.N]
              && type != nType[Due.E]
              && type != nType[Due.S]
              && type != nType[Due.W]
              && type != nType[Due.NE]
              && type != nType[Due.SE]
              && type != nType[Due.SW]
              && type != nType[Due.NW] )
                
                return TileUVDirectory[key + "_16"];
            
            return TileUVDictionary[key];
        }
    }
}
