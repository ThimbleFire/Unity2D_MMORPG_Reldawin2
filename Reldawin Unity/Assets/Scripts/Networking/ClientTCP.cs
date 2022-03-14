using System;
using System.Net.Sockets;
using UnityEngine;

namespace LowCloud.Reldawin
{
    public class ClientTCP : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad( gameObject );
            DontDestroyOnLoad( this );
        }

        private void Start()
        {
            Connect();
        }

        public static void SendData( byte[] input )
        {
            clientSocket.Send( input );
        }

        public void Connect()
        {
            const int numberOfRetries = 3;
            int retryCount = numberOfRetries;
            bool connected = false;

            while ( !connected && retryCount > 0 )
            {
                try
                {
                    clientSocket.BeginConnect( "127.0.0.1"
                                                 , 5555
                                                 , new AsyncCallback( ConnectCallback )
                                                 , clientSocket
                                                 );

                    connected = true;
                }
                catch ( TimeoutException tex )
                {
                    Debug.LogWarning( "[ClientTCP] " + tex.Message );

                    retryCount--;

                    if ( retryCount == 0 )
                    {
                        throw;
                    }
                }
            }
        }

        private static void OnRecieve()
        {
            byte[] sizeInfo = new byte[4];

            try
            {

                int totalRead;
                int currentRead = totalRead = clientSocket.Receive( sizeInfo );
                if ( totalRead <= 0 )
                {
                    Debug.Log( "[Client] You are not connected to the server" );
                }
                else
                {
                    while ( totalRead < sizeInfo.Length && currentRead > 0 )
                    {
                        currentRead = clientSocket.Receive( sizeInfo, totalRead, sizeInfo.Length - totalRead, SocketFlags.None );
                        totalRead += currentRead;
                    }

                    int messageSize = 0;
                    messageSize |= sizeInfo[0];
                    messageSize |= ( sizeInfo[1] << 08 );
                    messageSize |= ( sizeInfo[2] << 16 );
                    messageSize |= ( sizeInfo[3] << 24 );

                    byte[] data = new byte[messageSize];

                    totalRead = 0;
                    currentRead = totalRead = clientSocket.Receive( data, totalRead, data.Length - totalRead, SocketFlags.None );

                    while ( totalRead < messageSize && currentRead > 0 )
                    {
                        currentRead = clientSocket.Receive( data, totalRead, data.Length - totalRead, SocketFlags.None );
                        totalRead += totalRead;
                    }

                    ClientHandleNetworkPackets.HandleNetworkInformation( data );
                }
            }
            catch ( Exception e )
            {
                Debug.Log( e.Message );
                clientSocket.Close();
            }
        }

        private void ConnectCallback( IAsyncResult ar )
        {
            try
            {
                clientSocket.EndConnect( ar );
            }
            catch ( Exception )
            {
                Debug.LogError( "The server is not running" );
                close = true;
            }

            while ( close == false )
            {
                OnRecieve();
            }
        }

        private void OnApplicationQuit()
        {
            clientSocket.Close();
            close = true;
        }

        public static void SendConfirmRecieve()
        {
            using PacketBuffer buffer = new PacketBuffer( Packet.ConnectionOK );
            SendData( buffer.ToArray() );
        }
        public static void SavePositionToServer( Vector2Int newPosition )
        {
            using PacketBuffer buffer = new PacketBuffer( Packet.SavePositionToServer );
            buffer.WriteInteger( newPosition.x );
            buffer.WriteInteger( newPosition.y );
            SendData( buffer.ToArray() );
        }
        public static void SendPing()
        {
            using PacketBuffer buffer = new PacketBuffer( Packet.PingTest );
            SendData( buffer.ToArray() );
        }
        public static void RequestMapDetails()
        {
            using PacketBuffer buffer = new PacketBuffer( Packet.RequestSeed );
            buffer.WriteInteger( Game.dbID );
            SendData( buffer.ToArray() );
        }
        public static void ToggleRunning()
        {
            using PacketBuffer buffer = new PacketBuffer( Packet.ToggleRunning );
            buffer.WriteInteger( Game.dbID );
            SendData( buffer.ToArray() );
        }
        public static void SendInteractDoodad( Vector2Int doodadWorldCellPosition )
        {
            using PacketBuffer buffer = new PacketBuffer( Packet.StartInteract );
            buffer.WriteInteger( (int)doodadWorldCellPosition.x );
            buffer.WriteInteger( (int)doodadWorldCellPosition.y );
            buffer.WriteInteger( Game.dbID );
            SendData( buffer.ToArray() );
        }
        public static void SendLoginAttemptQuery( string username, string password )
        {
            using PacketBuffer buffer = new PacketBuffer( Packet.Account_Login_Query );
            buffer.WriteString( username );
            buffer.WriteString( password );
            SendData( buffer.ToArray() );
        }
        public static void SendChunkDataQuery( Vector2Int chunkPosition )
        {
            using PacketBuffer buffer = new PacketBuffer( Packet.Load_Chunk );
            buffer.WriteInteger( chunkPosition.x );
            buffer.WriteInteger( chunkPosition.y );
            SendData( buffer.ToArray() );
        }
        public static void SendChunkDoodadQuery( Vector2Int chunkPosition )
        {
            using PacketBuffer buffer = new PacketBuffer( Packet.Load_Doodads );
            buffer.WriteInteger( chunkPosition.x );
            buffer.WriteInteger( chunkPosition.y );
            SendData( buffer.ToArray() );
        }
        public static void SendInterrupt()
        {
            using PacketBuffer buffer = new PacketBuffer( Packet.StopInteract );
            SendData( buffer.ToArray() );
        }
        public static void SendUsernameQuery( string text )
        {
            using PacketBuffer buffer = new PacketBuffer( Packet.DoesUserExist );
            buffer.WriteString( text );
            SendData( buffer.ToArray() );
        }
        public static void AnnounceMovementToNearbyPlayers( int ID, Vector2 position, bool hasInventorySpace )
        {
            using PacketBuffer buffer = new PacketBuffer( Packet.AnnounceMovementToNearbyPlayers );
            buffer.WriteFloat( position.x );
            buffer.WriteFloat( position.y );
            buffer.WriteInteger( ID );
            SendData( buffer.ToArray() );
        }
        public static void SendCreateAccountQuery( string username, string password )
        {
            using PacketBuffer buffer = new PacketBuffer( Packet.Account_Create_Query );
            buffer.WriteString( username );
            buffer.WriteString( password );
            SendData( buffer.ToArray() );
        }
        public static void OtherPlayerCharacterListRequest()
        {
            using PacketBuffer buffer = new PacketBuffer( Packet.OtherPlayerCharacterListRequest );
            SendData( buffer.ToArray() );
        }

        public static Socket clientSocket = new Socket( AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp );
        private bool close = false;
    }
}