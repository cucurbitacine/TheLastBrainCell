using System;
using UnityEngine;

namespace CucuTools.Blend
{
    public class CucuBlendRotateAround : CucuBlendTransformBase
    {
        public Transform Center;
        public float StartAngle;
        public float Radius;
        public Vector3 Axis;
        public Vector3 Axis2;
        public Vector3 Cross;

        private Quaternion prev;
        
        protected override void UpdateEntityInternal()
        {
            //if (Center != null) Target.RotateAround(Center.position, Axis, StartAngle);
            if (Center != null)
            {
                //Axis = Tar.forward;
                Axis2 = Axis + Vector3.one;
                Cross = Vector3.Cross(Axis, Axis2);

                prev = Target.rotation;
                Target.position = Center.position + Cross.normalized * Radius;
                Target.RotateAround(Center.position, Axis, StartAngle + 360 * Blend);
                Target.rotation = prev;
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (Center != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(Center.position, Center.position + Axis.normalized);
                Gizmos.color = Color.green;
                Gizmos.DrawLine(Center.position, Center.position + Axis2.normalized);
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(Center.position, Center.position + Cross.normalized);
            }
        }
    }
}