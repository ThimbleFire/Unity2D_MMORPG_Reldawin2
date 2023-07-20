using Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReldawinServerMaster
{
    class DoodadLoader
    {
        public static void HandleLoadDoodadsQuery( int index, PacketBuffer buffer )
        {
            int chunkX = buffer.ReadInteger();
            int chunkY = buffer.ReadInteger();

            ServerTCP.SendChunkDoodadsToPlayer( index, chunkX, chunkY );
        }
    }
}
