using System.Collections.Generic;
using System.Linq;
using Game.Characters;
using Game.Characters.Npc;
using Game.Stats;
using Game.Stats.Impl;
using UnityEngine;

namespace Game.Dev
{
    public class NpcSettingsController : MonoBehaviour
    {
        public NpcController template;
        public NpcController[] npcs;
        
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
        
        private List<HealthIntBehaviour> _healths;
        private List<StaminaIntBehaviour> _staminas;
        private List<ValueIntRecovery> _healthRestores;
        private List<ValueIntRecovery> _staminaRestores;
        private List<MoveSetting> _moves;
        private List<JumpSetting> _jumps;

        private void Awake()
        {
            npcs = FindObjectsOfType<NpcController>();

            _healths = npcs.Select(npc => npc.Health).ToList();
            _staminas = npcs.Select(npc => npc.Stamina).ToList();

            _moves = npcs.Select(npc => npc.MoveSetting).ToList();
            _jumps = npcs.Select(npc => npc.JumpSetting).ToList();

            _healthRestores = _healths.Select(health => health.GetComponent<ValueIntRecovery>()).ToList();
            _staminaRestores = _staminas.Select(stamina => stamina.GetComponent<ValueIntRecovery>()).ToList();
        }

        private void Start()
        {
            UpdateViews();

            healthMaxValue.sliderView.onValueChanged.AddListener(t => _healths.ForEach(h => h.MaxValue = (int)t));
            healthRestoreAmount.sliderView.onValueChanged.AddListener(t => _healthRestores.ForEach(h => h.RecoveryAmount = (int)t));
            healthRestorePeriod.sliderView.onValueChanged.AddListener(t => _healthRestores.ForEach(h => h.RecoveryPeriod = t));
            
            staminaMaxValue.sliderView.onValueChanged.AddListener(t => _staminas.ForEach(s => s.MaxValue = (int)t));
            staminaRestoreAmount.sliderView.onValueChanged.AddListener(t => _staminaRestores.ForEach(s => s.RecoveryAmount = (int)t));
            staminaRestorePeriod.sliderView.onValueChanged.AddListener(t => _staminaRestores.ForEach(s => s.RecoveryPeriod = t));
            
            speed.sliderView.onValueChanged.AddListener(t => _moves.ForEach(m=>m.speedMax = t));
            jumpSpeed.sliderView.onValueChanged.AddListener(t => _jumps.ForEach(j => j.speed = t));
        }

        public void UpdateViews()
        {
            var health = template.Health;
            var stamina = template.Stamina;

            var move = template.MoveSetting;
            var jump = template.JumpSetting;
            
            var healthRestore = health.GetComponent<ValueIntRecovery>();
            var staminaRestore = stamina.GetComponent<ValueIntRecovery>();
            
            healthMaxValue.value = health.MaxValue;
            healthRestoreAmount.value = healthRestore.RecoveryAmount;
            healthRestorePeriod.value = healthRestore.RecoveryPeriod;
            
            staminaMaxValue.value = stamina.MaxValue;
            staminaRestoreAmount.value = staminaRestore.RecoveryAmount;
            staminaRestorePeriod.value = staminaRestore.RecoveryPeriod;
            
            speed.value = move.speedMax;
            jumpSpeed.value = jump.speed;
        }
    }
}