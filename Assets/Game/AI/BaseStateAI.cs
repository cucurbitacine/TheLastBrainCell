using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

namespace Game.AI
{
    /// <summary>
    /// Base state behaviour for enemy AI 
    /// </summary>
    /// <typeparam name="TEnemyAI"></typeparam>
    public class BaseStateAI<TEnemyAI> : StateMachineBehaviour where TEnemyAI : EnemyAIController
    {
        public TEnemyAI ai;

        private static readonly Dictionary<Animator, string> PreviousStates = new Dictionary<Animator, string>();
        
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);

            InitState(animator);

            DebugLogOnStateEnter(animator);
        }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex,
            AnimatorControllerPlayable controller)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex, controller);

            InitState(animator);
            
            DebugLogOnStateEnter(animator);
        }

        private void InitState(Component component)
        {
            if (ai == null) ai = component.GetComponentInChildren<TEnemyAI>();
        }

        private void DebugLogOnStateEnter(Animator animator)
        {
            if (ai.debugMode)
            {
                var currentState = GetType().Name;

                if (!PreviousStates.TryGetValue(animator, out var previousState))
                {
                    previousState = string.Empty;
                    PreviousStates.Add(animator, previousState);
                }
            
                Debug.Log($"{animator.name} :: [{currentState}] <- [{previousState}]");
            
                PreviousStates[animator] = currentState;
            }
        }
    }
}