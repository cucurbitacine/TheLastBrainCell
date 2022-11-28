using UnityEngine;

namespace Game.AI.Fear
{
    public class FearSearchStateAI : FearStateAI
    {
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);
            
            ai.movement.TryFollowToPoint(ai.lastPlayerPoint);
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateUpdate(animator, stateInfo, layerIndex);
            
            ai.npc.View(ai.npc.MoveSetting.velocity);
        }
    }
}