using System.Collections.Generic;
using CucuTools.Surfaces;
using UnityEngine;

namespace CucuTools.Clothes
{
    public class CucuClothes : MonoBehaviour
    {
        private MeshFilter _meshFilter = default;
        private Mesh _mesh = default;

        private Vector3 _previous = default;

        private Point[] _points = default;
        private List<Connection> _connections = default;

        private Vector3[] _vertices = default;
        private int[] _triangles = default;
        private Vector2[] _uv = default;

        private SphereCollider _penetrationCollider = default;
        private Collider[] _penetrationOverlaps = default;
        
        [Header("Simulation")]
        public int depthSimulation = 8;

        public bool fixUp = true;
        public bool fixDown = false;
        public bool fixLeft = false;
        public bool fixRight = false;
        
        [Space]
        [Range(0f, 1f)] public float fading = 0.01f;

        [Space]
        public bool useKinematic = true;
        [Range(0f, 1f)] [SerializeField] private float weightKinematic = 0.05f;

        [Space]
        public bool useGravity = true;
        [Range(0f, 1f)] [SerializeField] private float weightGravity = 1f;

        [Space]
        public bool useWind = true;
        [Range(0f, 1f)] [SerializeField] private float weightWind = 1f;
        [Range(0f , 1f)] public float smoothWind = 0.5f;
        public WindZone windZone;

        [Space]
        public bool usePenetration = true;
        [Range(0f, 1f)] [SerializeField] private float weightPenetration = 1f;
        [Range(0.001f, 0.1f)] public float radiusPenetration = 0.01f;

        [Header("Surface")]
        public bool useCrossing = true;
        public SurfaceBehaviour surface = default;

        [Header("Gizmos")]
        public bool drawGizmos = true;

        public float WeightKinematic
        {
            get => weightKinematic;
            set => weightKinematic = Mathf.Clamp01(value);
        }

        public float WeightGravity
        {
            get => weightGravity;
            set => weightGravity = Mathf.Clamp01(value);
        }

        public float WeightWind
        {
            get => weightWind;
            set => weightWind = Mathf.Clamp01(value);
        }

        public float WeightPenetration
        {
            get => weightPenetration;
            set => weightPenetration = Mathf.Clamp01(value);
        }
        
        private int GetPointIndex(int i, int j)
        {
            return i * surface.gizmos.SizeV + j;
        }

        #region Initiation

        private void InitMesh()
        {
            /*
             * Init mesh
             */

            _meshFilter = GetComponent<MeshFilter>();

            if (_meshFilter != null)
            {
                if (_meshFilter.sharedMesh != null) _mesh = _meshFilter.sharedMesh;
                else
                {
                    _mesh = new Mesh();
                    _mesh.name = _meshFilter.gameObject.name;
                }
            }

            var quadCount = (surface.gizmos.SizeU - 1) * (surface.gizmos.SizeV - 1);
            _vertices = new Vector3[surface.gizmos.SizeU * surface.gizmos.SizeV];
            _triangles = new int[quadCount * 2 * 3];
            _uv = new Vector2[_vertices.Length];
        }

        private void InitPoints()
        {
            /*
             * Init points
             */

            _points = new Point[surface.gizmos.SizeU * surface.gizmos.SizeV];

            for (var i = 0; i < surface.gizmos.SizeU; i++)
            {
                var u = surface.gizmos.GridU[i];

                for (var j = 0; j < surface.gizmos.SizeV; j++)
                {
                    var v = surface.gizmos.GridV[j];

                    var point = surface.GetLocalPoint(u, v);

                    var index = GetPointIndex(i, j);
                    _points[index] = new Point(point);

                    if (fixUp) _points[index].locked |= j == surface.gizmos.SizeV - 1;
                    if (fixDown) _points[index].locked |= j == 0;
                    if (fixLeft) _points[index].locked |= i == 0;
                    if (fixRight) _points[index].locked |= i == surface.gizmos.SizeU - 1;
                }
            }
        }

