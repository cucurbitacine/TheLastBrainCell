using UnityEngine;

namespace Game.AI.Apathy
{
    public class ApathyAIController : NpcAIController
    {
        [Space]
        [Min(0f)]
        public float attackDelay = 0.5f;
        
        public void Attack()
        {
            npc.Animator.SetTrigger(AttackTrigger);
        }
        
        private void UpdateAttacking()
        {
            if (!npc.Animator.GetBool(VisiblePlayer))
            {
                npc.Animator.SetBool(ReadyAttack, false);
                
                return;
            }
            
            var needStamina = npc.AttackSetting.staminaCost;
            var totalStamina = npc.Stamina.Value;
            
            var readyAttack = needStamina <= totalStamina;

            npc.Animator.SetBool(ReadyAttack, readyAttack);
        }

        private void Update()
        {
            UpdateAttacking();
        }
    }
}
