using System.Collections.Generic;
using UnityEngine;

namespace LowCloud.Reldawin
{
    public class ChunkLoader : SceneBehaviour
    {
        public List<Chunk> activeChunks = new List<Chunk>();
        public List<Chunk> inactiveChunks = new List<Chunk>();
        private readonly Dictionary<Vector2Int, Chunk> chunkLookup = new Dictionary<Vector2Int, Chunk>();
        [SerializeField] private  LocalPlayerCharacter localPlayerCharacter;
        public DoodadLoader doodadLoader;
        public static ChunkLoader Instance { get; set; }

        private void Awake()
        {
            Instance = this;

            base.Awake();

            EventProcessor.AddInstructionParams( Packet.Load_Chunk, HandleChunkData );
        }
        
        public Node GetTile(Vector2Int tilePositionInWorld)
        {
            int chunkX = Mathf.FloorToInt( tilePositionInWorld.x / Chunk.Size );
            int chunkY = Mathf.FloorToInt( tilePositionInWorld.y / Chunk.Size );

            Vector2Int chunkIndex = new Vector2Int( chunkX, chunkY );

            Chunk c = GetChunk( chunkIndex );
            
            int tileX = tilePositionInWorld.x % Chunk.Size;
            int tileY = tilePositionInWorld.y % Chunk.Size;

            Node n = c.Nodes[tileX, tileY];

            return n;
        }

        public void PlayerSpawnStartUp( )
        {
            CreateChunk( localPlayerCharacter.InCurrentChunk );

            Vector2Int[] neighbouringChunks = localPlayerCharacter.GetNeighbouringChunks;

            /// create neighbour chunks
            foreach ( Vector2Int neighbour in neighbouringChunks )
            {
                CreateChunk( neighbour );
            }

            LocalPlayerCharacter.OnChunkChanged += Movement_OnChunkChanged;
            Chunk.OnChunkDestroyed += Chunk_OnChunkDestroyed;
        }

        private void Chunk_OnChunkDestroyed( List<Doodad> doodads )
        {
            bool result = chunkLookup.TryGetValue( doodads[0].data.chunkIndex, out Chunk data );

            if ( result )
            {
                inactiveChunks.Add( data );
                activeChunks.Remove( data );
                chunkLookup.Remove( doodads[0].data.chunkIndex );
            }
        }

        private void CreateChunk( Vector2Int chunkIndex )
        {
            if ( chunkIndex.x < 0 || chunkIndex.x >= World.Instance.Width / Chunk.Size
              || chunkIndex.y < 0 || chunkIndex.y >= World.Instance.Height / Chunk.Size )
            {
                // chunk out of bounds
                return;
            }

            //find chunk 'x', 'y'
            bool result = chunkLookup.TryGetValue( chunkIndex, out _ );

            //if chunk 'x', 'y' already exists, eyy, forget about it...
            if ( result )
            {
                return;
            }

            ClientTCP.SendChunkDataQuery( chunkIndex );
        }

        private void HandleChunkData( params object[] args )
        {
            Vector2Int chunkIndex = new Vector2Int( (int)args[0], (int)args[1] );
            string data = (string)args[2];

            Chunk newChunk = inactiveChunks[0];
            inactiveChunks.Remove( inactiveChunks[0] );
            activeChunks.Add( newChunk );
            chunkLookup.Add( chunkIndex, newChunk );
            doodadLoader.chunksToLoad++;
            newChunk.CreateTiles( chunkIndex, data );
        }

        private void Movement_OnChunkChanged( Vector2Int newChunk, Vector2Int lastChunk )
        {
            Vector2Int dirOfTravel = newChunk - lastChunk;

            for ( int i = -1; i <= 1; i++ )
            {
                var dirTravelX = dirOfTravel.x == 0 ? i : dirOfTravel.x;
                var dirTravelY = dirOfTravel.y == 0 ? i : dirOfTravel.y;

                Vector2Int createChunkIndex = new Vector2Int( newChunk.x + dirTravelX, newChunk.y + dirTravelY );
                Vector2Int removeChunkIndex = new Vector2Int( lastChunk.x - dirTravelX, lastChunk.y - dirTravelY );

                RemoveChunk( removeChunkIndex );
                CreateChunk( createChunkIndex );
            }
        }

        private void RemoveChunk( Vector2Int chunkIndex )
        {
            if ( chunkIndex.x < 0 || chunkIndex.y < 0 )
                return;

            bool result = chunkLookup.TryGetValue( chunkIndex, out Chunk data );

            if ( result )
                data.Disable();
        }
        
        public Chunk GetChunk(Vector2Int chunkIndex, bool clamp = false)
        {
            chunkLookup.TryGetValue(chunkIndex, out Chunk data);

            return data;
        }
        
        public Chunk[,] GetChunkMap()
        {
            Vector2Int[] neighbours = localPlayerCharacter.GetNeighbouringChunks;

            return new Chunk[3, 3]
            {
                { 
                    GetChunk( neighbours[6] ),                          // [0, 0]   bottom-left
                    GetChunk( neighbours[7] ),                          // [0, 1]   left
                    GetChunk( neighbours[0] )                           // [0, 2]   top-left
                }, { 
                    GetChunk( neighbours[5] ),                          // [1, 0]   bottom
                    GetChunk( localPlayerCharacter.InCurrentChunk  ),   // [1, 1]   middle
                    GetChunk( neighbours[1] )                           // [1, 2]   top
                }, { 
                    GetChunk( neighbours[4] ),                          // [2, 0]   bottom-right
                    GetChunk( neighbours[3] ),                          // [2, 1]   right
                    GetChunk( neighbours[2] )                           // [2, 2]   top-right
                }
            };
        }

        private void OnDestroy()
        {
            LocalPlayerCharacter.OnChunkChanged -= Movement_OnChunkChanged;
            Chunk.OnChunkDestroyed -= Chunk_OnChunkDestroyed;
        }

    }
}