        private void InitConnections()
        {
            /*
             * Init connection
             */

            _connections = new List<Connection>();

            for (var i = 0; i < surface.gizmos.SizeU; i++)
            {
                for (var j = 0; j < surface.gizmos.SizeV; j++)
                {
                    var index = GetPointIndex(i, j);

                    if (j + 1 < surface.gizmos.SizeV)
                    {
                        var up = GetPointIndex(i, j + 1);
                        var connection = new Connection(_points[index], _points[up]);
                        _connections.Add(connection);
                    }

                    if (i + 1 < surface.gizmos.SizeU)
                    {
                        var right = GetPointIndex(i + 1, j);
                        var connection = new Connection(_points[index], _points[right]);
                        _connections.Add(connection);
                    }

                    if (useCrossing)
                    {
                        if (i + 1 < surface.gizmos.SizeU && j + 1 < surface.gizmos.SizeV)
                        {
                            var rightCorner = GetPointIndex(i + 1, j + 1);
                            var connection = new Connection(_points[index], _points[rightCorner]);
                            _connections.Add(connection);
                        }

                        if (0 <= i - 1 && j + 1 < surface.gizmos.SizeV)
                        {
                            var leftCorner = GetPointIndex(i - 1, j + 1);
                            var connection = new Connection(_points[index], _points[leftCorner]);
                            _connections.Add(connection);
                        }
                    }
                }
            }
        }

        private void InitPenetration()
        {
            /*
             * Init penetration
             */

            _penetrationOverlaps = new Collider[8];
            _penetrationCollider = new GameObject("Penetration").AddComponent<SphereCollider>();
            _penetrationCollider.transform.SetParent(transform, false);
            _penetrationCollider.enabled = false;
            _penetrationCollider.radius = radiusPenetration;
        }

        #endregion

        #region Computing
        
        private void UpdateMesh()
        {
            /*
             * Update mesh
             */

            if (_meshFilter == null) return;

            var tri = 0;

            for (var i = 0; i < surface.gizmos.SizeU; i++)
            {
                var u = surface.gizmos.GridU[i];

                for (var j = 0; j < surface.gizmos.SizeV; j++)
                {
                    var v = surface.gizmos.GridV[j];

                    var index = GetPointIndex(i, j);

                    _vertices[index] = _points[index].position;
                    _uv[index] = new Vector2(u, v);

                    if (i < surface.gizmos.SizeU - 1 && j < surface.gizmos.SizeV - 1)
                    {
                        var up = GetPointIndex(i, j + 1);
                        var corner = GetPointIndex(i + 1, j + 1);
                        var right = GetPointIndex(i + 1, j);

                        _triangles[tri++] = index;
                        _triangles[tri++] = up;
                        _triangles[tri++] = corner;

                        _triangles[tri++] = index;
                        _triangles[tri++] = corner;
                        _triangles[tri++] = right;
                    }
                }
            }

            _mesh.vertices = _vertices;
            _mesh.triangles = _triangles;
            _mesh.uv = _uv;
            _mesh.RecalculateNormals();
            _mesh.Optimize();

            _meshFilter.sharedMesh = _mesh;
        }

        private float _lastWindMain = 0f;
        private float _timerWindPulse = 0f;
        private float _pulseMagnitudeTarget = 0f;
        
