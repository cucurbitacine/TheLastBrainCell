using System;
using UnityEngine;
using UnityEngine.Events;

namespace CucuTools.Interactables
{
    /// <summary>
    /// Extended Base realisation of Interactable object
    /// </summary>
    [AddComponentMenu(Cucu.AddComponent + Cucu.InteractableGroup + ObjectName, 0)]
    public class InteractableBehaviourExt : InteractableBehaviour
    {
        public new const string ObjectName = "Interactable Extended";
        
        [Space]
        [SerializeField] private TransitEvents transitEvents = default;

        public TransitEvents TransitEvents => transitEvents ?? (transitEvents = new TransitEvents());
        
        private bool _wasIdle = default;
        private bool _wasHover = default;
        private bool _wasPress = default;

        private void HandleEvents()
        {
            if (_wasIdle)
            {
                if (InteractInfo.Hover) TransitEvents.OnIdleToHover.Invoke();
            }

            if (_wasHover)
            {
                if (InteractInfo.Idle) TransitEvents.OnHoverToIdle.Invoke();
                if (InteractInfo.Press) TransitEvents.OnHoverToPress.Invoke();
            }

            if (_wasPress)
            {
                if (InteractInfo.Hover) TransitEvents.OnPressToHover.Invoke();
            }
            
            _wasIdle = InteractInfo.Idle;
            _wasHover = InteractInfo.Hover;
            _wasPress = InteractInfo.Press;
        }

        protected virtual void Update()
        {
            if (IsEnabled) HandleEvents();
        }
    }

    [Serializable]
    public class TransitEvents
    {
        [Header("Idle")]
        [SerializeField] private UnityEvent onIdleToHover = default;
        [Header("Hover")]
        [SerializeField] private UnityEvent onHoverToIdle = default;
        [SerializeField] private UnityEvent onHoverToPress = default;
        [Header("Press")]
        [SerializeField] private UnityEvent onPressToHover = default;

        public UnityEvent OnPressToHover => onPressToHover ?? (onPressToHover = new UnityEvent());
        public UnityEvent OnHoverToPress => onHoverToPress ?? (onHoverToPress = new UnityEvent());
        public UnityEvent OnHoverToIdle => onHoverToIdle ?? (onHoverToIdle = new UnityEvent());
        public UnityEvent OnIdleToHover => onIdleToHover ?? (onIdleToHover = new UnityEvent());
    }
}