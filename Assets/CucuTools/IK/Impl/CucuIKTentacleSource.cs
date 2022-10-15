using UnityEngine;

namespace CucuTools.IK.Impl
{
    public sealed class CucuIKTentacleSource : CucuIKSource
    {
        [Header("Tentacle")]
        [Min(0f)]
        public float Length = 1f;
        [Min(2)]
        public int PointCount = 4;

        public Vector3 Direction = Vector3.forward;
        
        public override Vector3[] GetPoints()
        {
            var points = new Vector3[PointCount];

            var dir = Direction.normalized;

            var t = Cucu.LinSpace(0, Length, PointCount);
            
            for (var i = 0; i < t.Length; i++)
            {
                points[i] = t[i] * dir;
            }

            return points;
        }

        private void OnValidate()
        {
            SetPoints();
        }
    }
}