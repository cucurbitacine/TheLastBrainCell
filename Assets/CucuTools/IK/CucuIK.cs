using System;
using System.Collections.Generic;
using UnityEngine;

namespace CucuTools.IK
{
    [Serializable]
    public sealed class CucuIK
    {
        #region SerializeField

        [SerializeField] private Vector3 modificator = Vector3.one;
        
        [SerializeField] private bool freeOrigin = false;
        
        [Min(2)]
        [SerializeField] private int maxIteration = 8;
        [Min(0.001f)]
        [SerializeField] private float maxTolerance = 0.01f;
        
        #endregion

        #region Properties

        public bool FreeOrigin
        {
            get => freeOrigin;
            set => freeOrigin = value;
        }
        
        public float MaxTolerance
        {
            get => maxTolerance;
            set => maxTolerance = value;
        }

        public int MaxIteration
        {
            get => maxIteration;
            set => maxIteration = value;
        }

        public Vector3 Modificator
        {
            get => modificator;
            set => modificator = value;
        }
        
        #endregion

        #region Static API

        public static CucuIK Default { get; }
        
        static CucuIK()
        {
            Default = new CucuIK();
        }

        public static float[] GetLengths(params Vector3[] points)
        {
            if (points == null || points.Length < 1) return Array.Empty<float>();
            var lengths = new float[points.Length - 1];
            for (var i = 0; i < points.Length - 1; i++)
            {
                lengths[i] = Vector3.Distance(points[i], points[i + 1]);
            }
            return lengths;
        }
        
        public static bool SolveDefault(Vector3 target, Vector3[] points)
        {
            return Default.Solve(target, points);
        }

        public static bool SolveDefault(Vector3 target, Vector3[] points, float[] lengths)
        {
            return Default.Solve(target, points, lengths);
        }
        
        #endregion
        
        #region API

        public bool Solve(Vector3 target, Vector3[] points)
        {
            return Solve(target, points, GetLengths(points));
        }

        public bool Solve(Vector3 target, Vector3[] points, float[] lengths)
        {
            var start = points[0];
            var end = points[points.Length - 1];

            for (var n = 0; n < 3; n++)
            {
                target[n] = Mathf.Lerp(end[n], target[n], Modificator[n]);    
            }
            
            var startFromTarget = false;
            for (var iter = 0; iter < MaxIteration + MaxIteration % 2; iter++)
            {
                startFromTarget = iter % 2 == 0;

                Array.Reverse(points);
                Array.Reverse(lengths);

                points[0] = startFromTarget ? target : start;

                for (var i = 1; i < points.Length; i++)
                {
                    var dir = (points[i] - points[i - 1]).normalized;
                    var point = points[i - 1] + dir * lengths[i - 1];
                    points[i] = point;
                }

                if (FreeOrigin)
                {
                    Array.Reverse(points);
                    Array.Reverse(lengths);

                    return true;
                }
                
                if (!startFromTarget)
                {
                    var tolerance = Vector3.Distance(points[points.Length - 1], target);
                    if (tolerance <= MaxTolerance) return true;
                }
            }

            if (startFromTarget)
            {
                Array.Reverse(points);
                Array.Reverse(lengths);
            }
            
            return false;
        }

        #endregion
    }
}