using UnityEngine;

namespace CucuTools.Workflows.Core
{
    [DisallowMultipleComponent]
    public abstract class StateEntity : CucuBehaviour
    {
        public abstract TransitionEntity[] Transitions { get; set; }
        
        public abstract bool IsPlaying { get; }

        public abstract bool IsLast { get; }

        public abstract bool TryGetNextState(out StateEntity nextState);

        public abstract void StartState();
        
        public abstract void StopState();
    }
}