using System;
using CucuTools.Colors;
using UnityEngine;

namespace CucuTools.Avatar
{
    [RequireComponent(typeof(CapsuleCollider))]
    [RequireComponent(typeof(Rigidbody))]
    public class CucuAvatar : MonoBehaviour
    {
        private Vector3 _velocityGravity;
        private Vector3 _velocityPlatform;
        private Vector3 _velocityPlatformPrevious;
        private Vector3 _velocityMovement;
        private Vector3 _velocitySnap;
        private Vector3 _velocity;
        private Quaternion _bodyRotation;
        private Quaternion _headRotation;
        
        private float _speedBeforeFall;
        private bool _needJump;
        private bool _wasJump;
        private bool _crouchingExpected;
        private bool _crouchingReal;
        
        private Rigidbody _rigidbody;
        private CapsuleCollider _capsule;

        #region SerializeField

        [SerializeField] private CucuBrain brain = default;
        [SerializeField] private Transform head = default;
        [SerializeField] private GroundInfo groundInfo = default;
        [SerializeField] private CharacterSetting characterSetting = default;
        [SerializeField] private MovementSetting movementSetting = default;
        [SerializeField] private ViewSetting viewSetting = default;
        [SerializeField] private GroundSetting groundSetting = default;
        [SerializeField] private JumpSetting jumpSetting = default;
        [SerializeField] private AirSetting airSetting = default;
        [SerializeField] private CrouchSetting crouchSetting = default;
        [SerializeField] private SnapSetting snapSetting = default;
        [SerializeField] private GravitySetting gravitySetting = default;

        #endregion

        #region Const

        private const float OffsetCastGround = 0.05f;
        private const float JumpDelay = 0.1f;

        #endregion
        
        #region Properties

        public CucuBrain Brain
        {
            get => brain != null ? brain : (brain = GetComponentInChildren<CucuBrain>());
            set => brain = value;
        }

        public InputInfo InputInfo => Brain.InputInfo;

        public GroundInfo GroundInfo
        {
            get => groundInfo;
            private set
            {
                PreviousGroundInfo = groundInfo;
                groundInfo = value;
            }
        }

        public GroundInfo PreviousGroundInfo { get; private set; }
        
        public CharacterSetting CharacterSetting
        {
            get => characterSetting ?? (CharacterSetting = new CharacterSetting());
            set => characterSetting = value;
        }

        public GroundSetting GroundSetting
        {
            get => groundSetting ?? (GroundSetting = new GroundSetting());
            set => groundSetting = value;
        }

        public MovementSetting MovementSetting
        {
            get => movementSetting ?? (MovementSetting = new MovementSetting());
            set => movementSetting = value;
        }

        public JumpSetting JumpSetting
        {
            get => jumpSetting ?? (JumpSetting = new JumpSetting());
            set => jumpSetting = value;
        }

        public AirSetting AirSetting
        {
            get => airSetting ?? (AirSetting = new AirSetting());
            set => airSetting = value;
        }

        public CrouchSetting CrouchSetting
        {
            get => crouchSetting ?? (CrouchSetting = new CrouchSetting());
            set => crouchSetting = value;
        }

        public SnapSetting SnapSetting
        {
            get => snapSetting ?? (SnapSetting = new SnapSetting());
            set => snapSetting = value;
        }

        public ViewSetting ViewSetting
        {
            get => viewSetting ?? (ViewSetting = new ViewSetting());
            set => viewSetting = value;
        }

        public GravitySetting GravitySetting
        {
            get => gravitySetting ?? (GravitySetting = new GravitySetting());
            set => gravitySetting = value;
        }

        public Vector3 Gravity
        {
            get => GravitySetting.useCustomGravity ? GravitySetting.gravityCustom : Physics.gravity;
            
            set
            {
                if (GravitySetting.useCustomGravity) GravitySetting.gravityCustom = value;
            }
        }

        public Vector3 Velocity
        {
            get => _velocity;
            set
            {
                _velocity = value;
                Rigidbody.velocity = _velocity;
            }
        }

        public Vector3 VelocitySelf => Velocity - _velocityPlatform - _velocitySnap;
        
        public Rigidbody Rigidbody => _rigidbody != null ? _rigidbody : (_rigidbody = GetComponent<Rigidbody>());
        
