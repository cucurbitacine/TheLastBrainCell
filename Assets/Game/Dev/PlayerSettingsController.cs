using CucuTools;
using CucuTools.Attributes;
using Game.Characters;
using Game.Characters.Player;
using Game.Stats;
using Game.Stats.Impl;
using UnityEngine;

namespace Game.Dev
{
    public class PlayerSettingsController : CucuBehaviour
    {
        public PlayerController player;
        
        [Space]
        public ParamView healthMaxValue;
        public ParamView healthRestoreAmount;
        public ParamView healthRestorePeriod;

        [Space]
        public ParamView staminaMaxValue;
        public ParamView staminaRestoreAmount;
        public ParamView staminaRestorePeriod;
        
        [Space]
        public ParamView speed;
        public ParamView jumpSpeed;
        
        private HealthIntBehaviour _health;
        private StaminaIntBehaviour _stamina;
        private ValueIntRecovery _healthRestore;
        private ValueIntRecovery _staminaRestore;
        private MoveSetting _move;
        private JumpSetting _jump;

        private void Awake()
        {
            _health = player.Health;
            _stamina = player.Stamina;

            _move = player.MoveSetting;
            _jump = player.JumpSetting;
            
            _healthRestore = _health.GetComponent<ValueIntRecovery>();
            _staminaRestore = _stamina.GetComponent<ValueIntRecovery>();
        }

        private void Start()
        {
            UpdateViews();

            healthMaxValue.sliderView.onValueChanged.AddListener(t => _health.MaxValue = (int)t);
            healthRestoreAmount.sliderView.onValueChanged.AddListener(t => _healthRestore.RecoveryAmount = (int)t);
            healthRestorePeriod.sliderView.onValueChanged.AddListener(t => _healthRestore.RecoveryPeriod = t);
            
            staminaMaxValue.sliderView.onValueChanged.AddListener(t => _stamina.MaxValue = (int)t);
            staminaRestoreAmount.sliderView.onValueChanged.AddListener(t => _staminaRestore.RecoveryAmount = (int)t);
            staminaRestorePeriod.sliderView.onValueChanged.AddListener(t => _staminaRestore.RecoveryPeriod = t);

            speed.sliderView.onValueChanged.AddListener(t => _move.speedMax = t);
            jumpSpeed.sliderView.onValueChanged.AddListener(t => _jump.speed = t);
        }

        [CucuButton()]
        public void UpdateViews()
        {
            _health = player.Health;
            _stamina = player.Stamina;

            _move = player.MoveSetting;
            _jump = player.JumpSetting;
            
            _healthRestore = _health.GetComponent<ValueIntRecovery>();
            _staminaRestore = _stamina.GetComponent<ValueIntRecovery>();
            
            healthMaxValue.value = _health.MaxValue;
            healthRestoreAmount.value = _healthRestore.RecoveryAmount;
            healthRestorePeriod.value = _healthRestore.RecoveryPeriod;
            
            staminaMaxValue.value = _stamina.MaxValue;
            staminaRestoreAmount.value = _staminaRestore.RecoveryAmount;
            staminaRestorePeriod.value = _staminaRestore.RecoveryPeriod;
            
            speed.value = _move.speedMax;
            jumpSpeed.value = _jump.speed;
        }
    }
}