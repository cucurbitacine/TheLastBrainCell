using CucuTools.Attributes;
using CucuTools.Workflows.Core;
using UnityEngine;

namespace CucuTools.Workflows
{
    [AddComponentMenu(AddSwitch + ObjectName)]
    public class SwitchStringBehaviour : SwitchEntity<string>
    {
        public const string ObjectName = "Switch String";
        
        [CucuButton("Trigger", group:"Add...")]
        private void AddTrigger()
        {
            gameObject.AddComponent<StateTrigger>();
        }
    }
}