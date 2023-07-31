/*   Reldawin-0.3::World.cs
/    Author: Tony Boothroyd
/    Created: 21/07/2023
/    Description: 
*/

// https://stackoverflow.com/questions/1940165/how-to-access-to-the-parent-object-in-c-sharp

using System;
using System.Collections.Generic;
using System.Xml;
using Unity.Mathematics;
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
        public char associatedCharacter;
        public List<TileBase> tileBases;

        public const byte Width = 64;
        public const byte Height = 31;
    }
    public interface IChunk { Vector3Int _index { get; } }
    public class Chunk : IChunk
    {
        public static event ChunkDestroyImminent OnChunkDestroyed;
        public delegate void ChunkDestroyImminent( Vector3Int chunkIndex, List<SceneObject> sceneObjects );

        public const byte width = 20;
        public const byte height = 20;
        public Vector3Int Index { get; set; }
        public Vector3Int _index { get { return Index; } }
        public Node[,] Nodes { get; set; } = new Node[width, height];
        public List<SceneObject> activeSceneObjects = new List<SceneObject>();
        
        public Chunk() {
            for( int y = 0; y < height; y++ )
            for( int x = 0; x <  width; x++ )
                Nodes[x, y] = new Node( this, new Vector3Int( x, y) );
        }
        public void Erase(Tilemap tileMap) {
            // Is erasing tiles neccesary? They'll just be overwritten when Reload is called.
            //for( int y = 0; y < height; y++ )
            //for( int x = 0; x < width; x++ )
            //    tileMap.SetTile( Nodes[x, y].CellPositionInWorld, null );
            activeSceneObjects.ForEach( x => x.gameObject.SetActive( false ) );
            OnChunkDestroyed?.Invoke( Index, activeSceneObjects );
            activeSceneObjects.Clear();
        }
        public void Reload( Tilemap tileMap, Vector3Int index, string data, List<SceneObjectData> sceneObjectData, List<SceneObject> inactiveSceneObjects ) {
            Index = index;
            int iteration = 0;
            for( int y = 0; y < height; y++ )
            for( int x = 0; x < width; x++ ) {
                tileMap.SetTile(
                    Nodes[x, y].CellPositionInWorld, 
                    ResourceRepository.GetTilebaseOfType( data[iteration++] ) 
                    );
            }
            foreach( SceneObjectData objectData in sceneObjectData ) {
                Vector3 worldPosition = Nodes[objectData.x - ( index.x * Chunk.width ), objectData.y - ( index.y * Chunk.height )].WorldPosition;
                inactiveSceneObjects[0].Setup( ResourceRepository.GetSprite( objectData.Type ) );
                inactiveSceneObjects[0].transform.position = worldPosition;
                activeSceneObjects.Add( inactiveSceneObjects[0] );
                inactiveSceneObjects.RemoveAt( 0 );
            }
        }
    }

    public class World : SceneBehaviour
    {
        public static event ClickAction OnClicked;
        public delegate void ClickAction( Vector3Int cellClicked, Vector2 pointClicked );

        public const int Width = 1000;
        public const int Height = 1000;
        public Tilemap tileMap;
        public Grid grid;
        private List<Chunk> activeChunks = new List<Chunk>();
        private List<Chunk> inactiveChunks = new List<Chunk>();
        public List<SceneObject> inactiveSceneObjects = new List<SceneObject>();
        private Dictionary<Vector3Int, Chunk> chunkLookup = new();
        public LocalPlayerCharacter lpc;
        private byte chunksToLoad = 0;

        private void Awake() {
            //Maybe consider making chunks actual gameobjects? 
            for( int y = 0; y < 12; y++ ) {
                inactiveChunks.Add( new Chunk() );
            }
            EventProcessor.AddInstructionParams( Packet.Load_Chunk, ReceivedChunkData );
            EventProcessor.AddInstructionParams( Packet.RequestSpawn, ReceivedSpawnCoordinates );
            Chunk.OnChunkDestroyed += OnChunkDestroyed;
        }
        private void Start() {
            LocalPlayerCharacter.LPCOnChunkChange += LocalPlayerChangedChunk;
            using PacketBuffer buffer = new PacketBuffer( Packet.RequestSpawn );
            buffer.WriteInteger( Game.dbID );
            ClientTCP.SendData( buffer.ToArray() );
        }
        public void Update() {
            if( Input.GetMouseButtonDown( 0 ) ) {
                if( EventSystem.current.IsPointerOverGameObject() )
                    return;
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
            chunksToLoad++;
            ClientTCP.SendChunkDataQuery( chunkIndex );
        }
        private void RemoveChunk( Vector3Int chunkIndex ) {
            if( IsChunkOutOfBounds( chunkIndex ) )
                return;
            bool result = chunkLookup.TryGetValue( chunkIndex, out Chunk chunk );
            if( result == false ) {
                Debug.LogError("Attempting to get a chunk that does not exist");
                return;
            }
            inactiveChunks.Add( chunk );
            activeChunks.Remove( chunk );
            chunkLookup.Remove( chunkIndex );
            chunk.Erase( tileMap );
        }
        private bool IsChunkOutOfBounds( Vector3Int chunkIndex ) {
            if( chunkIndex.x < 0 || chunkIndex.y < 0 || chunkIndex.x >= World.Width / Chunk.width || chunkIndex.y >= World.Height / Chunk.height )
                return true;
            else return false;
        }
        private void UpdateTilemap() {
            tileMap.CompressBounds();
            // cheap fix, we should improve this
            // Pathfinder map { 0, 0 } starts at the bottom of the screen. We need to offset that to the bottom-most loaded chunk.
            Vector3Int offset =
                activeChunks.Find( x => x.Index == lpc.GetSurroundingChunks[0] ) != null ? lpc.GetSurroundingChunks[0] :
                activeChunks.Find( x => x.Index == lpc.GetSurroundingChunks[3] ) != null ? lpc.GetSurroundingChunks[3] :
                activeChunks.Find( x => x.Index == lpc.GetSurroundingChunks[1] ) != null ? lpc.GetSurroundingChunks[1] :
                lpc.InCurrentChunk;
            Pathfinder.Populate( activeChunks, offset );
// This might not be needed anymore. Experiment by removing the BoxCollider2D component. !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            BoxCollider2D collider = GetComponent<BoxCollider2D>();
            collider.size = new Vector3( tileMap.size.x * grid.cellSize.x, tileMap.size.y * grid.cellSize.y );
            collider.offset = new Vector2( tileMap.size.x * grid.cellSize.x / 2, 0.0f );
// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        }
        private void LocalPlayerChangedChunk( Vector3Int lastChunk, Vector3Int newChunk ) {
            using( DebugTimer timer = new DebugTimer( $"Loading Chunks" ) ) {
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
        }
        private void ReceivedChunkData(params object[] args) {
            Vector3Int chunkIndex = new Vector3Int( (int)args[0], (int)args[1] );
            string data = (string)args[2];
            Chunk newChunk = inactiveChunks[0];
            inactiveChunks.Remove( inactiveChunks[0] );
            activeChunks.Add( newChunk );
            chunkLookup.Add( chunkIndex, newChunk );
            List<SceneObjectData> objects = (List<SceneObjectData>)args[3];
            newChunk.Reload( tileMap, chunkIndex, data, objects, inactiveSceneObjects.GetRange( 0, objects.Count ) );
            inactiveSceneObjects.RemoveRange( 0, objects.Count );
            chunksToLoad--;
            if(chunksToLoad <= 0)
                UpdateTilemap();
        }
        private void ReceivedSpawnCoordinates( object[] args ) {
            EventProcessor.RemoveInstructionParams( Packet.RequestSpawn );
            Vector3Int coordinates = new Vector3Int( (int)args[0], (int)args[1] );
            // Teleport is the method for spawning the player character and should only be used as such. This is not a method for moving the entity around the game.
            lpc.Teleport( coordinates );
            // Create the starting chunks the player spawns in
            CreateChunk( lpc.InCurrentChunk );
            foreach( Vector3Int neighbour in lpc.GetSurroundingChunks )
                CreateChunk( neighbour );
        }
        private void OnChunkDestroyed( Vector3Int chunkIndex, List<SceneObject> sceneObjectsToRecycle ) {
            inactiveSceneObjects.AddRange( sceneObjectsToRecycle );
        }
    }
}
