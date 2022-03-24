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
            transform.position = MyMath.CellToIsometric( chunkIndex ) * Chunk.Size;
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
            }

            for ( ushort _y = 0; _y < Chunk.Size; _y++ )
            for ( ushort _x = 0; _x < Chunk.Size; _x++ )
            {
                Nodes[_x, _y] = new Node()
                {
                    type = -1,
                    CellPositionInWorld = Tiles[_x, _y].CellPositionInWorld
                };
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
            {
                for ( int _y = 1; _y < Chunk.Size + 1; _y++ )
                {
                    Tile[] neighbours = GetNeighbours( _x, _y );
                    Vector2[] UVs;

                    foreach ( Tile neighbour in neighbours )
                    {
                        if ( channel == 0 )
                        {
                            UVs = SpriteLoader.TileUVDictionary[XMLLoader.Tile[Tiles[_x, _y].TileType].name + "_" + Random.Range(0, 16)];
                            break;
                        }
                        
                        UVs = SpriteLoader.TileUVDictionary["Empty"];
                        if ( neighbour.TileType != Tiles[_x, _y].TileType && neighbour.GetLayer > Tiles[_x, _y].GetLayer )
                        {
                            if ( channel == 1 )
                            {
                                UVs = SpriteLoader.GetTileUVs( Tiles[_x, _y].TileType, neighbours );
                                break;
                            }
                        }
                    }

                    //_x and _y are decreased by 1 because the nested for loops above do shit. 
                    int r = ( ( _x - 1 ) * Chunk.Size + ( _y - 1 ) ) * 4;

                    for ( int index = 0; index < 4; index++ )
                    {
                        uvs[r + index] = UVs[index];
                    }
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
                    if ( EventSystem.current.IsPointerOverGameObject() )
                    {

                    }
                    else
                    {
                        OnClicked?.Invoke( p, hit.point );
                    }
                }
            }
        }

        public void SetDoodads( List<Doodad.Data> doodads, List<Doodad> inactiveDoodads )
        {
            foreach ( Doodad.Data data in doodads )
            {
                inactiveDoodads[0].Setup( data );
                inactiveDoodads[0].transform.SetParent( transform ); // something like this
                activeDoodads.Add( inactiveDoodads[0] );
                int x = data.tilePositionInChunk.x;
                int y = data.tilePositionInChunk.y;
                inactiveDoodads.RemoveAt( 0 );

                Nodes[x, y].type = data.type;
            }
        }

        public Tile[] GetNeighbours( int x, int y )
        {
            try
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
            catch ( System.Exception )
            {
                Debug.LogError( string.Format( "{0} , {1}", x, y ) );
            }

            return null;
        }

        public void Disable()
        {
            gameObject.name = "Disabled Chunk";
            gameObject.SetActive( false );

            foreach ( Doodad doodad in activeDoodads )
            {
                doodad.gameObject.SetActive( false );
            }

            OnChunkDestroyed?.Invoke( ChunkIndex, activeDoodads );
            activeDoodads.Clear();
        }

        public const ushort Size = 30;

        public static event ClickAction OnClicked;

        public static event ChunkDestroyImminent OnChunkDestroyed;

        public delegate void ClickAction( Vector2Int cellClicked, Vector2 pointClicked );

        public delegate void ChunkDestroyImminent( Vector2Int chunkIndex, List<Doodad> doodads );

        private Mesh Mesh
        {
            get
            {
                return meshFilter.mesh;
            }
        }
    }
}
