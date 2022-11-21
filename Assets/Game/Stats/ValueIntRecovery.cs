using UnityEngine;

namespace Game.Stats
{
    public class ValueIntRecovery : MonoBehaviour
    {
        [Space]
        [SerializeField] private bool recoveryActive = true;
        
        [Space]
        [Min(0)]
        [SerializeField] private int recoveryAmount = 1;
        [Min(0)]
        [SerializeField] private float recoveryPeriod = 1f;
        
        [Space]
        [SerializeField] private ValueIntBehaviour intBehaviour;
        
        private float _timer = 0f;
        
        public ValueIntBehaviour IntBehaviour => intBehaviour ??= GetComponent<ValueIntBehaviour>();

        public bool RecoveryActive
        {
            get => recoveryActive;
            set
            {
                if (recoveryActive == value) return;
                
                _timer = 0f;
                recoveryActive = value;
            }
        }

        public int RecoveryAmount
        {
            get => recoveryAmount;
            set => recoveryAmount = Mathf.Max(0, value);
        }

        public float RecoveryPeriod
        {
            get => recoveryPeriod;
            set => recoveryPeriod = Mathf.Max(0f, value);
        }

        public bool IsEnabled => IntBehaviour != null;
        
        private void RecoveryUpdate(float deltaTime)
        {
            if (IntBehaviour.Value >= IntBehaviour.MaxValue) return;
            
            if (_timer >= RecoveryPeriod)
            {
                _timer = 0f;

                IntBehaviour.Value += RecoveryAmount;
            }
                
            _timer += deltaTime;
        }
        
        private void Update()
        {
            if (IsEnabled && RecoveryActive) RecoveryUpdate(Time.deltaTime);
        }
    }
}