using System;
using UnityEngine;

namespace CucuTools.Surfaces
{
    /// <summary>
    /// Circle Surface
    /// </summary>
    [AddComponentMenu(Cucu.AddComponent + Cucu.SurfaceGroup + ObjectName, 1)]
    public sealed class CircleSurface : SurfaceBehaviour<CircleEntity>
    {
        public const string ObjectName = "Circle Surface";

        #region Properties

        public float SectorAngle
        {
            get => Entity.SectorAngle;
            set => Entity.SectorAngle = value;
        }

        public float RadiusInner
        {
            get => Entity.RadiusInner;
            set => Entity.RadiusInner = value;
        }
        
        public float RadiusOuter
        {
            get => Entity.RadiusOuter;
            set => Entity.RadiusOuter = value;
        }

        public Vector3 Normal => GetNormal(0, 0);
        public Vector3 LocalNormal => GetLocalNormal(0, 0);
        
        #endregion

        #region SurfaceEntity

        protected override void SurfaceDrawGizmos()
        {
            CucuGizmos.DrawCircle(position, Normal, RadiusOuter);
            if (RadiusInner > 0f) CucuGizmos.DrawCircle(position, Normal, RadiusInner);
        }
        
        #endregion
    }

    [Serializable]
    public class CircleEntity : SurfaceEntity
    {
        public const float MinSectorAngle = 0f;
        public const float MaxSectorAngle = 360f;

        [Range(MinSectorAngle, MaxSectorAngle)]
        [SerializeField] private float _sectorAngle = 360f;
        [Min(0)]
        [SerializeField] private float radiusInner = 0f;
        [Min(0)]
        [SerializeField] private float radiusOuter = 0.5f;

        public float SectorAngle
        {
            get => _sectorAngle;
            set => _sectorAngle = Mathf.Clamp(value, MinSectorAngle, MaxSectorAngle);
        }
        
        public float RadiusInner
        {
            get => radiusInner;
            set => radiusInner = Mathf.Max(0, value);
        }
        
        public float RadiusOuter
        {
            get => radiusOuter;
            set => radiusOuter = Mathf.Max(0, value);
        }

        public Vector3 Normal => -Vector3.forward;
        
        public override Vector3 GetPoint(Vector2 uv)
        {
            var u = 1 - uv.x;
            var v = 1 - uv.y;
            
            var r = (1 - u) * RadiusInner + u * RadiusOuter;
            var deg = v * SectorAngle;
            var rad = Mathf.Deg2Rad * deg;
            return new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0) * r;
        }

        public override Vector3 GetNormal(Vector2 uv)
        {
            return Normal;
        }
    }
}