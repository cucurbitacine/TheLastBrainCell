using System.Collections;
using CucuTools.DamageSystem;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Stats.Impl
{
    public class HealthDamageReceiver : MonoBehaviour
    {
        [SerializeField] private bool sleepAfterDamage = false;
        [SerializeField] private float sleepDuration = 0.2f;
        [SerializeField] private bool isSleeping = false;

        [Space]
        [SerializeField] private UnityEvent<DamageEvent> onDamageReceive;

        [Space]
        [SerializeField] private HealthIntBehaviour health;
        [SerializeField] private DamageReceiver damageReceiver;
        
        private Coroutine _sleeping = null;

        public bool SleepAfterDamage
        {
            get => sleepAfterDamage;
            set => sleepAfterDamage = value;
        }

        public float SleepDuration
        {
            get => sleepDuration;
            set => sleepDuration = value;
        }

        public bool IsSleeping
        {
            get => isSleeping;
            private set => isSleeping = value;
        }

        public UnityEvent<DamageEvent> OnDamageReceive => onDamageReceive ??= new UnityEvent<DamageEvent>();

        public HealthIntBehaviour Health => health ??= GetComponent<HealthIntBehaviour>();
        public DamageReceiver DamageReceiver => damageReceiver ??= GetComponent<DamageReceiver>();

        public void ReceiveDamage(DamageEvent e)
        {
            if (e.receiver != DamageReceiver) return;

            if (SleepAfterDamage && IsSleeping) return;

            Health.Value -= e.damage.amount;

            OnDamageReceive.Invoke(e);

            if (SleepAfterDamage) Sleep(SleepDuration);
        }

        public void Sleep(float duration)
        {
            if (_sleeping != null) StopCoroutine(_sleeping);
            _sleeping = StartCoroutine(Sleeping(duration));
        }

        private IEnumerator Sleeping(float duration)
        {
            IsSleeping = true;

            yield return new WaitForSeconds(duration);
            
            IsSleeping = false;
        }
        
        private void OnEnable()
        {
            DamageReceiver.OnDamageReceived.AddListener(ReceiveDamage);
        }
        
        private void OnDisable()
        {
            DamageReceiver.OnDamageReceived.RemoveListener(ReceiveDamage);
        }
    }
}