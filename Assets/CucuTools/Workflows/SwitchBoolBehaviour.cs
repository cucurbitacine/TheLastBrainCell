using System.Linq;
using CucuTools.Attributes;
using CucuTools.Workflows.Core;
using UnityEngine;

namespace CucuTools.Workflows
{
    [AddComponentMenu(AddSwitch + ObjectName)]
    public class SwitchBoolBehaviour : SwitchEntity<bool>
    {
        public const string ObjectName = "Switch Bool";

        protected override void OnValidate()
        {
            base.OnValidate();

            if (caseValues == null || caseValues.Length == 0) caseValues = new CaseValue<bool>[2];
            if (caseValues.Length == 1) caseValues = caseValues.Concat(new CaseValue<bool>[1]).ToArray();
            if (caseValues.Length > 2) caseValues = caseValues.Take(2).ToArray();

            for (var i = 0; i < 2; i++)
                if (caseValues[i] == null) caseValues[i] = new CaseValue<bool>();
            
            caseValues[0].value = false;
            caseValues[1].value = true;
        }
        
        [CucuButton("Trigger", group:"Add...")]
        private void AddTrigger()
        {
            gameObject.AddComponent<StateTrigger>();
        }
    }
}