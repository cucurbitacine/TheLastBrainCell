using System.Collections.Generic;
using CucuTools.Attributes;
using UnityEngine;
using UnityEngine.Rendering;

namespace CucuTools.Voxels
{
    public class VoxelBehaviour : CucuBehaviour
    {
        [CucuLayer] public int LayerVoxel = 0;

        [Space] public CucuCluster Cluster;

        [Header("Origin")] public GameObject Origin;

        [Header("Editor")] public bool drawGizmos = false;
        
        [CucuButton()]
        public void Clear()
        {
            if (gameObject.TryGetComponent<MeshFilter>(out var meshFilter))
            {
                DestroyImmediate(meshFilter.sharedMesh);
                meshFilter.sharedMesh = null;
            }

            if (gameObject.TryGetComponent<MeshCollider>(out var meshCollider))
            {
                meshCollider.sharedMesh = null;
            }
        }

        [CucuButton()]
        public void Build()
        {
            Cluster.Position = transform.position;
            Cluster.Reset();
            
            var meshFilter = gameObject.GetComponent<MeshFilter>();
            if (meshFilter == null) return;

            var colliders = GetColliders(Origin, out var destroy, true);
            
            meshFilter.sharedMesh = Voxelization(ref Cluster, LayerVoxel, colliders);
            meshFilter.sharedMesh.name = Origin.name;
            
            if (gameObject.TryGetComponent<MeshCollider>(out var meshCollider))
            {
                meshCollider.sharedMesh = meshFilter.sharedMesh;
            }

            if (Application.isPlaying)
            {
                for (var i = 0; i < destroy.Count; i++)
                {
                    var mc = destroy[i];
                    Destroy(mc);
                }
            }
            else
            {
                for (var i = 0; i < destroy.Count; i++)
                {
                    var mc = destroy[i];
                    DestroyImmediate(mc);
                }
            }
        }

        public static Collider[] GetColliders(GameObject origin, out List<MeshCollider> toDestroy, bool useMeshCollider = false)
        {
            toDestroy = new List<MeshCollider>();
            
            var meshFilters = origin.GetComponentsInChildren<MeshFilter>();
            var colliders = new List<Collider>();
            for (var i = 0; i < meshFilters.Length; i++)
            {
                var meshFilter = meshFilters[i];
                var collider = meshFilter.GetComponent<Collider>();
                if (useMeshCollider && collider == null)
                {
                    var meshCollider = meshFilter.gameObject.AddComponent<MeshCollider>();
                    meshCollider.convex = true;
                    meshCollider.sharedMesh = meshFilter.sharedMesh;

                    toDestroy.Add(meshCollider);
                    
                    collider = meshCollider;
                }

                if (collider != null) colliders.Add(collider);
            }

            return colliders.ToArray();
        }
        
        public static Mesh Voxelization(ref CucuCluster cluster, int layerVoxel, params Collider[] colliders)
        {
            var meshInfo = new MeshInfo();

            var bounds = new Bounds();

            for (var i = 0; i < colliders.Length; i++)
            {
                if (i == 0)
                {
                    bounds = colliders[i].bounds;
                }
                else
                {
                    bounds.Encapsulate(colliders[i].bounds);
                }
            }

            cluster.Size = bounds.size;
            
            foreach (var collider in colliders)
            {
                meshInfo.Add(GetMeshInfo(ref cluster, layerVoxel, collider.gameObject, bounds));
            }
            
            var mesh = GetMesh(meshInfo);
            return mesh;
        }

