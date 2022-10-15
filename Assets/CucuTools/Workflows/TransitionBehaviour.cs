using System.Linq;
using CucuTools.Attributes;
using CucuTools.Workflows.Core;
using UnityEngine;

namespace CucuTools.Workflows
{
    [AddComponentMenu(Cucu.AddComponent + Cucu.WorkflowGroup + ObjectName, 2)]
    public sealed class TransitionBehaviour : TransitionEntity
    {
        #region SerializeField

        [Header("Overrides")]
        [SerializeField] private bool mute;
        [SerializeField] private bool solo;
        
        [Header("Target")]
        [SerializeField] private StateEntity target;
        
        [Header("Condition")]
        [SerializeField] private ConditionMode mode;
        [SerializeField] private ConditionEntity[] conditions;

        #endregion
        
        public const string ObjectName = "Transition";
        
        public override bool IsReady => IsReadyInternal();

        public override StateEntity Target
        {
            get => target;
            set => target = value;
        }

        public override ConditionEntity[] Conditions
        {
            get => conditions;
            set => conditions = value;
        }
        
        public ConditionMode Mode
        {
            get => mode;
            set => mode = value;
        }
        
        #region Private API

        private void Setup()
        {
            conditions = GetComponentsInChildren<ConditionEntity>();
        }


        private bool IsReadyInternal()
        {
            if (mute) return false;
            if (solo) return true;

            return Mode == ConditionMode.All ? Conditions.All(c => c.Done) : Conditions.Any(c => c.Done);
        }

        #endregion

        #region MonoBehaviour

        private void Awake()
        {
            Setup();
        }

        protected override void OnValidate()
        {
            base.OnValidate();

            name = $"-> {(Target != null ? (Target.name.StartsWith("* ") || Target.name.StartsWith("# ") ? Target.name.Substring(2) : Target.name) : "")}";
        }

        #endregion

        #region Editor

        [CucuButton("Condition", group:"Create...")]
        private void CreateCondition()
        {
            new GameObject("Condition").AddComponent<ConditionBehaviour>().transform.SetParent(transform);
        }
        
        [CucuButton("Trigger", group:"Add...")]
        private void AddTrigger()
        {
            gameObject.AddComponent<StateTrigger>();
        }

        #endregion
        
        public enum ConditionMode
        {
            All,
            Any
        }
    }
}