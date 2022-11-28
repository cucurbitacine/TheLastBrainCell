using System.Collections;
using UnityEngine;

namespace Game.AI.Fear
{
    public class FearIdleStateAI : FearStateAI
    {
        public float stayDuration = 5f;
        public int index;

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
                ai.movement.TryFollowToPoint(ai.initPosition, () => ai.enemy.View(ai.initDirection));
            }
            else
            {
                Debug.LogWarning($"Going to [{index}] {ai.patrolPath[index].name}");
                
                var pos = ai.patrolPath[index].position;
                var dir = ai.patrolPath[index].up;
                
                yield return new WaitForSeconds(delay);
                
                ai.movement.TryFollowToPoint(pos, () =>
                {
                    ai.enemy.View(dir);
                    
                    GoToNextPoint(stayDuration);
                });
                
                index = (index + 1) % ai.patrolPath.Length;
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

            if (ai.enemy.CharacterInfo.isMoving)
            {
                ai.enemy.View(ai.enemy.MoveSetting.velocity);
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateExit(animator, stateInfo, layerIndex);
            
            if (_goingToNextPoint != null) ai.StopCoroutine(_goingToNextPoint);
        }
    }
}