        public CapsuleCollider Capsule => _capsule != null ? _capsule : (_capsule = GetComponent<CapsuleCollider>());

        public Transform Body => Rigidbody.transform;
        
        public Transform Head
        {
            get
            {
                if (head == null)
                {
                    var cameraTransform = GetComponentInChildren<Camera>()?.transform;
                    if (cameraTransform != null)
                    {
                        var cameraParent = cameraTransform.parent;
                        head = cameraParent != Body ? cameraParent : cameraTransform;
                    }
                }

                if (head == null)
                {
                    if (Brain != null)
                    {
                        if (Brain.transform.IsChildOf(Body.transform) && Brain.transform != Body.transform)
                        {
                            head = Brain.transform;
                        }
                    }
                }
                
                return head;
            }
            set => head = value;
        }

        public Vector3 position
        {
            get => Body.position;
            set => Body.position = value;
        }

        public Quaternion rotation
        {
            get => Body.rotation;
            set => Body.rotation = value;
        }

        #endregion

        #region Input
        
        private void UpdateInput()
        {
            if (JumpSetting.isOn && !_needJump) _needJump = InputInfo.jumpDown;
            if (CrouchSetting.isOn)
            {
                if (CrouchSetting.stayCrouch)
                {
                    if (InputInfo.crouchDown) _crouchingExpected = !_crouchingExpected;
                }
                else
                {
                    _crouchingExpected = InputInfo.crouch;    
                }
            }
        }
        
        #endregion

        #region Ground

        protected virtual GroundInfo GetGround()
        {
            var ground = new GroundInfo();
            
            var direction = -Body.up;
            var radius = Capsule.radius * GroundSetting.radiusScaleCast;
            var origin = Body.TransformPoint(Capsule.center) + direction * (Capsule.height * 0.5f - radius) - direction * OffsetCastGround;
            var distance = GroundSetting.distanceCast + OffsetCastGround;
            var layer = GroundSetting.layerGround;
            
            var wasHit = Physics.SphereCast(origin, radius, direction, out var hitInfo, distance, layer, GroundSetting.interaction);

            if (wasHit)
            {
                hitInfo.distance -= OffsetCastGround;
                ground.Apply(hitInfo);

                ground.isGrounded = Vector3.Angle(Body.up, ground.normal) < GroundSetting.angleMaxGround;
                ground.isPlatform = ground.rigidbody != null && ground.rigidbody.isKinematic;
            }
            else
            {
                wasHit = Physics.Raycast(origin, direction, distance, layer, GroundSetting.interaction);
            
                if (wasHit)
                {
                    hitInfo.distance -= OffsetCastGround;
                    ground.Apply(hitInfo);

                    ground.isGrounded = ground.collider != Capsule && Vector3.Angle(Body.up, ground.normal) < GroundSetting.angleMaxGround;
                    ground.isPlatform = ground.rigidbody != null && ground.rigidbody.isKinematic;
                } 
            }

            if (_wasJump) ground.isGrounded = false;
            
            return ground;
        }

        private void UpdateGround()
        {
            GroundInfo = GetGround();
        }
        
        #endregion

        #region Velocity

