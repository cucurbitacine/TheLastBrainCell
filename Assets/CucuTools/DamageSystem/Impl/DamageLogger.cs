using UnityEngine;

namespace CucuTools.DamageSystem.Impl
{
    /// <summary>
    /// Damage logger which using <see cref="DamageEventNotificator"/>.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class DamageLogger : MonoBehaviour
    {
        private static DamageLogger Instance { get; set; }
        
        public bool isOn = true;

        public void OnDamageEvent(DamageEvent e)
        {
            if (isOn) Log(DamageEventMessage(e));
        }

        public void Log(string msg)
        {
            Debug.Log($"{msg}");
        }

        private string DamageMessage(DamageInfo damage)
        {
            return $"{damage.amount} {damage.type}{(damage.isCritical ? " CRITICAL" : "")}";
        }

        private string DamageEventMessage(DamageEvent e)
        {
            if (e.source != null)
                return $"Receiver: {e.receiver.name} | Damage: {DamageMessage(e.damage)} | Source: {e.source.name}";

            return $"Receiver: {e.receiver.name} | Damage: {DamageMessage(e.damage)} | Source: NULL";
        }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void OnEnable()
        {
            DamageEventNotificator.OnDamageEvent += OnDamageEvent;
        }

        private void OnDisable()
        {
            DamageEventNotificator.OnDamageEvent -= OnDamageEvent;
        }
    }
}