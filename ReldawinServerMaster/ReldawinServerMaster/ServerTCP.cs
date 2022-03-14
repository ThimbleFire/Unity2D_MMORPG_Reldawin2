using Bindings;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace ReldawinServerMaster
{
    internal class ServerTCP
    {
        public static Client[] clients = new Client[Log.MAX_PLAYERS];
        public static Socket serverSocket = new Socket( AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp );

        public static void SetupServer()
        {
            for ( int i = 0; i < Log.MAX_PLAYERS; i++ )
            {
                clients[i] = new Client();
            }

            serverSocket.Bind( new IPEndPoint( IPAddress.Any, 5555 ) );
            serverSocket.Listen( Log.BUFFER_PLAYERS );
            serverSocket.BeginAccept( new AsyncCallback( AcceptCallback ), null );
        }

        public static void InitializeClient( int index, int x, int y, int id )
        {
            clients[index].Setup( x, y, id );
        }

        public static void SubscribeToInteract( int index, int tileX, int tileY, int id )
        {
            using ( new DebugTimer( clients[index].properties.Username + " SubscribeToInteract" ) )
            {
                List<object> doodad = CommonSQL.GetDoodad( tileX, tileY );

                if ( doodad != null )
                {
                    clients[index].SubscribeInteract( Convert.ToInt32(doodad[0]), id );
                }
            }
        }

        public static void UnsubscribeFromInteract(int index)
        {
            using ( new DebugTimer( clients[index].properties.Username + " UnsubscribeFromInteract" ) )
            {
                clients[index].UnsubscribeInteract();
            }
        }

        public static void TickResult( int index, int yieldItemID )
        {
            Harvest( index, yieldItemID );
        }

        public static void ConfirmStartInteract( int index, int id )
        {
            using ( new DebugTimer( clients[index].properties.Username + " ConfirmStartInteract" ) )
            {
                List<Client> clientList = FetchOtherClients( index );

                if ( clientList.Count == 0 )
                    return;

                using ( PacketBuffer buffer = new PacketBuffer( Packet.StartInteract ) )
                {
                    buffer.WriteInteger( id );

                    foreach ( Client client in clientList )
                        SendDataTo( client.index, buffer.ToArray() );
                }
            }
        }

        public static void Harvest( int index, int yieldItemID )
        {
            using ( new DebugTimer( clients[index].properties.Username + " Harvest" ) )
            {
                using ( PacketBuffer buffer = new PacketBuffer( Packet.YieldInteract ) )
                {
                    buffer.WriteInteger( yieldItemID );

                    SendDataTo( index, buffer.ToArray() );
                }
            }
        }

        public static void AnnounceDisconnect(int index, int ID)
        {
            List<Client> clientList = FetchOtherClients( index );

            if ( clientList.Count == 0 )
                return;

            using ( PacketBuffer buffer = new PacketBuffer( Packet.Disconnect ) )
            {
                buffer.WriteInteger( ID );

                foreach ( Client client in clientList )
                    SendDataTo( client.index, buffer.ToArray() );
            }
        }

        public static void Interrupt(int index, bool includeSender)
        {
            using ( new DebugTimer( clients[index].properties.Username + " Interrupt" ) )
            {
                // get other players
                List<Client> clientList = FetchOtherClients( index );

                if ( includeSender )
                {
                    // add the player character calling the event
                    clientList.Add( clients[index] );
                }

                using ( PacketBuffer buffer = new PacketBuffer( Packet.StopInteract ) )
                {
                    buffer.WriteInteger( clients[index].properties.ID );

                    foreach ( Client client in clientList )
                        SendDataTo( client.index, buffer.ToArray() );
                }
            }
        }

        public static void ChangeClientPosition( int index, int x, int y )
        {
            clients[index].MovePosition( x, y );
            CommonSQL.SetEntityCoodinates( x, y, clients[index].properties.ID );

            // if the tile being walked on is water and we're not already swimming
            if ( World.tiles[x, y] == 5 && clients[index].properties.Swimming == false )
            {
                clients[index].properties.Swimming = true;

                List<Client> clientList = FetchOtherClients( index );
                clientList.Add( clients[index] );

                using ( PacketBuffer buffer = new PacketBuffer( Packet.ToggleSwimming ) )
                {
                    buffer.WriteInteger( clients[index].properties.ID );

                    foreach ( Client client in clientList )
                        SendDataTo( client.index, buffer.ToArray() );
                }
            }
            if(World.tiles[x, y] != 5 && clients[index].properties.Swimming == true)
            {
                clients[index].properties.Swimming = false;

                List<Client> clientList = FetchOtherClients( index );
                clientList.Add( clients[index] );

                using ( PacketBuffer buffer = new PacketBuffer( Packet.ToggleSwimming ) )
                {
                    buffer.WriteInteger( clients[index].properties.ID );

                    foreach ( Client client in clientList )
                        SendDataTo( client.index, buffer.ToArray() );
                }
            }
        }

        
        public static void SendToggleRunningToAllPlayers(int index, int ID)
        {
            using ( new DebugTimer( clients[index].properties.Username + " SendToggleRunningToAllPlayers" ) )
            {
                clients[index].properties.Running = !clients[index].properties.Running;
                
                List<Client> clientList = FetchOtherClients( index );
                
                if ( clientList.Count == 0 )
                    return;

                using ( PacketBuffer buffer = new PacketBuffer( Packet.ToggleRunning ) )
                {
                    buffer.WriteInteger( ID );

                    foreach ( Client client in clientList )
                        SendDataTo( client.index, buffer.ToArray() );
                }
            }
        }

        public static void SendConnectionOK( int index )
        {
            using ( new DebugTimer( "SendConnectionOK" ) )
            {
                using ( PacketBuffer buffer = new PacketBuffer(Packet.ConnectionOK) )
                {
                    SendDataTo( index, buffer.ToArray() );
                    Console.WriteLine( "[ServerTCP] " + "Send ConnectionOK" );
                }
            }
        }

        public static void SendLoginFail( int index, string args )
        {
            using ( new DebugTimer( "? SendLoginFail" ) )
            {
                using ( PacketBuffer buffer = new PacketBuffer( Packet.Account_Login_Fail ) )
                {
                    buffer.WriteString( args );
                    SendDataTo( index, buffer.ToArray() );
                    Console.WriteLine( "[ServerTCP] " + args );
                }
            }
        }

        public static void ReturnDoesUserExist( int index, bool result )
        {
            using ( new DebugTimer( clients[index].properties.Username + " ReturnDoesUserExist" ) )
            {
                using ( PacketBuffer buffer = new PacketBuffer( Packet.DoesUserExist ) )
                {
                    buffer.WriteByte( (byte)( result == true ? 1 : 0 ) );
                    SendDataTo( index, buffer.ToArray() );
                }
            }
        }

        public static void SendMoveQueryToAllPlayers( int index, int ID, float pointX, float pointY )
        {
            using ( new DebugTimer( clients[index].properties.Username + " SendMoveQueryToAllPlayers" ) )
            {
                List<Client> clientList = FetchOtherClients( index );

                // If there are no other players to tell our position has changed, forget it.
                if ( clientList.Count == 0 )
                    return;

                using ( PacketBuffer buffer = new PacketBuffer( Packet.AnnounceMovementToNearbyPlayers ) )
                {
                    buffer.WriteInteger( ID );
                    buffer.WriteFloat( pointX );
                    buffer.WriteFloat( pointY );
                    buffer.WriteBoolean( clients[index].properties.items.Count < 20 );

                    foreach ( Client client in clientList )
                        SendDataTo( client.index, buffer.ToArray() );
                }
            }
        }

        public static void SendChunkDataToPlayer( int index, int chunkX, int chunkY )
        {
            using ( new DebugTimer( clients[index].properties.Username + " SendChunkDataToPlayer" ) )
            {
                string data = World.GetChunkData( chunkX, chunkY );

                using ( PacketBuffer buffer = new PacketBuffer( Packet.Load_Chunk ) )
                {
                    buffer.WriteInteger( chunkX );
                    buffer.WriteInteger( chunkY );
                    buffer.WriteString( data );
                    SendDataTo( index, buffer.ToArray() );
                }
            }
            return;
        }

        public static void SendChunkDoodadsToPlayer( int index, int chunkX, int chunkY )
        {
            using ( new DebugTimer( clients[index].properties.Username + " SendChunkDoodadsToPlayer" ) )
            {
                List<Doodad> doodads = World.GetDoodads( chunkX, chunkY );

                using ( PacketBuffer buffer = new PacketBuffer( Packet.Load_Doodads ) )
                {
                    buffer.WriteInteger( chunkX );
                    buffer.WriteInteger( chunkY );
                    buffer.WriteInteger( doodads.Count );

                    foreach ( Doodad doodad in doodads )
                    {
                        buffer.WriteByte( (byte)doodad.type );
                        buffer.WriteInteger( doodad.tileX );
                        buffer.WriteInteger( doodad.tileY );
                    }

                    SendDataTo( index, buffer.ToArray() );
                }
            }
        }

        public static void SendAccountCreateFail( int index, string args )
        {
            using ( new DebugTimer( "? SendAccountCreateFail" ) )
            {
                using ( PacketBuffer buffer = new PacketBuffer( Packet.Account_Create_Fail ) )
                {
                    buffer.WriteString( args );
                    SendDataTo( index, buffer.ToArray() );
                }
            }
        }

        public static void SendAccountCreateSuccess( int index, string args )
        {
            using ( new DebugTimer( "? SendAccountCreateSuccess" ) )
            {
                using ( PacketBuffer buffer = new PacketBuffer( Packet.Account_Create_Success ) )
                {
                    buffer.WriteString( args );
                    SendDataTo( index, buffer.ToArray() );
                }

                Console.WriteLine( "[ServerTCP] " + args );
            }
        }

        public static void SendLoginSuccess( int index, int id, string username )
        {
            using ( new DebugTimer( username + " SendLoginSuccess" ) )
            {
                using ( PacketBuffer buffer = new PacketBuffer( Packet.Account_Login_Success ) )
                {
                    buffer.WriteInteger( id );
                    SendDataTo( index, buffer.ToArray() );
                }

                clients[index].properties.Username = username;

                // Alert other players of other players logging in

                using ( PacketBuffer buffer = new PacketBuffer( Packet.OtherPlayerCharacterLoggedIn ) )
                {
                    buffer.WriteString( username );

                    object[] result = CommonSQL.GetEntityCoordinates( id );

                    if ( result == null )
                    {
                        Console.WriteLine( "[ServerTCP] [DbError] Could not get {0}'s coordinates.", username );
                        return;
                    }

                    // Send players the logging-in players coordinates so they can decide whether they're worth loading in their game
                    buffer.WriteInteger( Convert.ToInt32( result[0] ) );
                    buffer.WriteInteger( Convert.ToInt32( result[1] ) );
                    buffer.WriteInteger( id );

                    // Tell all the players another player has logged in.
                    for ( int i = 0; i < clients.Length; i++ )
                    {
                        // Player clients are of course not null
                        if ( clients[i].loggedIn != false )
                        {
                            // And we don't need to tell <index> since they're the player logging in
                            if ( clients[i].index != index )
                            {
                                SendDataTo( clients[i].index, buffer.ToArray() );
                            }
                        }
                    }
                }
            }
        }

        public static void SendOtherPlayerCharacterListRequest( int index )
        {
            using ( new DebugTimer( clients[index].properties.Username + " SendOtherPlayerCharacterListRequest" ) )
            {
                List<Client> clientList = FetchOtherClients( index );

                using ( PacketBuffer buffer = new PacketBuffer( Packet.OtherPlayerCharacterListRequest ) )
                {
                    buffer.WriteInteger( clientList.Count );

                    if ( clientList.Count > 0 )
                    {
                        foreach ( Client client in clientList )
                        {
                            buffer.WriteString( client.properties.Username );
                            buffer.WriteInteger( client.properties.Position.x );
                            buffer.WriteInteger( client.properties.Position.y );
                            buffer.WriteInteger( client.properties.ID );
                            buffer.WriteBoolean( client.properties.Running );
                        }
                    }
                    SendDataTo( index, buffer.ToArray() );
                }
            }
        }

        public static void SendPingTest( int index )
        {
            using ( PacketBuffer buffer = new PacketBuffer( Packet.PingTest ) )
            {
                SendDataTo( index, buffer.ToArray() );
            }
        }

        public static void SendSeedAndStartupDetails( int index, int playerPosX, int playerPosY )
        {
            using ( new DebugTimer( clients[index].properties.Username + " SendSeedAndStartupDetails" ) )
            {
                using ( PacketBuffer buffer = new PacketBuffer( Packet.RequestSeed ) )
                {
                    buffer.WriteInteger( World.Seed );
                    buffer.WriteInteger( World.Width );
                    buffer.WriteInteger( World.Height );
                    buffer.WriteInteger( playerPosX );
                    buffer.WriteInteger( playerPosY );

                    int time = DateTime.UtcNow.Hour * 60 * 60;
                    time += DateTime.UtcNow.Minute * 60;
                    time += DateTime.UtcNow.Second;

                    buffer.WriteInteger( time /* / World.DaysInGamePerRealDays*/ );

                    SendDataTo( index, buffer.ToArray() );
                }

                ChangeClientPosition( index, playerPosX, playerPosY );
            }
        }

        private static void AcceptCallback( IAsyncResult ar )
        {
            Socket socket = serverSocket.EndAccept( ar );
            serverSocket.BeginAccept( new AsyncCallback( AcceptCallback ), null );

            for ( int i = 0; i < Log.MAX_PLAYERS; i++ )
            {
                if ( clients[i].socket == null )
                {
                    clients[i].socket = socket;
                    clients[i].index = i;
                    clients[i].ip = socket.RemoteEndPoint.ToString();
                    clients[i].StartClient();
                    Console.WriteLine( "[ServerTCP] " + Log.SERVER_LOBBY_JOIN, i );
                    SendConnectionOK( i );
                    return;
                }
            }
        }

        private static void SendDataTo( int index, byte[] data )
        {
            byte[] sizeInfo = new byte[4];
            sizeInfo[0] = (byte)data.Length;
            sizeInfo[1] = (byte)( data.Length >> 08 );
            sizeInfo[2] = (byte)( data.Length >> 16 );
            sizeInfo[2] = (byte)( data.Length >> 24 );

            clients[index].socket.Send( sizeInfo );
            clients[index].socket.Send( data );
        }

        private static List<Client> FetchOtherClients( int index )
        {
            List<Client> clientList = new List<Client>();

            // Get a list of all the clients

            foreach ( Client client in clients )
                if ( client.loggedIn != false )
                    if ( client.index != index )
                        clientList.Add( client );

            return clientList;
        }
    }
}
