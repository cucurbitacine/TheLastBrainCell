using System;
using System.Collections.Concurrent;
using System.Linq;
using UnityEngine;

namespace CucuTools.Avatar
{
    public class CucuAvatar2D : MonoBehaviour
    { 
        private readonly RaycastHit2D[] _potentialGrounds = new RaycastHit2D[32];
        private readonly ConcurrentQueue<Collider2D> _ignoreGrounds = new ConcurrentQueue<Collider2D>();

        private PhysicsMaterial2D _physicsMaterial2D = default;
        private CapsuleCollider2D _capsule2D = default;
        private Rigidbody2D _rigidbody2D = default;

        private bool _needJump = default;
        private bool _wasJump = default;

        #region SerializeField

        [SerializeField] private CucuBrain2D brain = default;
        [Space]
        [SerializeField] private GroundInfo2D groundInfo2D = default;
        [Space]
        [SerializeField] private CharacterSetting2D playerSetting = default;
        [SerializeField] private GroundSetting2D groundSetting = default;
        [SerializeField] private MovementSetting2D movementSetting = default;

        #endregion

        #region Properties

        public InputInfo2D InputInfo2D => Brain.InputInfo2D;
    
        public GroundInfo2D GroundInfo2D
        {
            get => groundInfo2D;
            private set => groundInfo2D = value;
        }

        public CharacterSetting2D PlayerSetting
        {
            get => playerSetting ?? (playerSetting = new CharacterSetting2D());
            set => playerSetting = value;
        }

        public GroundSetting2D GroundSetting
        {
            get => groundSetting ?? (groundSetting = new GroundSetting2D());
            set => groundSetting = value;
        }

        public MovementSetting2D MovementSetting
        {
            get => movementSetting ?? (movementSetting = new MovementSetting2D());
            set => movementSetting = value;
        }
    
        public CucuBrain2D Brain => brain != null ? brain : (brain = GetComponentInChildren<CucuBrain2D>());
        public Transform Body => transform;
        public CapsuleCollider2D Capsule2D => _capsule2D != null ? _capsule2D : (_capsule2D = InitializeCapsule());
        public Rigidbody2D Rigidbody2D => _rigidbody2D != null ? _rigidbody2D : (_rigidbody2D = InitializeRigidbody());

        public Vector2 Gravity => Physics2D.gravity * Rigidbody2D.gravityScale;

        public PhysicsMaterial2D PhysicsMaterial2D =>
            _physicsMaterial2D != null
                ? _physicsMaterial2D
                : (_physicsMaterial2D = new PhysicsMaterial2D()
                {
                    name = name,
                    bounciness = 0f,
                    friction = 0f,
                });

        public Vector2 Velocity { get; private set; }
        public Vector2 VelocitySelf { get; private set; }
        public Vector2 VelocityFall { get; private set; }
        
        private float SpeedOfJump => Mathf.Sqrt(Gravity.magnitude * 2 * MovementSetting.jumpHeight);

        #endregion
        
        public void UpdatePlayer()
        {
            UpdateRigidbody();
            UpdateCapsule();
        }
        
        #region Capsule2D

        private void UpdateCapsule()
        {
            UpdateCapsule(Capsule2D);
        }
    
        private void UpdateCapsule(CapsuleCollider2D capsule2D)
        {
            capsule2D.size = PlayerSetting.size;
            capsule2D.offset = Vector2.up * (capsule2D.size.y * 0.5f);
        }
    
        private CapsuleCollider2D InitializeCapsule()
        {
            var capsule2D = Body.GetComponent<CapsuleCollider2D>();

            if (capsule2D == null)
            {
                capsule2D = Body.gameObject.AddComponent<CapsuleCollider2D>();
            }
        
            capsule2D.sharedMaterial = PhysicsMaterial2D;
            capsule2D.isTrigger = false;
            capsule2D.direction = CapsuleDirection2D.Vertical;
        
            UpdateCapsule(capsule2D);
            return capsule2D;
        }

        #endregion

        #region Rigidbody2D

        private void UpdateRigidbody()
        {
            UpdateRigidbody(Rigidbody2D);
        }
    
        private void UpdateRigidbody(Rigidbody2D other)
        {
            other.mass = PlayerSetting.mass;
            other.gravityScale = PlayerSetting.gravityScale;
        }
    
