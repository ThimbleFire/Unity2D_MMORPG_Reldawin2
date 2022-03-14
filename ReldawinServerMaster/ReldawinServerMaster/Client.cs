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

        public void SubscribeInteract( int t, int id )
        {
            if ( properties.items.Count < 20 )
            {
                ServerTCP.ConfirmStartInteract( index, id );

                properties.type = t;

                //prevent subscribing multiple times
                World.OnTick -= OnTick;
                World.OnTick += OnTick;
            }
        }

        public void UnsubscribeInteract()
        {
            World.OnTick -= OnTick;
        }

        public void OnTick()
        {
            bool resourceDepleted = World.random.Next( 100 ) > 90;

            int yieldItemID = XMLDevice.GetDoodad(properties.type).GetYieldRoll;

            // if yieldItemID is empty, we either failed a yield roll, or the ID is invalid.            
            if ( yieldItemID == -1 )
                return;

            properties.items.Add( yieldItemID );

            ServerTCP.TickResult( index, yieldItemID );

            // if the inventory is full or the object is destroyed, force stop
            if ( properties.items.Count >= 20 || resourceDepleted )
            {
                World.OnTick -= OnTick;
                ServerTCP.Interrupt( index, true );
            }

            if ( resourceDepleted )
            {
                //destroy the doodad
            }
        }

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
            World.OnTick -= OnTick;
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
