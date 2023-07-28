using System;
using System.Collections.Generic;
using UnityEngine;

namespace AlwaysEast
{
    internal class ClientHandleNetworkPackets : MonoBehaviour
    {
        private static EventProcessor eventProcessor;
        private static Dictionary<int, Packet_> packets;

        private delegate void Packet_( byte[] data );

        public static void InitializeNetworkPackages()
        {
            packets = new Dictionary<int, Packet_>
            {
                { (int)Packet.ConnectionOK, HandleConnectionOK },
                { (int)Packet.Account_Create_Success, HandleAccountCreateSuccess },
                { (int)Packet.Account_Login_Fail, HandleLoginFail },
                { (int)Packet.Account_Login_Success, HandleLoginSuccess },
                { (int)Packet.RequestSpawn, HandleSpawnRequest },
                { (int)Packet.PingTest, HandlePingTest },
                { (int)Packet.DoesUserExist, HandleDoesUserExist },
                { (int)Packet.OtherPlayerCharacterLoggedIn, HandleOtherPlayerCharacterLogin },
                { (int)Packet.OtherPlayerCharacterListRequest, HandleOtherPlayerCharacterListResponse },
                { (int)Packet.AnnounceMovementToNearbyPlayers, HandleEntityMoveQuery },
                { (int)Packet.Load_Chunk, HandleLoadChunkQuery },
                //{ (int)Packet.Load_Doodads, HandleLoadChunkDoodadQuery },
                { (int)Packet.YieldInteract, HandleYieldInteract },
                { (int)Packet.StopInteract, HandleStopInteract },
                { (int)Packet.ToggleRunning, HandleToggleRunning },
                { (int)Packet.StartInteract, HandleConfirmStartInteract },
                { (int)Packet.ToggleSwimming, HandleToggleSwimming },
                { (int)Packet.Disconnect, HandleDisconnect }
            };
        }

        private static void HandleConfirmStartInteract(byte[] data)
        {
            using PacketBuffer buffer = new PacketBuffer( data );
            int cpIndex = buffer.ReadInteger();
            int id = buffer.ReadInteger();
            eventProcessor.QueueEvent( (Packet)cpIndex, id );
        }

        private static void HandleToggleSwimming( byte[] data )
        {
            using PacketBuffer buffer = new PacketBuffer( data );
            int cpIndex = buffer.ReadInteger();
            int entityID = buffer.ReadInteger();

            eventProcessor.QueueEvent( (Packet)cpIndex, entityID );
        }

        private static void HandleToggleRunning( byte[] data )
        {
            using PacketBuffer buffer = new PacketBuffer( data );
            int cpIndex = buffer.ReadInteger();
            int entityID = buffer.ReadInteger();

            eventProcessor.QueueEvent( (Packet)cpIndex, entityID );
        }

        private static void HandleDisconnect(byte[] data)
        {
            using PacketBuffer buffer = new PacketBuffer( data );
            int cpIndex = buffer.ReadInteger();
            int entityID = buffer.ReadInteger();

            eventProcessor.QueueEvent( (Packet)cpIndex, entityID );
        }

        private static void HandleStopInteract( byte[] data )
        {
            using PacketBuffer buffer = new PacketBuffer( data );
            int packetNum = buffer.ReadInteger();
            int ID = buffer.ReadInteger();

            eventProcessor.QueueEvent( (Packet)packetNum, ID );
        }

        private static void HandleYieldInteract( byte[] data )
        {
            using PacketBuffer buffer = new PacketBuffer( data );
            int packetNum = buffer.ReadInteger();
            int itemID = buffer.ReadInteger();

            eventProcessor.QueueEvent( (Packet)packetNum, itemID );
        }

        public static void HandleNetworkInformation( byte[] data )
        {
            using PacketBuffer buffer = new PacketBuffer( data );
            int packetNum = buffer.ReadInteger();

            if ( packets.TryGetValue( packetNum, out Packet_ packet ) )
            {
                packet.Invoke( data );
            }
        }

        private static void HandleConnectionOK( byte[] data )
        {
            using PacketBuffer buffer = new PacketBuffer( data );
            int cpIndex = buffer.ReadInteger();
            ClientTCP.SendConfirmRecieve();

            eventProcessor.QueueEvent( (Packet)cpIndex );
        }

        private static void HandleAccountCreateSuccess( byte[] data )
        {
            using PacketBuffer buffer = new PacketBuffer( data );
            int cpIndex = buffer.ReadInteger();
            string message = buffer.ReadString();

            eventProcessor.QueueEvent( (Packet)cpIndex, message );
        }

        private static void HandleLoginFail( byte[] data )
        {
            using PacketBuffer buffer = new PacketBuffer( data );
            int cpIndex = buffer.ReadInteger();
            string message = buffer.ReadString();

            eventProcessor.QueueEvent( (Packet)cpIndex, message );
        }

