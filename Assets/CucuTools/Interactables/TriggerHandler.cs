using System;
using System.Collections.Generic;
using System.Linq;
using CucuTools.Attributes;
using UnityEngine;

namespace CucuTools.Interactables
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Collider))]
    public class TriggerHandler : CucuBehaviour
    {
        private Dictionary<TriggerMode, TriggerEvent[]> _triggers = default;

        [Space] [SerializeField] private LayerMask layerMask = 1;
        [Space] [SerializeField] private Collider[] specific = default;

        public Collider Collider { get; private set; }

        public Collider[] Specific
        {
            get => specific;
            set => specific = value;
        }

        public LayerMask LayerMask
        {
            get => layerMask;
            set => layerMask = value;
        }

        public void Trigger(Collider other, TriggerMode mode)
        {
            if (other == null) return;

            if (!LayerMask.Contains(other.gameObject.layer)) return;

            if (!_triggers.TryGetValue(mode, out var triggerEvents)) return;

            if (Specific != null && Specific.Any())
            {
                if (specific.Contains(other))
                {
                    Trigger(other, triggerEvents);
                }
            }
            else
            {
                Trigger(other, triggerEvents);
            }
        }

        private static void Trigger(Collider other, params TriggerEvent[] triggerEvents)
        {
            if (triggerEvents == null || triggerEvents.Length == 0) return;

            foreach (var triggerEvent in triggerEvents)
            {
                triggerEvent.Trigger(other);
            }
        }

        [CucuButton("Add Event")]
        private void AddEvent()
        {
            gameObject.AddComponent<TriggerEvent>();
        }

        #region MonoBehaviour

        private void Awake()
        {
            Collider = GetComponent<Collider>();
            Collider.isTrigger = true;

            Specific = Specific.Where(s => s != null).ToArray();
        }

        private void Start()
        {
            _triggers = GetComponentsInChildren<TriggerEvent>()
                .GroupBy(t => t.TriggerMode)
                .ToDictionary(t => t.Key, t => t.ToArray());
        }

        private void OnTriggerEnter(Collider other)
        {
            Trigger(other, TriggerMode.OnEnter);
        }

        private void OnTriggerStay(Collider other)
        {
            Trigger(other, TriggerMode.OnStay);
        }

        private void OnTriggerExit(Collider other)
        {
            Trigger(other, TriggerMode.OnExit);
        }

        #endregion
    }
}