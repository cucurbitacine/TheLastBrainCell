using System;
using CucuTools.Attributes;
using UnityEngine;

namespace CucuTools.IK.Example.Spider
{
    public class SpiderController : CucuBehaviour
    {
        public float Radius = 2f;
        public float MinRadius = 1f;
        public float MaxRadius = 3.5f;

        public Transform Body;
        
        public LegIK[] Legs;
        
        public Vector3 GetCenter()
        {
            return transform.position;
        }

        public float RaycastDistance = 2f;
        public LayerMask RaycastLayerMask = 1;

        [Range(-1f, 1f)]
        public float dotMin = 0.2f;
        [Range(0f, 180f)]
        public float angleMax = 15f;

        [CucuButton()]
        private void BuildLegs()
        {
            for (var i = 0; i < Legs.Length; i++)
            {
                var leg = Legs[i];
                if (leg == null) continue;
                if (leg.brain == null) continue;

                leg.brain.transform.localRotation = Quaternion.identity;
                
                if(i < 4)
                {
                    leg.brain.transform.Rotate(Vector3.up, (i + 1) * 36f, Space.Self);
                }
                else
                {
                    leg.brain.transform.Rotate(Vector3.up, -(i - 4 + 1) * 36f, Space.Self);
                }
                
                leg.offset = (leg.brain.transform.localRotation * Vector3.forward) * Radius;

                var point = transform.TransformPoint(leg.offset);

                leg.position = point;
                leg.brain.TargetIK.TargetPosition = leg.position;
            }
        }
        
        private void UpdateLegs()
        {
            var center = GetCenter();
            
            foreach (var leg in Legs)
            {
                if (leg == null) continue;
                if (leg.brain == null) continue;

                var bestPosition = transform.TransformPoint(leg.offset);
                
                var direction = leg.brain.GetTarget() - center;
                var bestDirection = bestPosition - center;

                var angle = Vector3.Angle(direction, bestDirection);
                var dot = Vector3.Dot(direction, bestDirection);

                var distance = direction.magnitude;

                if (angleMax < angle || dot < dotMin || distance < MinRadius || MaxRadius < distance)
                {
                    var shift = bestPosition - leg.brain.GetTarget();
                    var origin = bestPosition + shift * 0.5f + transform.up;
                    var dir = -transform.up;
                    var ray = new Ray(origin, dir);

                    if (Physics.Raycast(ray, out var raycastHit, RaycastDistance, RaycastLayerMask))
                    {
                        leg.position = raycastHit.point;
                    }
                    else
                    {
                        leg.position = bestPosition;
                    }
                }
                
                leg.brain.SetTarget(leg.position);
            }
        }

        public float Speed = 1f;
        public float SpeedBody = 0.5f; 
        private Vector3 bodyPosition;
        
        private void Awake()
        {
            bodyPosition = Body.localPosition;
            
            BuildLegs();
            
            UpdateLegs();
        }
        
        private void Update()
        {
            var velocity = Vector3.zero;
            
            if (Input.GetKey(KeyCode.W))
            {
                velocity += transform.TransformDirection(Vector3.forward);
            }

            if (Input.GetKey(KeyCode.S))
            {
                velocity += transform.TransformDirection(Vector3.back);
            }

            transform.position += velocity.normalized * (Speed * Time.deltaTime);

            if (Input.GetKey(KeyCode.A))
            {
                var angle = Speed * 10f * Time.deltaTime;
                transform.Rotate(Vector3.up, -angle, Space.Self);
            }
            
            if (Input.GetKey(KeyCode.D))
            {
                var angle = Speed * 10f * Time.deltaTime;
                transform.Rotate(Vector3.up, angle, Space.Self);
            }
            
            Body.localPosition = bodyPosition + Vector3.up * (Mathf.Sin(SpeedBody * Time.time * Mathf.PI * 2) * 0.25f);
            
            UpdateLegs();
        }

        private void OnDrawGizmos()
        {
            var center = GetCenter();
            
            foreach (var leg in Legs)
            {
                if (leg == null) continue;
                if (leg.brain == null) continue;
                
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireCube(leg.position, Vector3.one * 0.2f);
                Gizmos.DrawWireCube(transform.TransformPoint(leg.offset), Vector3.one * 0.2f);
                Gizmos.DrawLine(transform.TransformPoint(leg.offset), leg.position);
                
            }
        }

        [Serializable]
        public class LegIK
        {
            public CucuIKBrain brain;
            public Vector3 offset;
            public Vector3 position;
        }
    }
}