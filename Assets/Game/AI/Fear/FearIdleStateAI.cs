using UnityEngine;

namespace Game.AI.Fear
{
    public class FearIdleStateAI : FearStateAI
    {
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);

            ai.movement.TryFollowToPoint(ai.initPosition, () => ai.enemy.View(ai.initDirection));
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateUpdate(animator, stateInfo, layerIndex);
            
            ai.enemy.View(ai.enemy.MoveSetting.velocity);
        }
    }
}