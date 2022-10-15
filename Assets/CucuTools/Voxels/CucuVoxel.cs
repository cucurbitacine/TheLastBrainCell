using System;
using System.Collections.Generic;
using UnityEngine;

namespace CucuTools.Voxels
{
    [Serializable]
    public struct CucuVoxel
    {
        [SerializeField] private Vector3Int cell;
        [SerializeField] private float size;

        public const float MinSize = 0.01f;
        
        public bool Value { get; set; }
        
        public Vector3Int Cell
        {
            get => cell;
            set => cell = value;
        }

        public float Size
        {
            get => size;
            set => size = value;
        }

        public Vector3 Size3 => Vector3.one * Size;

        public Vector3 Position => GetPosition(Cell, Size3);
        public Vector3 Center => GetCenter(Cell, Size3);
        
        public int i => Cell.x;
        public int j => Cell.y;
        public int k => Cell.z;

        public float x => Size3.x;
        public float y => Size3.y;
        public float z => Size3.z;
        
        public Vector3 Corner(CornerType cornerType)
        {
            return GetCorner(Position, Size3, cornerType);
        }
        
        public Vector3[] Corners(params CornerType[] cornerTypes)
        {
            var corners = new Vector3[cornerTypes.Length];
            for (var n = 0; n < cornerTypes.Length; n++)
                corners[n] = Corner(cornerTypes[n]);
            return corners;
        }

        public void Update(CucuVoxel voxel)
        {
            Cell = voxel.Cell;
            Size = voxel.Size;
        }
        
        #region Static

        public static CucuVoxel Create(Vector3Int cell, float size = 1f)
        {
            return new CucuVoxel(){Cell = cell, Size = size};
        }
        
        public static CucuVoxel Create(int i, int j, int k, float size = 1f)
        {
            return Create(new Vector3Int(i, j, k), size);
        }
        
        public static CucuVoxel Create(float size = 1f)
        {
            return Create(Vector3Int.zero, size);
        }
        
        public static explicit operator Vector3Int(CucuVoxel voxel)
        {
            return voxel.Cell;
        }
        
        public static implicit operator CucuVoxel(Vector3Int cell)
        {
            return new CucuVoxel() {Cell = cell};
        }
        
        public static Vector3 GetPosition(Vector3Int cell, Vector3 size)
        {
            return Vector3.Scale(cell, size);
        }
        
        public static Vector3 GetPosition(Vector3Int cell, float size = 1f)
        {
            return GetPosition(cell, Vector3.one * size);
        }
        
        public static Vector3 GetPosition(int i, int j, int k, Vector3 size)
        {
            return GetPosition(new Vector3Int(i, j, k), size);
        }
        
        public static Vector3 GetPosition(int i, int j, int k, float size = 1f)
        {
            return GetPosition(new Vector3Int(i, j, k), size);
        }
        
        public static Vector3 GetCenter(Vector3Int cell, Vector3 size)
        {
            return GetPosition(cell, size) + size * 0.5f;
        }
        
        public static Vector3 GetCenter(Vector3Int cell, float size = 1f)
        {
            return GetCenter(cell, Vector3.one * size);
        }

        public static Vector3 GetCenter(int i, int j, int k, Vector3 size)
        {
            return GetCenter(new Vector3Int(i, j, k), size);
        }
        
        public static Vector3 GetCenter(int i, int j, int k, float size = 1f)
        {
            return GetCenter(new Vector3Int(i, j, k), size);
        }
        
        public static Vector3Int GetCell(Vector3 position, Vector3 size)
        {
            var pos = new Vector3(
                position.x /= size.x,
                position.y /= size.y,
                position.z /= size.z);

            return new Vector3Int(
                Mathf.FloorToInt(pos.x),
                Mathf.FloorToInt(pos.y),
                Mathf.FloorToInt(pos.z));
        }

        public static Vector3Int GetCell(Vector3 position, float size = 1f)
        {
            return GetCell(position, Vector3.one * size);
        }

        public static Vector3 GetCorner(Vector3 position, Vector3 size, CornerType cornerType)
        {
            return position + Vector3.Scale(size, CornerPositionByType[cornerType]);
        }
        
        public static readonly Dictionary<CornerType, Vector3> CornerPositionByType = new Dictionary<CornerType, Vector3>()
        {
            {CornerType.RightTopForward, new Vector3(1, 1, 1)},
            {CornerType.RightTopBack, new Vector3(1, 1, 0)},
            {CornerType.RightBottomForward, new Vector3(1, 0, 1)},
            {CornerType.RightBottomBack, new Vector3(1, 0, 0)},

            {CornerType.LeftTopForward, new Vector3(0, 1, 1)},
            {CornerType.LeftTopBack, new Vector3(0, 1, 0)},
            {CornerType.LeftBottomForward, new Vector3(0, 0, 1)},
            {CornerType.LeftBottomBack, new Vector3(0, 0, 0)},
        };

        #endregion
    }

    public enum CornerType
    {
        RightTopForward,
        RightTopBack,
        RightBottomForward,
        RightBottomBack,
        
        LeftTopForward,
        LeftTopBack,
        LeftBottomForward,
        LeftBottomBack,
    }
}
