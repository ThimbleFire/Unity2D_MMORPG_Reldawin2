using System.Collections.Generic;
using UnityEngine;

namespace LowCloud.Reldawin
{
    public class DoodadLoader : SceneBehaviour
    {
        public List<Doodad> inactiveDoodads = new List<Doodad>();
        public ChunkLoader chunkLoader;

        public int chunksToLoad = 0;

        protected override void Awake()
        {
            base.Awake();

            EventProcessor.AddInstructionParams( Packet.Load_Doodads, HandleDoodadData );
            //EventProcessor.AddInstructionParams( Packet.Finished_Loading_Doodads, HandleSendChunksFinishedLoading );

            Chunk.OnChunkDestroyed += Chunk_OnChunkDestroyed;
        }

        private void HandleSendChunksFinishedLoading( params object[] args )
        {
            Pathfinder.Populate();
        }

        private void HandleDoodadData( object[] obj )
        {
            // get the chunk index
            Vector2Int chunkIndex = (Vector2Int)obj[0];

            // get the doodads
            List<Doodad.Data> doodads = (List<Doodad.Data>)obj[1];

            Chunk chunk = chunkLoader.GetChunk( chunkIndex );

            // pass the doodads and a range of inactive doodads (gameobjects) to the chunk
            chunk.SetDoodads( doodads, inactiveDoodads.GetRange( 0, doodads.Count ) );

            // remove the range of doodads given to chunk
            inactiveDoodads.RemoveRange( 0, doodads.Count );

            chunksToLoad--;

            if ( chunksToLoad <= 0 )
            {
                Pathfinder.Populate();
            }
        }

        private void Chunk_OnChunkDestroyed( Vector2Int chunkIndex, List<Doodad> doodadsToRecycle )
        {
            foreach ( Doodad doodad in doodadsToRecycle )
            {
                doodad.transform.SetParent(transform);
            }
            
            inactiveDoodads.AddRange( doodadsToRecycle );
        }
    }
}
