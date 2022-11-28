using UnityEngine;

namespace Game.Dev.AI
{
    public class DeadStateAI : BaseStateAI
    {
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);
            
            ai.movement.StopCharacter();
        }
    }
}