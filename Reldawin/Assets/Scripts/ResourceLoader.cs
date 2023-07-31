using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEditor.Progress;

namespace AlwaysEast
{
    public class ResourceRepository
    {
        // tileTypes stored in a dictionary
        public static Dictionary<char, List<TileBase>> keyValuePairs = new();

        public static Dictionary<int, Sprite> sprites = new Dictionary<int, Sprite>();
        public static Texture2D map;

        public static TileBase GetTilebaseOfType( char v ) {
            return keyValuePairs[v][UnityEngine.Random.Range( 0, keyValuePairs[v].Count )];
        }

        public static Sprite GetSprite( int i ) {
            return sprites[i];
        }

        public static Tilemap tilemap;
    }

    public class ResourceLoader : MonoBehaviour
    {
        public Tile[] _tileTypes;
        public Sprite[] _sprites;
        public Tilemap tileMap;

        private void Awake() {
            foreach( Tile t in _tileTypes )
                ResourceRepository.keyValuePairs.Add( t.associatedCharacter, t.tileBases );
            for( int i = 0; i < _sprites.Length; i++ )
                ResourceRepository.sprites.Add( i, _sprites[i] );
            ResourceRepository.tilemap = tileMap;
            Destroy( this );
        }
    }
}