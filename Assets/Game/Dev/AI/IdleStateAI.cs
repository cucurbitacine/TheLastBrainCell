using Game.AI;
using Game.Characters;
using UnityEngine;

namespace Game.Dev.AI
{
    public class IdleStateAI : BaseStateAI
    {
        public void OnDetectionChanged(DetectionSample sample)
        {
            var player = sample.collider.GetComponent<PlayerController>();

            if (player == null) return;

            if (sample.status == DetectionStatus.Detected)
            {
                Enemy.Animator.SetBool(PlayerDetected, true);
            }
        }
        
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);

            Detection.OnStatusChanged.AddListener(OnDetectionChanged);
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateExit(animator, stateInfo, layerIndex);
            
            Detection.OnStatusChanged.RemoveListener(OnDetectionChanged);
        }
    }
}