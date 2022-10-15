using CucuTools.Attributes;
using CucuTools.Workflows.Core;
using UnityEngine;

namespace CucuTools.Workflows
{
    [AddComponentMenu(Cucu.AddComponent + Cucu.WorkflowGroup + ObjectName, 3)]
    public sealed class ConditionBehaviour : ConditionEntity
    {
        [Header("Condition")]
        [SerializeField] private bool done;
        
        public const string ObjectName = "Condition";
        
        public override bool Done
        {
            get => done;
            set => done = value;
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            
            if (!name.StartsWith("?")) name = $"? {name}";
            else if (!name.StartsWith("? ")) name = $"? {name.Substring(1)}";
        }

        [CucuButton("Trigger", group:"Add...")]
        private void AddTrigger()
        {
            gameObject.AddComponent<StateTrigger>();
        }
    }
}