using System;
using UnityEngine;

namespace LowCloud.Reldawin
{
    public class LocalPlayerCharacter : Entity
    {
        private CameraScript cameraScript;
        [SerializeField] private Inventory inventory;

        private void Awake()
        {
            MovementSpeed = WalkSpeed;
            cameraScript = Camera.main.GetComponent<CameraScript>();
            Chunk.OnClicked += Chunk_OnClicked;
            Running = false;
        }

        private void Update()
        {
            if ( Input.GetKeyDown( KeyCode.R ) )
            {
                ToggleRunning();
                animationConroller.ToggleRun(Running);
            }
        }
        
        public override void ToggleRunning()
        {
            base.ToggleRunning();
        
            ClientTCP.ToggleRunning();
        }

        protected override void OnMovedTile( Vector2Int newPoint, Vector2Int lastTile )
        {
            ClientTCP.SavePositionToServer( newPoint );

            base.OnMovedTile( newPoint, lastTile );
        }

        protected override void OnMovedChunk( Vector2Int currentChunk, Vector2Int lastChunk )
        {
            OnChunkChanged?.Invoke( InCurrentChunk, InLastChunk );

            base.OnMovedChunk( currentChunk, lastChunk );
        }

        private void Chunk_OnClicked( Vector2Int cellClicked, Vector2 pointClicked )
        {
            path = Pathfinder.GetPath( CellPositionInWorld, cellClicked, out lastNodeOccupied );

            HasInventorySpace = inventory.GetEmptySlots > 0;

            if(Interacting)
                Interrupt();

            ClientTCP.AnnounceMovementToNearbyPlayers( Game.dbID, pointClicked, inventory.GetEmptySlots > 0 );
            
            base.pointClicked = pointClicked;

            if ( path == null )
            {
                SetTargetPosition( pointClicked, true );

                return;
            }

            if ( path.Count == 0 )
            {
                return;
            }

            MoveToNextNode();
        }

        public void Setup( Vector2Int cellPosition )
        {
            // set Vector2Int data
            CellPositionInWorld = cellPosition;
            InLastChunk = InCurrentChunk;

            // set Vector2 data
            transform.position = MyMath.CellToIsometric( cellPosition );
            cameraScript.transform.SetParent( transform );
            cameraScript.SetFocus( gameObject.transform );
            MovingToward = transform.position;

            DatabaseID = Game.dbID;

            gameObject.name = string.Format( "PC: {0} [{1}]", Game.username, Game.dbID );
            Debug.Log( string.Format( "Your database ID is {0}", DatabaseID ) );
        }

        public override void Interrupt( params object[] args )
        {
            base.Interrupt();

            ClientTCP.SendInterrupt();

            Interacting = false;
        }

        public override void Interact()
        {
            ClientTCP.SendInteractDoodad( interactingWithCell );

            base.Interact();
        }

        private void OnDestroy()
        {
            Chunk.OnClicked -= Chunk_OnClicked;
        }

        public delegate void ChunkChangedEventHandler( Vector2Int newChunkIndex, Vector2Int lastChunkIndex );
        public static event ChunkChangedEventHandler OnChunkChanged;

        public delegate void TileChangedEventHandler( Vector2Int newTileIndex, Vector2Int lastTileIndex );
        public static event TileChangedEventHandler OnTileChanged;
    }
}
