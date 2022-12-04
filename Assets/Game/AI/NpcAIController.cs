using System;
using Game.Characters.Npc;
using Game.Characters.Player;
using Game.Inputs;
using Game.Navigations;
using UnityEngine;

namespace Game.AI
{
    /// <summary>
    /// Base npc AI controller
    /// </summary>
    public abstract class NpcAIController : InputController
    {
        public bool debugMode = false;

        [Space]
        public NpcController npc;
        public MovementController movement;
        public DetectionController detection;
        
        [Space]
        public bool visiblePlayer;
        public Vector2 followPlayerPoint;
        public Vector2 lastPlayerPoint;
        public PlayerController detectedPlayer;
        
        protected static readonly int VisiblePlayer = Animator.StringToHash("VisiblePlayer");
        protected static readonly int PlayerDetected = Animator.StringToHash("PlayerDetected");
        protected static readonly int HealthValue = Animator.StringToHash("HealthValue");
        protected static readonly int DeadTrigger = Animator.StringToHash("Dead");
        
        protected static readonly int ReadyAttack = Animator.StringToHash("ReadyAttack");
        protected static readonly int NeedAvoid = Animator.StringToHash("NeedAvoid");
        protected static readonly int AttackTrigger = Animator.StringToHash("Attack");
        
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
        
        protected virtual void Awake()
        {
            if (npc == null) npc = GetComponentInParent<NpcController>();
            if (movement == null) movement = npc.GetComponentInChildren<MovementController>();
            if (detection == null) detection = npc.GetComponentInChildren<DetectionController>();
        }
        
        protected virtual void OnEnable()
        {
            detection.OnStatusChanged.AddListener(OnDetectionChanged);

            npc.Health.Events.OnValueChanged.AddListener(OnHealthChanged);
            UpdateHealth();
        }

        protected virtual void OnDisable()
        {
            detection.OnStatusChanged.RemoveListener(OnDetectionChanged);
            
            npc.Health.Events.OnValueChanged.RemoveListener(OnHealthChanged);
        }
    }
}