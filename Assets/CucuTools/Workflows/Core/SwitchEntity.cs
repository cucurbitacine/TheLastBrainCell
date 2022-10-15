using System;
using System.Linq;
using UnityEngine;

namespace CucuTools.Workflows.Core
{
    public abstract class SwitchEntity : TransitionEntity, IStateTrigger
    {
        public abstract void Invoke(StateInvoke mode);
        
        public abstract StateEntity[] Targets { get; }
    }
    
    public abstract class SwitchEntity<T> : SwitchEntity
    {
        public const string AddSwitch = Cucu.AddComponent + Cucu.WorkflowGroup + "Switch/";

        public override StateEntity[] Targets => caseValues.Select(c => c.target).ToArray();
        
        public override ConditionEntity[] Conditions { get; set; }
        public override bool IsReady => CheckCase();
        public override StateEntity Target
        {
            get => target;
            set => target = value;
        }

        public T Value
        {
            get => value;
            set
            {
                this.value = value;
                canSwitch = true;
            }
        }


        [Header("Info")]
        [SerializeField] protected bool autoSwitch;
        [SerializeField] protected bool canSwitch;
        
        [Header("Info")]
        [SerializeField] protected T value;
        [SerializeField] protected StateEntity target;
        
        [Header("Cases")]
        [SerializeField] protected CaseValue<T>[] caseValues;
        
        public override void Invoke(StateInvoke mode)
        {
            if (Owner == null) return;
            if (mode != StateInvoke.OnStart) return;
            canSwitch = autoSwitch;
        }
        
        private bool CheckCase()
        {
            var caseValue = caseValues.FirstOrDefault(c => c.value.Equals(Value));
            if (caseValue == null) return false;
            Target = caseValue.target;
            return canSwitch;
        }

        private void Awake()
        {
            if (autoSwitch) canSwitch = true;
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            
            if (!name.StartsWith("-<")) name = $"-< {name}";
            else if (!name.StartsWith("-< ")) name = $"-< {name.Substring(1)}";
        }
    }
    
    [Serializable]
    public class CaseValue<T>
    {
        public T value;
        public StateEntity target;
    }
}
