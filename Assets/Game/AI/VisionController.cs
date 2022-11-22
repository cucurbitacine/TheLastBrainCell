using System;
using System.Collections.Generic;
using CucuTools;
using UnityEngine;
using UnityEngine.Events;

namespace Game.AI
{
    public class VisionController : MonoBehaviour
    {
        #region SerializeField

        [Min(0f)]
        [SerializeField] private float distance = 8f;
        [Min(0f)]
        [SerializeField] private float nearPlane = 0.618f;
        [Range(0f, 360f)]
        [SerializeField] private float fov = 120f;
        [Range(0.001f, 1f)]
        [SerializeField] private float percentVisibility = 0.382f;
        
        [Space]
        [SerializeField] private LayerMask targetLayer = 1;
        [SerializeField] private LayerMask obstacleLayer = 0;
        
        [Space]
        [SerializeField] private VisionEvents events = null;
        
        [Space]
        [SerializeField] private Transform self = null;
        
        [Space]
        [SerializeField] private bool debugMode = false;
        
        #endregion

        #region Private Fields

        private readonly List<Collider2D> _visibleColliders = new List<Collider2D>();
        
        private const int countMaxOverlap = 32;
        private const int countMaxRaycast = 9;
        
        private int _countTotalOverlap = 0;
        private int _countValidOverlap = 0;
        private Collider2D[] _overlaps = null;

        #endregion

        #region Public Properties

        public Transform Self => self ??= transform;

        public Vector2 position
        {
            get => Self.position;
            set => Self.position = value;
        }
        
        public Quaternion rotation
        {
            get => Self.rotation;
            set => Self.rotation = value;
        }
        
        public Vector2 direction
        {
            get => Self.up;
            set => Self.up = value;
        }

        public float angleRotation => rotation.eulerAngles.z;
        
        public float Distance
        {
            get => distance;
            set => distance = value;
        }

        public float NearPlane
        {
            get => nearPlane;
            set => nearPlane = value;
        }

        public float FOV
        {
            get => fov;
            set => fov = value;
        }

        public float PercentVisibility
        {
            get => percentVisibility;
            set => percentVisibility = value;
        }

        public LayerMask TargetLayer
        {
            get => targetLayer;
            set => targetLayer = value;
        }

        public LayerMask ObstacleLayer
        {
            get => obstacleLayer;
            set => obstacleLayer = value;
        }

        public VisionEvents Events => events ??= new VisionEvents();

        #endregion

        public void Initialize(Transform self)
        {
            this.self = self;
        }
        
        public void Deinitialize()
        {
            this.self = null;
        }
        
        #region Private API

        private static void EvaluateOverlapBox(VisionController vision, out Vector2 point, out Vector2 size)
        {
            var radius = vision.Distance;
            var fov = vision.FOV;
            var self = vision.Self;
            
            var phiHalf = Mathf.Deg2Rad * fov * 0.5f;

            if (fov <= 180f)
            {
                size.x = 2 * radius * Mathf.Sin(phiHalf);
                size.y = radius;

                point.x = 0f;
                point.y = size.y * 0.5f;
            }
            else
            {
                size.x = 2 * radius;
                size.y = radius * Mathf.Sin(phiHalf - Mathf.PI * 0.5f) + radius;

                point.x = 0f;
                point.y = radius - size.y * 0.5f;
            }
            
            point = self.TransformPoint(point);
        }

        private void UpdateOverlap()
        {
            EvaluateOverlapBox(this, out var pointBox, out var sizeBox);

            _countValidOverlap = 0;
            _countTotalOverlap = Physics2D.OverlapBoxNonAlloc(pointBox, sizeBox, angleRotation, _overlaps, TargetLayer);

            for (var i = 0; i < _countTotalOverlap; i++)
            {
                var cld = _overlaps[i];

                if (cld.transform == Self) continue;

                var bnd = cld.bounds;
                var center = (Vector2)bnd.center;
                var size = bnd.size;
                var min = Mathf.Min(size.x, size.y);
                var max = Mathf.Max(size.x, size.y);
                var width = Mathf.Lerp(min, max, 1f - min / max);
                
                var dir = (center - position).normalized;
                var shl = Vector2.Perpendicular(dir);

                if (debugMode)
                {
                    Debug.DrawLine(center, center + shl * (width * 0.5f), Color.blue);
                    Debug.DrawLine(center, center - shl * (width * 0.5f), Color.red);
                }
                
                var countValidRaycast = 0;
                var steps = Cucu.LinSpace(0f, width, countMaxRaycast);
                for (var j = 0; j < steps.Length; j++)
                {
                    var trgRaycast = center + shl * (width * 0.5f) - shl * steps[j];
                    var vec2Trg = trgRaycast - position;

                    var dirRaycast = vec2Trg.normalized;
                    var orgRaycast = position + dirRaycast * NearPlane;
                    var dstRaycast = vec2Trg.magnitude;
                    
                    var hit = Physics2D.Raycast(orgRaycast, dirRaycast, dstRaycast, TargetLayer | ObstacleLayer);

                    if (hit.collider == cld)
                    {
                        var pointHit = hit.point;

                        var angleHit = Vector2.Angle(direction, pointHit - position);
                        var distanceHit = Vector2.Distance(pointHit, position);

                        if (angleHit <= FOV * 0.5f && distanceHit <= Distance)
                        {
                            countValidRaycast++;

                            if (debugMode) Debug.DrawLine(orgRaycast, pointHit, Color.green);
                        }
                        else
                        {
                            if (debugMode) Debug.DrawLine(orgRaycast, pointHit, Color.red);
                        }
                    }
                    else
                    {
                        if (debugMode) Debug.DrawLine(orgRaycast, hit.collider != null ? hit.point : trgRaycast, Color.red);
                    }
                }

                var visibility = (float)countValidRaycast / countMaxRaycast;

                if (visibility < PercentVisibility) continue;
                
                _overlaps[_countValidOverlap] = cld;
                _countValidOverlap++;
            }
        }

