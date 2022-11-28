using System;
using Game.Characters;
using Game.Navigations;
using UnityEngine;

namespace Game.AI.Fear
{
    public class FearAIController : EnemyAIController
    {
        [Header("Settings")]
        [Min(0f)]
        public float periodUpdatePath = 0.1f;

        [Space]
        public Transform[] patrolPath;
        
        [Header("Information")]
        public Vector2 initPosition;
        public Vector2 initDirection;
        
        [Space]
        public bool visiblePlayer;
        public Vector2 followPlayerPoint;
        public Vector2 lastPlayerPoint;
        public PlayerController detectedPlayer;

        [Header("References")]
        public EnemyController enemy;
        public MovementController movement;
        public DetectionController detection;

        private static readonly int VisiblePlayer = Animator.StringToHash("VisiblePlayer");
        private static readonly int PlayerDetected = Animator.StringToHash("PlayerDetected");
        private static readonly int ReadyAttack = Animator.StringToHash("ReadyAttack");
        private static readonly int NeedAvoid = Animator.StringToHash("NeedAvoid");
        private static readonly int HealthValue = Animator.StringToHash("HealthValue");
        private static readonly int DeadTrigger = Animator.StringToHash("Dead");
        private static readonly int AttackTrigger = Animator.StringToHash("Attack");

        public void Attack()
        {
            enemy.Animator.SetTrigger(AttackTrigger);
        }
        
        private void OnDetectionChanged(DetectionSample sample)
        {
            if (sample == null || sample.collider == null) return;
            
            var player = sample.collider.GetComponent<PlayerController>();

            if (detectedPlayer == null)
            {
                detectedPlayer = player;
            }
            else
            {
                if (player != detectedPlayer) return;
            }

            switch (sample.status)
            {
                case DetectionStatus.Undefined:
                    visiblePlayer = false;
                    enemy.Animator.SetBool(VisiblePlayer, false);
                    enemy.Animator.SetBool(PlayerDetected, false);
                    detectedPlayer = null;
                    break;
                
                case DetectionStatus.Detecting:
                    visiblePlayer = true;
                    enemy.Animator.SetBool(VisiblePlayer, true);
                    enemy.Animator.SetBool(PlayerDetected, false);
                    break;
                
                case DetectionStatus.Detected:
                    visiblePlayer = true;
                    enemy.Animator.SetBool(VisiblePlayer, true);
                    enemy.Animator.SetBool(PlayerDetected, true);
                    break;
                
                case DetectionStatus.Losing:
                    visiblePlayer = false;
                    lastPlayerPoint = detectedPlayer.position;
                    enemy.Animator.SetBool(VisiblePlayer, false);
                    enemy.Animator.SetBool(PlayerDetected, true);
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnHealthChanged(int diff)
        {
            UpdateHealth();
        }

        private void UpdateHealth()
        {
            enemy.Animator.SetInteger(HealthValue, enemy.Health.Value);

            if (enemy.Health.Value == 0) enemy.Animator.SetTrigger(DeadTrigger);
        }
        
        private void UpdateAvoiding()
        {
            var needStamina = 0f;
            needStamina += enemy.JumpSetting.useStamina ? enemy.JumpSetting.staminaCost : 0f;
            needStamina += enemy.AttackSetting.useStamina ? enemy.AttackSetting.staminaCost : 0f;

            var totalStamina = enemy.Stamina.Value;

            var needAvoid = needStamina > totalStamina;
            
            enemy.Animator.SetBool(NeedAvoid, needAvoid);
        }
        
        private void UpdateAttacking()
        {
            if (!enemy.Animator.GetBool(VisiblePlayer)) return;
            
            var distanceToPlayer = Vector2.Distance(enemy.position, detectedPlayer.position);
            
            var needStamina = enemy.JumpSetting.staminaCost + enemy.AttackSetting.staminaCost;
            var totalStamina = enemy.Stamina.Value;
            
            var readyAttack = distanceToPlayer <= enemy.JumpSetting.distance &&
                              needStamina <= totalStamina;

            enemy.Animator.SetBool(ReadyAttack, readyAttack);
        }
        
        private void Awake()
        {
            if (enemy == null) enemy = GetComponentInParent<EnemyController>();
            if (movement == null) movement = enemy.GetComponentInChildren<MovementController>();
            if (detection == null) detection = enemy.GetComponentInChildren<DetectionController>();

            initPosition = enemy.position;
            initDirection = enemy.direction;
        }

        private void OnEnable()
        {
            detection.OnStatusChanged.AddListener(OnDetectionChanged);
            
            enemy.Health.Events.OnValueChanged.AddListener(OnHealthChanged);
            
            UpdateHealth();
        }

        private void Update()
        {
            UpdateAvoiding();

            UpdateAttacking();
        }

        private void OnDisable()
        {
            detection.OnStatusChanged.RemoveListener(OnDetectionChanged);
            
            enemy.Health.Events.OnValueChanged.RemoveListener(OnHealthChanged);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = visiblePlayer ? Color.green : Color.yellow;
            Gizmos.DrawWireSphere(visiblePlayer ? followPlayerPoint : lastPlayerPoint, 0.2f);

            if (patrolPath != null && patrolPath.Length > 1)
            {
                for (int i = 0; i < patrolPath.Length - 1; i++)
                {
                    if (patrolPath[i] != null && patrolPath[i + 1] != null)
                    {
                        Gizmos.DrawLine(patrolPath[i].position, patrolPath[i + 1].position);
                    } 
                }
            }
        }
    }
}
