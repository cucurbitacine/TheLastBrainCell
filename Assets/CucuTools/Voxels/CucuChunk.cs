using System;
using UnityEngine;

namespace CucuTools.Voxels
{
    [Serializable]
    public class CucuChunk
    {
        #region SerializeField
        
        [SerializeField] private Vector3Int cell = Vector3Int.zero;
        [Range(MinRes, MaxRes)]
        [SerializeField] private int resolution = 16;
        [Min(0f)]
        [SerializeField] private float size = 1f;

        #endregion

        #region Const

        public const int MinRes = 1;
        public const int MaxRes = 64;

        #endregion

        #region Properties & Fields

        private CucuVoxel[,,] _voxels = default;
        
        private CucuVoxel[,,] Voxels
        {
            get => _voxels;
            set => _voxels = value;
        }

        public Vector3Int Cell
        {
            get => cell;
            set => cell = value;
        }

        public int Resolution
        {
            get => resolution;
            set => resolution = Mathf.Clamp(value, MinRes, MaxRes);
        }

        public float Size
        {
            get => size;
            set => size = Mathf.Max(0f, value);
        }

        public Vector3 Size3 => Vector3.one * Size;

        public float SizeVoxel
        {
            get => Size / Resolution;
            set => Size = value * Resolution;
        }
        
        public Vector3 SizeVoxel3 => Vector3.one * SizeVoxel;

        public Vector3 Position => Vector3.Scale(Cell, Size3);
        public Vector3 Center => Position + Size3 * 0.5f;
        
        public int i => Cell.x;
        public int j => Cell.y;
        public int k => Cell.z;
        
        public float x => Size3.x;
        public float y => Size3.y;
        public float z => Size3.z;

        public CucuVoxel this[int i, int j, int k]
        {
            get => Get(new Vector3Int(i, j, k));
            set => Set(new Vector3Int(i, j, k), value);
        }

        #endregion

        public CucuChunk(Vector3Int cell, int resolution, float size = 1f)
        {
            Cell = cell;
            Resolution = resolution;
            Size = size;
            Voxels = new CucuVoxel[Resolution, Resolution, Resolution];
        }
        
        public CucuChunk(int resolution, float size = 1f) : this(Vector3Int.zero, resolution, size)
        {
        }
        
        public CucuVoxel Get(int i, int j, int k)
        {
            var voxel = Voxels[i, j, k];
            voxel.Cell = new Vector3Int(i, j, k);
            voxel.Size = SizeVoxel;
            return voxel;
        }

        public CucuVoxel Get(Vector3Int cell)
        {
            return Get(cell.x, cell.y, cell.z);
        }
        
        public void Set(Vector3Int cell, CucuVoxel voxel)
        {
            Voxels[cell.x, cell.y, cell.z].Value = voxel.Value;
        }
        
        public bool IsValidCell(Vector3Int cell)
        {
            for (var n = 0; n < 3; n++)
            {
                if (cell[n] < 0 || Resolution <= cell[n]) return false;
            }

            return true;
        }

        public bool IsValidCell(int i, int j, int k)
        {
            return IsValidCell(new Vector3Int(i, j, k));
        }
        
        public bool TryGetVoxel(Vector3 position, out CucuVoxel voxel)
        {
            var cell = CucuVoxel.GetCell(position, SizeVoxel);
            voxel = CucuVoxel.Create(cell, SizeVoxel);
            
            if (!IsValidCell(cell)) return false;

            voxel = Get(cell);
            return true;
        }
        
        public void Reset()
        {
            Voxels = new CucuVoxel[Resolution, Resolution, Resolution];
        }

        public void Validate()
        {
            Resolution = Resolution;
            Size = Size;
        }
    }
}