using System;
using System.Collections.Generic;
using UnityEngine;

namespace LowCloud.Reldawin
{
    /// <summary>
    /// Base class for all living entities
    /// </summary>
    public class Entity : MonoBehaviour
    {
        // Position
        protected float MovementSpeed { get; set; }
        protected Vector2 MovingToward = Vector2.zero;
        public Vector2Int CellPositionInWorld;
        protected Vector2Int InLastChunk { get; set; }
        public const float RunSpeed = 0.025f;
        public const float WalkSpeed = 0.010f;
        protected bool Running { get; set; }
        protected bool Swimming { get; set; }
        public Vector2Int InCurrentChunk
        {
            get
            {
                return new Vector2Int( Mathf.FloorToInt( CellPositionInWorld.x / Chunk.Size ), Mathf.FloorToInt( CellPositionInWorld.y / Chunk.Size ) );
            }
        }
        public Vector2Int[] GetNeighbouringChunks
        {
            get
            {
                return new Vector2Int[8]
                {
                                                                            //      Example localPlayerCharacter.InCurrentChunk == 1, 1
                        InCurrentChunk + Vector2Int.up + Vector2Int.left,   // 0    (0, 2)
                        InCurrentChunk + Vector2Int.up,                     // 1    (1, 2)
                        InCurrentChunk + Vector2Int.up + Vector2Int.right,  // 2    (2, 2)
                        InCurrentChunk + Vector2Int.right,                  // 3    (2, 1)
                        InCurrentChunk + Vector2Int.right + Vector2Int.down,// 4    (2, 0)
                        InCurrentChunk + Vector2Int.down,                   // 5    (1, 0)
                        InCurrentChunk + Vector2Int.down + Vector2Int.left, // 6    (0, 0)
                        InCurrentChunk + Vector2Int.left                    // 7    (0, 1)
                };
            }
        }
        protected bool Interacting { get; set; }
        protected int interactingWith;
        protected Vector2Int interactingWithCell;

        protected Queue<Node> path;
        protected bool lastNodeOccupied = false;
        protected Vector2 pointClicked;
        protected bool HasInventorySpace = true;

        public AnimationController animationConroller;

        [SerializeField] private SpriteRenderer spriteRenderer;

        // Database
        public ushort DatabaseID { get; set; }

        private void FixedUpdate()
        {
            // If the player has not met their destination...
            if ( transform.position != (Vector3)MovingToward )
            {
                transform.position = Vector3.MoveTowards( transform.position, MovingToward, MovementSpeed );
                Vector2Int newPoint = MyMath.IsometricToCell( transform.position );

                if ( newPoint != CellPositionInWorld )
                {
                    OnMovedTile( newPoint, CellPositionInWorld );

                    if ( InCurrentChunk != InLastChunk )
                        OnMovedChunk( InCurrentChunk, InLastChunk );
                }

                // If moving the transform puts us in the position
                if ( transform.position == (Vector3)MovingToward )
                {
                    MoveToNextNode();
                }
            }
        }

        protected void MoveToNextNode()
        {
            using ( DebugTimer debugTimer = new DebugTimer( "MoveToNextNode" ) )
            {
                if ( path == null )
                {
                    OnAnimationDestinationMet();
                    return;
                }

                if ( path.Count == 0 )
                {
                    OnAnimationDestinationMet();
                    return;
                }

                Vector2 worldPosition = path.Peek().WorldPosition;

                if ( path.Count > 1 )
                {
                    SetTargetPosition( worldPosition );
                }
                else if ( path.Count == 1 )
                {
                    if ( lastNodeOccupied )
                    {
                        Vector2Int worldCell = path.Peek().CellPositionInWorld;
                        interactingWith = path.Peek().type;

                        path.Clear();
                        FaceDirection( worldPosition );
                        MovingToward = transform.position;
                        interactingWithCell = worldCell;

                        if ( HasInventorySpace )
                            Interact();

                        return;
                    }
                    else
                    {
                        SetTargetPosition( pointClicked );
                    }
                }

                path.Dequeue();
            }
        }
        
        protected virtual void OnMovedTile( Vector2Int newPoint, Vector2Int lastTile )
        {
            CellPositionInWorld = newPoint;
            spriteRenderer.sortingOrder = CellPositionInWorld.x * Chunk.Size + CellPositionInWorld.y;
        }

        //fired by LocalPlayerCharacter, OtherPlayerCharacter and by the network (forced stop)
        public virtual void Interrupt( params object[] args )
        {
            animationConroller.Interrupt();
        }

        protected virtual void OnMovedChunk( Vector2Int currentChunk, Vector2Int lastChunk )
        {
            InLastChunk = InCurrentChunk;
        }

        public virtual void Interact()
        {
            Interacting = true;

            animationConroller.Interact( interactingWithCell, interactingWith );
        }

        public virtual void ToggleRunning()
        {
            Running = !Running;

            MovementSpeed = Running ? RunSpeed : WalkSpeed;

            if ( Swimming )
                MovementSpeed = WalkSpeed;
            
            animationConroller.ToggleRun( Running );
        }

        public virtual void ToggleSwimming()
        {
            Swimming = !Swimming;

            if ( Running )
                ToggleRunning();

            animationConroller.ToggleSwimming( Swimming );
        }

        private void OnAnimationDestinationMet()
        {
            animationConroller.OnAnimationDestinationMet();
        }

        private void FaceDirection(Vector2 worldDirection)
        {
            animationConroller.FaceDirection( worldDirection );
        }

        public void SetTargetPosition( Vector2 worldPosition )
        {
            MovingToward = worldPosition;

            animationConroller.SetAnimationMoveDirection( (Vector2)transform.position - MovingToward );
        }
    }
}
