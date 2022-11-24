﻿using UnityEngine;

namespace Game.Stats
{
    /// <summary>
    /// Integer value's recover
    /// </summary>
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

        #region Public API

        /// <summary>
        /// Value's keeper
        /// </summary>
        public ValueIntBehaviour IntBehaviour => intBehaviour ??= GetComponent<ValueIntBehaviour>();

        /// <summary>
        /// Do Recovery or not
        /// </summary>
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

        /// <summary>
        /// Amount which will be recovered
        /// </summary>
        public int RecoveryAmount
        {
            get => recoveryAmount;
            set => recoveryAmount = Mathf.Max(0, value);
        }

        /// <summary>
        /// Time in seconds between recovery
        /// </summary>
        public float RecoveryPeriod
        {
            get => recoveryPeriod;
            set => recoveryPeriod = Mathf.Max(0f, value);
        }

        #endregion

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
            if (IntBehaviour != null && RecoveryActive) RecoveryUpdate(Time.deltaTime);
        }
    }
}