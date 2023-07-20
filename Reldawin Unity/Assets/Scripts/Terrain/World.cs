using System.Collections.Generic;
using UnityEngine;

namespace LowCloud.Reldawin
{
    public class World : SceneBehaviour
    {
        private ushort _height;
        private ushort _width;

        [SerializeField] private CameraScript cameraScript;
        [SerializeField] private ChunkLoader chunkLoader;
        [SerializeField] private Clock clock;
        [SerializeField] private LocalPlayerCharacter localPlayerCharacter;

        protected override void Awake()
        {
            base.Awake();

            Instance = this;

            //EventProcessor.AddInstructionParams( Packet.RequestSeed, OnNetworkRequestSeedResponse );

            SpriteLoader.Setup();
            XMLLoader.Setup();
        }

        private void Start()
        {
            //ClientTCP.RequestMapDetails();

            localPlayerCharacter.Setup( new Vector2Int( 256, 256 ) );
            chunkLoader.PlayerSpawnStartUp();
            ClientTCP.OtherPlayerCharacterListRequest();
        }

        //private void OnNetworkRequestSeedResponse( params object[] args )
        //{
        //    EventProcessor.RemoveInstructionParams( Packet.RequestSeed );

        //    _width = (ushort)( (int)args[1] );
        //    _height = (ushort)( (int)args[2] );
        //    Vector2Int cellPosition = new Vector2Int( (int)args[3], (int)args[4] );
        //    int _internalGameTime = (int)args[5];

        //    clock.Setup( _internalGameTime );

        //    localPlayerCharacter.Setup( cellPosition );

        //    chunkLoader.PlayerSpawnStartUp();

        //    ClientTCP.OtherPlayerCharacterListRequest();
        //}

        public static World Instance { get; set; }
        public ushort Height
        { get { return 512; } }
        public ushort Width
        { get { return 512; } }
    }
}