        private void UpdateEvents()
        {
            _visibleColliders.RemoveAll(vs => vs == null);
            
            for (var i = _visibleColliders.Count - 1; i >= 0; i--)
            {
                var cld = _visibleColliders[i];

                var match = false;
                for (var j = 0; j < _countValidOverlap; j++)
                {
                    match = _overlaps[j] == cld;
                    if (match) break;
                }

                if (!match)
                {
                    _visibleColliders.RemoveAt(i);

                    if (cld != null) Events.Lost.Invoke(cld);
                }
            }
            
            for (var i = 0; i < _countValidOverlap; i++)
            {
                var cld = _overlaps[i];

                if (!_visibleColliders.Contains(cld))
                {
                    _visibleColliders.Add(cld);
                    
                    Events.Found.Invoke(cld);
                }
            }
        }

        private void DrawGizmos()
        {
            if (Self == null) return;
            
            Gizmos.color = Color.cyan;
            
            var resolution = 32;
            var step = FOV / (resolution - 1);
            var lastPointFar = Vector2.zero;
            var lastPointNear = Vector2.zero;
            for (var i = 0; i < resolution; i++)
            {
                var phi = -FOV * 0.5f + i * step;
                phi *= Mathf.Deg2Rad;
                var x = Mathf.Sin(phi);
                var y = Mathf.Cos(phi);
                
                var pointFar = (Vector3)position + Self.TransformDirection(new Vector2(x, y)) * Distance;
                var pointNear = (Vector3)position + Self.TransformDirection(new Vector2(x, y)) * NearPlane;

                if (i == 0 || i == resolution - 1)
                {
                    var dir = ((Vector2)pointFar - position).normalized;
                    Gizmos.DrawLine(position + dir * NearPlane, pointFar);
                }

                if (i > 0)
                {
                    Gizmos.DrawLine(lastPointFar, pointFar);
                    Gizmos.DrawLine(lastPointNear, pointNear);
                }

                lastPointFar = pointFar;
                lastPointNear = pointNear;
            }

            if (_overlaps != null && 0 < _countValidOverlap && _countValidOverlap < _overlaps.Length)
            {
                Gizmos.color = Color.green;

                for (var i = 0; i < _countValidOverlap; i++)
                {
                    var cld = _overlaps[i];
                    if (cld != null)
                    {
                        var bnd = cld.bounds;

                        Gizmos.DrawWireCube(bnd.center, bnd.size);
                    }
                }
            }
            
            if (!debugMode) return; 
            
            EvaluateOverlapBox(this, out var point, out var size);

            Gizmos.color = Color.yellow;
            CucuGizmos.DrawWiredCube(point, size, rotation);
        }

        #endregion

        #region MonoBehaviour

        private void Awake()
        {
            _overlaps = new Collider2D[countMaxOverlap];
        }

        private void FixedUpdate()
        {
            UpdateOverlap();

            UpdateEvents();
        }

        private void OnDrawGizmos()
        {
            if (debugMode) DrawGizmos();
        }

        private void OnDrawGizmosSelected()
        {
            if (!debugMode) DrawGizmos();
        }

        #endregion
        
        [Serializable]
        public class VisionEvents
        {
            [SerializeField] private UnityEvent<Collider2D> found = null;
            [SerializeField] private UnityEvent<Collider2D> lost = null;
            
            public UnityEvent<Collider2D> Found => found ??= new UnityEvent<Collider2D>();

            public UnityEvent<Collider2D> Lost => lost ??= new UnityEvent<Collider2D>();
        }
    }
}