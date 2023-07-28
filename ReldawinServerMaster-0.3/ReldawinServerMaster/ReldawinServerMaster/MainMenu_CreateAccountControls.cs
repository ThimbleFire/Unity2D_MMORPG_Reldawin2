using Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReldawinServerMaster
{
    class MainMenu_CreateAccountControls
    {
        public static void DoesUserExist( int index, PacketBuffer buffer )
        {
            string username = buffer.ReadString();
            object result = SQLReader.GetEntityId( username );

            ServerTCP.ReturnDoesUserExist( index, result == null ? false : true );
        }
    }
}
