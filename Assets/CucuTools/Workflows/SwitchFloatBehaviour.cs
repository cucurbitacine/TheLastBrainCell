using CucuTools.Attributes;
using CucuTools.Workflows.Core;
using UnityEngine;

namespace CucuTools.Workflows
{
    [AddComponentMenu(AddSwitch + ObjectName)]
    public class SwitchFloatBehaviour : SwitchEntity<float>
    {
        public const string ObjectName = "Switch Float";
        
        [CucuButton("Trigger", group:"Add...")]
        private void AddTrigger()
        {
            gameObject.AddComponent<StateTrigger>();
        }
    }
}