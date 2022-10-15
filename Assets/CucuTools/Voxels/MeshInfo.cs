using System.Collections.Generic;
using UnityEngine;

namespace CucuTools.Voxels
{
    public class MeshInfo
    {
        public List<Vector3> vertices;
        public List<int> triangles;
        public List<Vector2> uv;

        public MeshInfo()
        {
            vertices = new List<Vector3>();
            triangles = new List<int>();
            uv = new List<Vector2>();
        }

        public void Add(MeshInfo meshInfo)
        {
            var verticesCount = vertices.Count;

            for (var i = 0; i < meshInfo.vertices.Count; i++)
            {
                vertices.Add(meshInfo.vertices[i]);
                uv.Add(meshInfo.uv[i]);
            }

            for (var i = 0; i < meshInfo.triangles.Count; i++)
            {
                triangles.Add(verticesCount + meshInfo.triangles[i]);
            }
        }
    }
}