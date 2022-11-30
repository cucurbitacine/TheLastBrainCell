using UnityEngine;

namespace Game.AI.Fear
{
    public class FearAvoidState : FearAIState
    {
        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateUpdate(animator, stateInfo, layerIndex);

            if (ai.detectedPlayer == null) return;
            
            var directionToPlayer = ai.detectedPlayer.position - ai.npc.position;
            
            ai.npc.Move(-directionToPlayer);
            ai.npc.View(ai.npc.MoveSetting.velocity);
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateExit(animator, stateInfo, layerIndex);
            
            ai.movement.StopCharacter();
        }
    }
}