        protected virtual Vector3 MovementVelocity(float deltaTime)
        {
            if (!MovementSetting.isOn) return _velocityMovement = Vector3.zero;
            
            if (GroundInfo.isGrounded) // movement on ground
            {
                if (!PreviousGroundInfo.isGrounded)
                {
                    _velocityMovement = Vector3.ProjectOnPlane(_velocityMovement, Body.up);
                }
                
                var acc = (InputInfo.sprint ? MovementSetting.sprintScale : 1f) * MovementSetting.acceleration;
                var dir = InputInfo.move.normalized;
                var speedMax = MovementSetting.speedMax * (InputInfo.sprint ? MovementSetting.sprintScale : 1f);
                
                var accDir = acc * dir;
                accDir = Body.TransformDirection(accDir);
                accDir = Vector3.ProjectOnPlane(accDir, GroundInfo.normal);
                
                _velocityMovement += accDir * deltaTime;
                _velocityMovement = Vector3.ClampMagnitude(_velocityMovement, speedMax);

                if (accDir.sqrMagnitude <= 0)
                {
                    _velocityMovement = Vector3.Lerp(_velocityMovement, Vector3.zero, MovementSetting.groundDump * deltaTime);
                }
            }
            else // movement on air
            {
                if (PreviousGroundInfo.isGrounded)
                {
                    _velocityMovement += _velocityPlatformPrevious;
                    _speedBeforeFall = _velocityMovement.magnitude;
                }

                if (AirSetting.isOn)
                {
                    var acc = AirSetting.airScale * MovementSetting.acceleration;
                    var dir = InputInfo.move.normalized;
                    var speedMax = Mathf.Max(MovementSetting.speedMax, _speedBeforeFall);
                
                    var accDir = acc * dir;
                    accDir = Body.TransformDirection(accDir);
                    accDir = Vector3.ProjectOnPlane(accDir, GroundInfo.normal);
                
                    _velocityMovement += accDir * deltaTime;
                    _velocityMovement = Vector3.ClampMagnitude(_velocityMovement, speedMax);

                    if (accDir.sqrMagnitude <= 0)
                    {
                        _velocityMovement = Vector3.Lerp(_velocityMovement, Vector3.zero, AirSetting.airDump * deltaTime);
                    }
                }
                else
                {
                    _velocityMovement = Vector3.Lerp(_velocityMovement, Vector3.zero, AirSetting.airDump * deltaTime);
                }
            }

            return _velocityMovement;
        }

        private void WasJump()
        {
            _needJump = false;
            _wasJump = true;
            
            var info = new GroundInfo();
            info.Apply(GroundInfo);
            info.isGrounded = false;
            GroundInfo = info;
            
            Invoke(nameof(ResetJump), JumpDelay);
        }
        
        private void ResetJump()
        {
            _wasJump = false;
        }
        
        protected virtual Vector3 GravityVelocity(float deltaTime)
        {
            if (JumpSetting.isOn && GroundInfo.isGrounded && !_wasJump && _needJump)
            {
                _velocityGravity += Body.up * Mathf.Sqrt(2 * Gravity.magnitude * JumpSetting.jumpHeight);
                
                WasJump();
            }
            
            if (GravitySetting.isOn && !GroundInfo.isGrounded)
            {
                _velocityGravity += Gravity * deltaTime;
            }
            else
            {
                _velocityGravity = Vector3.zero;
            }

            return _velocityGravity;
        }

        protected virtual Vector3 PlatformVelocity(float deltaTime)
        {
            _velocityPlatformPrevious = _velocityPlatform;
            
            if (GroundInfo.isGrounded && GroundInfo.isPlatform)
            {
                var point = Body.position;
                _velocityPlatform = GroundInfo.rigidbody.GetPointVelocity(point);
            }
            else
            {
                _velocityPlatform = Vector3.zero;
            }

            return _velocityPlatform;
        }

        protected virtual Vector3 SnapGroundVelocity(float deltaTime)
        {
            if (SnapSetting.isOn)
            {
                if (GroundInfo.isGrounded && GroundInfo.distance > SnapSetting.minSnapDistance)
                {
                    var position = Body.position;
                
                    var direction = -Body.up;
                    var distance = Vector3.Project(GroundInfo.point - position, direction).magnitude;
                    var snapOffset = distance * direction;
                    
                    _velocitySnap = Vector3.Lerp(_velocitySnap, snapOffset / deltaTime, SnapSetting.snapDamp * deltaTime);
                    
                    return _velocitySnap;
                }
            }

            return _velocitySnap = Vector3.zero;
        }

        private void UpdateVelocity(float deltaTime)
        {
            Velocity = Vector3.zero;

            Velocity += GravityVelocity(deltaTime);
            
            Velocity += PlatformVelocity(deltaTime);
            
            Velocity += MovementVelocity(deltaTime);

            Velocity += SnapGroundVelocity(deltaTime);
        }

        private void PushOnGround()
        {
            if (GroundInfo.isGrounded && GroundInfo.rigidbody != null && !GroundInfo.rigidbody.isKinematic)
            {
                var forse = Rigidbody.mass * Gravity;
                GroundInfo.rigidbody.AddForceAtPosition(forse, GroundInfo.point, ForceMode.Force);
            }
        }
        
        #endregion

        #region Other Update