        private Rigidbody2D InitializeRigidbody()
        {
            var other = Body.GetComponent<Rigidbody2D>();

            if (other == null)
            {
                other = Body.gameObject.AddComponent<Rigidbody2D>();
            }
        
            other.bodyType = RigidbodyType2D.Dynamic;
        
            if (other.sharedMaterial == null)
            {
                other.sharedMaterial = PhysicsMaterial2D;
            }
        
            other.freezeRotation = true;

            other.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            other.interpolation = RigidbodyInterpolation2D.Interpolate;

            UpdateRigidbody(other);
            return other;
        }

        #endregion

        #region Ground

        private void UpdateGround()
        {
            GroundInfo2D = GetGroundInfo(this);

            if (GroundInfo2D.isGrounded && InputInfo2D.climbDown)
            {
                if (GroundInfo2D.collider.usedByEffector)
                {
                    if (GroundInfo2D.transform.GetComponent<PlatformEffector2D>() != null)
                    {
                        ClimbDownFrom(GroundInfo2D.collider);
                    }
                }
            }
        }

        private void ClimbDownFrom(Collider2D other)
        {
            if (other == null) return;
            
            _ignoreGrounds.Enqueue(other);

            Physics2D.IgnoreCollision(other, Capsule2D, true);

            Invoke(nameof(StopClimbDown), GroundSetting.climbDownDuration);
        }

        private void StopClimbDown()
        {
            while (_ignoreGrounds.TryDequeue(out var other))
            {
                if (other == null) continue;
                Physics2D.IgnoreCollision(other, Capsule2D, false);
            }
        }
        
        private static GroundRequest2D GetGroundRequest(CucuAvatar2D avatar2D)
        {
            var size = avatar2D.Capsule2D.size;
        
            var origin = (Vector2) avatar2D.Body.TransformPoint(avatar2D.Capsule2D.offset);
            var radius = size.x * 0.5f * avatar2D.GroundSetting.scaleRadiusCircleCast;
            var direction = -(Vector2) avatar2D.Body.up;
            var distance = size.y * 0.5f - radius + avatar2D.GroundSetting.maxDistanceToGround;

            return new GroundRequest2D()
            {
                origin = origin,
                radius = radius,
                direction = direction,
                distance = distance,
            };
        }
        
        public static GroundInfo2D GetGroundInfo(CucuAvatar2D avatar2D)
        {
            var result = new GroundInfo2D();

            if (avatar2D.Velocity.y > 0) return result;
            
            result.distance = float.MaxValue;

            var request = GetGroundRequest(avatar2D);
        
            var layerMask = avatar2D.GroundSetting.layerMaskOfGround;
            var minDepth = avatar2D.GroundSetting.minDepth;
            var maxDepth = avatar2D.GroundSetting.maxDepth;

            var hitCount = Physics2D.CircleCastNonAlloc(request.origin, request.radius, request.direction, avatar2D._potentialGrounds, request.distance, layerMask, minDepth, maxDepth);
            
            for (var i = 0; i < hitCount; i++)
            {
                var hit = avatar2D._potentialGrounds[i];

                if (hit.transform == null) continue;
                if (hit.transform == avatar2D.Body) continue;
                if (hit.collider.isTrigger) continue;
                if (avatar2D._ignoreGrounds.Contains(hit.collider)) continue;
                if (hit.point.y > avatar2D.Body.transform.position.y + avatar2D.Capsule2D.size.x * 0.25f) continue;
                if (hit.distance < result.distance)
                {
                    result.Apply(hit);
                }
            }

            if (result.transform != null)
            {
                var angle = Vector2.Angle(-request.direction, result.normal);
                
                result.isGrounded = angle <= avatar2D.GroundSetting.maxAngleGround;
            }
            
            return result;
        }

        #endregion

        #region Movements

        private void UpdateSelf(float deltaTime)
        {
            VelocitySelf = Vector2.zero;
        
            if (Mathf.Abs(InputInfo2D.move.x) > 0.5f) VelocitySelf += Vector2.right * Mathf.Sign(InputInfo2D.move.x);

            var speedTotal = MovementSetting.speedMax * (InputInfo2D.sprint ? MovementSetting.sprintScale : 1);
            
            VelocitySelf = Vector3.ProjectOnPlane(VelocitySelf, GroundInfo2D.normal);
            VelocitySelf = VelocitySelf.normalized *speedTotal;
        }

