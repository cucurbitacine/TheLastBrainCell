using UnityEngine;

namespace Game.AI.Fear
{
    public class FearFollowState : FearAIState
    {
        private float _timer = 0f;
        
        private Vector2 GetFollowPoint()
        {
            var playerPosition = ai.detectedPlayer.position;

            var directionToPlayer = (playerPosition - ai.npc.position).normalized;

            var attackDistance = ai.npc.JumpSetting.distance;

            return playerPosition - directionToPlayer * attackDistance;
        }
        
        private void UpdateFollowing()
        {
            if (_timer <= 0)
            {
                ai.movement.TryFollowToPoint(ai.followPlayerPoint);
            }
            
            _timer += Time.deltaTime;

            if (_timer >= ai.periodUpdatePath)
            {
                _timer = 0f;
            }
        }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);
            
            _timer = 0f;
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateUpdate(animator, stateInfo, layerIndex);

            if (ai.detectedPlayer == null) return;

            if (!ai.visiblePlayer) return;

            ai.followPlayerPoint = GetFollowPoint();

            var vectorToPlayer = ai.detectedPlayer.position - ai.npc.position;
                
            ai.npc.View(vectorToPlayer);
                
            UpdateFollowing();
        }
        
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateExit(animator, stateInfo, layerIndex);
            
            _timer = 0f;
            
            ai.movement.StopCharacter();
        }
    }
}