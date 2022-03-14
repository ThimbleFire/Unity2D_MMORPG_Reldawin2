using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LowCloud.Reldawin
{
    public class OPCLoader : SceneBehaviour
    {
        public LocalPlayerCharacter localPlayerCharacter;
        public GameObject OtherPlayerCharacterPrefab;

        private readonly List<OtherPlayerCharacter> otherPlayerCharacters = new List<OtherPlayerCharacter>();

        protected override void Awake()
        {
            base.Awake();

            EventProcessor.AddInstructionParams( Packet.AnnounceMovementToNearbyPlayers, OnEntityMoveQuery );
            EventProcessor.AddInstructionParams( Packet.OtherPlayerCharacterLoggedIn, OnOtherPlayerCharacterLogin );
            EventProcessor.AddInstructionParams( Packet.OtherPlayerCharacterListRequest, HandleOtherPlayerList );
            EventProcessor.AddInstructionParams( Packet.ToggleRunning, OnOPCToggleRun );
            EventProcessor.AddInstructionParams( Packet.ToggleSwimming, OnOPCToggleSwimming );
            EventProcessor.AddInstructionParams( Packet.StopInteract, BroadcastActionInterruption );
            EventProcessor.AddInstructionParams( Packet.StartInteract, BroadcastActionBegin );
            EventProcessor.AddInstructionParams( Packet.Disconnect, OnOPCDisconnect );
        }

        private void BroadcastActionInterruption( params object[] args )
        {
            int ID = (int)args[0];

            if ( Game.dbID == ID )
                localPlayerCharacter.Interrupt();
            else
                GetPlayer( ID ).Interrupt();
        }

        private void BroadcastActionBegin(params object[] args)
        {
            int id = (int)args[0];
            GetPlayer( id ).Interact();
        }

        // when an OPC moves
        private void OnEntityMoveQuery( params object[] args )
        {
            int ID = (int)args[0];
            Vector2 worldPosition = new Vector2( (float)args[1], (float)args[2] );
            GetPlayer(ID).SetPath( worldPosition, (bool)args[3] );
        }

        private void OnOPCDisconnect(params object[] args)
        {
            int ID = (int)args[0];
            OtherPlayerCharacter opc = GetPlayer( ID );
            otherPlayerCharacters.Remove( opc );
            opc.Destroy();
        }

        // when an OPC toggles run
        private void OnOPCToggleRun( params object[] args )
        {
            int ID = (int)args[0];
            GetPlayer(ID).ToggleRunning();
        }

        private void OnOPCToggleSwimming(params object[] args)
        {
            int ID = (int)args[0];
            if ( Game.dbID == ID )
                localPlayerCharacter.ToggleSwimming();
            else
                GetPlayer( ID ).ToggleSwimming();
        }

        // when an OPC connects
        private void OnOtherPlayerCharacterLogin( params object[] args )
        {
            ///instead of using ClientParams, pass List<OtherPlayerCharacter>
            ClientParams p = (ClientParams)args[0];

            GameObject otherPlayerCharacter = Instantiate( OtherPlayerCharacterPrefab );
            OtherPlayerCharacter opc = otherPlayerCharacter.GetComponent<OtherPlayerCharacter>();
            opc.Setup( p );
            otherPlayerCharacters.Add( opc );
        }

        // when the PC connects
        private void HandleOtherPlayerList( params object[] args )
        {
            List<ClientParams> p = (List<ClientParams>)args[0];

            for ( int i = 0; i < p.Count; i++ )
            {
                GameObject otherPlayerCharacter = Instantiate( OtherPlayerCharacterPrefab );
                OtherPlayerCharacter opc = otherPlayerCharacter.GetComponent<OtherPlayerCharacter>();
                opc.Setup( p[i] );
                otherPlayerCharacters.Add( opc );
            }
        }
        
        private OtherPlayerCharacter GetPlayer(int ID)
        {
            return otherPlayerCharacters.Find( c => c.DatabaseID == ID );
        }
    }
}
