using UnityEngine;

namespace Game.AI.Apathy
{
    public class ApathyAttackState : ApathyAIState
    {
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);

            if (ai.detectedPlayer == null) return;

            var vector = ai.detectedPlayer.position - ai.npc.position;
            
            ai.npc.SetView(vector);

            ai.npc.Attack("apathy_rangeAttack");
        }
        
        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateUpdate(animator, stateInfo, layerIndex);

            if (ai.detectedPlayer == null) return;
            
            var vectorToPlayer = ai.detectedPlayer.position - ai.npc.position;
            
            ai.npc.View(vectorToPlayer);
        }
    }
}