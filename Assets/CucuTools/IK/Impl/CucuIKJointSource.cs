using System;
using System.Linq;
using UnityEngine;

namespace CucuTools.IK.Impl
{
    public sealed class CucuIKJointSource : CucuIKSource
    {
        [Header("Damp")]
        public bool UseDamp = false;
        public float Damp = 8f;
        
        [Header("Joints")]
        public bool SyncJoints = false;
        public SyncMode SyncMode = SyncMode.Position;
        public Transform[] Joints = default;
        
        public override Vector3[] GetPoints()
        {
            if (Joints == null) return Array.Empty<Vector3>();

            return Joints.Where(j => j != null).Distinct().Select(j => j.position).ToArray();
        }

        private Vector3[] Positions;
        private Quaternion[] Rotations;

        private void Awake()
        {
            Positions = new Vector3[Joints.Length];
            Rotations = new Quaternion[Joints.Length];
            
            for (var i = 0; i < Joints.Length; i++)
            {
                if (Joints[i] == null) continue;

                Positions[i] = UseWorldSpace ? Joints[i].position : Joints[i].localPosition;
                Rotations[i] = UseWorldSpace ? Joints[i].rotation : Joints[i].localRotation;
            }
        }

        private void LateUpdate()
        {
            if (Brain != null && SyncJoints && Joints != null)
            {
                if (SyncMode == SyncMode.Rotation)
                {
                    for (var curr = 0; curr < Brain.PointCount - 1; curr++)
                    {
                        if (Joints.Length <= curr) break;
                        if (Joints[curr] == null) continue;
                        
                        var next = curr + 1;
                        
                        if (Joints.Length <= next) break;
                        if (Joints[next] == null) continue;
                        
                        var currPoint = Brain.GetPoint(curr, UseWorldSpace);
                        var nextPoint = Brain.GetPoint(next, UseWorldSpace);

                        var currPos = Positions[curr];
                        var nextPos = Positions[next];

                        var direction = nextPos - currPos;
                        var bestDirection = nextPoint - currPoint;

                        var rotation = Quaternion.FromToRotation(direction, bestDirection) * Rotations[curr];

                        Quaternion.Lerp(Joints[curr].rotation, rotation, Damp * Time.deltaTime);
                        
                        if (UseDamp)
                        {
                            if (UseWorldSpace)
                                Joints[curr].rotation =
                                    Quaternion.Lerp(Joints[curr].rotation, rotation, Damp * Time.deltaTime);
                            else
                                Joints[curr].localRotation =
                                    Quaternion.Lerp(Joints[curr].localRotation, rotation, Damp * Time.deltaTime);
                        }
                        else
                        {
                            if (UseWorldSpace) Joints[curr].rotation = rotation;
                            else Joints[curr].localRotation = rotation;
                        }
                    } 
                }
                else
                {
                    for (var i = 0; i < Brain.PointCount; i++)
                    {
                        if (Joints.Length <= i) break;
                        if (Joints[i] == null) continue;
                        
                        var point = Brain.GetPoint(i, UseWorldSpace);
                        var position = UseWorldSpace ? Joints[i].position : Joints[i].localPosition;
                        
                        if (UseDamp) point = Vector3.Lerp(position, point, Damp * Time.deltaTime);
                        
                        if (UseWorldSpace) Joints[i].position = point;
                        else Joints[i].localPosition = point;
                    }    
                }
            }
        }

        private void OnValidate()
        {
            if (Brain != null) SetPoints();
        }
    }
    
    public enum SyncMode
    {
        Position,
        Rotation
    }
}