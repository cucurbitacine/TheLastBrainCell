using UnityEngine;

namespace Game.Dev.AI
{
    public class AttackStateAI : BaseStateAI
    {
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);

            if (ai.detectedPlayer == null) return;
            
            ai.enemy.direction = (ai.detectedPlayer.position - ai.enemy.position).normalized;
            
            ai.enemy.Jump();
            
            ai.enemy.Attack("attack");
        }
    }
}