using System;
using Game.AI;
using Game.Characters;
using Game.Navigations;
using UnityEngine;

namespace Game.Dev.AI
{
    public class EnemyAIController : MonoBehaviour
    {
        [Min(0f)]
        public float periodUpdatePath = 0.1f;
        public float minDistance = 0.1f;
        public Vector2 initPosition;
        public Vector2 initDirection;
        
        [Space]
        public bool visiblePlayer;
        public Vector2 followPlayerPoint;
        public Vector2 lastPlayerPoint;
        public PlayerController detectedPlayer;

        [Space]
        public EnemyController enemy;
        public MovementController movement;
        public DetectionController detection;

        public void OnDetectionChanged(DetectionSample sample)
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
                    enemy.Animator.SetBool(BaseStateAI.VisiblePlayer, false);
                    enemy.Animator.SetBool(BaseStateAI.PlayerDetected, false);
                    detectedPlayer = null;
                    break;
                
                case DetectionStatus.Detecting:
                    visiblePlayer = true;
                    enemy.Animator.SetBool(BaseStateAI.VisiblePlayer, true);
                    enemy.Animator.SetBool(BaseStateAI.PlayerDetected, false);
                    break;
                
                case DetectionStatus.Detected:
                    visiblePlayer = true;
                    enemy.Animator.SetBool(BaseStateAI.VisiblePlayer, true);
                    enemy.Animator.SetBool(BaseStateAI.PlayerDetected, true);
                    break;
                
                case DetectionStatus.Losing:
                    visiblePlayer = false;
                    lastPlayerPoint = detectedPlayer.position;
                    enemy.Animator.SetBool(BaseStateAI.VisiblePlayer, false);
                    enemy.Animator.SetBool(BaseStateAI.PlayerDetected, true);
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException();
            }
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
        }

        private void Update()
        {
            var needStamina = 0f;
            needStamina += enemy.JumpSetting.useStamina ? enemy.JumpSetting.staminaCost : 0f;
            needStamina += enemy.AttackSetting.useStamina ? enemy.AttackSetting.staminaCost : 0f;

            var totalStamina = enemy.Stamina.Value;

            var needAvoid = needStamina > totalStamina;
            
            enemy.Animator.SetBool(BaseStateAI.NeedAvoid, needAvoid);
        }

        private void OnDisable()
        {
            detection.OnStatusChanged.RemoveListener(OnDetectionChanged);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = visiblePlayer ? Color.green : Color.yellow;
            Gizmos.DrawWireSphere(visiblePlayer ? followPlayerPoint : lastPlayerPoint, 0.2f);
        }
    }
}
