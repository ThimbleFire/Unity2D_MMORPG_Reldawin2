using System;
using System.Collections.Generic;
using UnityEngine;

namespace AlwaysEast
{
    public class Node
    {
        public int type = -1;
        public bool Occupied 
        { 
            get 
            { 
                return type != -1; 
            }
        }
        private Vector3Int ChunkIndex { get; set; }
        public Vector3Int CellPositionInGrid { get; set; }
        public Vector3Int CellPositionInWorld
        {
            get
            {
                return new Vector3Int( CellPositionInGrid.x + Chunk.width * ChunkIndex.x, CellPositionInGrid.y + Chunk.height * ChunkIndex.y );
            }
        }
        public Vector3 WorldPosition
        { 
            get 
            {
                return World.gTileMap.CellToWorld( CellPositionInWorld );
            } 
        }
        public Node Parent { get; set; }

        public int GCost { get; set; }
        public int HCost { get; set; }
        public int FCost
        {
            get { return GCost + HCost; }
        }

        public Node( Vector3Int _cellPosGrid, Vector3Int _parentChunkIndex )
        {
            this.ChunkIndex = _parentChunkIndex;
            this.CellPositionInGrid = _cellPosGrid;
            GCost = 1;
            HCost = 0;
        }
    }

    public class Pathfinder
    {
        private static Node[,] nodes = new Node[Chunk.width * 3, Chunk.height * 3];

        private static Vector2Int BottomLeftNodeIndex { get; set; } = Vector2.zero;
        
        // We want to modify this so we're just sending chunks as a parameter.
        public static void Populate( List<Chunk> chunks, _bottomLeftNodeIndex) {

            foreach( Chunk chunk in chunks ) {

                foreach( Node n in chunk.Nodes ) {
                    nodes[n.CellPositionInWorld.x, n.CellPositionInWorld.y] = n;
                }
            }

            this.BottomLeftNodeIndex = _bottomLeftNodeIndex;            
        }
        public static Queue<Node> GetPath( Vector3Int start, Vector3Int destination ) {
            destination = new Vector3Int(destination.x, destination.y);
            if( start == destination )
                return null;
            Node startNode = nodes[start.x, start.y];
            Node destinationNode = nodes[destination.x, destination.y];
            List<Node> openSet = new List<Node>();
            HashSet<Node> closedSet = new HashSet<Node>();
            openSet.Add( startNode );
            while( openSet.Count > 0 ) {
                Node currentNode = openSet[0];
                for( int i = 1; i < openSet.Count; i++ ) {
                    if( openSet[i].FCost < currentNode.FCost || openSet[i].FCost == currentNode.FCost && openSet[i].HCost < currentNode.HCost ) {
                        currentNode = openSet[i];
                        break;
                    }
                }
                openSet.Remove( currentNode );
                closedSet.Add( currentNode );
                if( currentNode == destinationNode ) {
                    return RetracePath( startNode, destinationNode );
                }
                foreach( Node neighbour in GetNeighbours( currentNode ) ) {
                    if( !closedSet.Contains( neighbour ) ) {
                        if( neighbour.Occupied == true && neighbour != destinationNode ) {
                            continue;
                        }
                        int  newMovementCostToNeighbour = currentNode.GCost + GetDistance( currentNode, neighbour );
                        if( newMovementCostToNeighbour < neighbour.GCost || !openSet.Contains( neighbour ) ) {
                            neighbour.GCost = newMovementCostToNeighbour;
                            neighbour.HCost = GetDistance( neighbour, destinationNode );
                            neighbour.Parent = currentNode;
                            openSet.Add( neighbour );
                        }
                    }
                }
            }
            return null;
        }
        private static List<Node> GetNeighbours( Node n ) {
            List<Node> neighbours = new List<Node>();
            Vector3Int[] offset = new Vector3Int[8]
            {
                Vector3Int.down + Vector3Int.left,   // -1, -1
                Vector3Int.down,                     //  0, -1
                Vector3Int.down + Vector3Int.right,  // +1, -1
                Vector3Int.left,                     // -1, 0
                Vector3Int.right,                    // +1, 0
                Vector3Int.up + Vector3Int.left,     // -1, 1
                Vector3Int.up,                       //  0, 1
                Vector3Int.up + Vector3Int.right     // +1, 1
            };
            for( int i = 0; i < offset.Length; i++ ) {
                int checkX = n.CellPositionInWorld.x + offset[i].x - BottomLeftNodeIndex.x * Chunk.width;
                int checkY = n.CellPositionInWorld.y + offset[i].y - BottomLeftNodeIndex.y * Chunk.height;
                bool checkXInBounds = checkX >= 0 && checkX < nodes.GetLength( 0 );
                bool checkYInBounds = checkY >= 0 && checkY < nodes.GetLength( 1 );
                if( !checkXInBounds || !checkYInBounds ) continue;
                if( nodes[checkX, checkY] == null ) continue;
                if( nodes[checkX, checkY].Occupied == true ) continue;
                neighbours.Add( nodes[checkX, checkY] );
            }
            return neighbours;
        }
        private static Queue<Node> RetracePath( Node startNode, Node destinationNode ) {
            //We use a list because we want to reverse it later
            List<Node> path = new List<Node>();
            Node currentNode = destinationNode;
            // Remove nodes that aren't adjacent to an obsticle.
            // This allows floating-point, more natural movement
            while( currentNode != startNode ) {
                bool keep = false;
                foreach( Node n in GetNeighbours( currentNode ) ) {
                    if( n.Occupied == true || currentNode == startNode || currentNode == destinationNode )
                        keep = true;
                }
                if( keep )
                    path.Add( currentNode );
                currentNode = currentNode.Parent;
            }
            path.Reverse();
            return new Queue<Node>( path ); ;
        }
        private static int GetDistance( Node a, Node b ) {
            int disX = Mathf.Abs( a.CellPositionInWorld.x - b.CellPositionInWorld.x );
            int disY = Mathf.Abs( a.CellPositionInWorld.y - b.CellPositionInWorld.y );
            if( disX > disY )
                return 14 * disY + 10 * ( disX - disY );
            else
                return 14 * disX + 10 * ( disY - disX );
        }
    }
}
