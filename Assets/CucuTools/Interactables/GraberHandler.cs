using System;
using UnityEngine;

namespace CucuTools.Interactables
{
    [RequireComponent(typeof(InteractHandlerBehaviour))]
    public class GraberHandler : CucuBehaviour
    {
        private RigidbodyInfo _savedRigidbody = default;
        private Vector3 _cachedLocalPosition = default;
        private Vector3 _cachedLocalForward = default;
        private Vector3 _cachedLocalUp = default;
        private bool _previousPressed = default;
        private bool _throwedGrabbable = default;

        #region SerializeField
        
        [Header("Grabbed Object")]
        [SerializeField] private GrabbableEntity grabbable = default;
        [Space]
        [Header("Holding Settings")]
        [SerializeField] private HoldSettings holdSettings = default;
        [SerializeField] private RigidbodyInfo grabbableRigidody = RigidbodyInfo.Grabbable;
        [Space]
        [Header("References")]
        [SerializeField] private InteractHandlerBehaviour interactHandler = default;
        [SerializeField] private Transform relativeTransform = default;

        #endregion

        #region Properties

        public bool Grabbed => Grabbable != null;
        
        public Transform RelativeTransform
        {
            get => relativeTransform;
            set => relativeTransform = value;
        }

        public GrabbableEntity Grabbable
        {
            get => grabbable;
            private set => grabbable = value;
        }
        
        public GrabMode GrabMode
        {
            get => HoldSettings.grabMode;
            set => HoldSettings.grabMode = value;
        }

        public bool NeedGrab
        {
            get
            {
                if (GrabMode == GrabMode.Once)
                {
                    if (_throwedGrabbable && !InteractHandler.Clicked)
                    {
                        _throwedGrabbable = false;
                    }

                    if (_throwedGrabbable) return false;
                    
                    return InteractHandler.Clicked;
                }
                else
                {
                    var needGrab = !_previousPressed && InteractHandler.Pressed;
                    _previousPressed = InteractHandler.Pressed;
                    
                    if (_throwedGrabbable && !needGrab)
                    {
                        _throwedGrabbable = false;
                    }
                    
                    if (_throwedGrabbable) return false;
                    
                    return needGrab;
                }
            }
        }

        public bool NeedFree
        {
            get
            {
                if (Input.GetKeyDown(KeyCode.Mouse1))
                {
                    _throwedGrabbable = true;
                    return true;
                }
                
                if (GrabMode == GrabMode.Once)
                {
                    return InteractHandler.Clicked;
                }
                else
                {
                    return !InteractHandler.Pressed;
                }
            }
        }

        public Vector3 HoldPosition { get; private set; }
        public Vector3 HoldForward { get; private set; }
        public Vector3 HoldUp { get; private set; }
        public Quaternion HoldRotation => Quaternion.LookRotation(HoldForward, HoldUp);
        
        public HoldSettings HoldSettings
        {
            get => holdSettings ?? (holdSettings = new HoldSettings());
            set => holdSettings = value;
        }

        public RigidbodyInfo GrabbableRigidody
        {
            get => grabbableRigidody;
            set => grabbableRigidody = value;
        }

        public InteractHandlerBehaviour InteractHandler
        {
            get => interactHandler;
            set => interactHandler = value;
        }

        #endregion

        #region API

        private void TryGrab()
        {
            if (!TryGetGrabbable(out var grabbableEntity)) return;

            if (grabbableEntity.IsGrabbed) return;
            
            Grab(grabbableEntity);
        }

        
        private bool TryGetGrabbable(out GrabbableEntity grabbableEntity)
        {
            grabbableEntity = null;

            var interactable = InteractHandler.Interactable;
            
            if (interactable == null) return false;
            
            if (interactable is InteractableBridge bridge)
            {
                interactable = bridge.Target;
            }

            grabbableEntity = interactable.GetComponent<GrabbableEntity>();
            
            return grabbableEntity != null;
        }
        
        private void Grab(GrabbableEntity grabbableEntity)
        {
            InteractHandler.IsEnabled = false;
            Grabbable = grabbableEntity;
            Grabbable.Grab();

            if (Grabbable.IsRigid)
            {
                _savedRigidbody = new RigidbodyInfo(Grabbable.Rigidbody);

                GrabbableRigidody.Load(Grabbable.Rigidbody);
            }
            
            _cachedLocalPosition = RelativeTransform.InverseTransformPoint(Grabbable.transform.position);
            _cachedLocalForward = RelativeTransform.InverseTransformDirection(Grabbable.transform.forward);
            _cachedLocalUp = RelativeTransform.InverseTransformDirection(Grabbable.transform.up);
            
            HoldPosition = RelativeTransform.TransformPoint(_cachedLocalPosition);
            HoldForward = RelativeTransform.TransformDirection(_cachedLocalForward).normalized;
            HoldUp = RelativeTransform.TransformDirection(_cachedLocalUp).normalized;
        }

