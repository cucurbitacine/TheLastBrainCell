using System;
using CucuTools.Attributes;
using UnityEngine;
using Color = UnityEngine.Color;

namespace CucuTools.IK
{
    [DisallowMultipleComponent]
    public sealed class CucuIKBrain : CucuBehaviour
    {
        #region SerializeField
        
        [CucuReadOnly]
        [SerializeField] private bool solved = false;
        
        [Space]
        [SerializeField] private bool pause = false;
        
        [Header("IK")]
        [SerializeField] private CucuIK cucuIK = default;
        [SerializeField] private bool useInitialPoints = true;
        
        [Header("Target")]
        [SerializeField] private Vector3 targetLocal = default;
        [SerializeField] private CucuIKTarget targetIK = default;
        
        [Header("Points")]
        [SerializeField] private Vector3[] pointsLocal = default;

        #endregion
        
        #region Properties & Fields
        
        private Vector3[] _initialPoints = default;
        private float[] _lengths = default;
        private Vector3[] _gizmosPoints = default;

        public bool Solved
        {
            get => solved;
            private set => solved = value;
        }

        public CucuIK CucuIK
        {
            get => cucuIK != null ? cucuIK : (cucuIK = new CucuIK());
            set => cucuIK = value;
        }

        public bool UseInitialPoints
        {
            get => useInitialPoints;
            set => useInitialPoints = value;
        }

        private Vector3 TargetLocal
        {
            get => targetLocal;
            set => targetLocal = value;
        }

        public CucuIKTarget TargetIK
        {
            get => targetIK;
            set => targetIK = value;
        }
        
        private Vector3[] PointsLocal
        {
            get => pointsLocal != null ? pointsLocal : (pointsLocal = Array.Empty<Vector3>());
            set => pointsLocal = value;
        }
        
        private Vector3[] InitialPoints
        {
            get => _initialPoints;
            set => _initialPoints = value;
        }

        private float[] Lengths
        {
            get => _lengths;
            set => _lengths = value;
        }
        
        public int PointCount => PointsLocal.Length;
        
        #endregion

        #region IK

        public void SetTarget(Vector3 target, bool useWorldSpace = true)
        {
            TargetLocal = useWorldSpace ? transform.InverseTransformPoint(target) : target;
            
            if (TargetIK != null)
            {
                if (TargetIK.UseWorldSpace == useWorldSpace)
                {
                    TargetIK.TargetPosition = target;
                }
                else
                {
                    TargetIK.TargetPosition = TargetIK.UseWorldSpace
                        ? transform.TransformPoint(target)
                        : TargetLocal;
                }
            }
        }
        
        public Vector3 GetTarget(bool useWorldSpace = true)
        {
            if (TargetIK != null)
            {
                TargetLocal = TargetIK.UseWorldSpace
                    ? transform.InverseTransformPoint(TargetIK.TargetPosition)
                    : TargetIK.TargetPosition;

                if (TargetIK.UseWorldSpace == useWorldSpace) return TargetIK.TargetPosition;
            }
            
            return useWorldSpace ? transform.TransformPoint(TargetLocal) : TargetLocal;
        }
        
        public void SetPoints(Vector3[] points, bool useWorldSpace = true)
        {
            InitialPoints = new Vector3[points.Length];
            PointsLocal = new Vector3[points.Length];

            for (var i = 0; i < points.Length; i++)
            {
                InitialPoints[i] = useWorldSpace ? transform.InverseTransformPoint(points[i]) : points[i];
            }
            
            if (Application.isEditor && !Application.isPlaying)
            {
                Array.Copy(InitialPoints, PointsLocal, PointsLocal.Length);
            }
            
            Lengths = CucuIK.GetLengths(InitialPoints);
        }

        public Vector3 GetPoint(int index, bool useWorldSpace = true)
        {
            return useWorldSpace ? transform.TransformPoint(PointsLocal[index]) : PointsLocal[index];
        }
        
        public bool Solve()
        {
            if (PointCount < 2) return false;
            
            if (UseInitialPoints)
            {
                for (var i = 0; i < InitialPoints.Length; i++)
                {
                    PointsLocal[i] = InitialPoints[i];
                }
            }

            return CucuIK.Solve(GetTarget(false), PointsLocal, Lengths);
        }

        #endregion

        #region MonoBehaviour

        public bool Pause
        {
            get => pause;
            set => pause = value;
        }

        private void Awake()
        {
            SetPoints(PointsLocal, true);
        }

        private void Update()
        {
            if (Pause) return;

            Solved = Solve();
        }

        private void OnValidate()
        {
            if (TargetIK != null)
            {
                GetTarget();
            }
        }

        private void OnDrawGizmos()
        {
            if (PointsLocal.Length < 1) return;
            
            if (_gizmosPoints == null) _gizmosPoints = new Vector3[PointsLocal.Length];
            if (_gizmosPoints.Length != PointsLocal.Length) Array.Resize(ref _gizmosPoints, PointsLocal.Length);
            Array.Copy(PointsLocal, _gizmosPoints, PointsLocal.Length);
            
            for (var i = 0; i < _gizmosPoints.Length; i++)
            {
                _gizmosPoints[i] = transform.TransformPoint(_gizmosPoints[i]);
            }
            
            Gizmos.color = Color.green;
            CucuGizmos.DrawLines(_gizmosPoints);
            CucuGizmos.DrawWireSpheres(_gizmosPoints, 0.1f);

            var targetWorld = GetTarget(true);
            
            if (!Application.isPlaying)
            {
                CucuIK.Solve(targetWorld, _gizmosPoints, CucuIK.GetLengths(_gizmosPoints));

                Gizmos.color = Color.yellow;
                CucuGizmos.DrawLines(_gizmosPoints);
                CucuGizmos.DrawWireSpheres(_gizmosPoints, 0.1f);
            }

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(targetWorld, 0.1f);
        }
        
        #endregion
    }
}
