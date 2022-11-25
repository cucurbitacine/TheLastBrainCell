using UnityEngine;

namespace Game.Dev.AI
{
    public class FollowStateAI : BaseStateAI
    {
        public float timer = 0f;
        
        private Vector2 GetFollowPoint()
        {
            var playerPosition = ai.detectedPlayer.position;

            var directionToPlayer = (playerPosition - ai.enemy.position).normalized;

            var attackDistance = ai.enemy.JumpSetting.distance;

            return playerPosition - directionToPlayer * attackDistance;
        }

        private bool IsReadyAttack()
        {
            var distanceToPlayer = Vector2.Distance(ai.enemy.position, ai.detectedPlayer.position);
            
            var needStamina = ai.enemy.JumpSetting.staminaCost + ai.enemy.AttackSetting.staminaCost;
            var totalStamina = ai.enemy.Stamina.Value;
            
            return distanceToPlayer <= ai.enemy.JumpSetting.distance &&
                   needStamina <= totalStamina;
        }
        
        private void UpdateFollowing()
        {
            if (timer <= 0)
            {
                ai.movement.TryFollowToPoint(ai.followPlayerPoint);
            }
            
            timer += Time.deltaTime;

            if (timer >= ai.periodUpdatePath)
            {
                timer = 0f;
            }
        }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);
            
            timer = 0f;
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateUpdate(animator, stateInfo, layerIndex);

            if (ai.detectedPlayer == null) return;
            
            ai.followPlayerPoint = GetFollowPoint();

            var vectorToPlayer = ai.detectedPlayer.position - ai.enemy.position;
                
            ai.enemy.View(vectorToPlayer);
                
            if (IsReadyAttack())
            {
                ai.enemy.Animator.SetTrigger(ReadyAttack);
            }
            else
            {
                UpdateFollowing();
            }
        }
        
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateExit(animator, stateInfo, layerIndex);
            
            timer = 0f;
        }
    }
}