        public void UpdateCharacter()
        {
            Capsule.radius = CharacterSetting.radius;
            Capsule.height = CharacterSetting.Height;
            Capsule.center = Vector3.up * Capsule.height / 2;

            Rigidbody.mass = CharacterSetting.mass;
            
            if (Head != null)
                Head.position = Body.TransformPoint(Capsule.height * Vector3.up + CharacterSetting.headOffset);
        }
        
        private void UpdateCapsule(float deltaTime)
        {
            if (_crouchingReal)
            {
                if (!_crouchingExpected)
                {
                    var radius = Capsule.radius;
                    var origin = Body.TransformPoint(Vector3.up * radius);
                    var direction = Body.up;
                    var distance = CharacterSetting.heightDefault - 2 * radius - OffsetCastGround;

                    if (!Physics.SphereCast(origin, radius, direction, out var hit, distance))
                    {
                        _crouchingReal = _crouchingExpected;
                    }
                }
            }
            else
            {
                _crouchingReal = _crouchingExpected;
            }
            
            var scale = _crouchingReal ? CrouchSetting.crouchScale : 1;
            CharacterSetting.heightScale = Mathf.Lerp(CharacterSetting.heightScale, scale, CrouchSetting.crouchDamp * deltaTime);

            UpdateCharacter();
        }
        
        private void UpdateGravity(float deltaTime)
        {
            if (GravitySetting.isOn)
            {
                if (GroundInfo.isGrounded && GravitySetting.gravityDependNormal)
                {
                    var mag = Gravity.magnitude;
                    var dir = -GroundInfo.normal;
                    Gravity = mag * Vector3.Lerp(Gravity.normalized, dir, GravitySetting.gravityDamp * deltaTime).normalized;
                }
                
                if (GravitySetting.upDependGravity)
                {
                    var up = -Gravity.normalized;
                    var frw = Vector3.ProjectOnPlane(Body.forward, up).normalized;

                    _bodyRotation = Quaternion.Slerp(_bodyRotation, Quaternion.LookRotation(frw, up), GravitySetting.upDamp * deltaTime);    
                }
            }
        }
        
        private void UpdateView(float deltaTime)
        {
            if (GroundInfo.isGrounded && GroundInfo.isPlatform)
            {
                var platform = GroundInfo.rigidbody;
                var angularVelocity = Vector3.Project(platform.angularVelocity, Body.up);
                _bodyRotation *= Quaternion.Euler(angularVelocity * (Mathf.Rad2Deg * deltaTime));
                Rigidbody.MoveRotation(_bodyRotation);
            }
            
            if (ViewSetting.isOn)
            {
                var yaw = InputInfo.view.x;
                var pitch = InputInfo.view.y;

                var yawRotation = Quaternion.Euler(Vector3.up * yaw);
                var pitchRotation = Quaternion.Euler(Vector3.right * pitch);
                
                _bodyRotation *= yawRotation;
                _headRotation *= pitchRotation;

                var euler = _headRotation.eulerAngles;
                if (euler.x > 180) euler.x -= 360f;
                euler.x = Mathf.Clamp(euler.x, -ViewSetting.maxAngle, -ViewSetting.minAngle);
                _headRotation = Quaternion.Euler(euler.x, 0, 0);
                
                var bodyRot = _bodyRotation;
                var headRot = _headRotation;
                
                if (ViewSetting.smooth)
                {
                    bodyRot = Quaternion.Slerp(Body.rotation, _bodyRotation, ViewSetting.smoothDamp * deltaTime);
                    headRot = Quaternion.Slerp(Head.localRotation, _headRotation, ViewSetting.smoothDamp * deltaTime);
                }
                
                Rigidbody.MoveRotation(bodyRot);
                Head.localRotation = headRot;
            }
        }

        #endregion

        #region MonoBehaviour

        private void Awake()
        {
            Rigidbody.useGravity = false;
            Rigidbody.freezeRotation = true;
            Rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
            Rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

            if (Capsule.sharedMaterial == null)
            {
                Capsule.sharedMaterial = new PhysicMaterial
                {
                    name = "CucuPhysicMaterial",
                    dynamicFriction = 0f,
                    staticFriction = 0f,
                    bounciness = 0f,
                    frictionCombine = PhysicMaterialCombine.Multiply,
                    bounceCombine = PhysicMaterialCombine.Average,
                };
            }

            _bodyRotation = Body.rotation;
            _headRotation = Head.localRotation;

            UpdateCharacter();
        }

