using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace AlwaysEast
{
    public class LocalPlayerCharacter : MonoBehaviour
    {
        public Tilemap tilemap;

        public static event LPCOnChunkChangeHandler LPCOnChunkChange;
        public delegate void LPCOnChunkChangeHandler( Vector3Int lastChunk, Vector3Int newChunk );

        public Vector3Int CellPositionInWorld { get { return tilemap.WorldToCell( transform.position ); } }
        public Vector3Int InCurrentChunk { get { return new Vector3Int( -Mathf.FloorToInt( CellPositionInWorld.y / Chunk.width ), Mathf.FloorToInt( CellPositionInWorld.x / Chunk.height ) ); } }
        public Vector3Int inLastChunk = new Vector3Int(-1, -1);

        private void Awake() => World.OnClicked += World_OnClicked;

        private void Update() {
            if( InCurrentChunk != inLastChunk) {
                LPCOnChunkChange?.Invoke( inLastChunk, InCurrentChunk );
                inLastChunk = InCurrentChunk;
            }
        }

        public void MoveToWorldSpace(Vector3 position, Vector2Int chunkCoordinates) {
            transform.position = position;
        }

        private void World_OnClicked( Vector3Int cellClicked, Vector2 pointClicked )
        {
            //path = Pathfinder.GetPath( CellPositionInWorld, cellClicked, out lastNodeOccupied );
            //if(Interacting)
            //    Interrupt();
            //base.pointClicked = pointClicked;
            //if ( path == null )
            //{
            //    SetTargetPosition( pointClicked );
            //    return;
            //}
            //if ( path.Count == 0 )
            //{
            //    return;
            //}
            //MoveToNextNode();
        }

        private void OnDestroy()
        {
            World.OnClicked -= World_OnClicked;
        }

        public Vector3Int[] GetSurroundingChunks
        {
            get
            {
                return new Vector3Int[8]
                {
                                                                            //      Example localPlayerCharacter.InCurrentChunk == 1, 1
                        InCurrentChunk + Vector3Int.up + Vector3Int.left,   // 0    (0, 2)
                        InCurrentChunk + Vector3Int.up,                     // 1    (1, 2)
                        InCurrentChunk + Vector3Int.up + Vector3Int.right,  // 2    (2, 2)
                        InCurrentChunk + Vector3Int.right,                  // 3    (2, 1)
                        InCurrentChunk + Vector3Int.right + Vector3Int.down,// 4    (2, 0)
                        InCurrentChunk + Vector3Int.down,                   // 5    (1, 0)
                        InCurrentChunk + Vector3Int.down + Vector3Int.left, // 6    (0, 0)
                        InCurrentChunk + Vector3Int.left                    // 7    (0, 1)
                };
            }
        }
    }
}
