/*   Reldawin/Assets/Scripts/World.cs
/    Author: Tony Boothroyd
/    Created: 21/07/2023
/    Description:
*/
// https://stackoverflow.com/questions/1940165/how-to-access-to-the-parent-object-in-c-sharp
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
namespace AlwaysEast
{
    public class World : SceneBehaviour
    {
        public static event ClickAction OnClicked;
        public delegate void ClickAction( Vector3Int cellClicked, Vector2 pointClicked );
        public const int Width = 1000;
        public const int Height = 1000;
        public Tilemap tileMap;
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
            EventProcessor.AddInstructionParams( Packet.Load_Chunk, ReceivedChunkDataCallback );
            EventProcessor.AddInstructionParams( Packet.RequestSpawn, ReceivedSpawnCoordinatesCallback );
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
                Vector3Int coordinate = tileMap.WorldToCell(mouseWorldPos);
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
                Debug.LogError( "Attempting to get a chunk that does not exist" );
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

            Vector3Int offset = lpc.InCurrentChunk;
            if(offset.x > 0) --offset.x;
            if(offset.y > 0) --offset.y;
            
            Pathfinder.Populate( activeChunks, offset );
        }
        private void LocalPlayerChangedChunk( Vector3Int lastChunk, Vector3Int newChunk ) {
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
        private void ReceivedChunkDataCallback( params object[] args ) {
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
            if( chunksToLoad <= 0 )
                UpdateTilemap();
        }
        private void ReceivedSpawnCoordinatesCallback( object[] args ) {
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
