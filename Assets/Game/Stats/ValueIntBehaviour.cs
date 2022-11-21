using System;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Stats
{
    public class ValueIntBehaviour : MonoBehaviour
    {
        [Space]
        [Min(0)]
        [SerializeField] private int value = 1;
        [Min(0)]
        [SerializeField] private int maxValue = 1;

        [Space]
        [SerializeField] private ValueIntEvents valueEvents = null;

        public int Value
        {
            get => value;
            set => SetValue(value);
        }

        public int MaxValue
        {
            get => maxValue;
            set => SetMaxValue(value);
        }

        public ValueIntEvents Events => valueEvents ??= new ValueIntEvents();

        private void SetValue(int newValue)
        {
            newValue = Mathf.Clamp(newValue, 0, MaxValue);
            
            if (value == newValue) return;

            value = newValue;
            
            Events.OnValueChanged.Invoke(value);

            if (value == 0) Events.OnValueIsEmpty.Invoke();
            else if (value == maxValue) Events.OnValueIsFull.Invoke();
        }

        private void SetMaxValue(int newMaxValue)
        {
            newMaxValue = Mathf.Max(0, newMaxValue);
            
            if (maxValue == newMaxValue) return;

            maxValue = newMaxValue;

            SetValue(value);
        }
        
        private void OnValidate()
        {
            SetValue(Value);
            
            Events.OnValueChanged.Invoke(value);
        }
    }

    [Serializable]
    public class ValueIntEvents
    {
        [SerializeField] private UnityEvent<int> onValueChanged = new UnityEvent<int>();
        [SerializeField] private UnityEvent onValueIsEmpty = new UnityEvent();
        [SerializeField] private UnityEvent onValueIsFull = new UnityEvent();
        
        public UnityEvent<int> OnValueChanged => onValueChanged ??= new UnityEvent<int>();
        public UnityEvent OnValueIsEmpty => onValueIsEmpty ??= new UnityEvent();
        public UnityEvent OnValueIsFull => onValueIsFull ??= new UnityEvent();
    }
}