        private void Free()
        {
            if (Grabbable.IsRigid)
            {
                _savedRigidbody.Load(Grabbable.Rigidbody);
            }
            
            Grabbable.Free();

            if (HoldSettings.canThrow && _throwedGrabbable && Grabbable.IsRigid)
            {
                Grabbable.Rigidbody.AddForce(RelativeTransform.forward * HoldSettings.throwForce, HoldSettings.throwForceMode);
            }
            
            Grabbable = null;
            InteractHandler.IsEnabled = true;
        }

        private void HoldGrabbable(float deltaTime)
        {
            var position = HoldSettings.holdSmooth
                ? Vector3.Lerp(Grabbable.transform.position, HoldPosition, HoldSettings.holdDamp * deltaTime)
                : HoldPosition;
            
            var rotation = HoldSettings.holdSmooth
                ? Quaternion.Lerp(Grabbable.transform.rotation, HoldRotation, HoldSettings.holdDamp * deltaTime)
                : HoldRotation;
            
            if (Grabbable.IsRigid)
            {
                Grabbable.Rigidbody.SyncPos(position, HoldSettings.maxSyncVelocity, HoldSettings.syncWight, deltaTime);
                Grabbable.Rigidbody.SyncRot(rotation, HoldSettings.syncWight, deltaTime);
            }
            else
            {
                Grabbable.transform.position = position;
            }
        }

        #endregion

        #region MonoBehaviour

        private void Update()
        {
            if (Grabbed)
            {
                HoldPosition = RelativeTransform.TransformPoint(_cachedLocalPosition);
                HoldForward = RelativeTransform.TransformDirection(_cachedLocalForward).normalized;
                HoldUp = RelativeTransform.TransformDirection(_cachedLocalUp).normalized;
                
                if (NeedFree)
                {
                    Free();
                }
                else
                {
                    if (!Grabbable.IsRigid) HoldGrabbable(Time.deltaTime);
                }
            }
            else
            {
                if (NeedGrab)
                {
                    TryGrab();
                }
            }
        }

        private void FixedUpdate()
        {
            if (Grabbable && !NeedFree && Grabbable.IsRigid) HoldGrabbable(Time.fixedDeltaTime);
        }

        #endregion
    }

    public enum GrabMode
    {
        Once,
        Hold,
    }

    [Serializable]
    public class HoldSettings
    {
        public GrabMode grabMode = GrabMode.Hold;
        public bool holdSmooth = true;
        public float holdDamp = 32f;

        [Header("Rigidbody")]
        [Min(0)] public float maxSyncVelocity = 500f;
        [Range(0, 1)] public float syncWight = 0.95f;
        
        public bool canThrow = true;
        public float throwForce = 10;
        public ForceMode throwForceMode = ForceMode.Impulse;
    }
    
    [Serializable]
    public struct RigidbodyInfo
    {
        public static RigidbodyInfo Grabbable =>
            new RigidbodyInfo
            {
                angularDrag = 10, 
                useGravity = false, 
                isKinematic = false, 
                interpolation = RigidbodyInterpolation.Interpolate, 
                collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic, 
            };

        public float angularDrag;
        public bool useGravity;
        public bool isKinematic;
        public RigidbodyInterpolation interpolation;
        public CollisionDetectionMode collisionDetectionMode;

        public RigidbodyInfo(Rigidbody rigidbody) : this()
        {
            Save(rigidbody);
        }
        
        public void Save(Rigidbody rigidbody)
        {
            angularDrag = rigidbody.angularDrag;
            useGravity = rigidbody.useGravity;
            isKinematic = rigidbody.isKinematic;
            interpolation = rigidbody.interpolation;
            collisionDetectionMode = rigidbody.collisionDetectionMode;
        }

        public void Load(Rigidbody rigidbody)
        {
            rigidbody.angularDrag = angularDrag;
            rigidbody.useGravity = useGravity;
            rigidbody.isKinematic = isKinematic;
            rigidbody.interpolation = interpolation;
            rigidbody.collisionDetectionMode = collisionDetectionMode;
        }
    }
}