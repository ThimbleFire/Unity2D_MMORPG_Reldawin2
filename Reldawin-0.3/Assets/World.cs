/*   Reldawin-0.3::World.cs
/    Author: Tony Boothroyd
/    Created: 21/07/2023
/    Description: 
*/

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
    public class Tile {
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
    public Tile[] tileTypes;
    public Grid grid;

    // tileTypes stored in a dictionary
    private Dictionary<string, TileBase> keyValuePairs = new();
    private Dictionary<Vector2Int, Chunk> loadedChunks = new();

    public LocalPlayerCharacter lpc;

    private void Awake() {
        // setup the grid cell size during runtime in case we change tile dimensions. Kind of unneccesary, remove once done debugging
        grid.cellSize = new Vector2( Tile.Width / 100, Tile.Height / 100 );
        //catalogue tileTypes in the form of a dictionary so we can access them easily
        foreach( Tile t in tileTypes ) {
            keyValuePairs.Add( t.color.ToHexString(), t.tileBase );
        }
    }

    private void LocalPlayerCharacter_LPCOnChunkChange( Vector3Int lastChunk, Vector3Int newChunk ) {
        Vector2Int dirOfTravel = newChunk - lastChunk;

        for ( int i = -1; i <= 1; i++ )
        {
            var dirTravelX = dirOfTravel.x == 0 ? i : dirOfTravel.x;
            var dirTravelY = dirOfTravel.y == 0 ? i : dirOfTravel.y;

            Vector2Int createChunkIndex = new Vector2Int( newChunk.x + dirTravelX, newChunk.y + dirTravelY );
            Vector2Int removeChunkIndex = new Vector2Int( lastChunk.x - dirTravelX, lastChunk.y - dirTravelY );

            RemoveChunk( removeChunkIndex );
            CreateChunk( createChunkIndex );
        }
    }

    private void Start() {
        // Create the starting chunks the player spawns in
        CreateChunk(lpc.InCurrentChunk);
        foreach(Vector2Int neighbour in lpc.GetSurroundingChunks)
            CreateChunk(neighbour);
        
        LocalPlayerCharacter.LPCOnChunkChange += LocalPlayerCharacter_LPCOnChunkChange;
    }

    private void OnMouseDown() {
        if( EventSystem.current.IsPointerOverGameObject() )
            return;

        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Vector3Int cellCoordinates = tileMap.WorldToCell(worldPosition);
        Debug.Log( cellCoordinates );
        OnClicked?.Invoke( cellCoordinates, worldPosition );
    }

    private void CreateChunk(Vector2Int chunkIndex) {
            
        if(loadedChunks.ContainsKey(chunkIndex))
            return;
    
        if(IsChunkOutOfBounds(chunkIndex))
            return;

        Chunk chunk = new Chunk();
        
        // Populate the world with default tiles
        for( int y = Chunk.height * chunkIndex.y; y < Chunk.height * ( chunkIndex.y + 1 ); y++ ) {
            for( int x = Chunk.width * chunkIndex.x; x < Chunk.width * ( chunkIndex.x + 1 ); x++ ) {
                tileMap.SetTile(
                    new Vector3Int( y, -x ),
                    keyValuePairs[map.GetPixel( x, y ).ToHexString()]
                );
            }
        }
        
        ///////////////////////////////////////////////////////

        //Send a message to the server to get scene object data

        ///////////////////////////////////////////////////////
        
        loadedChunks.Add(cell, chunk);

        UpdateTilemap();    
    }
    
    private void RemoveChunk( Vector2Int chunkIndex )
    {
        bool result = loadedChunks.TryGetValue( chunkIndex, out Chunk chunk );

         if ( result == false)
             return;
             
        if(IsChunkOutOfBounds(chunkIndex))
            return;

        //chunk.RecycleSceneObjects();

        // remove tiles that are out of bounds
        for( int y = Chunk.height * chunkIndex.y; y < Chunk.height * ( chunkIndex.y + 1 ); y++ ) {
            for( int x = Chunk.width * chunkIndex.x; x < Chunk.width * ( chunkIndex.x + 1 ); x++ ) {
                tileMap.SetTile( new Vector3Int( y, -x ), null);
            }
        }
    }

    private bool IsChunkOutOfBounds(Vector2Int chunkIndex) {
        if ( chunkIndex.x < 0 || 
             chunkIndex.y < 0 || 
             chunkIndex.x > map.Width / Chunk.width || 
             chunkIndex.y > map.Height / chunk.height )
             return true;
        else return false;
    }

    private void UpdateTilemap()  
        tileMap.CompressBounds();

        // Update Pathfinding
        //Pathfind.Setup( tileMap );
        
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        collider.size = new Vector3( tileMap.size.x * grid.cellSize.x, tileMap.size.y * grid.cellSize.y );
        collider.offset = new Vector2( tileMap.size.x * grid.cellSize.x / 2, 0.0f );
    }

    [SerializeField]
    private Texture2D map;
}
