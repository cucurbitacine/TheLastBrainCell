using UnityEngine;

namespace Game.AI.Fear
{
    public class FearDeadState : FearAIState
    {
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);
            
            ai.movement.StopCharacter();
        }
    }
}