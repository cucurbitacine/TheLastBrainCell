using CucuTools.Surfaces.Tools;
using UnityEngine;

namespace CucuTools.Surfaces
{
    /// <summary>
    /// Surface Entity which defined by UV coordinates
    /// </summary>
    [DisallowMultipleComponent]
    public abstract class SurfaceBehaviour : CucuBehaviour
    {
        public SurfaceGizmos gizmos = new SurfaceGizmos();
        
        #region Abstract

        /// <summary>
        /// Get local point by coordinates
        /// </summary>
        /// <param name="uv"></param>
        /// <returns></returns>
        public abstract Vector3 GetLocalPoint(Vector2 uv);
        
        /// <summary>
        /// Get local normal at point by coordinates
        /// </summary>
        /// <param name="uv"></param>
        /// <returns></returns>
        public abstract Vector3 GetLocalNormal(Vector2 uv);

        #endregion
        
        #region Virtual

        /// <summary>
        /// Root Transform of the relative of which the surface is defined
        /// </summary>
        public virtual Transform Root => transform;
        
        #endregion
        
        #region API
        
        /// <summary>
        /// Position of Root Transform
        /// </summary>
        public Vector3 position
        {
            get => Root.position;
            set => Root.position = value;
        }

        /// <summary>
        /// Rotation of Root Transform
        /// </summary>
        public Quaternion rotation
        {
            get => Root.rotation;
            set => Root.rotation = value;
        }
        
        /// <summary>
        /// Get world point 
        /// </summary>
        /// <param name="uv"></param>
        /// <returns></returns>
        public Vector3 GetPoint(Vector2 uv)
        {
            return Root.TransformPoint(GetLocalPoint(uv));
        }

        /// <summary>
        /// Get world normal
        /// </summary>
        /// <param name="uv"></param>
        /// <returns></returns>
        public Vector3 GetNormal(Vector2 uv)
        {
            return Root.TransformDirection(GetLocalNormal(uv));
        }
        
        /// <summary>
        /// Get local point
        /// </summary>
        /// <param name="u"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public Vector3 GetLocalPoint(float u, float v)
        {
            return GetLocalPoint(new Vector2(u, v));
        }

        /// <summary>
        /// Get local normal at point
        /// </summary>
        /// <param name="u"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public Vector3 GetLocalNormal(float u, float v)
        {
            return GetLocalNormal(new Vector2(u, v));
        }
        
        /// <summary>
        /// Get point
        /// </summary>
        /// <param name="u"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public Vector3 GetPoint(float u, float v)
        {
            return GetPoint(new Vector2(u, v));
        }

        /// <summary>
        /// Get normal
        /// </summary>
        /// <param name="u"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public Vector3 GetNormal(float u, float v)
        {
            return GetNormal(new Vector2(u, v));
        }

        /// <summary>
        /// Get point with normal
        /// </summary>
        /// <param name="uv"></param>
        /// <param name="normal"></param>
        /// <returns></returns>
        public Vector3 GetPoint(Vector2 uv, out Vector3 normal)
        {
            normal = GetNormal(uv);
            return GetPoint(uv);
        }
        
        /// <summary>
        /// Get point with normal
        /// </summary>
        /// <param name="u"></param>
        /// <param name="v"></param>
        /// <param name="normal"></param>
        /// <returns></returns>
        public Vector3 GetPoint(float u, float v, out Vector3 normal)
        {
            return GetPoint(new Vector2(u, v), out normal);
        }
        
        /// <summary>
        /// Get local point with local normal
        /// </summary>
        /// <param name="uv"></param>
        /// <param name="localNormal"></param>
        /// <returns></returns>
        public Vector3 GetLocalPoint(Vector2 uv, out Vector3 localNormal)
        {
            localNormal = GetLocalNormal(uv);
            return GetLocalPoint(uv);
        }
        
        /// <summary>
        /// Get local point with local normal
        /// </summary>
        /// <param name="u"></param>
        /// <param name="v"></param>
        /// <param name="localNormal"></param>
        /// <returns></returns>
        public Vector3 GetLocalPoint(float u, float v, out Vector3 localNormal)
        {
            return GetLocalPoint(new Vector2(u, v), out localNormal);
        }
        
        #endregion

        protected virtual void OnDrawGizmos()
        {
            if (gizmos.Drawing && !gizmos.OnlySelected)
            {
                gizmos.Draw(this);
            }
            
            if(!gizmos.Drawing) SurfaceDrawGizmos();
        }

        protected virtual void OnDrawGizmosSelected()
        {
            if (gizmos.Drawing && gizmos.OnlySelected)
            {
                gizmos.Draw(this);
            }
            
            if(!gizmos.Drawing) SurfaceDrawGizmosSelected();
        }

        protected virtual void SurfaceDrawGizmos()
        {
        }
        
        protected virtual void SurfaceDrawGizmosSelected()
        {
        }
        
        #region Static

        public static Vector3 LerpPoint(SurfaceBehaviour A, SurfaceBehaviour B, Vector2 uv, float t)
        {
            return Vector3.Lerp(A.GetPoint(uv), B.GetPoint(uv), t);
        }

        public static Vector3 LerpLocalPoint(SurfaceBehaviour A, SurfaceBehaviour B, Vector2 uv, float t)
        {
            return Vector3.Lerp(A.GetLocalPoint(uv), B.GetLocalPoint(uv), t);
        }

        public static Vector3 LerpNormal(SurfaceBehaviour A, SurfaceBehaviour B, Vector2 uv, float t)
        {
            return Vector3.Lerp(A.GetNormal(uv), B.GetNormal(uv), t);
        }

        public static Vector3 LerpLocalNormal(SurfaceBehaviour A, SurfaceBehaviour B, Vector2 uv, float t)
        {
            return Vector3.Lerp(A.GetLocalNormal(uv), B.GetLocalNormal(uv), t);
        }

        public static Vector3 Lerp(SurfaceBehaviour A, SurfaceBehaviour B, Vector2 uv, float t, out Vector3 normal)
        {
            normal = LerpNormal(A, B, uv, t);
            return LerpPoint(A, B, uv, t);
        }
        
        public static Vector3 LerpLocal(SurfaceBehaviour A, SurfaceBehaviour B, Vector2 uv, float t, out Vector3 localNormal)
        {
            localNormal = LerpLocalNormal(A, B, uv, t);
            return LerpLocalPoint(A, B, uv, t);
        }
        
        #endregion
    }

    [DisallowMultipleComponent]
    public abstract class SurfaceBehaviour<TSurface> : SurfaceBehaviour where TSurface : SurfaceEntity, new()
    {
        [SerializeField] private TSurface entity;

        public TSurface Entity
        {
            get => entity ?? (Entity = new TSurface());
            set => entity = value;
        }

        /// <inheritdoc />
        public override Vector3 GetLocalPoint(Vector2 uv)
        {
            return Entity.GetPoint(uv);
        }

        /// <inheritdoc />
        public override Vector3 GetLocalNormal(Vector2 uv)
        {
            return Entity.GetNormal(uv);
        }
    }
    
    public abstract class SurfaceEntity
    {
        public abstract Vector3 GetPoint(Vector2 uv);
        public abstract Vector3 GetNormal(Vector2 uv);

        public Vector3 GetPoint(float u, float v) => GetPoint(new Vector2(u, v));
        public Vector3 GetNormal(float u, float v) => GetNormal(new Vector2(u, v));
        
        public virtual TSurface CloneEntity<TSurface>() where TSurface : SurfaceEntity, new()
        {
            return Cucu.Clone<TSurface>((TSurface) this);
        }
    }
}