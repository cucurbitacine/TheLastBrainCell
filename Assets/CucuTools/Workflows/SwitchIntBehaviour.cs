using CucuTools.Attributes;
using CucuTools.Workflows.Core;
using UnityEngine;

namespace CucuTools.Workflows
{
    [AddComponentMenu(AddSwitch + ObjectName)]
    public class SwitchIntBehaviour : SwitchEntity<int>
    {
        public const string ObjectName = "Switch Int";
        
        [CucuButton("Trigger", group:"Add...")]
        private void AddTrigger()
        {
            gameObject.AddComponent<StateTrigger>();
        }
    }
}