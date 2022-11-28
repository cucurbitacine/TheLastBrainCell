using UnityEngine;

namespace Game.AI.Fear
{
    public class FearAttackStateAI : FearStateAI
    {
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);

            if (ai.detectedPlayer == null) return;

            var vector = ai.detectedPlayer.position - ai.npc.position;
            
            ai.npc.SetView(vector);

            ai.npc.Jump();
            
            ai.npc.Attack("attack");
        }
    }
}