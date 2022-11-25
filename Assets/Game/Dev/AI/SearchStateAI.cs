using Game.AI;
using Game.Characters;
using UnityEngine;

namespace Game.Dev.AI
{
    public class SearchStateAI : BaseStateAI
    {
        private void OnDetectionChanged(DetectionSample sample)
        {
            var player = sample.collider.GetComponent<PlayerController>();

            if (player != ai.detectedPlayer) return;

            if (sample.status == DetectionStatus.Detected)
            {
                ai.visiblePlayer = true;
                ai.enemy.Animator.SetBool(VisiblePlayer, true);
            }
            else if (sample.status == DetectionStatus.Undefined)
            {
                ai.detectedPlayer = null;
                ai.enemy.Animator.SetBool(PlayerDetected, false);
            }
        }
        
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);
            
            ai.detection.OnStatusChanged.AddListener(OnDetectionChanged);
            
            ai.movement.TryFollowToPoint(ai.lastPlayerPoint);
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateUpdate(animator, stateInfo, layerIndex);
            
            ai.enemy.View(ai.enemy.MoveSetting.velocity);
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateExit(animator, stateInfo, layerIndex);
            
            ai.detection.OnStatusChanged.RemoveListener(OnDetectionChanged);
        }
    }
}