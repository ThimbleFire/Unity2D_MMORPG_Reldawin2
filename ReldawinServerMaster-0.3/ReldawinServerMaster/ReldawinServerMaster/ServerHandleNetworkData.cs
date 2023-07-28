using Bindings;
using System;
using System.Collections.Generic;

namespace ReldawinServerMaster
{
    internal class ServerHandleNetworkData
    {
        private static Dictionary<int, Packet_> packets;
        private delegate void Packet_( int index, PacketBuffer buffer );

        public static void InitializeNetworkPackages()
        {
            Console.WriteLine( "[ServerHandleNetworkData] Initialize Network Packages" );
            packets = new Dictionary<int, Packet_>
            {
                { (int)Packet.ConnectionOK, HandleOnUserConnect },
                { (int)Packet.Account_Login_Query, HandleUserLoginQuery },
                { (int)Packet.RequestSpawn, NetworkWorld.HandleSpawnRequest },
                { (int)Packet.SavePositionToServer, HandlePlayerCharacterMovedPosition },
                { (int)Packet.Account_Create_Query, HandleAccountCreateQuery },
                { (int)Packet.PingTest, HandlePingTest  },
                { (int)Packet.DoesUserExist, MainMenu_CreateAccountControls.DoesUserExist },
                { (int)Packet.OtherPlayerCharacterListRequest, HandleOtherPlayerCharacterListRequest },
                { (int)Packet.Load_Chunk, ChunkLoader.HandleLoadChunkQuery },
                { (int)Packet.Load_Doodads, DoodadLoader.HandleLoadDoodadsQuery },
                { (int)Packet.AnnounceMovementToNearbyPlayers, LocalPlayerCharacter.HandleMoveQuery },
                { (int)Packet.StartInteract, LocalPlayerCharacter.StartInteract },
                { (int)Packet.StopInteract, LocalPlayerCharacter.StopInteract },
                { (int)Packet.ToggleRunning, LocalPlayerCharacter.ToggleRunning }
            };
        }

        public static void HandleNetworkInformation( int index, byte[] data )
        {
            using ( PacketBuffer buffer = new PacketBuffer() )
            {
                int packetNum = -1;

                buffer.WriteBytes( data );

                while ( buffer.GetReadPosition() < data.Length )
                {
                    packetNum = buffer.ReadInteger();
                    Console.WriteLine( $"Client::RecieveCallback ({(Packet)packetNum})" );

                    if ( packets.TryGetValue( packetNum, out Packet_ packet ) )
                    {
                        packet.Invoke( index, buffer );
                    }
                }
            }
        }

        public static void HandlePingTest( int index, PacketBuffer buffer )
        {
            ServerTCP.SendPingTest( index );
        }

        public static void HandlePlayerCharacterMovedPosition( int index, PacketBuffer buffer )
        {
            //we increase this by 1 because [zero, zero] on in-game is out of bounds but needs to be defined
            //so that tiles at the edge of the world have a tile type to blend with.

            int newPosX = buffer.ReadInteger() + 1;
            int newPosY = buffer.ReadInteger() + 1;

            ServerTCP.ChangeClientPosition( index, newPosX, newPosY );
        }

        public static void HandleOtherPlayerCharacterListRequest( int index, PacketBuffer buffer )
        {
            ServerTCP.SendOtherPlayerCharacterListRequest( index );
        }

        private static void HandleAccountCreateQuery( int index, PacketBuffer buffer )
        {
            string username = buffer.ReadString();
            string password = buffer.ReadString();

            //SQL stuff
            Console.WriteLine( Log.SERVER_CREATE_ACCOUNT_QUERY, username );

            object result = SQLReader.GetEntityId( username );

            if ( result != null )
            {
                ServerTCP.SendAccountCreateFail( index, Log.DatabaseAccountAlreadyExists );
                return;
            }

            SQLReader.CreateAccount( username, password );

            result = SQLReader.GetEntityId( username );

            // Create the newly created account entity
            int entityID = Convert.ToInt32( result );

            SQLReader.CreateEntity( 50, 50, entityID );

            ServerTCP.SendAccountCreateSuccess( index, Log.DatabaseAccountCreated );
        }

        private static void HandleOnUserConnect( int index, PacketBuffer buffer )
        {
            Console.WriteLine( "[ServerHandleNetworkData][HandleOnUserConnect] " );
        }

        private static void HandleUserLoginQuery( int index, PacketBuffer buffer )
        {
            string username = buffer.ReadString();
            string password = buffer.ReadString();
            SQLReader.GetPlayerIDAndPassword( username, out string pwordOnDB, out int id );

            if ( password == null )
            {
                ServerTCP.SendLoginFail( index, Log.DatabaseUsernameMismatch );
                return;
            }

            if ( pwordOnDB == password )
            {
                Console.WriteLine( "[ServerHandleNetworkData] " + Log.SERVER_LOGIN_SUCCESS, username );
                ServerTCP.SendLoginSuccess( index, id, username );
            }
            else
            {
                ServerTCP.SendLoginFail( index, Log.DatabasePasswordMismatch );
            }
        }
    }
}
