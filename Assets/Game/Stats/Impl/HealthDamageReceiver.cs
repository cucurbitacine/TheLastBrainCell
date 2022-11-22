using System.Collections;
using CucuTools.DamageSystem;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Stats.Impl
{
    /// <summary>
    /// Damage handler which effects on health
    /// </summary>
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

        #region Public API

        /// <summary>
        /// Ignore next damage after received damage or not
        /// </summary>
        public bool SleepAfterDamage
        {
            get => sleepAfterDamage;
            set => sleepAfterDamage = value;
        }

        /// <summary>
        /// Duration time in seconds while damage will be ignored
        /// </summary>
        public float SleepDuration
        {
            get => sleepDuration;
            set => sleepDuration = value;
        }

        /// <summary>
        /// Are ignoring damage right now or not
        /// </summary>
        public bool IsSleeping
        {
            get => isSleeping;
            private set => isSleeping = value;
        }

        /// <summary>
        /// Event of damage which applied on health
        /// </summary>
        public UnityEvent<DamageEvent> OnDamageReceive => onDamageReceive ??= new UnityEvent<DamageEvent>();

        public HealthIntBehaviour Health => health ??= GetComponent<HealthIntBehaviour>();
        public DamageReceiver DamageReceiver => damageReceiver ??= GetComponent<DamageReceiver>();

        /// <summary>
        /// Receive damage which can be applied on health
        /// </summary>
        /// <param name="e"></param>
        public void ReceiveDamage(DamageEvent e)
        {
            if (e.receiver != DamageReceiver) return;

            if (SleepAfterDamage && IsSleeping) return;

            Health.Value -= e.damage.amount;

            OnDamageReceive.Invoke(e);

            if (SleepAfterDamage) Sleep(SleepDuration);
        }

        /// <summary>
        /// Start ignoring damage for <paramref name="duration"/> seconds
        /// </summary>
        /// <param name="duration"></param>
        public void Sleep(float duration)
        {
            if (_sleeping != null) StopCoroutine(_sleeping);
            _sleeping = StartCoroutine(Sleeping(duration));
        }

        #endregion

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