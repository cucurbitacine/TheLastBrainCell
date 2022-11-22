using System.Collections;
using Game.Stats.Impl;
using UnityEngine;

namespace Game.Characters
{
    public abstract class CharacterController : MonoBehaviour
    {
        #region SerializeField
        
        [SerializeField] private bool enable = true;
        [Space]
        [SerializeField] private CharacterInfo characterInfo = null;
        
        [Header("Settings")]
        
        [SerializeField] private MoveSetting moveSetting = null;
        [SerializeField] private ViewSetting viewSetting = null;
        [SerializeField] private JumpSetting jumpSetting = null;
        [SerializeField] private AttackSetting attackSetting = null;

        [Header("References")]
        [SerializeField] private HealthIntBehaviour health = null;
        [SerializeField] private StaminaIntBehaviour stamina = null;
        
        #endregion

        #region Private Fields

        private Coroutine _jumpProcess = null;
        private Coroutine _attackProcess = null;
        
        private Transform _self = null;
        private Rigidbody2D _rigidbody;
        private Animator _animator = null;

        #endregion

        #region Public Properies

        public bool Enable
        {
            get => enable;
            set => enable = value;
        }

        public Transform Self => _self ??= transform;
        public Rigidbody2D Rigidbody => _rigidbody ??= Self.GetComponent<Rigidbody2D>();

        public Animator Animator => _animator ??= Self.GetComponent<Animator>();

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
        
        public CharacterInfo CharacterInfo => characterInfo??= new CharacterInfo();

        public MoveSetting MoveSetting => moveSetting ??= new MoveSetting();
        public ViewSetting ViewSetting => viewSetting ??= new ViewSetting();
        public JumpSetting JumpSetting => jumpSetting ??= new JumpSetting();
        public AttackSetting AttackSetting => attackSetting ??= new AttackSetting();

        public HealthIntBehaviour Health => health ??= GetComponentInChildren<HealthIntBehaviour>();
        public StaminaIntBehaviour Stamina => stamina ??= GetComponentInChildren<StaminaIntBehaviour>();

        #endregion

        #region Public API

        public void Stop()
        {
            Move(Vector2.zero);
        }
        
        public void Move(Vector2 dir)
        {
            MoveSetting.direction = dir.normalized;
        }

        public void View(Vector2 dir)
        {
            dir.Normalize();
            
            if (dir.sqrMagnitude <= 0.001f) return;

            ViewSetting.direction = dir;
        }
        
        public void Jump()
        {
            if (!CanJump()) return;

            if (JumpSetting.useStamina) Stamina.Value -= JumpSetting.staminaCost;
            
            if (_jumpProcess != null) StopCoroutine(_jumpProcess);
            _jumpProcess = StartCoroutine(JumpProcess());
        }

        public void Attack(string attackName)
        {
            if (!CanAttack()) return;

            if (AttackSetting.useStamina) Stamina.Value -= AttackSetting.staminaCost;
            
            if (_attackProcess != null) StopCoroutine(_attackProcess);
            _attackProcess = StartCoroutine(AttackProcess(attackName));
        }

        public virtual bool CanMove()
        {
            return MoveSetting.enabled && !CharacterInfo.isJumping &&
                   (MoveSetting.ableWhileAttack || !CharacterInfo.isAttacking);
        }

        public virtual bool CanView()
        {
            return ViewSetting.enabled && !CharacterInfo.isJumping &&
                   (ViewSetting.ableWhileAttack || !CharacterInfo.isAttacking);
        }
        
        public virtual bool CanJump()
        {
            return JumpSetting.enabled &&
                   JumpSetting.useStamina && Stamina.Value >= JumpSetting.staminaCost &&
                   !CharacterInfo.isJumping && !CharacterInfo.isAttacking;
        }

        public virtual bool CanAttack()
        {
            return AttackSetting.enabled &&
                   AttackSetting.useStamina && Stamina.Value >= AttackSetting.staminaCost &&
                   (AttackSetting.ableWhileAttack || !CharacterInfo.isAttacking);
        }

        #endregion

        #region Private API

        private IEnumerator JumpProcess()
        {
            CharacterInfo.isJumping = true;

            var durationJump = Time.fixedDeltaTime * Mathf.FloorToInt(JumpSetting.duration / Time.fixedDeltaTime);
            var speedJump = JumpSetting.distance / durationJump;
            
            CharacterInfo.velocityJump = direction * (1.75f * speedJump);

            var timer = durationJump;
            while (timer > 0f)
            {
                var t = timer / durationJump;

                CharacterInfo.velocityJump = direction * Mathf.Lerp(0.25f * speedJump, 1.75f * speedJump, t);
                
                timer -= Time.deltaTime;
                yield return null;
            }
            
            CharacterInfo.velocityJump = Vector2.zero;
            
            CharacterInfo.isJumping = false;
        }

        protected abstract IEnumerator AttackProcess(string attackStateName);
        
        private void UpdateVelocity(float deltaTime)
        {
            var velocityMoveTarget = CanMove() ? MoveSetting.velocity : Vector2.zero;
            
            CharacterInfo.velocityMove = Vector2.Lerp(CharacterInfo.velocityMove, velocityMoveTarget, MoveSetting.damp * deltaTime);

            CharacterInfo.isMoving = CharacterInfo.velocityTotal.sqrMagnitude > 0.001f;
        }

        private void UpdateView(float deltaTime)
        {
            var rotationViewTarget = CanView() ? Quaternion.LookRotation(Vector3.forward, ViewSetting.direction) : CharacterInfo.rotationView;

            CharacterInfo.rotationView = Quaternion.Lerp(CharacterInfo.rotationView, rotationViewTarget, ViewSetting.damp * deltaTime);
        }
        
        private void UpdateRigidbody()
        {
            Rigidbody.velocity = CharacterInfo.velocityTotal;
            
            Rigidbody.SetRotation(CharacterInfo.rotationView);
        }

        #endregion

        #region MonoBehaviour

        private void Awake()
        {
            CharacterInfo.rotationView = Self.rotation;

            if (Enable && Rigidbody == null) Enable = false;
        }

        private void Update()
        {
            if (Enable)
            {
                UpdateVelocity(Time.deltaTime);

                UpdateView(Time.deltaTime);
            }
        }

        private void FixedUpdate()
        {
            if (Enable) UpdateRigidbody();
        }

        #endregion
    }
}
