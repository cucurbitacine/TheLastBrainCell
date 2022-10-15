using UnityEngine;

namespace CucuTools.Surfaces.Deformers
{
    public class WaveSurface : SurfaceDeformer
    {
        public Vector3 Abscissa = Vector3.right;
        public Vector3 Ordinate = Vector3.back;
        
        public float Offset = 0f;
        public float Scale = 1f;
        public float Power = 1f;
        
        public override Vector3 GetLocalPoint(Vector2 uv)
        {
            if (Surface == null) return Vector3.zero;
            
            var point = Surface.GetPoint(uv).ToLocalPoint(Root);
            var project = Vector3.Project(point, Abscissa);
            
            var x = project.magnitude * Mathf.Sign(Vector3.Dot(project, Abscissa));

            x *= 180;
            x += Offset;
            x *= Scale;

            var y = Mathf.Sin(x * Mathf.Deg2Rad) * Power;
            
            return point + Ordinate.normalized * y;
        }

        public override Vector3 GetLocalNormal(Vector2 uv)
        {
            if (Surface == null) return Vector3.zero;
            
            return Surface.GetNormal(uv).ToLocalDirection(Root);
        }

        private void OnValidate()
        {
            Offset = Mathf.Repeat(Offset, 360f);
        }
    }
}