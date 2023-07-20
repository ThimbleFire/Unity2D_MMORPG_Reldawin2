using Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReldawinServerMaster
{
    class ChunkLoader
    {
        public static void HandleLoadChunkQuery( int index, PacketBuffer buffer )
        {
            int chunkX = buffer.ReadInteger();
            int chunkY = buffer.ReadInteger();

            ServerTCP.SendChunkDataToPlayer( index, chunkX, chunkY );
        }
    }
}