        private void UpdateFall(float deltaTime)
        {
            _needJump = !_wasJump && InputInfo2D.jump;
            _wasJump = InputInfo2D.jump;
            
            if (GroundInfo2D.isGrounded)
            {
                VelocityFall = Vector2.zero;
                
                if (_needJump)
                {
                    VelocityFall = Body.up * SpeedOfJump;
                }
            }
            else
            {
                VelocityFall += Gravity * deltaTime;
            }

            VelocityFall = Vector2.ClampMagnitude(VelocityFall, SpeedOfJump); 
        }

        private void UpdateVelocity(float deltaTime)
        {
            UpdateSelf(deltaTime);
            UpdateFall(deltaTime);

            Velocity = VelocitySelf + VelocityFall;

            Rigidbody2D.velocity = Velocity;
        }

        #endregion

        #region MonoBehavioirs

        private void Awake()
        {
            UpdatePlayer();
        }

        private void Update()
        {
            UpdatePlayer();
        }

        private void FixedUpdate()
        {
            UpdateGround();

            UpdateVelocity(Time.fixedDeltaTime);
        }

        private void OnValidate()
        {
            UpdatePlayer();
        }

        private void OnDrawGizmos()
        {
            var request = GetGroundRequest(this);
        
            var start = request.origin;
            var target = request.origin + request.direction * request.distance;
        
            var groundInfo = Application.isPlaying ? GroundInfo2D : GetGroundInfo(this);
            if (groundInfo.isGrounded)
            {
                target = groundInfo.point + groundInfo.normal * groundInfo.distance;
            }

            Gizmos.color = groundInfo.isGrounded ? Color.green : Color.red;

            if (groundInfo.isGrounded)
            {
                Gizmos.DrawWireSphere(groundInfo.point, request.radius / 4f);
                Gizmos.DrawLine(groundInfo.point, groundInfo.point + groundInfo.normal);
            }

            Gizmos.color = Color.Lerp(Gizmos.color, Color.gray, 0.5f);
            Gizmos.DrawWireSphere(start, request.radius);
            Gizmos.DrawLine(start, target);
            Gizmos.DrawWireSphere(target, request.radius);
        }

        #endregion
    }

    [Serializable]
    public struct GroundInfo2D
    {
        public bool isGrounded;

        public float distance;
        public Vector2 point;
        public Vector2 normal;
    
        public Transform transform;
        public Collider2D collider;
        public Rigidbody2D rigidbody;
    
        public GroundInfo2D(RaycastHit2D hit2D) : this()
        {
            Apply(hit2D);
        }

        public GroundInfo2D(GroundInfo2D other) : this()
        {
            Apply(other);
        }
    
        public void Apply(RaycastHit2D hit2D)
        {
            distance = hit2D.distance;
            point = hit2D.point;
            normal = hit2D.normal;
        
            transform = hit2D.transform;
            collider = hit2D.collider;
            rigidbody = hit2D.rigidbody;
        }
    
        public void Apply(GroundInfo2D other)
        {
            isGrounded = other.isGrounded;
        
            distance = other.distance;
            point = other.point;
            normal = other.normal;
        
            transform = other.transform;
            collider = other.collider;
            rigidbody = other.rigidbody;
        }
    }

    [Serializable]
    public struct GroundRequest2D
    {
        public Vector2 origin;
        public float radius;
        public Vector2 direction;
        public float distance;
    }

    [Serializable]
    public class CharacterSetting2D
    {
        public float width = 1f;
        public float height = 2f;
        public float mass = 50f;
        public float gravityScale = 1f;

        public Vector2 size => width > height ? new Vector2(width, width) : new Vector2(width, height);
    }

    [Serializable]
    public class GroundSetting2D
    {
        public float maxAngleGround = 60f;
        public float scaleRadiusCircleCast = 1f;
        public float maxDistanceToGround = 0.05f;
        public LayerMask layerMaskOfGround = 1;
        public float minDepth = float.NegativeInfinity;
        public float maxDepth = float.PositiveInfinity;
        public float climbDownDuration = 0.2f;
    }

    [Serializable]
    public class MovementSetting2D
    {
        public float speedMax = 8f;
        public float sprintScale = 2f;
        public float jumpHeight = 5f;
    }
}