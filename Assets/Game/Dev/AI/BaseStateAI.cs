using UnityEngine;
using UnityEngine.Animations;

namespace Game.Dev.AI
{
    public class BaseStateAI : StateMachineBehaviour
    {
        public EnemyAIController ai;
        
        public static readonly int VisiblePlayer = Animator.StringToHash("VisiblePlayer");
        public static readonly int PlayerDetected = Animator.StringToHash("PlayerDetected");
        public static readonly int ReadyAttack = Animator.StringToHash("ReadyAttack");
        public static readonly int NeedAvoid = Animator.StringToHash("NeedAvoid");
        
        private static string _previousState = string.Empty;
        
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            var currentState = GetType().Name;
            Debug.Log($"[{currentState}] <- [{_previousState}]");
            _previousState = currentState;
                
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
            if (ai == null) ai = component.GetComponentInChildren<EnemyAIController>();
        }
    }
}