        private static void HandleLoginSuccess( byte[] data )
        {
            using PacketBuffer buffer = new PacketBuffer( data );
            int cpIndex = buffer.ReadInteger();
            Game.dbID = (ushort)buffer.ReadInteger();

            eventProcessor.QueueEvent( (Packet)cpIndex );
        }

        private static void HandleSpawnRequest( byte[] data )
        {
            using PacketBuffer buffer = new PacketBuffer( data );
            int cpIndex = buffer.ReadInteger();
            int cellPosX = buffer.ReadInteger();
            int cellPosY = buffer.ReadInteger();
            eventProcessor.QueueEvent( (Packet)cpIndex, cellPosX, cellPosY );
        }

        private static void HandlePingTest( byte[] data )
        {
            using PacketBuffer buffer = new PacketBuffer( data );
            int cpIndex = buffer.ReadInteger();
            eventProcessor.QueueEvent( (Packet)cpIndex );
        }

        private static void HandleDoesUserExist( byte[] data )
        {
            using PacketBuffer buffer = new PacketBuffer( data );
            int cpIndex = buffer.ReadInteger();
            byte result = buffer.ReadByte();
            eventProcessor.QueueEvent( (Packet)cpIndex, result == 1 );
        }

        /// <summary>Fired when another player character logs in</summary>
        private static void HandleOtherPlayerCharacterLogin( byte[] data )
        {
            using PacketBuffer buffer = new PacketBuffer( data );
            int cpIndex = buffer.ReadInteger();
            string otherPCUsername = buffer.ReadString();
            int cellPosX = buffer.ReadInteger();
            int cellPosY = buffer.ReadInteger();
            int otherPCID = buffer.ReadInteger();
            ClientParams clientParams = new ClientParams( otherPCUsername, cellPosX, cellPosY, otherPCID, false, false );
            eventProcessor.QueueEvent( (Packet)cpIndex, clientParams );
        }

        /// <summary>Fired when the local player character logs in</summary>
        private static void HandleOtherPlayerCharacterListResponse( byte[] data )
        {
            using PacketBuffer buffer = new PacketBuffer( data );
            List<ClientParams> clientParams = new List<ClientParams>();
            int cpIndex = buffer.ReadInteger();
            int playerCount = buffer.ReadInteger();

            for ( int i = 0; i < playerCount; i++ )
            {
                ClientParams p = new ClientParams
                {
                    username = buffer.ReadString(),
                    cellPosX = buffer.ReadInteger(),
                    cellPosY = buffer.ReadInteger(),
                    ID = buffer.ReadInteger(),
                    Running = buffer.ReadBoolean()
                };

                clientParams.Add( p );
            }

            eventProcessor.QueueEvent( (Packet)cpIndex, clientParams );
        }

        /// <summary>Fired when the local player character clicks to move</summary>
        private static void HandleEntityMoveQuery( byte[] data )
        {
            using PacketBuffer buffer = new PacketBuffer( data );
            int cpIndex = buffer.ReadInteger();
            int entityID = buffer.ReadInteger();
            float worldPosX = buffer.ReadFloat();
            float worldPosY = buffer.ReadFloat();
            bool hasInventorySpace = buffer.ReadBoolean();
            eventProcessor.QueueEvent( (Packet)cpIndex, entityID, worldPosX, worldPosY, hasInventorySpace );
        }

        /// <summary>Fires when the local player character's position exceeds their current chunks border</summary>
        private static void HandleLoadChunkQuery( byte[] data )
        {
            using PacketBuffer buffer = new PacketBuffer( data );
            int cpIndex = buffer.ReadInteger();
            int chunkX = buffer.ReadInteger();
            int chunkY = buffer.ReadInteger();
            string chunkData = buffer.ReadString();
            eventProcessor.QueueEvent( (Packet)cpIndex, chunkX, chunkY, chunkData );
        }

        /// <summary>Fires when a chunk has finished loading and is ready to be populated by objects</summary>
        //private static void HandleLoadChunkDoodadQuery( byte[] data )
        //{
        //    using PacketBuffer buffer = new PacketBuffer( data );
        //    int cpIndex = buffer.ReadInteger();
        //    Vector2Int chunkIndex = new Vector2Int( buffer.ReadInteger(), buffer.ReadInteger() );
        //    int count = buffer.ReadInteger();
        //    List<Doodad.Data> doodads = new List<Doodad.Data>();
        //    for ( int i = 0; i < count; i++ )
        //    {
        //        int doodadType = buffer.ReadByte();
        //        int tileX = buffer.ReadInteger();
        //        int tileY = buffer.ReadInteger();
        //        Vector2Int tilePos = new Vector2Int( tileX, tileY );
        //        doodads.Add( new Doodad.Data( doodadType, tilePos, chunkIndex ) );
        //    }
        //    eventProcessor.QueueEvent( (Packet)cpIndex, chunkIndex, doodads );
        //}

        private void Awake()
        {
            DontDestroyOnLoad( this );
            InitializeNetworkPackages();
            eventProcessor = GetComponent<EventProcessor>();
        }
    }
}
