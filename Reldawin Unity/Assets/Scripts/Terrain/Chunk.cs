using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace LowCloud.Reldawin
{
    public class Chunk : MonoBehaviour
    {
        public DoodadLoader doodadLoader;
        public List<Doodad> activeDoodads = new List<Doodad>(); //public for testing purposes
        public MeshFilter meshFilter;
        public Vector2Int ChunkIndex;
        public Tile[,] Tiles { get; set; }
        public Node[,] Nodes { get; set; }

        public void CreateTiles( Vector2Int chunkIndex, string data )
        {
            Vector2 position = MyMath.CellToIsometric( chunkIndex )* Chunk.Size;
            transform.position = position ;
            this.ChunkIndex = chunkIndex;
            Tiles = new Tile[Chunk.Size + 2, Chunk.Size + 2];
            Nodes = new Node[Chunk.Size, Chunk.Size];

            int index = 0;

            for ( ushort _y = 0; _y < Chunk.Size + 2; _y++ )
                for ( ushort _x = 0; _x < Chunk.Size + 2; _x++ )
                {
                    Vector2Int cellPosition = new Vector2Int( _x, _y );

                    Tiles[_x, _y] = Tile.GetTileByIndex( data[index++] );
                    Tiles[_x, _y].CellPositionInWorld = ( chunkIndex * Chunk.Size + cellPosition );
                    Tiles[_x, _y].CellPositionInChunk = cellPosition;

                    if(_x < Chunk.Size && _y < Chunk.Size)
                    {
                        Nodes[_x, _y] = new Node()
                        {
                            type = -1,
                            CellPositionInWorld = Tiles[_x, _y].CellPositionInWorld
                        };
                    }
                }

            SetUVChannel( 0 );  //regular tiles
            SetUVChannel( 1 ); //corners that overlap

            gameObject.name = "Enabled Chunk";
            gameObject.SetActive( true );

            ClientTCP.SendChunkDoodadQuery( chunkIndex );
        }

        public void SetUVChannel( int channel )
        {
            List<Vector2> uvs = new List<Vector2>();
            Mesh.GetUVs( 0, uvs ); // Get layer zero as its uvs are preset

            for ( int _x = 1; _x < Chunk.Size + 1; _x++ )
                for ( int _y = 1; _y < Chunk.Size + 1; _y++ )
                {
                    Tile[] neighbours = GetNeighbours( _x, _y );

                    Vector2[] quad1UVs = null;
                    Vector2[] quad2UVs = null;
                    Vector2[] quad3UVs = null;
                    Vector2[] quad4UVs = null;

                    switch ( channel )
                    {
                        case 0:
                            quad1UVs = SpriteLoader.GetQuadrantUVs( Tiles[_x, _y].TileType, 1 );
                            quad2UVs = SpriteLoader.GetQuadrantUVs( Tiles[_x, _y].TileType, 2 );
                            quad3UVs = SpriteLoader.GetQuadrantUVs( Tiles[_x, _y].TileType, 3 );
                            quad4UVs = SpriteLoader.GetQuadrantUVs( Tiles[_x, _y].TileType, 4 );
                            foreach ( Tile neighbour in neighbours )
                            {
                                // If it has a neighbour that is below it of a different type
                                if ( neighbour.TileType != Tiles[_x, _y].TileType && neighbour.GetLayer > Tiles[_x, _y].GetLayer )
                                {
                                    //Set the tile base to appear as part of its neighbour
                                    quad1UVs = SpriteLoader.GetQuadrantUVs( neighbour.TileType, 1 );
                                    quad2UVs = SpriteLoader.GetQuadrantUVs( neighbour.TileType, 2 );
                                    quad3UVs = SpriteLoader.GetQuadrantUVs( neighbour.TileType, 3 );
                                    quad4UVs = SpriteLoader.GetQuadrantUVs( neighbour.TileType, 4 );
                                    break;
                                }
                            }
                            break;

                        case 1:
                            //
                            quad1UVs = SpriteLoader.GetEmpty;
                            quad2UVs = SpriteLoader.GetEmpty;
                            quad3UVs = SpriteLoader.GetEmpty;
                            quad4UVs = SpriteLoader.GetEmpty;
                            foreach ( Tile neighbour in neighbours )
                            {
                                // If it has a neighbour that is below it of a different type
                                if ( neighbour.TileType != Tiles[_x, _y].TileType && neighbour.GetLayer > Tiles[_x, _y].GetLayer )
                                {
                                    quad1UVs = SpriteLoader.GetQuadrantUVs( Tiles[_x, _y].TileType, 1, neighbours );
                                    quad2UVs = SpriteLoader.GetQuadrantUVs( Tiles[_x, _y].TileType, 2, neighbours );
                                    quad3UVs = SpriteLoader.GetQuadrantUVs( Tiles[_x, _y].TileType, 3, neighbours );
                                    quad4UVs = SpriteLoader.GetQuadrantUVs( Tiles[_x, _y].TileType, 4, neighbours );
                                    break;
                                }
                            }
                            break;
                    }

                    int quad1 = ( ( ( ( _x - 1 ) * 2 ) + 1 ) * ( Chunk.Size * 2 ) + ( ( ( _y - 1 ) * 2 ) + 1 ) ) * 4;
                    int quad2 = ( ( ( ( _x - 1 ) * 2 ) + 1 ) * ( Chunk.Size * 2 ) + ( ( ( _y - 1 ) * 2 ) + 0 ) ) * 4;
                    int quad3 = ( ( ( ( _x - 1 ) * 2 ) + 0 ) * ( Chunk.Size * 2 ) + ( ( ( _y - 1 ) * 2 ) + 0 ) ) * 4;
                    int quad4 = ( ( ( ( _x - 1 ) * 2 ) + 0 ) * ( Chunk.Size * 2 ) + ( ( ( _y - 1 ) * 2 ) + 1 ) ) * 4;

                    for ( int index = 0; index < 4; index++ )
                    {
                        uvs[quad1 + index] = quad1UVs[index];
                        uvs[quad2 + index] = quad2UVs[index];
                        uvs[quad3 + index] = quad3UVs[index];
                        uvs[quad4 + index] = quad4UVs[index];
                    }
                }

            Mesh.SetUVs( channel, uvs );
        }

        private void OnMouseDown()
        {
            Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition );

            if ( Physics.Raycast( ray, out RaycastHit hit ) && hit.transform.CompareTag( "Terrain" ) && hit.transform == transform )
            {
                Vector2Int p = MyMath.IsometricToCell( hit.point );

                if ( Input.GetMouseButtonDown( 0 ) )
                {
                    if ( EventSystem.current.IsPointerOverGameObject())
                    {

                    }
                    else
                    {
                        OnClicked?.Invoke( p, hit.point );
                    }
                }
            }
        }
        
        public void SetDoodads(List<Doodad.Data> doodads, List<Doodad> inactiveDoodads)
        {        
            foreach ( Doodad.Data data in doodads)
            {
                inactiveDoodads[0].Setup(data);
                inactiveDoodads[0].transform.SetParent(transform); // something like this
                activeDoodads.Add( inactiveDoodads[0] );
                int x = data.tilePositionInChunk.x;
                int y = data.tilePositionInChunk.y;
                inactiveDoodads.RemoveAt(0);

                Nodes[x, y].type = data.type;
            }
        }

        public Tile[] GetNeighbours( int x, int y )
        {
            Tile[] neighbors = new Tile[8]
            {
                 Tiles[x + 0, y + 1], //forward
                 Tiles[x + 1, y + 0], //right
                 Tiles[x + 0, y - 1], //down
                 Tiles[x - 1, y + 0], //left
                 Tiles[x + 1, y + 1], //forard-right
                 Tiles[x + 1, y - 1], //back-right
                 Tiles[x - 1, y - 1], //back-left
                 Tiles[x - 1, y + 1]  //forward-left
            };

            return neighbors;
        }

        public void Disable()
        {
            gameObject.name = "Disabled Chunk";
            gameObject.SetActive( false );
            
            foreach ( Doodad doodad in activeDoodads )
            {
                doodad.gameObject.SetActive(false);
            }            
            
            OnChunkDestroyed?.Invoke( activeDoodads );
            activeDoodads.Clear();
        }

        public const ushort Size = 15;

        public static event ClickAction OnClicked;

        public static event ChunkDestroyImminent OnChunkDestroyed;

        public delegate void ClickAction( Vector2Int cellClicked, Vector2 pointClicked );

        public delegate void ChunkDestroyImminent( List<Doodad> doodadsToRecycle );

        private Mesh Mesh
        {
            get
            {
                return meshFilter.mesh;
            }
        }
    }
}
