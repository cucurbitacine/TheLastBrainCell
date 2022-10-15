using System;
using System.Collections.Generic;
using UnityEngine;

namespace CucuTools.Surfaces.Tools
{
    [Serializable]
    public class SurfaceMesh
    {
        [Header("Settings")]
        [SerializeField] private bool autoNormal = true;
        [SerializeField] private bool invertNormal = false;
        [SerializeField] private bool twoSide = false;
        
        [Header("Size")]
        [Range(SizeMin, SizeMax)]
        [SerializeField] private int sizeU = SizeDefault;
        [Range(SizeMin, SizeMax)]
        [SerializeField] private int sizeV = SizeDefault;

        public const int SizeDefault = 17;
        public const int SizeMin = 2;
        public const int SizeMax = 128;
        
        public bool AutoNormal
        {
            get => autoNormal;
            set => autoNormal = value;
        }

        public bool InvertNormal
        {
            get => invertNormal;
            set => invertNormal = value;
        }
        
        public bool TwoSide
        {
            get => twoSide;
            set => twoSide = value;
        }

        public int SizeU
        {
            get => sizeU;
            set => sizeU = Mathf.Clamp(value, SizeMin, SizeMax);
        }

        public int SizeV
        {
            get => sizeV;
            set => sizeV = Mathf.Clamp(value, SizeMin, SizeMax);
        }

        public Vector2Int Size
        {
            get => new Vector2Int(SizeU, SizeV);
            set
            {
                SizeU = value.x;
                SizeV = value.y;
            }
        }
        
        public static bool Build(SurfaceEntity surface, SurfaceMesh setting, out Mesh mesh)
        {
            mesh = new Mesh();
            
            if (surface == null) return false;
            if (setting == null) return false;
            
            if (setting.SizeU <= 1 || setting.SizeV <= 1) return false; 
            
            var u = Cucu.LinSpace(setting.SizeU);
            var v = Cucu.LinSpace(setting.SizeV);

            mesh.name = $"{(surface.GetType().Name)}_mesh";
            mesh.vertices = FillVertices(surface, u, v, setting.TwoSide);
            mesh.triangles = FillTriangles(u, v, setting.InvertNormal, setting.TwoSide);
            if (!setting.AutoNormal) mesh.normals = FillNormals(surface, u, v, setting.InvertNormal, setting.TwoSide);
            mesh.uv = FillUV(u, v, setting.TwoSide);
            
            if (setting.AutoNormal) mesh.RecalculateNormals();
            
            mesh.RecalculateBounds();
            mesh.RecalculateTangents();
            
            return true;
        }
        
        public static bool Build(SurfaceBehaviour surface, SurfaceMesh setting, out Mesh mesh)
        {
            mesh = new Mesh();
            
            if (surface == null) return false;
            if (setting == null) return false;
            
            if (setting.SizeU <= 1 || setting.SizeV <= 1) return false; 
            
            var u = Cucu.LinSpace(setting.SizeU);
            var v = Cucu.LinSpace(setting.SizeV);

            mesh.name = $"{(surface.GetType().Name)}_mesh";
            mesh.vertices = FillVertices(surface, u, v, setting.TwoSide);
            mesh.triangles = FillTriangles(u, v, setting.InvertNormal, setting.TwoSide);
            if (!setting.AutoNormal) mesh.normals = FillNormals(surface, u, v, setting.InvertNormal, setting.TwoSide);
            mesh.uv = FillUV(u, v, setting.TwoSide);
            
            
            if (setting.AutoNormal) mesh.RecalculateNormals();
            
            mesh.RecalculateBounds();
            mesh.RecalculateTangents();
            
            return true;
        }
        
        public bool Build(SurfaceEntity surface, out Mesh mesh)
        {
            return Build(surface, this, out mesh);
        }
        
        public bool Build(SurfaceBehaviour surface, out Mesh mesh)
        {
            return Build(surface, this, out mesh);
        }

        public static Vector3[] FillVertices(SurfaceEntity surface, IReadOnlyList<float> u, IReadOnlyList<float> v, bool twoSide = false)
        {
            var vertices = new List<Vector3>();
            for (int j = 0; j < v.Count; j++)
            {
                for (int i = 0; i < u.Count; i++)
                {
                    vertices.Add(surface.GetPoint(u[i], v[j]));
                }
            }

            if (twoSide)
            {
                var count = vertices.Count;
                for (int i = 0; i < count; i++)
                {
                    vertices.Add(vertices[i]);
                }
            }
            
            return vertices.ToArray();
        }

        public static Vector3[] FillVertices(SurfaceBehaviour surface, IReadOnlyList<float> u, IReadOnlyList<float> v, bool twoSide = false)
        {
            var vertices = new List<Vector3>();
            for (int j = 0; j < v.Count; j++)
            {
                for (int i = 0; i < u.Count; i++)
                {
                    vertices.Add(surface.GetLocalPoint(u[i], v[j]));
                }
            }

            if (twoSide)
            {
                var count = vertices.Count;
                for (int i = 0; i < count; i++)
                {
                    vertices.Add(vertices[i]);
                }
            }
            
            return vertices.ToArray();
        }
        
