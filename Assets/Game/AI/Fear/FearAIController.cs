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

        public void Attack()
        {
            npc.Animator.SetTrigger(AttackTrigger);
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
        
        protected override void Awake()
        {
            base.Awake();

            initPosition = npc.position;
            initDirection = npc.direction;
        }
        
        private void Update()
        {
            UpdateAvoiding();

            UpdateAttacking();
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
