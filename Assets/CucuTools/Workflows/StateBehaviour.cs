using System.Linq;
using CucuTools.Attributes;
using CucuTools.Workflows.Core;
using UnityEngine;

namespace CucuTools.Workflows
{
    [AddComponentMenu(Cucu.AddComponent + Cucu.WorkflowGroup + ObjectName, 1)]
    public sealed class StateBehaviour : StateEntity
    {
        #region SerializeField

        [Header("Info")]
        [SerializeField] private bool isPlaying;
        
        [Header("Transitions")]
        [SerializeField] private TransitionEntity[] transitions;

        #endregion
        
        public const string ObjectName = "State";
        
        public override bool IsPlaying => isPlaying;

        public override bool IsLast => (Transitions?.Length ?? 0) == 0;

        public override TransitionEntity[] Transitions
        {
            get => transitions;
            set => transitions = value;
        }

        private TriggerHandler triggerHandler;

        #region Public API

        public override bool TryGetNextState(out StateEntity nextState)
        {
            nextState = null;
            return IsPlaying && (nextState = Transitions?.FirstOrDefault(t => t.IsReady)?.Target) != null;
        }

        public override void StartState()
        {
            if (IsPlaying) return;
            
            isPlaying = true;

            triggerHandler.Invoke(StateInvoke.OnStart);
        }

        public override void StopState()
        {
            if (!IsPlaying) return;
            
            isPlaying = false;
            
            triggerHandler.Invoke(StateInvoke.OnStop);
        }

        #endregion

        #region Private API

        private void Setup()
        {
            isPlaying = false;
            
            SetupTransitions();
            SetupTriggers();
        }
        
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

        #endregion

        #region MonoBehaviour

        private void Awake()
        {
            Setup();
        }

        private void OnValidate()
        {
            if (!name.StartsWith("*")) name = $"* {name}";
            else if (!name.StartsWith("* ")) name = $"* {name.Substring(1)}";
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