        private void SimulatePoints()
        {
            if (depthSimulation > 0)
            {
                /*
                 * Computing external forces
                 */

                var position = transform.position;

                var kinematic = -(position - _previous);
                kinematic = transform.InverseTransformDirection(kinematic.normalized) * kinematic.magnitude;

                var gravity = Physics.gravity * (Time.fixedDeltaTime * Time.fixedDeltaTime);
                gravity = transform.InverseTransformDirection(gravity.normalized) * gravity.magnitude;

                var wind = Vector3.zero;
                if (windZone != null)
                {
                    var period = 1f / windZone.windPulseFrequency;
                    
                    var windMain = windZone.windMain;
                    
                    _timerWindPulse -= Time.fixedDeltaTime;
                    if (_timerWindPulse <= 0f)
                    {
                        _pulseMagnitudeTarget = Random.value * windZone.windPulseMagnitude;
                        _timerWindPulse = period;
                    }
                    var rad = 2 * Mathf.PI * _timerWindPulse / period;
                    var pulseMagnitude = (Mathf.Sin(rad) + 1f) * 0.5f * _pulseMagnitudeTarget;
                    windMain += pulseMagnitude;
                    
                    windMain = Mathf.Lerp(windMain, _lastWindMain, smoothWind);
                    _lastWindMain = windMain;
                    
                    if (windZone.mode == WindZoneMode.Directional)
                    {
                        wind = windZone.transform.forward * (windMain * Time.fixedDeltaTime);
                    }
                    else
                    {
                        var dir = transform.position - windZone.transform.position;
                        var pow = Mathf.Max(windZone.radius - dir.magnitude, 0f) / windZone.radius;
                        if (pow > 0) wind = dir.normalized * (pow * windMain * Time.fixedDeltaTime);
                    }

                    wind = transform.InverseTransformDirection(wind.normalized) * wind.magnitude;
                }

                _penetrationCollider.radius = radiusPenetration;
                
                /*
                 * Simulate physics per point
                 */

                foreach (var point in _points)
                {
                    if (point.locked) continue;

                    var step = (point.position - point.previousPosition) * (1f - fading);

                    if (useKinematic)
                    {
                        step += weightKinematic * kinematic;
                    }

                    if (useGravity)
                    {
                        step += weightGravity * gravity;
                    }

                    if (useWind && windZone != null)
                    {
                        step += weightWind * wind;
                    }

                    if (usePenetration)
                    {
                        /*
                        * Compute penetretion
                        */
                                    
                        _penetrationCollider.transform.position = transform.TransformPoint(point.position);

                        var spherePos = _penetrationCollider.transform.position;

                        var count = Physics.OverlapSphereNonAlloc(spherePos, _penetrationCollider.radius, _penetrationOverlaps);

                        if (count > 0)
                        {
                            var penetration = Vector3.zero;
                            
                            _penetrationCollider.enabled = true;
                            for (var j = 0; j < count; j++)
                            {
                                var overlap = _penetrationOverlaps[j];
                                if (overlap.isTrigger) continue;
                                
                                var overlapPos = overlap.transform.position;
                                var overlapRot = overlap.transform.rotation;

                                Physics.ComputePenetration(
                                    _penetrationCollider, spherePos, Quaternion.identity,
                                    overlap, overlapPos, overlapRot,
                                    out var dir, out var dst);

                                penetration += dir * dst;
                            }
                            _penetrationCollider.enabled = false;
                            
                            penetration /= count;
                            penetration += penetration.normalized * radiusPenetration;
                            penetration *= weightPenetration;
                            penetration = transform.InverseTransformDirection(penetration.normalized) * penetration.magnitude;
                            
                            step += penetration;
                        }
                    }

                    point.previousPosition = point.position;
                    point.position += step;
                }
                
                _previous = position;
            }
        }

        private void SimulateConnections()
        {
            /*
             * Simulate physics per connection
             */

            for (var i = 0; i < depthSimulation; i++)
            {
                foreach (var connection in _connections)
                {
                    connection.Simulate();
                }
            }
        }

        #endregion

        #region MonoBehaviour

        private void Start()
        {
            _previous = transform.position;

            InitMesh();

            InitPoints();

            InitConnections();

            InitPenetration();
        }

        private void Update()
        {
            UpdateMesh();
        }

        private void FixedUpdate()
        {
            SimulateConnections();

            SimulatePoints();
        }

        private void OnDrawGizmos()
        {
            if (!drawGizmos) return;

            Vector3 F(Vector3 pos)
            {
                return transform.TransformPoint(pos);
            }

            if (_connections != null)
            {
                foreach (var conn in _connections)
                {
                    if (conn == null) continue;

                    var a = conn.a;
                    var b = conn.b;

                    if (a == null || b == null) continue;

                    Gizmos.color = Color.white;
                    Gizmos.DrawLine(F(a.position), F(b.position));
                }
            }

            if (_points != null)
            {
                foreach (var point in _points)
                {
                    Gizmos.color = point.locked ? Color.red : Color.white;
                    Gizmos.DrawWireSphere(F(point.position), 0.02f);
                }
            }
        }

        #endregion
    }
}