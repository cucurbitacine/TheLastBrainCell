using System;
using CucuTools.Attributes;
using UnityEngine;
using UnityEngine.Events;

namespace CucuTools.Interactables
{
    /// <summary>
    /// Base realisation of Interactable behavior
    /// </summary>
    [AddComponentMenu(Cucu.AddComponent + Cucu.InteractableGroup + ObjectName, 0)]
    public class InteractableBehaviour : InteractableEntity
    {
        public const string ObjectName = "Interactable";
        
        #region SerializeField

        [Space]
        [SerializeField] private bool isEnabled = true;
        
        [Space]
        [SerializeField] private InteractInfo _interactInfo;
        [Space]
        [SerializeField] private InteractEvents _interactEvents;

        #endregion

        public override bool IsEnabled
        {
            get => isEnabled;
            set
            {
                if (isEnabled == value) return;
                
                isEnabled = !isEnabled;

                if (isEnabled) InteractEvents.OnEnabled.Invoke();
                else InteractEvents.OnDisabled.Invoke();
            }
        }

        /// <inheritdoc />
        public override InteractInfo InteractInfo => _interactInfo ?? (_interactInfo = new InteractInfo());
        
        public InteractEvents InteractEvents => _interactEvents ?? (_interactEvents = new InteractEvents());
        
        /// <inheritdoc />
        public override void Idle()
        {
            if (!IsEnabled) return;
            
            if (InteractInfo.Idle) return;

            InteractInfo.Idle = true;
            
            if (InteractInfo.Hover)
            {
                InteractInfo.Hover = false;
                InteractEvents.OnHoverCancel.Invoke();
            }
            
            if (InteractInfo.Press)
            {
                InteractInfo.Press = false;
                InteractEvents.OnPressCancel.Invoke();
            }

            InteractEvents.OnIdleStart.Invoke();
        }

        /// <inheritdoc />
        public override void Hover(ICucuContext context)
        {
            if (!IsEnabled) return;
            
            if (InteractInfo.Hover) return;
            
            InteractInfo.Hover = true;
            
            if (InteractInfo.Idle)
            {
                InteractInfo.Idle = false;
                InteractEvents.OnIdleCancel.Invoke();
            }
            
            if (InteractInfo.Press)
            {
                InteractInfo.Press = false;
                InteractEvents.OnPressCancel.Invoke();
            }
            
            InteractEvents.OnHoverStart.Invoke(context);
        }

        /// <inheritdoc />
        public override void Press(ICucuContext context)
        {
            if (!IsEnabled) return;
            
            if (InteractInfo.Press) return;
            
            InteractInfo.Press = true;
            
            if (InteractInfo.Idle)
            {
                InteractInfo.Idle = false;
                InteractEvents.OnIdleCancel.Invoke();
            }
            
            if (InteractInfo.Hover)
            {
                InteractInfo.Hover = false;
                InteractEvents.OnHoverCancel.Invoke();
            }
            
            InteractEvents.OnPressStart.Invoke(context);
        }

        [CucuButton("Make it Grabbable")]
        protected void AddGrabbable()
        {
            var grabbable = gameObject.GetComponent<GrabbableEntity>();
            if (grabbable == null) gameObject.AddComponent<GrabbableBehaviour>();
        }
        
        protected virtual void Start()
        {
            if (IsEnabled && InteractInfo.Idle)
            {
                InteractEvents.OnIdleStart.Invoke();
            }
        }
    }

    [Serializable]
    public class InteractEvents
    {
        public UnityEvent OnEnabled => onEnabled ?? (onEnabled = new UnityEvent());
        public UnityEvent OnDisabled => onDisabled ?? (onDisabled = new UnityEvent());
        
        public UnityEvent OnIdleStart => onIdleStart ?? (onIdleStart = new UnityEvent());
        public UnityEvent OnIdleCancel => onIdleCancel ?? (onIdleCancel = new UnityEvent());

        public UnityEvent<ICucuContext> OnHoverStart => onHoverStart ?? (onHoverStart = new UnityEvent<ICucuContext>());
        public UnityEvent OnHoverCancel => onHoverCancel ?? (onHoverCancel = new UnityEvent());

        public UnityEvent<ICucuContext> OnPressStart => onPressStart ?? (onPressStart = new UnityEvent<ICucuContext>());
        public UnityEvent OnPressCancel => onPressCancel ?? (onPressCancel = new UnityEvent());

        [Header("Enable")]
        [SerializeField] private UnityEvent onEnabled;
        [SerializeField] private UnityEvent onDisabled;
        
        [Header("Idle")]
        [SerializeField] private UnityEvent onIdleStart;
        [SerializeField] private UnityEvent onIdleCancel;
        
        [Header("Hover")]
        [SerializeField] private UnityEvent<ICucuContext> onHoverStart;
        [SerializeField] private UnityEvent onHoverCancel;
        
        [Header("Press")]
        [SerializeField] private UnityEvent<ICucuContext> onPressStart;
        [SerializeField] private UnityEvent onPressCancel;
    }
}
