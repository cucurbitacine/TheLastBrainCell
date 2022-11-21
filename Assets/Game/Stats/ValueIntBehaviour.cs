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
        [SerializeField] private UnityEvent<int> onValueChanged = new UnityEvent<int>();

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
        
        public UnityEvent<int> OnValueChanged => onValueChanged ??= new UnityEvent<int>();

        private void SetValue(int newValue)
        {
            newValue = Mathf.Clamp(newValue, 0, MaxValue);
            
            if (value == newValue) return;

            value = newValue;
            
            OnValueChanged.Invoke(value);
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
            
            OnValueChanged.Invoke(value);
        }
    }
}
