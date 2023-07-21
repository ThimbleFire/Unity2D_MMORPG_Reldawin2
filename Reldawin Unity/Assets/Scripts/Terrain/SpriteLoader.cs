using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace LowCloud.Reldawin
{
    public class SpriteLoader
    {
        private enum Due { N, E, S, W, NE, SE, SW, NW };
        
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
            Sprite[] sprites = Resources.LoadAll<Sprite>( "Sprites/Interface/Items/items_32x32" );

            foreach ( Sprite s in sprites )
            {
                itemDictionary.Add( s.name, s );
            }            
        }
        
        private static void SetupDoodads()
        {
            doodadDictionary = new Dictionary<string, Sprite>();
            Sprite[] sprites = Resources.LoadAll<Sprite>( "Sprites/Enviroment/Terrain/terrainDetails" );

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
        public static Vector2[] GetTileUVs( int type, byte[] nType = null )
        {
            string key = XMLLoader.Tile[type].name;

            if ( type == nType[(byte)Due.N]
              && type == nType[(byte)Due.E]
              && type == nType[(byte)Due.S]
              && type == nType[(byte)Due.W]
              && type == nType[(byte)Due.NE]
              && type == nType[(byte)Due.SE]
              && type != nType[(byte)Due.NW]
              && type != nType[(byte)Due.SW] )

                return TileUVDictionary[key + "_58"];

            if ( type == nType[(byte)Due.N]
              && type == nType[(byte)Due.E]
              && type == nType[(byte)Due.S]
              && type == nType[(byte)Due.W]
              && type == nType[(byte)Due.NE]
              && type != nType[(byte)Due.SE]
              && type == nType[(byte)Due.NW]
              && type != nType[(byte)Due.SW] )

                return TileUVDictionary[key + "_59"];

            if ( type == nType[(byte)Due.N]
              && type == nType[(byte)Due.E]
              && type == nType[(byte)Due.S]
              && type == nType[(byte)Due.W]
              && type != nType[(byte)Due.NE]
              && type != nType[(byte)Due.SE]
              && type == nType[(byte)Due.NW]
              && type == nType[(byte)Due.SW] )

                return TileUVDictionary[key + "_60"];

            if ( type == nType[(byte)Due.N]
              && type == nType[(byte)Due.E]
              && type == nType[(byte)Due.S]
              && type == nType[(byte)Due.W]
              && type != nType[(byte)Due.NE]
              && type == nType[(byte)Due.SE]
              && type != nType[(byte)Due.NW]
              && type == nType[(byte)Due.SW] )

                return TileUVDictionary[key + "_61"];

            if ( type != nType[(byte)Due.N]
              && type != nType[(byte)Due.E]
              && type != nType[(byte)Due.S]
              && type != nType[(byte)Due.W]
              && type != nType[(byte)Due.NE]
              && type != nType[(byte)Due.SE]
              && type != nType[(byte)Due.SW]
              && type != nType[(byte)Due.NW] )
                
                return TileUVDictionary[key + "_16"];

            if ( type == nType[(byte)Due.N]
              && type == nType[(byte)Due.E]
              && type == nType[(byte)Due.S]
              && type != nType[(byte)Due.NE]
              && type != nType[(byte)Due.NE]
              && type != nType[(byte)Due.W] )

                return TileUVDictionary[key + "_35"];

            if ( type == nType[(byte)Due.N]
              && type == nType[(byte)Due.W]
              && type == nType[(byte)Due.S]
              && type != nType[(byte)Due.NW]
              && type != nType[(byte)Due.SW]
              && type != nType[(byte)Due.E] )

                return TileUVDictionary[key + "_36"];

            if ( type == nType[(byte)Due.N]
              && type == nType[(byte)Due.W]
              && type == nType[(byte)Due.E]
              && type != nType[(byte)Due.NE]
              && type != nType[(byte)Due.NW]
              && type != nType[(byte)Due.S] )

                return TileUVDictionary[key + "_37"];

            if ( type == nType[(byte)Due.E]
              && type == nType[(byte)Due.S]
              && type == nType[(byte)Due.W]
              && type != nType[(byte)Due.SE]
              && type != nType[(byte)Due.SW]
              && type != nType[(byte)Due.N] )

                return TileUVDictionary[key + "_38"];

            if ( type == nType[(byte)Due.N]
              && type == nType[(byte)Due.E]
              && type == nType[(byte)Due.S]
              && type == nType[(byte)Due.W]
              && type != nType[(byte)Due.NE]
              && type != nType[(byte)Due.SE]
              && type != nType[(byte)Due.SW]
              && type != nType[(byte)Due.NW] )

                return TileUVDictionary[key + "_39"];

            if ( type == nType[(byte)Due.N]
              && type == nType[(byte)Due.E]
              && type == nType[(byte)Due.S]
              && type != nType[(byte)Due.W]
              && type == nType[(byte)Due.NE]
              && type != nType[(byte)Due.SE] )

                return TileUVDictionary[key + "_40"];

            if ( type != nType[(byte)Due.N]
              && type == nType[(byte)Due.E]
              && type == nType[(byte)Due.S]
              && type == nType[(byte)Due.W]
              && type == nType[(byte)Due.SW]
              && type != nType[(byte)Due.SE] )

                return TileUVDictionary[key + "_41"];

            if ( type != nType[(byte)Due.E]
              && type == nType[(byte)Due.N]
              && type == nType[(byte)Due.S]
              && type == nType[(byte)Due.W]
              && type == nType[(byte)Due.SW]
              && type != nType[(byte)Due.NW] )

                return TileUVDictionary[key + "_42"];

            if ( type != nType[(byte)Due.S]
              && type == nType[(byte)Due.N]
              && type == nType[(byte)Due.E]
              && type == nType[(byte)Due.W]
              && type == nType[(byte)Due.NE]
              && type != nType[(byte)Due.NW] )

                return TileUVDictionary[key + "_43"];

            if ( type != nType[(byte)Due.S]
              && type == nType[(byte)Due.N]
              && type == nType[(byte)Due.W]
              && type == nType[(byte)Due.E]
              && type == nType[(byte)Due.NW]
              && type != nType[(byte)Due.NE] )

                return TileUVDictionary[key + "_44"];

            if ( type != nType[(byte)Due.N]
              && type != nType[(byte)Due.SW]
              && type == nType[(byte)Due.E]
              && type == nType[(byte)Due.S]
              && type == nType[(byte)Due.W]
              && type == nType[(byte)Due.SE] )

                return TileUVDictionary[key + "_45"];

            if ( type == nType[(byte)Due.N]
              && type == nType[(byte)Due.E]
              && type == nType[(byte)Due.S]
              && type == nType[(byte)Due.SE]
              && type != nType[(byte)Due.W]
              && type != nType[(byte)Due.NE] )

                return TileUVDictionary[key + "_46"];

            if ( type == nType[(byte)Due.N]
              && type == nType[(byte)Due.W]
              && type == nType[(byte)Due.S]
              && type == nType[(byte)Due.NW]
              && type != nType[(byte)Due.E]
              && type != nType[(byte)Due.SW] )

                return TileUVDictionary[key + "_47"];

            if ( type == nType[(byte)Due.N]
              && type == nType[(byte)Due.S]
              && type != nType[(byte)Due.E]
              && type != nType[(byte)Due.W] )
                
                return TileUVDictionary[key + "_17"];
                        
            if ( type == nType[(byte)Due.E]
              && type == nType[(byte)Due.W]
              && type != nType[(byte)Due.N]
              && type != nType[(byte)Due.S] )
                
                return TileUVDictionary[key + "_18"];
                        
            if ( type == nType[(byte)Due.E]
              && type == nType[(byte)Due.S]
              && type == nType[(byte)Due.SE]
              && type != nType[(byte)Due.N]
              && type != nType[(byte)Due.W] )
                
                return TileUVDictionary[key + "_19"];
                        
            if ( type == nType[(byte)Due.N]
              && type == nType[(byte)Due.E]
              && type == nType[(byte)Due.NE]
              && type != nType[(byte)Due.W]
              && type != nType[(byte)Due.S] )
                
                return TileUVDictionary[key + "_20"];
                        
            if ( type == nType[(byte)Due.N]
              && type == nType[(byte)Due.W]
              && type == nType[(byte)Due.NW]
              && type != nType[(byte)Due.S]
              && type != nType[(byte)Due.E] )
                
                return TileUVDictionary[key + "_21"];
                        
            if ( type == nType[(byte)Due.S]
              && type == nType[(byte)Due.W]
              && type == nType[(byte)Due.SW]
              && type != nType[(byte)Due.N]
              && type != nType[(byte)Due.E] )
                
                return TileUVDictionary[key + "_22"];
                        
            if ( type == nType[(byte)Due.S]
              && type != nType[(byte)Due.E]
              && type != nType[(byte)Due.W] )
                
                return TileUVDictionary[key + "_23"];
                        
            if ( type == nType[(byte)Due.W]
              && type != nType[(byte)Due.N]
              && type != nType[(byte)Due.S] )
                
                return TileUVDictionary[key + "_24"];
                        
            if ( type == nType[(byte)Due.N]
              && type != nType[(byte)Due.E]
              && type != nType[(byte)Due.W] )
                
                return TileUVDictionary[key + "_25"];
                        
            if ( type == nType[(byte)Due.E]
              && type != nType[(byte)Due.N]
              && type != nType[(byte)Due.S] )
                
                return TileUVDictionary[key + "_26"];
                        
            if ( type == nType[(byte)Due.N]
              && type == nType[(byte)Due.W]
              && type != nType[(byte)Due.E]
              && type != nType[(byte)Due.S]
              && type != nType[(byte)Due.NW] )
                
                return TileUVDictionary[key + "_27"];
                        
            if ( type == nType[(byte)Due.E]
              && type == nType[(byte)Due.S]
              && type != nType[(byte)Due.N]
              && type != nType[(byte)Due.W]
              && type != nType[(byte)Due.SE] )
                
                return TileUVDictionary[key + "_28"];
                        
            if ( type == nType[(byte)Due.W]
              && type == nType[(byte)Due.S]
              && type != nType[(byte)Due.N]
              && type != nType[(byte)Due.E]
              && type != nType[(byte)Due.SW] )
                
                return TileUVDictionary[key + "_29"];
                        
            if ( type == nType[(byte)Due.N]
              && type == nType[(byte)Due.E]
              && type != nType[(byte)Due.S]
              && type != nType[(byte)Due.W]
              && type != nType[(byte)Due.NE] )
                
                return TileUVDictionary[key + "_30"];
                        
            if ( type == nType[(byte)Due.N]
              && type == nType[(byte)Due.E]
              && type == nType[(byte)Due.S]
              && type != nType[(byte)Due.W] )
                
                return TileUVDictionary[key + "_31"];
                        
            if ( type == nType[(byte)Due.E]
              && type == nType[(byte)Due.S]
              && type == nType[(byte)Due.W]
              && type != nType[(byte)Due.N] )
                
                return TileUVDictionary[key + "_32"];
                        
            if ( type == nType[(byte)Due.N]
              && type == nType[(byte)Due.S]
              && type == nType[(byte)Due.W]
              && type != nType[(byte)Due.E] )
                
                return TileUVDictionary[key + "_33"];
                        
            if ( type == nType[(byte)Due.N]
              && type == nType[(byte)Due.E]
              && type == nType[(byte)Due.W]
              && type != nType[(byte)Due.S] )
                
                return TileUVDictionary[key + "_34"];

            if ( type == nType[(byte)Due.N]
              && type == nType[(byte)Due.E]
              && type == nType[(byte)Due.S]
              && type == nType[(byte)Due.W]
              && type == nType[(byte)Due.NE]
              && type != nType[(byte)Due.SE]
              && type != nType[(byte)Due.SW]
              && type != nType[(byte)Due.NW] )

                return TileUVDictionary[key + "_48"];

            if ( type == nType[(byte)Due.N]
              && type == nType[(byte)Due.E]
              && type == nType[(byte)Due.S]
              && type == nType[(byte)Due.W]
              && type != nType[(byte)Due.NE]
              && type != nType[(byte)Due.SE]
              && type == nType[(byte)Due.SW]
              && type != nType[(byte)Due.NW] )

                return TileUVDictionary[key + "_49"];

            if ( type == nType[(byte)Due.N]
              && type == nType[(byte)Due.E]
              && type == nType[(byte)Due.S]
              && type == nType[(byte)Due.W]
              && type != nType[(byte)Due.NE]
              && type != nType[(byte)Due.SE]
              && type != nType[(byte)Due.SW]
              && type == nType[(byte)Due.NW] )

                return TileUVDictionary[key + "_50"];

            if ( type == nType[(byte)Due.N]
              && type == nType[(byte)Due.E]
              && type == nType[(byte)Due.S]
              && type == nType[(byte)Due.W]
              && type != nType[(byte)Due.NE]
              && type == nType[(byte)Due.SE]
              && type != nType[(byte)Due.SW]
              && type != nType[(byte)Due.NW] )

                return TileUVDictionary[key + "_51"];

            if ( type == nType[(byte)Due.N]
              && type == nType[(byte)Due.E]
              && type == nType[(byte)Due.S]
              && type == nType[(byte)Due.W]
              && type != nType[(byte)Due.NW] )

                return TileUVDictionary[key + "_54"];

            if ( type == nType[(byte)Due.N]
              && type == nType[(byte)Due.E]
              && type == nType[(byte)Due.S]
              && type == nType[(byte)Due.W]
              && type != nType[(byte)Due.NE] )

                return TileUVDictionary[key + "_55"];

            if ( type == nType[(byte)Due.N]
              && type == nType[(byte)Due.E]
              && type == nType[(byte)Due.S]
              && type == nType[(byte)Due.W]
              && type != nType[(byte)Due.SE] )

                return TileUVDictionary[key + "_56"];

            if ( type == nType[(byte)Due.N]
              && type == nType[(byte)Due.E]
              && type == nType[(byte)Due.S]
              && type == nType[(byte)Due.W]
              && type != nType[(byte)Due.SW])

                return TileUVDictionary[key + "_57"];

            if ( type == nType[(byte)Due.N]
              && type == nType[(byte)Due.E]
              && type == nType[(byte)Due.S]
              && type == nType[(byte)Due.W]
              && type == nType[(byte)Due.NE]
              && type != nType[(byte)Due.SE]
              && type == nType[(byte)Due.SW]
              && type != nType[(byte)Due.NW] )

                return TileUVDictionary[key + "_52"];

            if ( type == nType[(byte)Due.N]
              && type == nType[(byte)Due.E]
              && type == nType[(byte)Due.S]
              && type == nType[(byte)Due.W]
              && type == nType[(byte)Due.NW]
              && type == nType[(byte)Due.SE]
              && type != nType[(byte)Due.SW]
              && type != nType[(byte)Due.NE] )

                return TileUVDictionary[key + "_53"];

            return TileUVDictionary["Void"];
        }
    
        public static Vector2[] GetTileUVs(string name) {

            int index = Random.Range( 0, 16 );

            if( TileUVDictionary.ContainsKey( name + "_" + index ) ) {
                return TileUVDictionary[name + "_" + index];
            } else
                return TileUVDictionary["Empty"];

        }
    }
}
