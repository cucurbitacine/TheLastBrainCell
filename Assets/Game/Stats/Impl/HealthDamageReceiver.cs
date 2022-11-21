using CucuTools.DamageSystem;
using UnityEngine;

namespace Game.Stats.Impl
{
    public class HealthDamageReceiver : MonoBehaviour
    {
        public HealthIntBehaviour healthInt;
        public DamageReceiver damageReceiver;

        public void ReceiveDamage(DamageEvent e)
        {
            if (e.receiver != damageReceiver) return;
            
            healthInt.Value -= e.damage.amount;
        }
        
        private void Awake()
        {
            if (healthInt == null) healthInt = GetComponent<HealthIntBehaviour>();
            if (damageReceiver == null) damageReceiver = GetComponent<DamageReceiver>();
        }

        private void OnEnable()
        {
            damageReceiver.OnDamageReceived.AddListener(ReceiveDamage);
        }

        private void OnDisable()
        {
            damageReceiver.OnDamageReceived.RemoveListener(ReceiveDamage);
        }
    }
}