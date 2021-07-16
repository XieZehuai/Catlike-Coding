using System;
using System.Collections;
using UnityEngine;

namespace MeshBasics
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class Grid : MonoBehaviour
    {
        public int xSize; // 网格的宽度
        public int ySize; // 网格的高度

        private Vector3[] vertices; // 保存所有顶点的坐标
        private Mesh mesh; // MeshFilter网格的引用

        private void Awake()
        {
            Generate();
        }

        private void Generate()
        {
            GetComponent<MeshFilter>().mesh = mesh = new Mesh();
            mesh.name = "Procedural Mesh";

            vertices = new Vector3[(xSize + 1) * (ySize + 1)];
            Vector2[] uv = new Vector2[vertices.Length];
            Vector4[] tangents = new Vector4[vertices.Length];
            Vector4 tangent = new Vector4(1f, 0f, 0f, -1f);

            for (int i = 0, y = 0; y <= ySize; y++)
            {
                for (int x = 0; x <= xSize; x++, i++)
                {
                    vertices[i] = new Vector3(x, y);
                    uv[i] = new Vector2((float)x / xSize, (float)y / ySize);
                    tangents[i] = tangent;
                }
            }

            mesh.vertices = vertices; // 指定网格的顶点
            mesh.uv = uv;
            mesh.tangents = tangents;

            int[] triangles = new int[xSize * ySize * 6];
            for (int ti = 0, vi = 0, y = 0; y < ySize; y++, vi++)
            {
                for (int x = 0; x < xSize; x++, ti += 6, vi++)
                {
                    triangles[ti] = vi;
                    triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                    triangles[ti + 4] = triangles[ti + 1] = vi + xSize + 1;
                    triangles[ti + 5] = vi + xSize + 2;
                }
            }

            mesh.triangles = triangles;
            mesh.RecalculateNormals();
        }

        private void OnDrawGizmos()
        {
            if (vertices == null) return;

            Gizmos.color = Color.black;
            for (int i = 0; i < vertices.Length; i++)
            {
                Gizmos.DrawSphere(vertices[i], 0.1f);
            }
        }
    }
}
