using Bindings;

namespace ReldawinServerMaster
{
    internal class ServerHandleNetworkData
    {
        private static Dictionary<int, Packet_> packets;

        private delegate void Packet_( int index, PacketBuffer buffer );

        public static void HandleLoadChunkQuery( int index, PacketBuffer buffer ) {
            int chunkX = buffer.ReadInteger();
            int chunkY = buffer.ReadInteger();

            ServerTCP.SendChunkDataToPlayer( index, chunkX, chunkY );
        }

        public static void HandleNetworkInformation( int index, byte[] data, string sender ) {
            using( PacketBuffer buffer = new PacketBuffer(data) ) {
                while( buffer.GetReadPosition < data.Length ) {
                    if( packets.TryGetValue( buffer.ReadInteger(), out Packet_ packet ) )
                        packet.Invoke( index, buffer );
                }
            }
        }

        public static void HandleOtherPlayerCharacterListRequest( int index, PacketBuffer buffer ) {
            ServerTCP.SendOtherPlayerCharacterListRequest( index );
        }

        public static void HandlePingTest( int index, PacketBuffer buffer ) {
            ServerTCP.SendPingTest( index );
        }

        public static void HandlePlayerCharacterMovedPosition( int index, PacketBuffer buffer ) {
            int newPosX = buffer.ReadInteger();
            int newPosY = buffer.ReadInteger();

            ServerTCP.ChangeClientPosition( index, newPosX, newPosY );
        }

        public static void InitializeNetworkPackages() {
            Console.WriteLine( "[ServerHandleNetworkData] Initialize Network Packages" );
            packets = new Dictionary<int, Packet_>
            {
                { (int)Packet.ConnectionOK, HandleOnUserConnect },
                { (int)Packet.Account_Login_Query, HandleUserLoginQuery },
                { (int)Packet.RequestSpawn, HandleSpawnRequest },
                { (int)Packet.SavePositionToServer, HandlePlayerCharacterMovedPosition },
                { (int)Packet.Account_Create_Query, HandleAccountCreateQuery },
                { (int)Packet.PingTest, HandlePingTest  },
                { (int)Packet.DoesUserExist, DoesUserExist },
                { (int)Packet.OtherPlayerCharacterListRequest, HandleOtherPlayerCharacterListRequest },
                { (int)Packet.Load_Chunk, HandleLoadChunkQuery },
                { (int)Packet.Load_Doodads, HandleLoadDoodadsQuery },
                { (int)Packet.AnnounceMovementToNearbyPlayers, LocalPlayerCharacter.HandleMoveQuery },
                { (int)Packet.StartInteract, LocalPlayerCharacter.StartInteract },
                { (int)Packet.StopInteract, LocalPlayerCharacter.StopInteract },
                { (int)Packet.ToggleRunning, LocalPlayerCharacter.ToggleRunning }
            };
        }
        private static void DoesUserExist( int index, PacketBuffer buffer ) {
            string username = buffer.ReadString();
            object result = SQLReader.GetEntityId( username );

            ServerTCP.ReturnDoesUserExist( index, result == null ? false : true );
        }

        private static void HandleAccountCreateQuery( int index, PacketBuffer buffer ) {
            string username = buffer.ReadString();
            string password = buffer.ReadString();

            //SQL stuff
            Console.WriteLine( Log.SERVER_CREATE_ACCOUNT_QUERY, username );

            object result = SQLReader.GetEntityId( username );

            if( result != null ) {
                ServerTCP.SendAccountCreateFail( index );
                return;
            }

            SQLReader.CreateAccount( username, password );

            result = SQLReader.GetEntityId( username );

            // Create the newly created account entity
            int entityID = Convert.ToInt32( result );

            SQLReader.CreateEntity( 50, 50, entityID );

            ServerTCP.SendAccountCreateSuccess( index );
        }

        private static void HandleLoadDoodadsQuery( int index, PacketBuffer buffer ) {
            int chunkX = buffer.ReadInteger();
            int chunkY = buffer.ReadInteger();

            ServerTCP.SendChunkDoodadsToPlayer( index, chunkX, chunkY );
        }

        private static void HandleOnUserConnect( int index, PacketBuffer buffer ) {
            Console.WriteLine( "[ServerHandleNetworkData][HandleOnUserConnect] " );
        }

        private static void HandleSpawnRequest( int index, PacketBuffer buffer ) {
            int playerID = buffer.ReadInteger();
            Vector2Int coordinates = SQLReader.GetEntityCoordinates( playerID );

            //redundant
            ServerTCP.InitializeClient( index, coordinates, playerID );
            ServerTCP.SendCoordinatesOnDatabase( index, coordinates );
        }
        private static void HandleUserLoginQuery( int index, PacketBuffer buffer ) {
            string username = buffer.ReadString();
            string password = buffer.ReadString();
            UserCredentials userCredentials = SQLReader.GetPlayerIDAndPassword( username, password );

            if( userCredentials == null ) {
                ServerTCP.SendLoginFail( index, Log.DatabaseUsernameMismatch );
                return;
            }
            else {
                Console.WriteLine( $"{username} has logged in." );
                ServerTCP.SendLoginSuccess( index, userCredentials.ID, username );
            }
        }
    }
}