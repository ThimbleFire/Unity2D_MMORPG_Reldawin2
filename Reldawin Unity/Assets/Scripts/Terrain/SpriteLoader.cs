using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

namespace LowCloud.Reldawin
{
    public class SpriteLoader
    {
        // remember to rename doodads after sprite names in atlas.
        public static Dictionary<string, Vector2[]> TileUVDictionary;
        public static Dictionary<string, Sprite> doodadDictionary;
        public static Dictionary<string, Sprite> itemDictionary;

        /// Width and Height specify the dimensions of Template2.png
        public static void Setup( int spriteMapWidth, int spriteMapHeight )
        {
            TileUVDictionary = new Dictionary<string, Vector2[]>();
            doodadDictionary = new Dictionary<string, Sprite>();
            itemDictionary = new Dictionary<string, Sprite>();

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

        //Eventually we shouldn't need to pass neighbours, just the key. Instead of calling GetNeighbours we can
        //pass the tile.Type and if the neighbour shares the same type we can return a bool and add _N, _E, _S, etc. Great work!
        public static Vector2[] GetTileUVs( int type, Tile[] neighbours = null )
        {
            if ( type == 0 )
                return TileUVDictionary["Empty"];

            if ( neighbours == null )
                return TileUVDictionary[XMLLoader.Tile[type].name + "_" + Random.Range( 0, 16 )];

            string key = XMLLoader.Tile[type].name;

            if ( type != neighbours[0].TileType
              && type != neighbours[2].TileType
              && type != neighbours[1].TileType
              && type != neighbours[3].TileType )
            {
                key += "_18";
                return TileUVDictionary[key];
            }
            if ( type != neighbours[0].TileType
              && type != neighbours[1].TileType
              && type != neighbours[2].TileType
              && type == neighbours[3].TileType )
            {
                key += "_24";
                return TileUVDictionary[key];
            }
            if ( type == neighbours[0].TileType
           && type != neighbours[1].TileType
           && type != neighbours[2].TileType
           && type != neighbours[3].TileType )
            {
                key += "_25";
                return TileUVDictionary[key];
            }
            if ( type == neighbours[2].TileType
             && type != neighbours[5].TileType
             && type != neighbours[6].TileType )
            {
                key += "_59";
                return TileUVDictionary[key];
            }
            if ( type == neighbours[0].TileType
             && type == neighbours[1].TileType
             && type == neighbours[2].TileType
             && type == neighbours[3].TileType
             && type != neighbours[4].TileType
             && type != neighbours[5].TileType )
            {
                key += "_60";
                return TileUVDictionary[key];
            }
            if ( type != neighbours[3].TileType && type != neighbours[0].TileType )
            {
                key += "_19";
                return TileUVDictionary[key];
            }
            if ( type != neighbours[2].TileType && type != neighbours[3].TileType && type != neighbours[6].TileType )
            {
                key += "_20";
                return TileUVDictionary[key];
            }
            if ( type != neighbours[1].TileType && type != neighbours[2].TileType )
            {
                key += "_21";
                return TileUVDictionary[key];
            }
            if ( type != neighbours[0].TileType && type != neighbours[1].TileType )
            {
                key += "_22";
                return TileUVDictionary[key];
            }
            if ( type == neighbours[2].TileType && type == neighbours[3].TileType && type != neighbours[6].TileType )
            {
                key += "_57";
                return TileUVDictionary[key];
            }
            if ( type == neighbours[1].TileType && type == neighbours[2].TileType && type != neighbours[5].TileType )
            {
                key += "_56";
                return TileUVDictionary[key];
            }
            if ( type == neighbours[0].TileType && type == neighbours[3].TileType && type != neighbours[7].TileType )
            {
                key += "_54";
                return TileUVDictionary[key];
            }
            if ( type == neighbours[0].TileType && type == neighbours[1].TileType && type != neighbours[4].TileType )
            {
                key += "_55";
                return TileUVDictionary[key];
            }
            if ( type != neighbours[3].TileType )
            {
                key += "_31";
                return TileUVDictionary[key];
            }
            if ( type != neighbours[0].TileType )
            {
                key += "_32";
                return TileUVDictionary[key];
            }
            if ( type != neighbours[1].TileType )
            {
                key += "_33";
                return TileUVDictionary[key];
            }
            if ( type != neighbours[2].TileType )
            {
                key += "_34";
                return TileUVDictionary[key];
            }

            return TileUVDictionary[key];
        }
    }
}
