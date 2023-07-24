/*   Reldawin-0.3::World.cs
/    Author: Tony Boothroyd
/    Created: 21/07/2023
/    Description: 
*/

// https://stackoverflow.com/questions/1940165/how-to-access-to-the-parent-object-in-c-sharp

using System;
using System.Collections.Generic;
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

        public const byte Width = 64;
        public const byte Height = 32;
    }
    public class Chunk
    {
        public string Name;
        public const byte width = 16;
        public const byte height = 16;

        public Vector3Int Index { get; set; }
        public Node[,] Nodes { get; set; } = new Node[width, height];
        
        public Chunk() {
            Name = "Inactive";
            for( int y = 0; y < height; y++ )
            for( int x = 0; x <  width; x++ )
                Nodes[x, y] = new Node( new Vector3Int( x, y) );
        }

        public void Erase() {
            for( int y = 0; y < height; y++ )
            for( int x = 0; x < width; x++ )
                World.gTileMap.SetTile( Nodes[x, y].CellPositionInWorld, null );
        }

        public void Reload( Vector3Int index ) {
            Name = index.ToString();
            for( int y = 0; y < height; y++ )
            for( int x = 0; x < width; x++ ) {
                    this.Index = index;
                    Nodes[x, y].ChunkIndex = Index;
                    World.gTileMap.SetTile(
                        Nodes[x, y].CellPositionInWorld, 
                        ResourceRepository.GetTileAt( Nodes[x, y].CellPositionInWorld ) 
                        );
            }
        }
    }
    public class ResourceRepository
    {
        // tileTypes stored in a dictionary
        public static Dictionary<string, TileBase> keyValuePairs = new();
        public static Texture2D map;
        public static TileBase GetTileAt(Vector3Int coords) {
            return keyValuePairs[map.GetPixel( coords.x, coords.y ).ToHexString().Substring( 0, 6 )];
        }
    }

    public class World : MonoBehaviour
    {
        public static event ClickAction OnClicked;
        public delegate void ClickAction( Vector3Int cellClicked, Vector2 pointClicked );
        
        [SerializeField]
        private Texture2D map;
        public Tilemap tileMap;
        public static Tilemap gTileMap;
        public Tile[] tileTypes;
        public Grid grid;
        private List<Chunk> activeChunks = new List<Chunk>();
        private List<Chunk> inactiveChunks = new List<Chunk>();
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

                OnClicked?.Invoke( coordinate, mouseWorldPos );
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

            newChunk.Reload( chunkIndex );
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
            chunk.Erase();
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

            // cheap fix, we should improve this
            Vector3Int offset =
                activeChunks.Find( x => x.Index == lpc.GetSurroundingChunks[0] ) != null ? lpc.GetSurroundingChunks[0] :
                activeChunks.Find( x => x.Index == lpc.GetSurroundingChunks[3] ) != null ? lpc.GetSurroundingChunks[3] :
                activeChunks.Find( x => x.Index == lpc.GetSurroundingChunks[1] ) != null ? lpc.GetSurroundingChunks[1] :
                lpc.InCurrentChunk;

            Pathfinder.Populate( activeChunks, offset );

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

            using( DebugTimer timer = new DebugTimer( $"Updating Tilemap" ) ) {
                UpdateTilemap();
            }
        }
    }
}
