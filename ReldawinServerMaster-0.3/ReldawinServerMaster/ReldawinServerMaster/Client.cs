using Bindings;
using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace ReldawinServerMaster
{
    internal class Client
    {
        public byte[] buffer = new byte[1024];
        public bool closing;
        public bool loggedIn = false;
        public int index;
        public string ip;
        public ClientProperties properties;
        public Socket socket;

        public void MovePosition( int x, int y )
        {
            properties.Position = new Vector2Int( x, y );
        }

        public void Setup( int x, int y, int id )
        {
            properties.Position = new Vector2Int( x, y );
            properties.ID = id;
            loggedIn = true;
        }

        public void StartClient()
        {
            socket.BeginReceive( buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback( RecieveCallback ), socket );
            closing = false;

            properties = new ClientProperties();
        }

        private void CloseClient( int index )
        {
            ServerTCP.AnnounceDisconnect(index, properties.ID);
            closing = true;
            Console.WriteLine( properties.Username + " safely disconnected." );
            socket.Close();
            socket = null;
            loggedIn = false;

            index = 0;
            properties.Clear();
        }

        private void RecieveCallback( IAsyncResult ar )
        {
            Socket socket = (Socket)ar.AsyncState;

            try
            {
                int bytesRead = socket.EndReceive( ar );

                if ( bytesRead <= 0 )
                {
                    CloseClient( index );
                }
                else
                {
                    byte[] dataBuffer = new byte[bytesRead];
                    Array.Copy( buffer, dataBuffer, bytesRead );

                    ServerHandleNetworkData.HandleNetworkInformation( index
                                                                    , dataBuffer
                                                                    );
                    socket.BeginReceive( buffer
                                       , 0
                                       , buffer.Length
                                       , SocketFlags.None
                                       , new AsyncCallback( RecieveCallback )
                                       , socket
                                       );
                }
            }
            catch ( Exception e )
            {
                Console.WriteLine( "[Client::RecieveCallBack] " + e.Message, index );
                CloseClient( index );
            }
        }
    }
}
