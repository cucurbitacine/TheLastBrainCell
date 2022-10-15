using System;
using UnityEngine;

namespace CucuTools.Surfaces
{
    /// <summary>
    /// Sphere Surface
    /// </summary>
    [AddComponentMenu(Cucu.AddComponent + Cucu.SurfaceGroup + ObjectName, 1)]
    public sealed class SphereSurface : SurfaceBehaviour<SphereEntity>
    {
        public const string ObjectName = "Sphere Surface";

        public float Radius
        {
            get => Entity.Radius;
            set => Entity.Radius = value;
        }
        
        /// <summary>
        /// Min Angle of Latitude.
        /// Like a split orange or tangerine
        /// </summary>
        public float Latitude
        {
            get => Entity.Latitude;
            set => Entity.Latitude = value;
        }

        /// <summary>
        /// Min Angle of Longitude.
        /// Like a pacman
        /// </summary>
        public float MinLongitude
        {
            get => Entity.MinLongitude;
            set => Entity.MinLongitude = value;
        }

        /// <summary>
        /// Max ANgle of Longitude.
        /// Like a pacman
        /// </summary>
        public float MaxLongitude
        {
            get => Entity.MaxLongitude;
            set => Entity.MaxLongitude = value;
        }

        #region SurfaceEntity

        protected override void SurfaceDrawGizmos()
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(position, Radius);
        }
        
        #endregion
    }

    [Serializable]
    public sealed class SphereEntity : SurfaceEntity
    {
        public const float MinLatitude = 0f;
        public const float MaxLatitude = 360f;
        public const float MinAngleLongitude = -90f;
        public const float MaxAngleLongitude = 90f;
        
        [Min(0)]
        [SerializeField] private float radius = 0.5f;
        
        [Header("Latitude")]
        [Range(MinLatitude, MaxLatitude)]
        [SerializeField] private float latitude = 360f;
        
        [Header("Longitude")]
        [Range(MinAngleLongitude, MaxAngleLongitude)]
        [SerializeField] private float minLongitude = -90f;
        [Range(MinAngleLongitude, MaxAngleLongitude)]
        [SerializeField] private float maxLongitude = 90f;

        public float Radius
        {
            get => radius;
            set => radius = Mathf.Max(0, value);
        }
        
        /// <summary>
        /// Min Angle of Latitude.
        /// Like a split orange or tangerine
        /// </summary>
        public float Latitude
        {
            get => latitude;
            set => latitude = Mathf.Clamp(value, MinLatitude, MaxLatitude);
        }

        /// <summary>
        /// Min Angle of Longitude.
        /// Like a pacman
        /// </summary>
        public float MinLongitude
        {
            get => minLongitude;
            set => minLongitude = Mathf.Clamp(value, MinAngleLongitude, MaxAngleLongitude);
        }

        /// <summary>
        /// Max ANgle of Longitude.
        /// Like a pacman
        /// </summary>
        public float MaxLongitude
        {
            get => maxLongitude;
            set => maxLongitude = Mathf.Clamp(value, MinAngleLongitude, MaxAngleLongitude);
        }

        public override Vector3 GetPoint(Vector2 uv)
        {
            var u = 1 - uv.x;
            var v = uv.y;
            
            var rad = (u * Latitude) * Mathf.Deg2Rad;
            var rad2 = ((1 - v) * MinLongitude + v * MaxLongitude)* Mathf.Deg2Rad;
            
            rad2 = Mathf.PI - (rad2 + Mathf.PI / 2);
            return new Vector3(Mathf.Cos(rad) * Mathf.Sin(rad2), Mathf.Sin(rad) * Mathf.Sin(rad2), Mathf.Cos(rad2)) * Radius;
        }

        public override Vector3 GetNormal(Vector2 uv)
        {
            return GetPoint(uv).normalized * Mathf.Sign(Radius);
        }
    }
}