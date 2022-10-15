using System;
using UnityEngine;

namespace CucuTools.Surfaces
{
    /// <summary>
    /// Pipe Surface
    /// </summary>
    [AddComponentMenu(Cucu.AddComponent + Cucu.SurfaceGroup + ObjectName, 1)]
    public sealed class PipeSurface : SurfaceBehaviour<PipeEntity>
    {
        public const string ObjectName = "Pipe Surface";
        
        #region Properties

        public float RadiusTop
        {
            get => Entity.RadiusTop;
            set => Entity.RadiusTop = value;
        }
        
        public float RadiusBottom
        {
            get => Entity.RadiusBottom;
            set => Entity.RadiusBottom = value;
        }
        
        public float Height
        {
            get => Entity.Height;
            set => Entity.Height = value;
        }

        public float Angle
        {
            get => Entity.Angle;
            set => Entity.Angle = value;
        }
        
        public Vector3 Direction => Root.TransformDirection(LocalDirection);
        public Vector3 LocalDirection => Entity.Direction;
        
        #endregion

        #region SurfaceEntity

        protected override void SurfaceDrawGizmos()
        {
            if (RadiusBottom != 0f) CucuGizmos.DrawCircle(position, Entity.Direction, RadiusBottom);
            if (RadiusTop != 0f) CucuGizmos.DrawCircle(position + Entity.Direction * Height, Entity.Direction, RadiusTop);

            var t = Cucu.LinSpace(0f, 0.75f, 4);
            for (int i = 0; i < t.Length; i++)
            {
                Gizmos.DrawLine(GetPoint(t[i], 0), GetPoint(t[i], 1));
            }
        }
        
        #endregion
    }

    [Serializable]
    public class PipeEntity : SurfaceEntity
    {
        public const float AngleMin = 0f;
        public const float AngleMax = 360f;
        
        [SerializeField] private float height = 2f;
        [SerializeField] private float radiusTop = 0.5f;
        [SerializeField] private float radiusBottom = 0.5f;
        [Range(AngleMin, AngleMax)]
        [SerializeField] private float angle = AngleMax;

        public float RadiusTop
        {
            get => radiusTop;
            set => radiusTop = value;
        }
        
        public float RadiusBottom
        {
            get => radiusBottom;
            set => radiusBottom = value;
        }
        
        public float Height
        {
            get => height;
            set => height = value;
        }

        public float Angle
        {
            get => angle;
            set => angle = Mathf.Clamp(value, AngleMin, AngleMax);
        }
        
        public Vector3 Direction => Vector3.forward; 
        
        public override Vector3 GetPoint(Vector2 uv)
        {
            var u = 1 - uv.x;
            var v = uv.y;

            var rad = u * Angle * Mathf.Deg2Rad;
            var r = (1f - v) * RadiusBottom + v * RadiusTop;
            var h = Height * v;

            return new Vector3(r * Mathf.Cos(rad), r * Mathf.Sin(rad), h);
        }

        public override Vector3 GetNormal(Vector2 uv)
        {
            var v = uv.y;
            
            var alpha = Mathf.Atan((RadiusBottom - RadiusTop) / Height) * Mathf.Rad2Deg;
            var r = (1f - v) * RadiusBottom + v * RadiusTop;

            var dir = GetPoint(uv).Scale(1, 1, 0).normalized * Mathf.Sign(r) * Mathf.Sign(Height);
            dir = Quaternion.AngleAxis(alpha, Vector3.Cross(dir, Direction)) * dir;

            if (v == 0f && RadiusBottom == 0f)
            {
                return Vector3.back;
            }

            if (Mathf.Abs(v - 1f) == 0f && RadiusTop == 0f)
            {
                return Vector3.forward;
            }
            
            return dir;
        }
    }
}