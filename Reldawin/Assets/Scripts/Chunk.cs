
using System;
using System.Collections.Generic;
using System.Xml;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using Color = UnityEngine.Color;

namespace AlwaysEast
{
  [Serializable]
    public class Tile
    {
        public char associatedCharacter;
        public List<TileBase> tileBases;
        public const byte Width = 64;
        public const byte Height = 32;
    }
    public interface IChunk { Vector3Int _index { get; } }
    public class Chunk : IChunk
    {
        public static event ChunkDestroyImminent OnChunkDestroyed;
        public delegate void ChunkDestroyImminent( Vector3Int chunkIndex, List<SceneObject> sceneObjects );

        public const byte width = 20;
        public const byte height = 20;
        public Vector3Int Index { get; set; }
        public Vector3Int _index { get { return Index; } }
        public Node[,] Nodes { get; set; } = new Node[width, height];
        public List<SceneObject> activeSceneObjects = new List<SceneObject>();
        
        public Chunk() {
            for( int y = 0; y < height; y++ )
            for( int x = 0; x <  width; x++ )
                Nodes[x, y] = new Node( this, new Vector3Int( x, y) );
        }
        public void Erase(Tilemap tileMap) {
            // Is erasing tiles neccesary? They'll just be overwritten when Reload is called.
            for( int y = 0; y < height; y++ )
                for( int x = 0; x < width; x++ )
                    tileMap.SetTile( Nodes[x, y].CellPositionInWorld, null );
            activeSceneObjects.ForEach( x => x.gameObject.SetActive( false ) );
            OnChunkDestroyed?.Invoke( Index, activeSceneObjects );
            activeSceneObjects.Clear();
        }
        public void Reload( Tilemap tileMap, Vector3Int index, string data, List<SceneObjectData> sceneObjectData, List<SceneObject> inactiveSceneObjects ) {
            Index = index;
            int iteration = 0;
            for( int y = 0; y < height; y++ )
            for( int x = 0; x < width; x++ ) {
                tileMap.SetTile(
                    Nodes[x, y].CellPositionInWorld, 
                    ResourceRepository.GetTilebaseOfType( data[iteration++] ) 
                    );
            }
            foreach( SceneObjectData objectData in sceneObjectData ) {
                Vector3 worldPosition = Nodes[objectData.x - ( index.x * Chunk.width ), objectData.y - ( index.y * Chunk.height )].WorldPosition;
                inactiveSceneObjects[0].Setup( ResourceRepository.GetSprite( objectData.Type ) );
                inactiveSceneObjects[0].transform.position = worldPosition;
                activeSceneObjects.Add( inactiveSceneObjects[0] );
                inactiveSceneObjects.RemoveAt( 0 );
            }
        }
    }
}
