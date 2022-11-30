using System.Collections;
using UnityEngine;

namespace Game.AI.Fear
{
    public class FearIdleState : FearAIState
    {
        private Coroutine _goingToNextPoint;
        
        private void GoToNextPoint(float delay)
        {
            if (_goingToNextPoint != null) ai.StopCoroutine(_goingToNextPoint);
            _goingToNextPoint = ai.StartCoroutine(GoingToNextPoint(delay));
        }

        private IEnumerator GoingToNextPoint(float delay)
        {
            if (ai.patrolPath == null || ai.patrolPath.Length == 0)
            {
                ai.movement.TryFollowToPoint(ai.initPosition, () => ai.npc.View(ai.initDirection));
            }
            else
            {
                var pos = ai.patrolPath[ai.indexPatrol].position;
                var dir = ai.patrolPath[ai.indexPatrol].up;
                
                yield return new WaitForSeconds(delay);
                
                ai.movement.TryFollowToPoint(pos, () =>
                {
                    ai.npc.View(dir);
                    
                    GoToNextPoint(ai.patrolStayDuration);
                });
                
                ai.indexPatrol = (ai.indexPatrol + 1) % ai.patrolPath.Length;
            }
        }
        
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);

            GoToNextPoint(0f);
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateUpdate(animator, stateInfo, layerIndex);

            if (ai.npc.CharacterInfo.isMoving)
            {
                ai.npc.View(ai.npc.MoveSetting.velocity);
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateExit(animator, stateInfo, layerIndex);
            
            if (_goingToNextPoint != null) ai.StopCoroutine(_goingToNextPoint);
        }
    }
}