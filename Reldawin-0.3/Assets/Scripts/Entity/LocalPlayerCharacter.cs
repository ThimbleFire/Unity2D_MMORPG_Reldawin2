using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace AlwaysEast
{
    public class LocalPlayerCharacter : MonoBehaviour
    {
        public Tilemap tilemap;

        public static event LPCOnChunkChangeHandler LPCOnChunkChange;
        public delegate void LPCOnChunkChangeHandler( Vector3Int lastChunk, Vector3Int newChunk );

        protected float MovementSpeed { get; set; } = WalkSpeed;
        protected Vector2 MovingToward = Vector2.zero;
        public const float RunSpeed = 0.025f;
        public const float WalkSpeed = 0.010f;
        protected bool Running { get; set; }

        public SpriteRenderer spriteRenderer;
        public AnimationController animationController;

        public Vector3Int CellPositionInWorld = new Vector3Int(0, 0, 0);

        public Vector3Int InCurrentChunk {
            get {
                return new Vector3Int( Mathf.FloorToInt( CellPositionInWorld.x / Chunk.width ), Mathf.FloorToInt( CellPositionInWorld.y / Chunk.height ) );
            }
        }
        public Vector3Int inLastChunk = new Vector3Int(0, 0);
        private Queue<Node> path;
        protected bool lastNodeOccupied = false;

        private void Awake() => World.OnClicked += World_OnClicked;

        private void Update() {
            if( InCurrentChunk != inLastChunk) {
                LPCOnChunkChange?.Invoke( inLastChunk, InCurrentChunk );
                inLastChunk = InCurrentChunk;
            }
        }

        private void FixedUpdate() {
            // If the player has not met their destination...
            if( transform.position != ( Vector3 )MovingToward ) {
                transform.position = Vector3.MoveTowards( transform.position, MovingToward, MovementSpeed );
                Vector3Int newPoint = World.gTileMap.WorldToCell( transform.position );

                if( newPoint != CellPositionInWorld ) {
                    OnMovedTile( newPoint, CellPositionInWorld );

                    if( InCurrentChunk != inLastChunk )
                        OnMovedChunk( InCurrentChunk, inLastChunk );
                }

                // If moving the transform puts us in the position
                if( transform.position == ( Vector3 )MovingToward ) {
                    MoveToNextNode();
                }
            }
        }

        protected void MoveToNextNode() {
            if( path == null ) {
                OnAnimationDestinationMet();
                return;
            }

            if( path.Count == 0 ) {
                OnAnimationDestinationMet();
                return;
            }

            Vector2 worldPosition = path.Peek().WorldPosition;

            if( path.Count > 1 ) {
                MovingToward = worldPosition;
            } else if( path.Count == 1 ) {
                if( lastNodeOccupied ) {
                    Vector3Int worldCell = path.Peek().CellPositionInWorld;

                    path.Clear();
                    FaceDirection( worldPosition );
                    MovingToward = transform.position;

                    return;
                } else {
                    MovingToward = worldPosition;
                }
            }

            path.Dequeue();
        }

        private void OnAnimationDestinationMet() {
            animationController.OnAnimationDestinationMet();
        }

        protected virtual void OnMovedTile( Vector3Int newPoint, Vector3Int lastTile ) {
            CellPositionInWorld = newPoint;
            spriteRenderer.sortingOrder = CellPositionInWorld.x * Chunk.height + CellPositionInWorld.y;
        }

        protected virtual void OnMovedChunk( Vector3Int currentChunk, Vector3Int lastChunk ) {
            inLastChunk = InCurrentChunk;
        }

        public void MoveToWorldSpace(Vector3 position, Vector2Int chunkCoordinates) {
            transform.position = position;
        }

        private void World_OnClicked( Vector3Int cellClicked, Vector2 pointClicked )
        {
            path = Pathfinder.GetPath( CellPositionInWorld, cellClicked );

            if( path == null ) {
                return;
            }
            if( path.Count == 0 ) {
                return;
            }

            if( path == null ) {
                MovingToward = pointClicked;

                return;
            }

            if( path.Count == 0 ) {
                return;
            }

            MoveToNextNode();
        }

        private void OnDestroy()
        {
            World.OnClicked -= World_OnClicked;
        }

        private void FaceDirection( Vector2 worldDirection ) {
            animationController.FaceDirection( worldDirection );
        }

        public Vector3Int[] GetSurroundingChunks
        {
            get
            {
                return new Vector3Int[8]
                {
                                                                            //     Example localPlayerCharacter.InCurrentChunk == 1, 1
                        InCurrentChunk + Vector3Int.down + Vector3Int.left, //     (0, 0)
                        InCurrentChunk + Vector3Int.left                    //     (0, 1)
                        InCurrentChunk + Vector3Int.up + Vector3Int.left,   //     (0, 2)
                        InCurrentChunk + Vector3Int.down,                   //     (1, 0)
                        InCurrentChunk + Vector3Int.up,                     //     (1, 2)
                        InCurrentChunk + Vector3Int.right + Vector3Int.down,//     (2, 0)
                        InCurrentChunk + Vector3Int.right,                  //     (2, 1)
                        InCurrentChunk + Vector3Int.up + Vector3Int.right,  //     (2, 2)
                };
            }
        }
    }
}
