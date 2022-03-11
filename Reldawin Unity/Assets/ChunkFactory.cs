using System;
using System.Collections.Generic;
using UnityEngine;

namespace LowCloud.Reldawin
{
    [RequireComponent( typeof( MeshFilter ) )]
    public class ChunkFactory : MonoBehaviour
    {
        Mesh mesh;

        Vector3[] vertices;
        int[] triangles;
        Vector2[] uvs;

        int width = 2;
        int height = 1;

        public List<int> indexes;

        void Start()
        {
            SpriteLoader.Setup( 512, 1024 );

            indexes = new List<int>( 4 ) { 1, 3, 0, 2 };

            mesh = new Mesh();
            GetComponent<MeshFilter>().mesh = mesh;

            CreateShape();
        }

        private void Update()
        {
            SetUVs();
        }

        private void CreateShape()
        {
            SetVertices();
            SetTriangles();
            //SetUVs();

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            mesh.RecalculateTangents();
        }

        private void SetVertices()
        {
            vertices = new Vector3[( width + 1 ) * ( height + 1 )];

            for ( int i = 0, y = 0; y <= height; y++ )
            {
                for ( int x = 0; x <= width; x++ )
                {
                    vertices[i++] = //new Vector3( x, y, 0 );
                                    MyMath.CellToIsometric( x, y );
                }
            }

            mesh.vertices = vertices;

        }

        private void SetTriangles()
        {
            triangles = new int[width * height * 6];

            int vert = 0, tris = 0;
            for ( int y = 0; y < height; y++ )
            {
                for ( int x = 0; x < width; x++ )
                {
                    triangles[tris + 0] = vert + 0;
                    triangles[tris + 1] = vert + width + 1;
                    triangles[tris + 2] = vert + 1;
                    triangles[tris + 3] = vert + 1;
                    triangles[tris + 4] = vert + width + 1;
                    triangles[tris + 5] = vert + width + 2;

                    vert++;
                    tris += 6;
                }
                vert++;
            }

            mesh.triangles = triangles;
        }

        private void SetUVs()
        {
            Vector2[] grassUV = SpriteLoader.GetQuadrantUVs( 0 );
            uvs = new Vector2[( width + 1 ) * ( height + 1 )];

            for ( int y = 0; y < height; y++ )
            for ( int x = 0; x < width; x++ )
            {
                int r = (x * height + y) * 2;

                    Debug.Log( r );

                    //vertical orientation
                        uvs[r + 1] = grassUV[0];
                        uvs[r + 2] = grassUV[3];
                    //horizontal orientation
                        uvs[r + 0] = grassUV[2];
                        uvs[r + 3] = grassUV[1];
            }

            mesh.SetUVs( 0, uvs );
        }
    }
}