using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using static World;

namespace LowCloud.Reldawin
{
    public class LocalPlayerCharacter : MonoBehaviour
    {
        public Tilemap tilemap;

        public static event LPCOnChunkChangeHandler LPCOnChunkChange;
        public delegate void LPCOnChunkChangeHandler( Vector3Int lastChunk, Vector3Int newChunk );

        public Vector3Int CellPositionInWorld { get { return tilemap.WorldToCell( transform.position ); } }
        public Vector3Int InCurrentChunk { get { return new Vector3Int( -Mathf.FloorToInt( CellPositionInWorld.y / Chunk.width ), Mathf.FloorToInt( CellPositionInWorld.x / Chunk.height ) ); } }
        public Vector3Int inLastChunk = new Vector3Int(-1, -1);

        private void Awake()
        {
            World.OnClicked += World_OnClicked;
        }

        private void Update() {
            if( InCurrentChunk != inLastChunk) {
                LPCOnChunkChange.Invoke( inLastChunk, InCurrentChunk );
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
    }
}
