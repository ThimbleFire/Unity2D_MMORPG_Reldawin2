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
        public static void HandleSeedRequest( int index, PacketBuffer buffer )
        {
            int playerID = buffer.ReadInteger();
            object[] result = CommonSQL.GetEntityCoordinates( playerID );
            int playerX = Convert.ToInt32( result[0] );
            int playerY = Convert.ToInt32( result[1] );

            //redundant
            ServerTCP.InitializeClient( index, playerX, playerY, playerID );
            ServerTCP.SendSeedAndStartupDetails( index, playerX, playerY );
        }
    }
}
