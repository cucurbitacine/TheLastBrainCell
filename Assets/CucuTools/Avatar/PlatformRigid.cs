using UnityEngine;
using Random = UnityEngine.Random;

namespace CucuTools.Avatar
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlatformRigid : CucuBehaviour
    {
        private Vector3 _initialLocalPoint;
        
        private Vector3 _cacheStartLocalPointSelf;
        private Vector3 _cacheStartWorldPointSelf;
        private Vector3 _cacheStartOffsetParent;
        
        private Vector3 _cacheTargetLocalPointSelf;
        private Vector3 _cacheTargetWorldPointSelf;
        private Vector3 _cacheTargetOffsetParent;
        
        private Rigidbody _rigidbody;

        [SerializeField] private bool paused;
        
        [Header("Info")]
        public float angle;
        public Vector3 position;
        
        [Header("Rotation")]
        public float rotationSpeed = 60f;
        public Vector3 rotationAxis = Vector3.up;
        public bool randomPhase = false;
        
        [Header("Movements")]
        public float movementSpeed = 1f;
        public Vector3 startPointLocal = Vector3.zero;
        public Vector3 targetPointLocal = Vector3.forward;
        
        public bool HaveParent => parent != null && parent != transform;
        [SerializeField] private Transform parent;

        public bool Paused
        {
            get => paused;
            set => paused = value;
        }

        public Rigidbody Rigidbody => _rigidbody != null ? _rigidbody : (_rigidbody = GetComponent<Rigidbody>());

        public Transform Parent
        {
            get => parent != null ? parent : (parent = transform);
            set => parent = value;
        }

        public Vector3 GetStartPoint()
        {
            if (HaveParent) return Parent.position + _cacheStartOffsetParent;

            return _cacheStartWorldPointSelf;
        }

        public Vector3 GetTargetPoint()
        {
            if (HaveParent) return Parent.position + _cacheTargetOffsetParent;

            return _cacheTargetWorldPointSelf;
        }

        private void CachePoints()
        {
            _cacheStartLocalPointSelf = startPointLocal;
            _cacheTargetLocalPointSelf = targetPointLocal;
            
            _cacheStartWorldPointSelf = transform.rotation * _cacheStartLocalPointSelf + transform.position;
            _cacheTargetWorldPointSelf = transform.rotation * _cacheTargetLocalPointSelf + transform.position;
            
            if (HaveParent)
            {
                _initialLocalPoint = Parent.InverseTransformPoint(Rigidbody.transform.position);

                _cacheStartOffsetParent = _cacheStartWorldPointSelf - Parent.position;
                _cacheTargetOffsetParent = _cacheTargetWorldPointSelf - Parent.position;
            }
        }
        
        private void Awake()
        {
            Rigidbody.useGravity = false;
            Rigidbody.isKinematic = true;

            Rigidbody.interpolation = RigidbodyInterpolation.Interpolate;

            CachePoints();
            
            if (randomPhase) angle = Random.value * 360f;
        }

        private Quaternion _targetRotation = Quaternion.identity;
        private Vector3 _targetPosition = Vector3.zero;

        public bool Smooth = false;
        public float SmoothDamp = 16f;

        private float _timer;
        
        private void FixedUpdate()
        {
            if (Paused) return;

            var deltaTime = Time.fixedDeltaTime;
            _timer += deltaTime;
            
            if (rotationSpeed > 0)
            {
                var axis = rotationAxis.normalized;
                
                angle += rotationSpeed * deltaTime;
                angle = Mathf.Repeat(angle, 360f);
                _targetRotation = Quaternion.Euler(axis * angle);

                var rot = _targetRotation;
                if (Smooth) rot = Quaternion.Slerp(Rigidbody.transform.rotation, rot, SmoothDamp * deltaTime);
                Rigidbody.MoveRotation(rot);
            }

            if (movementSpeed > 0)
            {
                var startPoint = GetStartPoint();
                var targetPoint = GetTargetPoint();
                
                var distance = Vector3.Distance(startPoint, targetPoint);
                var t = Mathf.PingPong(_timer * movementSpeed, distance) / distance;
                t = Mathf.SmoothStep(0, 1, t);
                position = Vector3.Lerp(startPoint, targetPoint, t);
                _targetPosition = position;

                var pos = _targetPosition;
                if (Smooth) pos =  Vector3.Lerp(Rigidbody.transform.position, pos, SmoothDamp * deltaTime);
                Rigidbody.MovePosition(pos);
            }
            else
            {
                if (HaveParent) Rigidbody.MovePosition(Parent.TransformPoint(_initialLocalPoint));
            }
        }

        private void OnValidate()
        {
            if (!Application.isPlaying) CachePoints();
        }

        private void OnDrawGizmos()
        {
            if (movementSpeed > 0) Gizmos.DrawLine(GetStartPoint(), GetTargetPoint());
        }
    }
}