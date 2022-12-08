using System.Collections;
using CucuTools.DamageSystem;
using Game.Stats.Impl;
using UnityEngine;

namespace Game.Characters
{
    public abstract class CharacterControllerBase : MonoBehaviour
    {
        #region SerializeField
        
        [SerializeField] private bool enable = true;
        
        [Space]
        [SerializeField] private CharacterInfo info = null;
        
        [Header("Settings")]
        [SerializeField] private MoveSetting moveSetting = null;
        [SerializeField] private ViewSetting viewSetting = null;
        [SerializeField] private JumpSetting jumpSetting = null;
        [SerializeField] private AttackSetting attackSetting = null;

        [Header("References")]
        [SerializeField] private HealthIntBehaviour health = null;
        [SerializeField] private StaminaIntBehaviour stamina = null;
        
        [Space]
        [SerializeField] private DamageReceiver damageReceiver = null;
        [SerializeField] private DamageSource[] damageSources = null;
        
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

        public bool alive => Health.Value > 0;
        public bool dead => !alive;

        public CharacterInfo Info => info ??= new CharacterInfo();

        public MoveSetting MoveSetting => moveSetting ??= new MoveSetting();
        public ViewSetting ViewSetting => viewSetting ??= new ViewSetting();
        public JumpSetting JumpSetting => jumpSetting ??= new JumpSetting();
        public AttackSetting AttackSetting => attackSetting ??= new AttackSetting();

        public HealthIntBehaviour Health => health ??= GetComponentInChildren<HealthIntBehaviour>();
        public StaminaIntBehaviour Stamina => stamina ??= GetComponentInChildren<StaminaIntBehaviour>();
        public DamageReceiver DamageReceiver => damageReceiver ??= GetComponentInChildren<DamageReceiver>();
        public DamageSource[] DamageSources => damageSources ??= GetComponentsInChildren<DamageSource>();

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
            
            JumpSetting.onJumped.Invoke();
            
            if (_jumpProcess != null) StopCoroutine(_jumpProcess);
            _jumpProcess = StartCoroutine(JumpProcess());
        }

        public void Attack(string attackName)
        {
            if (!CanAttack()) return;

            if (AttackSetting.useStamina) Stamina.Value -= AttackSetting.staminaCost;
            
            AttackSetting.onAttacked.Invoke();
            
            if (_attackProcess != null) StopCoroutine(_attackProcess);
            _attackProcess = StartCoroutine(AttackProcess(attackName));
        }

        public void SetView(Vector2 directionView)
        {
            directionView.Normalize();
            
            if (directionView.sqrMagnitude <= 0.001f) return;

            ViewSetting.direction = directionView;
            
            Info.rotationView = Quaternion.LookRotation(Vector3.forward, ViewSetting.direction);
            
            Rigidbody.SetRotation(Info.rotationView);
            
        }
        
        public virtual bool CanMove()
        {
            return MoveSetting.enabled && !Info.isJumping &&
                   (MoveSetting.ableWhileAttack || !Info.isAttacking);
        }

        public virtual bool CanView()
        {
            return ViewSetting.enabled && !Info.isJumping &&
                   (ViewSetting.ableWhileAttack || !Info.isAttacking);
        }
        
        public virtual bool CanJump()
        {
            return JumpSetting.enabled &&
                   JumpSetting.useStamina && Stamina.Value >= JumpSetting.staminaCost &&
                   !Info.isJumping && !Info.isAttacking;
        }

        public virtual bool CanAttack()
        {
            return AttackSetting.enabled &&
                   AttackSetting.useStamina && Stamina.Value >= AttackSetting.staminaCost &&
                   (AttackSetting.ableWhileAttack || !Info.isAttacking);
        }

        #endregion

        #region Private API

        private IEnumerator JumpProcess()
        {
            Info.isJumping = true;

            var durationJump = Time.fixedDeltaTime * Mathf.FloorToInt(JumpSetting.duration / Time.fixedDeltaTime);
            var speedJump = JumpSetting.distance / durationJump;
            
            Info.velocityJump = direction * (1.75f * speedJump);

            var timer = durationJump;
            while (timer > 0f)
            {
                var t = timer / durationJump;

                Info.velocityJump = direction * Mathf.Lerp(0.25f * speedJump, 1.75f * speedJump, t);
                
                timer -= Time.deltaTime;
                yield return null;
            }
            
            Info.velocityJump = Vector2.zero;
            
            Info.isJumping = false;
        }

        protected abstract IEnumerator AttackProcess(string attackName);
        
        private void UpdateVelocity(float deltaTime)
        {
            var velocityMoveTarget = CanMove() ? MoveSetting.velocity : Vector2.zero;
            
            Info.velocityMove = Vector2.Lerp(Info.velocityMove, velocityMoveTarget, MoveSetting.damp * deltaTime);

            Info.isMoving = Info.velocityTotal.sqrMagnitude > 0.001f;
        }

        private void UpdateView(float deltaTime)
        {
            var rotationViewTarget = CanView() ? Quaternion.LookRotation(Vector3.forward, ViewSetting.direction) : Info.rotationView;

            Info.rotationView = Quaternion.Lerp(Info.rotationView, rotationViewTarget, ViewSetting.damp * deltaTime);
        }
        
        private void UpdateRigidbody()
        {
            Rigidbody.velocity = Info.velocityTotal;
            
            Rigidbody.SetRotation(Info.rotationView);
        }

        #endregion

        #region MonoBehaviour

        private void Awake()
        {
            ViewSetting.direction = direction;
            Info.rotationView = Self.rotation;

            if (Enable && Rigidbody == null) Enable = false;

            damageSources = GetComponentsInChildren<DamageSource>();
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
