using UnityEngine;

namespace Game.Dev.AI
{
    public class AvoidStateAI : BaseStateAI
    {
        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateUpdate(animator, stateInfo, layerIndex);

            if (ai.detectedPlayer == null) return;
            
            var directionToPlayer = ai.detectedPlayer.position - ai.enemy.position;
            
            ai.enemy.Move(-directionToPlayer);
            ai.enemy.View(ai.enemy.MoveSetting.velocity);
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateExit(animator, stateInfo, layerIndex);
            
            ai.movement.StopCharacter();
        }
    }
}