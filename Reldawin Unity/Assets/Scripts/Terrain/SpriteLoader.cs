using System.Collections.Generic;
using UnityEngine;

namespace LowCloud.Reldawin
{
    public class SpriteLoader
    {
        // remember to rename doodads after sprite names in atlas.
        public static Dictionary<string, Vector2[]> tileUVMap;
        public static Dictionary<string, Sprite> doodadDictionary;
        public static Dictionary<string, Sprite> itemDictionary;

        public static Vector2[] GetEmpty
        {
            get
            {
                return tileUVMap["Empty"];
            }
        }

        /// Width and Height specify the dimensions of Template2.png
        public static void Setup( )
        {
            Sprite[] sprites;

            tileUVMap = new Dictionary<string, Vector2[]>();
            doodadDictionary = new Dictionary<string, Sprite>();
            itemDictionary = new Dictionary<string, Sprite>();

            Load( "Grass" );
            Load( "Sand" );
            Load( "Dirt" );
            Load( "Shallow_Water" );

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

            sprites = Resources.LoadAll<Sprite>( "Sprites/Interface/Items/items_32x32" );

            foreach ( Sprite s in sprites )
            {
                itemDictionary.Add( s.name, s );
            }
        }

        private static void Load(string filename)
        {
            Sprite[] sprites = Resources.LoadAll<Sprite>( "Sprites/Enviroment/Terrain/" + filename );

            foreach ( Sprite s in sprites )
            {
                float left = s.rect.x / 320;
                float right = ( s.rect.x + s.rect.width ) / 320;

                float top = s.rect.y / 320;
                float bot = ( s.rect.y + s.rect.height ) / 320;

                float middleX = ( s.rect.x + ( s.rect.width / 2 ) ) / 320;
                float middleY = ( s.rect.y + ( s.rect.height / 2 ) ) / 320;

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

                tileUVMap.Add( s.name, uvs );
            }
        }

        public static Vector2[] GetTileUVs( int type, Tile[] neighbours )
        {
            if ( type == 0 )
                return GetEmpty;

            string key = XMLLoader.GetTile( type ).name;

            key += XMLLoader.GetAtlas( type, neighbours );

            return GetTile( key );
        }

        private static bool IsSameType( int type, Tile neightbour )
        {
            if ( neightbour == null )
                return false;

            if ( neightbour.TileType == type )
                return true;

            return false;
        }

        private static Vector2[] GetTile( string key )
        {
            if ( tileUVMap.ContainsKey( key ) )
            {
                return tileUVMap[key];
            }
            else
            {
                Debug.LogError( key + " isn't in the tileUVMap dictionary" );
                return tileUVMap["Void"];
            }
        }

        public static Sprite GetDoodad( string key )
        {
            if ( doodadDictionary.ContainsKey( key ) )
            {
                return doodadDictionary[key];
            }
            else
            {
                Debug.LogError( key + " isn't in the SpriteLoader Doodad Dictionary" );
                return doodadDictionary["Empty"];
            }
        }

        public static Sprite GetItem( string key )
        {
            if ( itemDictionary.ContainsKey( key ) )
            {
                return itemDictionary[key];
            }
            else
            {
                Debug.LogError( key + " isn't in the SpriteLoader Item Dictionary" );
                return itemDictionary["FlintKnife"];
            }
        }
    }
}