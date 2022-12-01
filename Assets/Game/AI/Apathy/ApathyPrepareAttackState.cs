using System.Collections;
using UnityEngine;

namespace Game.AI.Apathy
{
    public class ApathyPrepareAttackState : ApathyAIState
    {
        private Coroutine _attacking;
        
        private void Attack()
        {
            if (_attacking != null) ai.StopCoroutine(_attacking);
            _attacking = ai.StartCoroutine(Attacking());
        }
        
        private IEnumerator Attacking()
        {
            yield return new WaitForSeconds(ai.attackDelay);

            ai.Attack();
        }
        
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);

            Attack();
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateUpdate(animator, stateInfo, layerIndex);

            if (ai.detectedPlayer == null) return;
            
            var vectorToPlayer = ai.detectedPlayer.position - ai.npc.position;
            
            ai.npc.View(vectorToPlayer);
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateExit(animator, stateInfo, layerIndex);
            
            if (_attacking != null) ai.StopCoroutine(_attacking);
        }
    }
}