        public static int[] FillTriangles(IReadOnlyCollection<float> u, IReadOnlyCollection<float> v, bool invertNormal = false, bool twoSide = false)
        {
            var quadsCount = (u.Count - 1) * (v.Count - 1);

            var triangles = new int[(quadsCount * 2 * 3) * (twoSide ? 2 : 1)];

            var half = triangles.Length / 2;
            var count = u.Count * v.Count;
            
            for (int i = 0; i < quadsCount; i++)
            {
                var indexes = GetVerticesByQuad(i, u.Count, v.Count);

                if (!twoSide)
                {
                    if (invertNormal)
                    {
                        triangles[6 * i + 0] = indexes[0];
                        triangles[6 * i + 1] = indexes[2];
                        triangles[6 * i + 2] = indexes[1];

                        triangles[6 * i + 3] = indexes[0];
                        triangles[6 * i + 4] = indexes[3];
                        triangles[6 * i + 5] = indexes[2];
                    }
                    else
                    {
                        triangles[6 * i + 0] = indexes[0];
                        triangles[6 * i + 1] = indexes[1];
                        triangles[6 * i + 2] = indexes[2];

                        triangles[6 * i + 3] = indexes[0];
                        triangles[6 * i + 4] = indexes[2];
                        triangles[6 * i + 5] = indexes[3];
                    }
                }
                else
                {
                    triangles[6 * i + 0 + half] = indexes[0] + count;
                    triangles[6 * i + 1 + half] = indexes[2] + count;
                    triangles[6 * i + 2 + half] = indexes[1] + count;

                    triangles[6 * i + 3 + half] = indexes[0] + count;
                    triangles[6 * i + 4 + half] = indexes[3] + count;
                    triangles[6 * i + 5 + half] = indexes[2] + count;
                    
                    triangles[6 * i + 0] = indexes[0];
                    triangles[6 * i + 1] = indexes[1];
                    triangles[6 * i + 2] = indexes[2];

                    triangles[6 * i + 3] = indexes[0];
                    triangles[6 * i + 4] = indexes[2];
                    triangles[6 * i + 5] = indexes[3];
                }
            }

            return triangles;
        }

        public static Vector3[] FillNormals(SurfaceEntity surface, IReadOnlyList<float> u, IReadOnlyList<float> v, bool invertNormal = false, bool twoSide = false)
        {
            var normals = new List<Vector3>();
            for (int j = 0; j < v.Count; j++)
            {
                for (int i = 0; i < u.Count; i++)
                {
                    if (invertNormal && !twoSide)
                    {
                        normals.Add(surface.GetNormal(u[i], v[j]) * -1);
                    }
                    else
                    {
                        normals.Add(surface.GetNormal(u[i], v[j]));
                    }
                }
            }

            if (twoSide)
            {
                var count = normals.Count;
                for (int i = 0; i < count; i++)
                {
                    normals.Add(-normals[i]);
                }
            }
            
            return normals.ToArray();
        }
        
        public static Vector3[] FillNormals(SurfaceBehaviour surface, IReadOnlyList<float> u, IReadOnlyList<float> v, bool invertNormal = false, bool twoSide = false)
        {
            var normals = new List<Vector3>();
            for (int j = 0; j < v.Count; j++)
            {
                for (int i = 0; i < u.Count; i++)
                {
                    if (invertNormal && !twoSide)
                    {
                        normals.Add(surface.GetLocalNormal(u[i], v[j]) * -1);
                    }
                    else
                    {
                        normals.Add(surface.GetLocalNormal(u[i], v[j]));
                    }
                }
            }

            if (twoSide)
            {
                var count = normals.Count;
                for (int i = 0; i < count; i++)
                {
                    normals.Add(-normals[i]);
                }
            }
            
            return normals.ToArray();
        }
        
        public static Vector2[] FillUV(IReadOnlyList<float> u, IReadOnlyList<float> v, bool twoSide = false)
        {
            var uv = new List<Vector2>();
            for (int j = 0; j < v.Count; j++)
            {
                for (int i = 0; i < u.Count; i++)
                {
                    uv.Add(new Vector2(u[i], v[j]));
                }
            }
            
            if (twoSide)
            {
                var count = uv.Count;
                for (int i = 0; i < count; i++)
                {
                    uv.Add(uv[i]);
                }
            }
            
            return uv.ToArray();
        }
        
        public static int[] GetVerticesByQuad(int indexQuad, int countU, int countV)
        {
            var u = indexQuad % (countU - 1);
            var v = indexQuad / (countU - 1);

            var index0 = v * countU + u;
            var index1 = (v + 1) * countU + u;
            var index2 = (v + 1) * countU + u + 1;
            var index3 = v * countU + u + 1;

            return new int[] { index0, index1, index2, index3 };
        }
    }
}