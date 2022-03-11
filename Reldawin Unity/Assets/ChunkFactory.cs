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

        int width = 16;
        int height = 16;

        void Start()
        {
            mesh = new Mesh();
            GetComponent<MeshFilter>().mesh = mesh;

            CreateShape();
            UpdateMesh();
        }

        private void Update()
        {

        }

        private void CreateShape()
        {
            vertices = new Vector3[( width + 1 ) * ( height + 1 )];

            for ( int i = 0, y = 0; y <= height; y++ )
            {
                for ( int x = 0; x <= width; x++ )
                {
                    vertices[i++] = MyMath.CellToIsometric( x, y );
                }
            }

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
        }

        private void UpdateMesh()
        {
            mesh.Clear();

            mesh.vertices = vertices;
            mesh.triangles = triangles;

            mesh.RecalculateNormals();
        }
    }
}