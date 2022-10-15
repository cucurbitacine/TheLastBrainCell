using System.Linq;
using UnityEngine;

namespace CucuTools.Interactables
{
    /// <summary>
    /// Bridge to Interactable object <see cref="Target"/>
    /// </summary>
    [AddComponentMenu(Cucu.AddComponent + Cucu.InteractableGroup + ObjectName, 0)]
    public class InteractableBridge : InteractableEntity
    {
        [Space]
        [SerializeField] private InteractableEntity target;

        public const string ObjectName = "Interactable Bridge";

        public override bool IsEnabled
        {
            get => Target?.IsEnabled ?? false;
            set
            {
                if (Target != null) Target.IsEnabled = value;
            }
        }

        public InteractableEntity Target => target;

        public override InteractInfo InteractInfo => Target?.InteractInfo;
        
        public override void Idle()
        {
            Target?.Idle();
        }

        public override void Hover(ICucuContext context)
        {
            Target?.Hover(context);
        }

        public override void Press(ICucuContext context)
        {
            Target?.Press(context);
        }

        private void OnValidate()
        {
            if (target == null) target = GetComponentsInChildren<InteractableEntity>().FirstOrDefault(c => c != this);
            if (target == null) target = GetComponentsInParent<InteractableEntity>().FirstOrDefault(c => c != this);
            
            if (target != null && target == this) target = null;
        }
    }
}