using System;
using Bindings;

namespace ReldawinServerMaster
{
    class LocalPlayerCharacter
    {
        public static void StartInteract( int index, PacketBuffer buffer )
        {
            int tileX = buffer.ReadInteger();
            int tileY = buffer.ReadInteger();
            int id = buffer.ReadInteger();

            ServerTCP.SubscribeToInteract( index, tileX, tileY, id );
        }

        public static void StopInteract( int index, PacketBuffer buffer )
        {
            //the local player knows they need to stop interacting. Unsubscribe them from the interact thread.
            ServerTCP.UnsubscribeFromInteract( index );

            //tell OPCs that user has interrupted their behaviour
            ServerTCP.Interrupt( index, false );
        }
        
        public static void HandleMoveQuery( int index, PacketBuffer buffer )
        {
            float pointX = buffer.ReadFloat();
            float pointY = buffer.ReadFloat();
            int ID = buffer.ReadInteger();

            ServerTCP.SendMoveQueryToAllPlayers( index, ID, pointX, pointY );
        }
        
        public static void ToggleRunning(int index, PacketBuffer buffer)
        {
            int ID = buffer.ReadInteger();
            
            ServerTCP.SendToggleRunningToAllPlayers(index, ID);
        }
    }
}
