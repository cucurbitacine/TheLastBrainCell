using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace CucuTools.Interactables
{
    public class TriggerEvent : CucuBehaviour
    {
        [Space]
        [SerializeField] private TriggerMode triggerMode;
        [Space]
        [SerializeField] private UnityEvent<Collider> onTrigger;

        
        public UnityEvent<Collider> OnTrigger => onTrigger ?? (onTrigger = new UnityEvent<Collider>());
        
        public TriggerMode TriggerMode
        {
            get => triggerMode;
            set => triggerMode = value;
        }

        public virtual void Trigger(Collider other)
        {
            OnTrigger.Invoke(other);
        }
    }

    public enum TriggerMode
    {
        OnEnter,
        OnStay,
        OnExit,
    }
}