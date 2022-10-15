using System;
using CucuTools.Blend;
using UnityEngine;

namespace CucuTools.Surfaces
{
    /// <summary>
    /// Blend Surface
    /// </summary>
    [AddComponentMenu(Cucu.AddComponent + Cucu.SurfaceGroup + ObjectName, 1)]
    public class BlendSurface : SurfaceBehaviour<BlendSurfaceEntity>, ICucuBlendable
    {
        public const string ObjectName = "Blend Surface";
        
        #region Properties

        public SurfaceBehaviour SurfaceA
        {
            get => Entity.SurfaceA;
            set => Entity.SurfaceA = value;
        }

        public SurfaceBehaviour SurfaceB
        {
            get => Entity.SurfaceB;
            set => Entity.SurfaceB = value;
        }

        #endregion

        #region ICucuBlendable

        /// <inheritdoc />
        public float Blend
        {
            get => Entity.Blend;
            set => Entity.Blend = value;
        }

        #endregion
        
        #region SurfaceEntity

        public override Vector3 GetLocalPoint(Vector2 uv)
        {
            return Root.InverseTransformPoint(Entity.GetPoint(uv));
        }

        public override Vector3 GetLocalNormal(Vector2 uv)
        {
            return Root.InverseTransformPoint(Entity.GetNormal(uv));
        }

        private void OnValidate()
        {
            if (SurfaceA != null && SurfaceA == this) SurfaceA = null;
            if (SurfaceB != null && SurfaceB == this) SurfaceB = null;
        }

        protected override void SurfaceDrawGizmos()
        {
            if (SurfaceA != null && SurfaceB != null)
            {
                Gizmos.color = Color.white;

                Gizmos.DrawLine(SurfaceA.GetPoint(0,0), SurfaceB.GetPoint(0,0));
                Gizmos.DrawLine(SurfaceA.GetPoint(1,0), SurfaceB.GetPoint(1,0));
                Gizmos.DrawLine(SurfaceA.GetPoint(1,1), SurfaceB.GetPoint(1,1));
                Gizmos.DrawLine(SurfaceA.GetPoint(0,1), SurfaceB.GetPoint(0,1));
            }
        }

        #endregion
    }

    [Serializable]
    public class BlendSurfaceEntity : SurfaceEntity, ICucuBlendable
    {
        [Range(0f, 1f)]
        [SerializeField] private float blend;
        [SerializeField] private SurfaceBehaviour surfaceA;
        [SerializeField] private SurfaceBehaviour surfaceB;

        /// <inheritdoc />
        public float Blend
        {
            get => blend;
            set => blend = Mathf.Clamp01(value);
        }

        public SurfaceBehaviour SurfaceA
        {
            get => surfaceA;
            set => surfaceA = value;
        }

        public SurfaceBehaviour SurfaceB
        {
            get => surfaceB;
            set => surfaceB = value;
        }
        
        public override Vector3 GetPoint(Vector2 uv)
        {
            if (SurfaceA == null || SurfaceB == null) return Vector3.zero;

            return SurfaceBehaviour.LerpPoint(SurfaceA, SurfaceB, uv, Blend);
        }

        public override Vector3 GetNormal(Vector2 uv)
        {
            if (SurfaceA == null || SurfaceB == null) return Vector3.zero;
            
            return SurfaceBehaviour.LerpNormal(SurfaceA, SurfaceB, uv, Blend);
        }
    }
}