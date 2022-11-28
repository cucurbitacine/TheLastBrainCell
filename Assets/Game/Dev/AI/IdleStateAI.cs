using UnityEngine;

namespace Game.Dev.AI
{
    public class IdleStateAI : BaseStateAI
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