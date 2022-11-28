using UnityEngine;

namespace Game.AI.Fear
{
    public class FearAttackStateAI : FearStateAI
    {
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);

            if (ai.detectedPlayer == null) return;

            var vector = ai.detectedPlayer.position - ai.enemy.position;
            
            ai.enemy.SetView(vector);

            ai.enemy.Jump();
            
            ai.enemy.Attack("attack");
        }
    }
}