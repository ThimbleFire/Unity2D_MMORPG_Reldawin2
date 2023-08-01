using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
namespace AlwaysEast
{
    public class LocalPlayerCharacter : MonoBehaviour
    {
        public static event LPCOnChunkChangeHandler LPCOnChunkChange;
        public delegate void LPCOnChunkChangeHandler( Vector3Int lastChunk, Vector3Int newChunk );
        protected float MovementSpeed { get; set; } = WalkSpeed;
        protected Vector2 MovingToward = Vector2.zero;
        public const float RunSpeed = 0.025f;
        public const float WalkSpeed = 0.010f;
        protected bool Running { get; set; }
        public SpriteRenderer spriteRenderer;
        public AnimationController animationController;
        public Vector3Int CellPositionInWorld = new Vector3Int( 0, 0, 0 );
        public Vector3Int InCurrentChunk {
            get {
                return new Vector3Int( Mathf.FloorToInt( CellPositionInWorld.x / Chunk.width ), Mathf.FloorToInt( CellPositionInWorld.y / Chunk.height ) );
            }
        }
        public Vector3Int inLastChunk;
        private Queue<Node> path;
        protected bool lastNodeOccupied = false;
        public Tilemap tilemap;
        public Vector2 pointClicked;
        private void Awake() {
            World.OnClicked += World_OnClicked;
            inLastChunk = InCurrentChunk;
            MovingToward = transform.position;
        }
        private void Update() {
            if( Input.GetKeyDown( KeyCode.R ) ) {
                ToggleRunning();
            }
        }
        private void FixedUpdate() {
            if( transform.position == (Vector3)MovingToward )
                return;
            transform.position = Vector3.MoveTowards( transform.position, MovingToward, MovementSpeed );
            Vector3Int newPoint = tilemap.WorldToCell( transform.position );
            if( newPoint != CellPositionInWorld ) {
                OnMovedTile( newPoint, CellPositionInWorld );
            }
            // If moving the transform puts us in the position
            if( transform.position == (Vector3)MovingToward ) {
                MoveToNextNode();
            }
        }
        private void ToggleRunning() {
            Running = !Running;
            MovementSpeed = Running ? RunSpeed : WalkSpeed;
            animationController.ToggleRun( Running );
        }
        private void MoveToNextNode() {
            if( path == null ) {
                OnAnimationDestinationMet();
                return;
            }
            if( path.Count == 0 ) {
                OnAnimationDestinationMet();
                return;
            }
            Vector2 nextNodeWorldPosition = path.Peek().WorldPosition;
            if( path.Count > 1 )
                SetTargetPosition( nextNodeWorldPosition );
            else if( path.Count == 1 ) {
                if( lastNodeOccupied ) {
                    path.Clear();
                    FaceDirection( nextNodeWorldPosition );
                    MovingToward = transform.position;
                    return;
                }
                else {
                    SetTargetPosition( pointClicked );
                }
            }
            path.Dequeue();
        }
        private void OnAnimationDestinationMet() {
            animationController.OnAnimationDestinationMet();
        }
        private void OnMovedTile( Vector3Int newPoint, Vector3Int lastTile ) {
            using PacketBuffer buffer = new PacketBuffer( Packet.SavePositionToServer );
            buffer.WriteInteger( newPoint.x );
            buffer.WriteInteger( newPoint.y );
            ClientTCP.SendData( buffer.ToArray() );
            CellPositionInWorld = newPoint;
            if( InCurrentChunk != inLastChunk )
                OnMovedChunk( InCurrentChunk );
        }
        private void OnMovedChunk( Vector3Int currentChunk ) {
            LPCOnChunkChange?.Invoke( inLastChunk, currentChunk );
            inLastChunk = currentChunk;
        }
        public void MoveToWorldSpace( Vector3 position, Vector2Int chunkCoordinates ) {
            transform.position = position;
        }
        private void World_OnClicked( Vector3Int cellClicked, Vector2 pointClicked ) {
            path = Pathfinder.GetPath( CellPositionInWorld, cellClicked );
            if( path == null ) {
                return;
            }
            if( path.Count == 0 ) {
                return;
            }
            //I mean... is this, does this... do ANTHING?
            this.pointClicked = pointClicked;
            if( path == null ) {
                SetTargetPosition( pointClicked );
                return;
            }
            if( path.Count == 0 ) {
                return;
            }
            MoveToNextNode();
        }
        private void OnDestroy() {
            World.OnClicked -= World_OnClicked;
        }
        private void FaceDirection( Vector2 worldDirection ) {
            animationController.FaceDirection( worldDirection );
        }
        public Vector3Int[] GetSurroundingChunks {
            get {
                return new Vector3Int[8]
                {
                                                                            //     Example localPlayerCharacter.InCurrentChunk == 1, 1
                        InCurrentChunk + Vector3Int.down + Vector3Int.left, //     (0, 0)
                        InCurrentChunk + Vector3Int.left,                    //     (0, 1)
                        InCurrentChunk + Vector3Int.up + Vector3Int.left,   //     (0, 2)
                        InCurrentChunk + Vector3Int.down,                   //     (1, 0)
                        InCurrentChunk + Vector3Int.up,                     //     (1, 2)
                        InCurrentChunk + Vector3Int.right + Vector3Int.down,//     (2, 0)
                        InCurrentChunk + Vector3Int.right,                  //     (2, 1)
                        InCurrentChunk + Vector3Int.up + Vector3Int.right   //     (2, 2)
                };
            }
        }
        public void SetTargetPosition( Vector2 worldPosition ) {
            MovingToward = worldPosition;
            animationController.SetAnimationMoveDirection( (Vector2)transform.position - MovingToward );
        }
        internal void Teleport( Vector3Int coordinates ) {
            transform.position = tilemap.CellToWorld( coordinates );
            this.CellPositionInWorld = coordinates;
            MovingToward = transform.position;
            inLastChunk = InCurrentChunk;
        }
    }
}