/*   Reldawin-0.3::World.cs
/    Author: Tony Boothroyd
/    Created: 21/07/2023
/    Description: 
*/

using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using Color = UnityEngine.Color;

namespace AlwaysEast
{

    [Serializable]
    public class Tile
    {
        public Color color;
        public TileBase tileBase;

        public const float Width = 64;
        public const float Height = 32;
    }
    public class Chunk
    {
        public event OnIndexChangeHandler OnIndexChanged;
        public delegate void OnIndexChangeHandler( Vector3Int index );
        
        public const int width = 16;
        public const int height = 16;

        public Vector3Int Index { get; set; }
        public Node[,] Nodes { get; set; } = new Node[width, height];

        public Chunk() {
            for( int y = 0; y < height; y++ )
            for( int x = 0; x <  width; x++ )
                Nodes[x, y] = new Node( new Vector3Int( x, y), OnIndexChanged );
        }
        
        public void Reload(Vector3Int index) {
            this.Index = index;
            OnIndexChanged?.Invoke(Index);
        }
    }
    public class ResourceRepository
    {
        // tileTypes stored in a dictionary
        public static Dictionary<string, TileBase> keyValuePairs = new();
        public static Texture2D map;
        public static TileBase GetTileAt(int x, int y) {
            return keyValuePairs[map.GetPixel( x, y ).ToHexString().Substring( 0, 6 )];
        }
    }

    public class World : MonoBehaviour
    {
        public static event ClickAction OnClicked;
        public delegate void ClickAction( Vector3Int cellClicked, Vector2 pointClicked );
        
        public Tilemap tileMap;
        public static Tilemap gTileMap;
        public Tile[] tileTypes;
        public Grid grid;

        public List<Chunk> activeChunks = new List<Chunk>();
        public List<Chunk> inactiveChunks = new List<Chunk>();
        private Dictionary<Vector3Int, Chunk> chunkLookup = new();

        public LocalPlayerCharacter lpc;

        private void Awake() {
            //catalogue tileTypes in the form of a dictionary so we can access them easily
            foreach( Tile t in tileTypes )
                ResourceRepository.keyValuePairs.Add( t.color.ToHexString().Substring( 0, 6 ), t.tileBase );
            ResourceRepository.map = this.map;
            gTileMap = tileMap;

            for( int y = 0; y < 12; y++ ) {
                inactiveChunks.Add( new Chunk() );
            }
        }

        private void Start() {
            // Create the starting chunks the player spawns in
            CreateChunk( lpc.InCurrentChunk );
            foreach( Vector3Int neighbour in lpc.GetSurroundingChunks )
                CreateChunk( neighbour );

            UpdateTilemap();

            LocalPlayerCharacter.LPCOnChunkChange += LocalPlayerCharacter_LPCOnChunkChange;
        }

        public void Update() {

            if( EventSystem.current.IsPointerOverGameObject() )
                return;

            if( Input.GetMouseButtonDown( 0 ) ) {

                Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint( Input.mousePosition );

                Vector3Int coordinate = grid.WorldToCell(mouseWorldPos);

                Debug.Log( coordinate );

                OnClicked?.Invoke( coordinate, Vector2.zero );
            }
        }

        private void CreateChunk( Vector3Int chunkIndex ) {

            if( chunkLookup.ContainsKey( chunkIndex ) )
                return;

            if( IsChunkOutOfBounds( chunkIndex ) )
                return;

            Chunk newChunk = inactiveChunks[0];
            inactiveChunks.Remove( inactiveChunks[0] );
            activeChunks.Add( newChunk );
            chunkLookup.Add( chunkIndex, newChunk );

            ///////////////////////////////////////////////////////

            //Send a message to the server to get scene object data

            ///////////////////////////////////////////////////////

            newChunk.Reload( tileMap, chunkIndex );
        }

        private void RemoveChunk( Vector3Int chunkIndex ) {

            if( IsChunkOutOfBounds( chunkIndex ) )
                return;

            bool result = chunkLookup.TryGetValue( chunkIndex, out Chunk chunk );

            if( result == false )
                return;

            inactiveChunks.Add( chunk );
            activeChunks.Remove( chunk );
            chunkLookup.Remove( chunkIndex );

            // remove tiles that are out of bounds
            for( int y = Chunk.height * chunkIndex.y; y < Chunk.height * ( chunkIndex.y + 1 ); y++ ) {
                for( int x = Chunk.width * chunkIndex.x; x < Chunk.width * ( chunkIndex.x + 1 ); x++ ) {
                    tileMap.SetTile( new Vector3Int( y, -x ), null );
                }
            }
        }

        private bool IsChunkOutOfBounds( Vector3Int chunkIndex ) {
            if( chunkIndex.x < 0 ||
                 chunkIndex.y < 0 ||
                 chunkIndex.x > map.width / Chunk.width ||
                 chunkIndex.y > map.height / Chunk.height )
                return true;
            else return false;
        }

        private void UpdateTilemap() {
            tileMap.CompressBounds();

            // Update Pathfinding
            Pathfinder.Populate( activeChunks, lpc.GetSurroundingChunks[0].Index );

            BoxCollider2D collider = GetComponent<BoxCollider2D>();
            collider.size = new Vector3( tileMap.size.x * grid.cellSize.x, tileMap.size.y * grid.cellSize.y );
            collider.offset = new Vector2( tileMap.size.x * grid.cellSize.x / 2, 0.0f );
        }

        private void LocalPlayerCharacter_LPCOnChunkChange( Vector3Int lastChunk, Vector3Int newChunk )
        {
            using( DebugTimer timer = new DebugTimer( $"Loading Chunks" ) ) 
            { 
                Vector3Int dirOfTravel = newChunk - lastChunk;

                for( int i = -1; i <= 1; i++ ) {
                    var dirTravelX = dirOfTravel.x == 0 ? i : dirOfTravel.x;
                    var dirTravelY = dirOfTravel.y == 0 ? i : dirOfTravel.y;

                    Vector3Int createChunkIndex = new Vector3Int( newChunk.x + dirTravelX, newChunk.y + dirTravelY );
                    Vector3Int removeChunkIndex = new Vector3Int( lastChunk.x - dirTravelX, lastChunk.y - dirTravelY );

                    RemoveChunk( removeChunkIndex );
                    CreateChunk( createChunkIndex );
                }
            }

            UpdateTilemap();
        }

        [SerializeField]
        private Texture2D map;
    }
}
