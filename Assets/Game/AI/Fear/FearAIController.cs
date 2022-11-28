using System;
using Game.Characters.Npc;
using Game.Characters.Player;
using Game.Navigations;
using UnityEngine;

namespace Game.AI.Fear
{
    public class FearAIController : NpcAIController
    {
        [Header("Settings")]
        [Min(0f)]
        public float periodUpdatePath = 0.1f;
        [Min(0f)]
        public float attackDelay = 0.5f;
        
        [Space]
        [Min(0f)]
        public float patrolStayDuration = 5f;
        public int indexPatrol;
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
            npc.Animator.SetTrigger(AttackTrigger);
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
                    npc.Animator.SetBool(VisiblePlayer, false);
                    npc.Animator.SetBool(PlayerDetected, false);
                    detectedPlayer = null;
                    break;
                
                case DetectionStatus.Detecting:
                    visiblePlayer = true;
                    npc.Animator.SetBool(VisiblePlayer, true);
                    npc.Animator.SetBool(PlayerDetected, false);
                    break;
                
                case DetectionStatus.Detected:
                    visiblePlayer = true;
                    npc.Animator.SetBool(VisiblePlayer, true);
                    npc.Animator.SetBool(PlayerDetected, true);
                    break;
                
                case DetectionStatus.Losing:
                    visiblePlayer = false;
                    lastPlayerPoint = detectedPlayer.position;
                    npc.Animator.SetBool(VisiblePlayer, false);
                    npc.Animator.SetBool(PlayerDetected, true);
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
            npc.Animator.SetInteger(HealthValue, npc.Health.Value);

            if (npc.Health.Value == 0) npc.Animator.SetTrigger(DeadTrigger);
        }
        
        private void UpdateAvoiding()
        {
            var needStamina = 0f;
            needStamina += npc.JumpSetting.useStamina ? npc.JumpSetting.staminaCost : 0f;
            needStamina += npc.AttackSetting.useStamina ? npc.AttackSetting.staminaCost : 0f;

            var totalStamina = npc.Stamina.Value;

            var needAvoid = needStamina > totalStamina;
            
            npc.Animator.SetBool(NeedAvoid, needAvoid);
        }
        
        private void UpdateAttacking()
        {
            if (!npc.Animator.GetBool(VisiblePlayer))
            {
                npc.Animator.SetBool(ReadyAttack, false);
                
                return;
            }
            
            var distanceToPlayer = Vector2.Distance(npc.position, detectedPlayer.position);
            
            var needStamina = npc.JumpSetting.staminaCost + npc.AttackSetting.staminaCost;
            var totalStamina = npc.Stamina.Value;
            
            var readyAttack = distanceToPlayer <= npc.JumpSetting.distance &&
                              needStamina <= totalStamina;

            npc.Animator.SetBool(ReadyAttack, readyAttack);
        }
        
        private void Awake()
        {
            if (npc == null) npc = GetComponentInParent<FearController>();
            if (movement == null) movement = npc.GetComponentInChildren<MovementController>();
            if (detection == null) detection = npc.GetComponentInChildren<DetectionController>();

            initPosition = npc.position;
            initDirection = npc.direction;
        }

        private void OnEnable()
        {
            detection.OnStatusChanged.AddListener(OnDetectionChanged);
            
            npc.Health.Events.OnValueChanged.AddListener(OnHealthChanged);
            
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
            
            npc.Health.Events.OnValueChanged.RemoveListener(OnHealthChanged);
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
