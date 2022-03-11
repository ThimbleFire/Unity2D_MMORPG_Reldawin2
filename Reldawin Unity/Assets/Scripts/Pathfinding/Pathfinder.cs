using System;
using System.Collections.Generic;
using UnityEngine;

namespace LowCloud.Reldawin
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
        public Vector2Int CellPositionInGrid { get; set; }
        public Vector2Int CellPositionInWorld { get; set; }
        public Vector2 WorldPosition
        { 
            get 
            { 
                return MyMath.CellToIsometric( CellPositionInWorld ); 
            } 
        }
        public Node Parent { get; set; }

        public int GCost { get; set; }
        public int HCost { get; set; }
        public int FCost
        {
            get { return GCost + HCost; }
        }

        public Node()
        {
            GCost = 1;
            HCost = 0;
        }
    }

    public class Pathfinder
    {
        private static Node[,] nodes;

        /// <summary>
        /// Populate uses ever tile as a node
        /// </summary>
        public static void Populate()
        {
            nodes = new Node[Chunk.Size * 3, Chunk.Size * 3];
                        
            Chunk[,] chunks = ChunkLoader.Instance.GetChunkMap();
           
            for (int chunkY = 0; chunkY < chunks.GetLength(1); chunkY++)
            {
                for(int chunkX = 0; chunkX < chunks.GetLength(0); chunkX++)
                {
                    // if tile is out of map bounds then just ignore...
                    if ( chunks[chunkX, chunkY] == null )
                        continue;

                    for ( int tileY = 0; tileY < Chunk.Size; tileY++ )
                    {
                        for ( int tileX = 0; tileX < Chunk.Size; tileX++ )
                        {
                            Vector2Int tileWithChunkOffset = new Vector2Int(
                                chunkX * Chunk.Size + tileX,
                                chunkY * Chunk.Size + tileY );

                            Node n = chunks[chunkX, chunkY].Nodes[tileX, tileY];

                            nodes[tileWithChunkOffset.x, tileWithChunkOffset.y] = n;
                            nodes[tileWithChunkOffset.x, tileWithChunkOffset.y].CellPositionInGrid = tileWithChunkOffset;
                        }
                    }
                }
            }
        }

        /// <param name="start">The world tile coordinates of the entity attempting to move.</param>
        /// <param name="destination">The world tile coordinates of the tile clicked.</param>
        /// <param name="lastNodeIsInteractive">If the last node is occupied (interactive).</param>
        public static Queue<Node> GetPath( Vector2Int start, Vector2Int destination, out bool lastNodeIsInteractive )
        {
            lastNodeIsInteractive = false;

            if ( start == destination )
                return null;

            Node startNode = ChunkLoader.Instance.GetTile( start );
            Node destinationNode = ChunkLoader.Instance.GetTile(destination);

            List<Node> openSet = new List<Node>();
            HashSet<Node> closedSet = new HashSet<Node>();

            openSet.Add( startNode );

            while ( openSet.Count > 0 )
            {
                Node currentNode = openSet[0];

                for ( int i = 1; i < openSet.Count; i++ )
                {
                    if ( openSet[i].FCost < currentNode.FCost || openSet[i].FCost == currentNode.FCost && openSet[i].HCost < currentNode.HCost )
                    {
                        currentNode = openSet[i];
                        break;
                    }
                }

                openSet.Remove( currentNode );
                closedSet.Add( currentNode );

                if ( currentNode == destinationNode )
                {
                    lastNodeIsInteractive = destinationNode.Occupied;

                    return RetracePath( startNode, destinationNode );
                }

                foreach ( Node neighbour in GetAdjacentNodes( currentNode ) )
                {
                    if ( !closedSet.Contains( neighbour ) )
                    {
                        if ( neighbour.Occupied == true && neighbour != destinationNode )
                        {
                            continue;
                        }

                        int newMovementCostToNeighbour = currentNode.GCost + GetDistance( currentNode, neighbour );

                        if ( newMovementCostToNeighbour < neighbour.GCost || !openSet.Contains( neighbour ) )
                        {
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

        private static List<Node> GetAdjacentNodes( Node n )
        {
            List<Node> neighbours = new List<Node>();

            for ( int x = -1; x <= 1; x++ )
            {
                for ( int y = -1; y <= 1; y++ )
                {
                    if ( x == 0 && y == 0 )
                        continue;

                    int checkX = n.CellPositionInGrid.x + x;
                    int checkY = n.CellPositionInGrid.y + y;

                    bool checkXInBounds = checkX >= 0 && checkX < nodes.GetLength( 0 );
                    bool checkYInBounds = checkY >= 0 && checkY < nodes.GetLength( 1 );

                    if ( checkXInBounds && checkYInBounds )
                    {
                        if ( nodes[checkX, checkY] != null )
                        {
                            neighbours.Add( nodes[checkX, checkY] );
                        }
                    }
                }
            }

            return neighbours;
        }

        private static Queue<Node> RetracePath( Node startNode, Node destinationNode)
        {
            //We use a list because we want to reverse it later
            List<Node> path = new List<Node>();
            Node currentNode = destinationNode;

            // Remove nodes that aren't adjacent to an obsticle.
            // This allows floating-point, more natural movement
            while ( currentNode != startNode )
            {
                bool keep = false;

                foreach ( Node n in GetAdjacentNodes(currentNode) )
                {
                    if ( n.Occupied == true || currentNode == startNode || currentNode == destinationNode )
                        keep = true;
                }

                if ( keep )
                    path.Add( currentNode );
                
                    currentNode = currentNode.Parent;
            }

            path.Reverse();

            return new Queue<Node>( path ); ;
        }

        private static int GetDistance( Node a, Node b )
        {
            int disX = Mathf.Abs( a.CellPositionInGrid.x - b.CellPositionInGrid.x );
            int disY = Mathf.Abs( a.CellPositionInGrid.y - b.CellPositionInGrid.y );

            if ( disX > disY )
                return 14 * disY + 10 * ( disX - disY );
            else
                return 14 * disX + 10 * ( disY - disX );
        }
    }
}