        private static MeshInfo GetMeshInfo(ref CucuCluster cluster, int layerVoxel, GameObject gameObject, Bounds bounds)
        {
            if (gameObject == null) return new MeshInfo();

            var layerOrigin = gameObject.layer;
            gameObject.layer = layerVoxel;
            var layerMask = LayerMask.GetMask(LayerMask.LayerToName(layerVoxel));

            var meshInfo = new MeshInfo();
            
            for (var i = 0; i < cluster.ResolutionChunk.x; i++)
            {
                for (var j = 0; j < cluster.ResolutionChunk.y; j++)
                {
                    for (var k = 0; k < cluster.ResolutionChunk.z; k++)
                    {
                        var chunk = cluster.Get(i, j, k);
                        var center = chunk.Center + cluster.Position + (bounds.center - cluster.Center);
                        
                        if (Physics.CheckBox(center, chunk.Size3 * 0.5f, Quaternion.identity, layerMask))
                        {
                            var mi = GetMeshInfo(ref chunk, layerVoxel, gameObject, cluster, bounds);
                            meshInfo.Add(mi);
                        }
                    }
                }
            }
            
            gameObject.layer = layerOrigin;
                
            return meshInfo;
        }

        private static MeshInfo GetMeshInfo(ref CucuChunk chunk, int layerVoxel, GameObject gameObject, CucuCluster cluster, Bounds bounds)
        {
            if (gameObject == null) return new MeshInfo();

            var layerOrigin = gameObject.layer;
            gameObject.layer = layerVoxel;
            var layerMask = LayerMask.GetMask(LayerMask.LayerToName(layerVoxel));

            for (var i = 0; i < chunk.Resolution; i++)
            {
                for (var j = 0; j < chunk.Resolution; j++)
                {
                    for (var k = 0; k < chunk.Resolution; k++)
                    {
                        var voxel = chunk.Get(i, j, k);
                        
                        var center = voxel.Center + chunk.Position + cluster.Position + (bounds.center - cluster.Center);
                        voxel.Value = Physics.CheckBox(center, voxel.Size3 * 0.5f, Quaternion.identity, layerMask);

                        Debug.DrawLine(cluster.Center, bounds.center, Color.red, 1f);

                        chunk[i, j, k] = voxel;
                    }
                }
            }
            
            gameObject.layer = layerOrigin;
            
            VoxelBuilder builder = new CubeBuilder();
            return builder.BuildChunks(chunk);
        }
        
        private static Mesh GetMesh(MeshInfo meshInfo)
        {
            var mesh = new Mesh();
            mesh.indexFormat = IndexFormat.UInt32;
            
            mesh.vertices = meshInfo.vertices.ToArray();
            mesh.triangles = meshInfo.triangles.ToArray();
            mesh.uv = meshInfo.uv.ToArray();
            
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            mesh.Optimize();

            return mesh;
        }
        
        private void OnValidate()
        {
        }

        private void OnDrawGizmos()
        {
            if (!drawGizmos) return;

            Cluster.Position = transform.position;

            if (Origin != null)
            {
                var colliders = Origin.GetComponentsInChildren<Collider>();
                var bounds = new Bounds();

                for (var i = 0; i < colliders.Length; i++)
                {
                    var boundsCollider = colliders[i].bounds;
                    if (i == 0)
                    {
                        bounds = boundsCollider;
                    }
                    else
                    {
                        bounds.Encapsulate(boundsCollider);
                    }
                }

                Cluster.Size = bounds.size;
                
                for (var i = 0; i < colliders.Length; i++)
                {
                    var boundsCollider = colliders[i].bounds;
                    if (colliders.Length > 1)
                    {
                        Gizmos.color = Color.yellow;
                        Gizmos.DrawWireCube(boundsCollider.center + (Cluster.Position - bounds.center + bounds.extents), boundsCollider.size);
                    }
                }
            }
            
            Gizmos.color = Color.white;
            Gizmos.DrawWireCube(Cluster.Center, Cluster.Size);
        }

        private void OnDrawGizmosSelected()
        {
            Cluster.Reset();
            
            Gizmos.color = Color.magenta;
            for (var i = 0; i < Cluster.ResolutionChunk.x; i++)
            {
                for (var j = 0; j < Cluster.ResolutionChunk.y; j++)
                {
                    for (var k = 0; k < Cluster.ResolutionChunk.z; k++)
                    {
                        if (!Cluster.IsValidChunk(i, j, k)) continue;
                        
                        var chunk = Cluster.Get(i, j, k);
                        
                        Gizmos.DrawWireCube(chunk.Center + Cluster.Position, chunk.Size3);
                    }
                }
            }
        }
    }
}
