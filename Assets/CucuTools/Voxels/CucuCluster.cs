using System;
using UnityEngine;

namespace CucuTools.Voxels
{
    [Serializable]
    public class CucuCluster
    {
        [Min(CucuVoxel.MinSize)]
        [SerializeField] private float sizeVoxel = 0.1f;
        [Range(CucuChunk.MinRes, CucuChunk.MaxRes)]
        [SerializeField] private int voxelPerChunk = 16;
        [SerializeField] private Vector3 size = Vector3.one;
        
        private CucuChunk[,,] _chunks = default;

        private CucuChunk[,,] Chunks
        {
            get => _chunks;
            set => _chunks = value;
        }
        
        public float SizeVoxel
        {
            get => sizeVoxel;
            set => sizeVoxel = Mathf.Max(CucuVoxel.MinSize, value);
        }

        public int VoxelPerChunk
        {
            get => voxelPerChunk;
            set => voxelPerChunk = Mathf.Clamp(value, CucuChunk.MinRes, CucuChunk.MaxRes);
        }

        public Vector3 Size
        {
            get => size;
            set => size = new Vector3(
                              Mathf.CeilToInt(value.x / SizeChunk),
                              Mathf.CeilToInt(value.y / SizeChunk),
                              Mathf.CeilToInt(value.z / SizeChunk)
                          ) * SizeChunk;
        }

        public Vector3 Position { get; set; }
        public Vector3 Center
        {
            get => Position + Size * 0.5f;
            set => Position = value - Size * 0.5f;
        }

        public Vector3Int ResolutionChunk => new Vector3Int(
            Mathf.CeilToInt(Size.x / SizeChunk),
            Mathf.CeilToInt(Size.y / SizeChunk),
            Mathf.CeilToInt(Size.z / SizeChunk)
        );

        public float SizeChunk => VoxelPerChunk * SizeVoxel;
        
        public CucuChunk Get(int i, int j, int k)
        {
            return Chunks[i, j, k];
        }

        public CucuChunk Get(Vector3Int cell)
        {
            return Get(cell.x, cell.y, cell.z);
        }
        
        public bool IsValidCell(Vector3Int cell)
        {
            for (var n = 0; n < 3; n++)
            {
                if (cell[n] < 0 || ResolutionChunk[n] <= cell[n]) return false;
            }

            return true;
        }

        public bool IsValidCell(int i, int j, int k)
        {
            return IsValidCell(new Vector3Int(i, j, k));
        }
        
        public bool IsValidChunk(Vector3Int cell)
        {
            return IsValidCell(cell) && Chunks != null;
        }

        public bool IsValidChunk(int i, int j, int k)
        {
            return IsValidChunk(new Vector3Int(i, j, k));
        }
        
        public bool TryGetVoxel(Vector3 position, out CucuVoxel voxel, out CucuChunk chunk)
        {
            voxel = CucuVoxel.Create(CucuVoxel.GetCell(position, SizeVoxel), SizeVoxel);
            chunk = default;
            
            var cell = CucuVoxel.GetCell(position, SizeChunk);

            if (!IsValidCell(cell)) return false;
            chunk = Get(cell);

            if (chunk == null) return false;

            return chunk.TryGetVoxel(position - chunk.Position, out voxel);
        }
        
        public void Reset()
        {
            var resChunk = ResolutionChunk;

            Chunks = new CucuChunk[resChunk.x, resChunk.y, resChunk.z];
            for (var i = 0; i < resChunk.x; i++)
            {
                for (var j = 0; j < resChunk.y; j++)
                {
                    for (var k = 0; k < resChunk.z; k++)
                    {
                        Chunks[i, j, k] = new CucuChunk(new Vector3Int(i, j, k), VoxelPerChunk, VoxelPerChunk * SizeVoxel);
                    }
                }
            }
        }
    }
}