using Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReldawinServerMaster
{
    class NetworkWorld
    {
        public static void HandleSpawnRequest( int index, PacketBuffer buffer )
        {
            int playerID = buffer.ReadInteger();
            Vector2Int coordinates = SQLReader.GetEntityCoordinates( playerID );

            //redundant
            ServerTCP.InitializeClient( index, coordinates, playerID );
            ServerTCP.SendCoordinatesOnDatabase( index );
        }
    }
}
