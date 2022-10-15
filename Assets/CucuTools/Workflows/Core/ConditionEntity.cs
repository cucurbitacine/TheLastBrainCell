using System;
using System.Collections;
using UnityEngine;

namespace CucuTools.Workflows.Core
{
    public abstract class ConditionEntity : OwnOfState, IStateTrigger
    {
        [SerializeField] private TriggerParam triggerParam; 
        
        public abstract bool Done { get; set; }

        public void Set(bool value)
        {
            Done = value;
        }
        
        public void True()
        {
            Set(true);
        }
        
        public void False()
        {
            Set(false);
        }
        
        public void Toggle()
        {
            Set(!Done);
        }

        public void TrueDelay(float delay)
        {
            StartCoroutine(SetDelay(this, true, delay));
        }
        
        public void FalseDelay(float delay)
        {
            StartCoroutine(SetDelay(this, false, delay));
        }

        private static IEnumerator SetDelay(ConditionEntity condition, bool value, float delay)
        {
            yield return new WaitForSeconds(delay);
            condition.Set(value);
        }
        
        public void Invoke(StateInvoke mode)
        {
            if (Owner == null) return;
            if (triggerParam == null) triggerParam = new TriggerParam();
            if (!triggerParam.resetValue) return;
            if (triggerParam.resetInvoke != mode) return;
            False();
        }

        [Serializable]
        private class TriggerParam
        {
            public bool resetValue;
            public StateInvoke resetInvoke;

            public TriggerParam()
            {
                resetValue = true;
                resetInvoke = StateInvoke.OnStart;
            }
        }
    }
}