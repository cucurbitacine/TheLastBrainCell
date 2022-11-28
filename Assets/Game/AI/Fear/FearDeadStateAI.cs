using UnityEngine;

namespace Game.AI.Fear
{
    public class FearDeadStateAI : FearStateAI
    {
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);
            
            ai.movement.StopCharacter();

            foreach (var characterAI in FindObjectsOfType<NpcAIController>())
            {
               
            }
        }
    }
}