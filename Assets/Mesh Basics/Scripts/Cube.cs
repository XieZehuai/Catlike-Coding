using System;
using System.Collections;
using UnityEngine;

namespace MeshBasics
{
	public class Cube : MonoBehaviour
	{
        public int xSize;
        public int ySize;
        public int zSize;

        private Vector3[] vertices; // 保存所有顶点的坐标
        private Mesh mesh; // MeshFilter网格的引用

        private void Awake()
        {
            IEnumerator enumerator = Generate();
        }

        private IEnumerator Generate()
        {
            GetComponent<MeshFilter>().mesh = mesh = new Mesh();
            mesh.name = "Procedural Cube";

            WaitForSeconds wait = new WaitForSeconds(0.05f);

            int cornerVertices = 8;
            int edgeVertices = (xSize + ySize + zSize - 3) * 4;
            int x = xSize - 1, y = ySize - 1, z = zSize - 1;
            int faceVertices = (x * y + x * z + y * z) * 2;
            vertices = new Vector3[cornerVertices + edgeVertices + faceVertices];

            yield return wait;
        }

        private void OnDrawGizmos()
        {
            if (vertices == null)
            {
                return;
            }

            Gizmos.color = Color.black;
            for (int i = 0; i < vertices.Length; i++)
            {
                Gizmos.DrawSphere(vertices[i], 0.1f);
            }
        }
    }
}
