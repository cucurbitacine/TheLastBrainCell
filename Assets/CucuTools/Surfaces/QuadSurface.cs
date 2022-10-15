using System;
using UnityEngine;

namespace CucuTools.Surfaces
{
    /// <summary>
    /// Quad Surface with Width and Height
    /// </summary>
    [AddComponentMenu(Cucu.AddComponent + Cucu.SurfaceGroup + ObjectName, 1)]
    public sealed class QuadSurface : SurfaceBehaviour<QuadEntity>
    {
        public const string ObjectName = "Quad Surface";

        #region Properties

        public Vector2 Size
        {
            get => Entity.Size;
            set => Entity.Size = value;
        }

        public float Width
        {
            get => Entity.Width;
            set => Entity.Width = value;
        }
        
        public float Height
        {
            get => Entity.Height;
            set => Entity.Height = value;
        }
        
        public Vector3 Normal => Root.TransformDirection(LocalNormal);
        public Vector3 LocalNormal => Entity.Normal;

        #endregion

        #region SurfaceEntity

        protected override void SurfaceDrawGizmos()
        {
            CucuGizmos.color = Color.white;
            CucuGizmos.DrawLines(GetPoint(0, 0), GetPoint(0, 1), GetPoint(1, 1), GetPoint(1, 0), GetPoint(0, 0));
        }
        
        #endregion
    }
    
    [Serializable]
    public sealed class QuadEntity : SurfaceEntity
    {
        [SerializeField] private Vector2 _size = Vector2.one;

        public Vector2 Size
        {
            get => _size;
            set => _size = value;
        }

        public float Width
        {
            get => Size.x;
            set
            {
                var size = Size;
                size.x = value;
                Size = size;
            }
        }
        
        public float Height
        {
            get => Size.y;
            set
            {
                var size = Size;
                size.y = value;
                Size = size;
            }
        }

        public Vector3 Normal => -Vector3.forward;
        
        public override Vector3 GetPoint(Vector2 uv)
        {
            return (uv - Vector2.one * 0.5f) * Size;
        }

        public override Vector3 GetNormal(Vector2 uv)
        {
            return Normal;
        }
    }
}