using System.Linq;
using CucuTools.Attributes;
using CucuTools.Workflows.Core;
using UnityEngine;

namespace CucuTools.Workflows
{
    [AddComponentMenu(Cucu.AddComponent + Cucu.WorkflowGroup + ObjectName, 0)]
    public sealed class WorkflowBehaviour : WorkflowEntity
    {
        #region SerializeField

        [Header("Info")]
        [SerializeField] private bool isPlaying;
        [SerializeField] private bool paused;
        [SerializeField] private StateEntity current;
        
        [Header("First")]
        [SerializeField] private StateEntity first;
        
        [Header("Transitions")]
        [SerializeField] private TransitionEntity[] transitions;

        [Header("Editor")]
        [SerializeField] private bool debugLog;

        #endregion
        
        public const string ObjectName = "Workflow";

        public override bool IsPlaying => isPlaying;

        public override bool IsLast => Current.IsLast && ((Transitions?.Length ?? 0) == 0);
        
        public override StateEntity Current => current;

        public override bool Paused
        {
            get => paused;
            set => paused = value;
        }
        
        public override TransitionEntity[] Transitions
        {
            get => transitions;
            set => transitions = value;
        }

        public StateEntity First => first;
        
        private TriggerHandler triggerHandler;

        #region Public API

        public void SetCurrent(StateEntity state)
        {
            if (debugLog) Debug.Log($"{(current != null ? current.name : "")} -> {(state.name)}");
            
            current = state;
        }
        
        public override bool TryGetNextState(out StateEntity nextState)
        {
            nextState = null;
            return IsPlaying && Current.IsLast && ((nextState = Transitions?.FirstOrDefault(t => t.IsReady)?.Target) != null);
        }

        public override void StartState()
        {
            if (IsPlaying) return;

            SetCurrent(First);
            
            isPlaying = true;

            triggerHandler.Invoke(StateInvoke.OnStart);

            Current.StartState();
        }

        public override void StopState()
        {
            if (!IsPlaying) return;

            isPlaying = false;
            
            triggerHandler.Invoke(StateInvoke.OnStop);
            
            Current.StopState();
        }

        public void Validate()
        {
            SetupTransitions();
            SetupTriggers();

            foreach (var transition in transitions)
            {
                //transition?.Validate();
            }

            //First?.Validate();
            
            if (!name.StartsWith("#")) name = $"# {name}";
            else if (!name.StartsWith("# ")) name = $"# {name.Substring(1)}";
        }
        
        #endregion

        #region Private API

        private void SetupTransitions()
        {
            transitions = GetComponentsInChildren<TransitionEntity>()
                .Where(t => t.Owner == this)
                .ToArray();
        }
        
        private void SetupTriggers()
        {
            triggerHandler = new TriggerHandler(this, GetComponentsInChildren<IStateTrigger>()
                .Where(t => t.Owner == this)
                .ToArray());
        }
        
        private void UpdateStateMachine()
        {
            if (Paused) return;
            
            if (Current.IsLast)
            {
                return;
            }

            if (Current.TryGetNextState(out var nextState))
            {
                Current.StopState();
                SetCurrent(nextState);
                Current.StartState();
            }
        }

        #endregion

        #region MonoBehaviour

        private void Awake()
        {
            isPlaying = false;
            
            Validate();
        }
        
        private void Update()
        {
            if (IsPlaying) UpdateStateMachine();
        }

        private void OnValidate()
        {
            Validate();
        }

        #endregion
        
        #region Editor

        private const string crtGrp = "Create...";
        private const string crtSwtGrp = "Create Switch...";
        private const string addGrp = "Add...";
        
        [CucuButton("Transition", group:crtGrp)]
        private void CreateTransition()
        {
            new GameObject("").AddComponent<TransitionBehaviour>().transform.SetParent(transform);
        }
        
        [CucuButton("Switch Bool", group:crtSwtGrp)]
        private void CreateSwitchBool()
        {
            new GameObject("").AddComponent<SwitchBoolBehaviour>().transform.SetParent(transform);
        }
        
        [CucuButton("Switch Int", group:crtSwtGrp)]
        private void CreateSwitchInt()
        {
            new GameObject("").AddComponent<SwitchIntBehaviour>().transform.SetParent(transform);
        }
        
        [CucuButton("Switch Float", group:crtSwtGrp)]
        private void CreateSwitchFloat()
        {
            new GameObject("").AddComponent<SwitchFloatBehaviour>().transform.SetParent(transform);
        }
        
        [CucuButton("Switch String", group:crtSwtGrp)]
        private void CreateSwitchString()
        {
            new GameObject("").AddComponent<SwitchFloatBehaviour>().transform.SetParent(transform);
        }
        
        [CucuButton("Trigger", group:addGrp)]
        private void AddTrigger()
        {
            gameObject.AddComponent<StateTrigger>();
        }

        #endregion
    }
}