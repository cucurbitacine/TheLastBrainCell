using System;
using System.Linq;
using CucuTools.Colors;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CucuTools.Surfaces.Deformers
{
    public class TwistSurface : SurfaceDeformer
    {
        public float Angle;
        [SerializeField] private Vector3 axis = Vector3.up;

        public Vector3 Axis
        {
            get => axis.normalized;
            set => axis = value.normalized;
        }

        public override Vector3 GetLocalPoint(Vector2 uv)
        {
            if (Surface == null) return Vector3.zero;

            var point = transform.InverseTransformPoint(Surface.GetPoint(uv));
            return GetTwist(point) * point;
        }

        public override Vector3 GetLocalNormal(Vector2 uv)
        {
            if (Surface == null) return Vector3.zero;
            
            var point = transform.InverseTransformPoint(Surface.GetPoint(uv));
            var normal = transform.InverseTransformDirection(Surface.GetNormal(uv));
            
            return GetTwist(point) * normal;
        }

        private Quaternion GetTwist(Vector3 point)
        {
            var project = Vector3.Project(point, Axis);

            var t = project.magnitude * Mathf.Sign(Vector3.Dot(project, Axis));

            var angle = t * Angle;
            
            return Quaternion.AngleAxis(angle, Axis);
        }

        private bool haveCross;
        private Vector3 cross;

        protected override void OnDrawGizmosSelected()
        {
            base.OnDrawGizmosSelected();
            
            if (!haveCross)
            {
                cross = Random.onUnitSphere;
                haveCross = true;
            }

            if (cross == Axis)
            {
                haveCross = false;
                return;
            }

            var left = Vector3.Cross(Axis, cross);
            var forward = Vector3.Cross(left, Axis);
            
            var c_d = -Axis;
            var c_u = Axis;

            var l_d = c_d + left;
            var r_d = c_d - left;
            
            var l_u = c_u + left;
            var r_u = c_u - left;

            var f_d = c_d + forward;
            var b_d = c_d - forward;
            
            var f_u = c_u + forward;
            var b_u = c_u - forward;
            
            var t = Cucu.LinSpace(32);

            var leftLine = t.Select(v =>
            {
                var ptn = Vector3.Lerp(l_d, l_u, v);
                ptn = GetTwist(ptn) * ptn;
                return transform.TransformPoint(ptn);
            }).ToArray();
            
            var rightLine = t.Select(v =>
            {
                var ptn = Vector3.Lerp(r_d, r_u, v);
                ptn = GetTwist(ptn) * ptn;
                return transform.TransformPoint(ptn);
            }).ToArray();
            
            var forwardLine = t.Select(v =>
            {
                var ptn = Vector3.Lerp(f_d, f_u, v);
                ptn = GetTwist(ptn) * ptn;
                return transform.TransformPoint(ptn);
            }).ToArray();
            
            var backLine = t.Select(v =>
            {
                var ptn = Vector3.Lerp(b_d, b_u, v);
                ptn = GetTwist(ptn) * ptn;
                return transform.TransformPoint(ptn);
            }).ToArray();

            var centerLine = new[] { transform.TransformPoint(c_d), transform.TransformPoint(c_u) };
            
            Gizmos.color = Color.white;
            CucuGizmos.DrawLines(centerLine);

            CucuGizmos.DrawLines(leftLine);
            CucuGizmos.DrawLines(rightLine);
            CucuGizmos.DrawLines(forwardLine);
            CucuGizmos.DrawLines(backLine);

            Gizmos.color = CucuColor.Jet.Evaluate(0);
            
            CucuGizmos.DrawLines(centerLine[0], leftLine[0]);
            CucuGizmos.DrawLines(centerLine[0], rightLine[0]);
            CucuGizmos.DrawLines(centerLine[0], forwardLine[0]);
            CucuGizmos.DrawLines(centerLine[0], backLine[0]);

            Gizmos.color = CucuColor.Jet.Evaluate(1);

            CucuGizmos.DrawLines(centerLine[centerLine.Length - 1], leftLine[leftLine.Length - 1]);
            CucuGizmos.DrawLines(centerLine[centerLine.Length - 1], rightLine[rightLine.Length - 1]);
            CucuGizmos.DrawLines(centerLine[centerLine.Length - 1], forwardLine[forwardLine.Length - 1]);
            CucuGizmos.DrawLines(centerLine[centerLine.Length - 1], backLine[backLine.Length - 1]);
        }
    }
}