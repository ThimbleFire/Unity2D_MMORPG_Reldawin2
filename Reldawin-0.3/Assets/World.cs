using LowCloud.Reldawin;
using System;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using Color = UnityEngine.Color;

public class World : MonoBehaviour
{
    [Serializable]
    public class Tile
    {
        public Color color;
        public TileBase tileBase;

        public const float Width = 64;
        public const float Height = 32;
    }
    public class Chunk {
        public const int width = 16;
        public const int height = 16;
    }

    public static event ClickAction OnClicked;
    public delegate void ClickAction( Vector3Int cellClicked, Vector2 pointClicked );

    public Tilemap tileMap;

    public Tile[] tiles;

    private Dictionary<string, TileBase> keyValuePairs = new();

    public LocalPlayerCharacter lpc;

    private void Awake() {
        foreach( Tile t in tiles ) {
            keyValuePairs.Add( t.color.ToHexString(), t.tileBase );
        }

        LocalPlayerCharacter.LPCOnChunkChange += LocalPlayerCharacter_LPCOnChunkChange;
    }

    private void LocalPlayerCharacter_LPCOnChunkChange( Vector3Int lastChunk, Vector3Int newChunk ) {

        for( int y = newChunk.y - 1; y < newChunk.y + 2; y++ ) {
            for( int x = newChunk.x - 1; x < newChunk.x + 2; x++ ) {

                if( x <= 0 || x >= map.width / Chunk.width ||
                    y <= 0 || y >= map.height / Chunk.height )
                    continue;
                else
                    LoadChunk( x, y );
            }
        }
    }

    private void Start() {

        //lpc.MoveToWorldSpace()
    }

    private void OnMouseDown() {
        if( EventSystem.current.IsPointerOverGameObject() )
            return;

        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Vector3Int cellCoordinates = tileMap.WorldToCell(worldPosition);
        Debug.Log( cellCoordinates );
        OnClicked?.Invoke( cellCoordinates, worldPosition );
    }

    private void LoadChunk(int w, int h) {
        // Set up the grid cell size so it accomodates the tile size
        Grid grid = GetComponent<Grid>();
        grid.cellSize = new Vector2( Tile.Width / 100, Tile.Height / 100 );

        // Populate the world with tiles
        for( int y = Chunk.height * h; y < Chunk.height * ( h + 1 ); y++ ) {
            for( int x = Chunk.width * w; x < Chunk.width * ( w + 1 ); x++ ) {
                tileMap.SetTile(
                    new Vector3Int( y, -x ),
                    keyValuePairs[map.GetPixel( x, y ).ToHexString()]
                );
            }
        }

        tileMap.CompressBounds();
        //Pathfind.Setup( floor );
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        collider.size = new Vector3( tileMap.size.x * grid.cellSize.x, tileMap.size.y * grid.cellSize.y );
        collider.offset = new Vector2( tileMap.size.x * grid.cellSize.x / 2, 0.0f );
    }

    [SerializeField]
    private Texture2D map;
}
