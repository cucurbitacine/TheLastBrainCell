using Game.AI;
using Game.Characters;
using UnityEngine;
using UnityEngine.Animations;

namespace Game.Dev.AI
{
    public class BaseStateAI : StateMachineBehaviour
    {
        public EnemyController Enemy
        {
            get => _enemy;
            private set => _enemy = value;
        }

        public DetectionController Detection
        {
            get => _detection;
            private set => _detection = value;
        }

        private DetectionController _detection;
        private EnemyController _enemy;

        protected static readonly int PlayerDetected = Animator.StringToHash("PlayerDetected");
        
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);

            InitState(animator);
        }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex,
            AnimatorControllerPlayable controller)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex, controller);

            InitState(animator);
        }

        private void InitState(Component component)
        {
            if (Enemy == null) Enemy = component.GetComponent<EnemyController>();
            if (Detection == null) Detection = Enemy.GetComponentInChildren<DetectionController>();
        }
    }
}