        private void Update()
        {
            UpdateInput();

            UpdateCapsule(Time.deltaTime);
            
            UpdateGravity(Time.deltaTime);
            
            UpdateView(Time.deltaTime);
        }

        private void FixedUpdate()
        {
            UpdateGround();
            
            UpdateVelocity(Time.fixedDeltaTime);

            PushOnGround();
        }

        private void OnValidate()
        {
            UpdateCharacter();
        }

        private void OnDrawGizmos()
        {
            if (GroundInfo.isGrounded)
            {
                var angle = Vector3.Angle(Body.up, GroundInfo.normal);
                
                Gizmos.color = Color.Lerp(Color.green, Color.red, angle / GroundSetting.angleMaxGround);
                Gizmos.DrawLine(GroundInfo.point, GroundInfo.point + GroundInfo.normal);
                
                Gizmos.color = Gizmos.color.AlphaTo(0.2f);
                Gizmos.DrawSphere(GroundInfo.point, 0.1f);
            }

            var position = Body.position;
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(position, position + Velocity / 10f);
        }

        #endregion
    }

    #region Infos

    [Serializable]
    public struct GroundInfo
    {
        public bool isGrounded;
        public bool isPlatform;

        public Vector3 point;
        public Vector3 normal;
        public float distance;
        
        public GameObject gameObject;
        public Collider collider;
        public Rigidbody rigidbody;

        public void Apply(RaycastHit hit)
        {
            point = hit.point;
            normal = hit.normal;
            distance = hit.distance;

            gameObject = hit.transform != null ? hit.transform.gameObject : null;
            collider = hit.collider;
            rigidbody = hit.rigidbody;
        }
        
        public void Apply(GroundInfo info)
        {
            isGrounded = info.isGrounded;
            isPlatform = info.isPlatform;
            
            point = info.point;
            normal = info.normal;
            distance = info.distance;
            
            gameObject = info.gameObject;
            collider = info.collider;
            rigidbody = info.rigidbody;
        }
    }

    #endregion

    #region Settings

    [Serializable]
    public class CharacterSetting
    {
        public float heightDefault = 2f;
        public float radius = 0.5f;
        public float mass = 60f;
        public float heightScale = 1f;
        public Vector3 headOffset = -0.325f * Vector3.up;
        
        public float Height
        {
            get => heightDefault * heightScale;
            set => heightDefault = value / heightScale;
        }
    }
    
    [Serializable]
    public class GroundSetting
    {
        public LayerMask layerGround = 1;
        public float distanceCast = 0.1f;
        public float radiusScaleCast = 0.99f;
        public float angleMaxGround = 60f;
        public QueryTriggerInteraction interaction = QueryTriggerInteraction.Ignore;
    }

    [Serializable]
    public class MovementSetting
    {
        public bool isOn = true;
        public float speedMax = 5f;
        public float sprintScale = 2f;
        public float acceleration = 32f;
        public float groundDump = 16f;
    }

    [Serializable]
    public class ViewSetting
    {
        public bool isOn = true;
        public float minAngle = -90;
        public float maxAngle = 90;
        public bool smooth = false;
        public float smoothDamp = 16f;
    }
    
    [Serializable]
    public class JumpSetting
    {
        public bool isOn = true;
        public float jumpHeight = 2f;
    }
    
    [Serializable]
    public class AirSetting
    {
        public bool isOn = true;
        public float airScale = 0.1f;
        public float airDump = 8f;
    }

    [Serializable]
    public class CrouchSetting
    {
        public bool isOn = true;
        public float crouchScale = 0.5f;
        public float crouchDamp = 16f;
        public bool stayCrouch = true;
    }

    [Serializable]
    public class SnapSetting
    {
        public bool isOn = true;
        public float minSnapDistance = 0.02f;
        public float snapDamp = 16f;
    }
    
    [Serializable]
    public class GravitySetting
    {
        public bool isOn = true;
        
        public bool useCustomGravity = true;
        public Vector3 gravityCustom = Vector3.down * 16f;
        
        public bool upDependGravity = true;
        public float upDamp = 8f;

        public bool gravityDependNormal = false;
        public float gravityDamp = 8f;
    }

    #endregion
}
