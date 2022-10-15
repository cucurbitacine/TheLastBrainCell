using CucuTools.Workflows.Core;
using UnityEngine;
using UnityEngine.Events;

namespace CucuTools.Workflows
{
    [AddComponentMenu(Cucu.AddComponent + Cucu.WorkflowGroup + ObjectName, 4)]
    public sealed class StateTrigger : OwnOfState, IStateTrigger
    {
        [SerializeField] private StateInvoke mode;
        [SerializeField] private UnityEvent onInvoke;
        
        public const string ObjectName = "Trigger";
        
        public StateInvoke Mode
        {
            get => mode;
            set => mode = value;
        }
        
        public void Invoke(StateInvoke mode)
        {
            if (Owner == null) return;
            if (Mode == mode) onInvoke?.Invoke();
        }
    }

    public enum StateInvoke
    {
        OnStart,
        OnStop,
    }
    
    public interface IStateTrigger : IOwnOfState
    {
        void Invoke(StateInvoke mode);
    }
}