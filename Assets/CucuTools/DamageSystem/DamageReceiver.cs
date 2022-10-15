using UnityEngine;
using UnityEngine.Events;

namespace CucuTools.DamageSystem
{
    /// <summary>
    /// Behaviour which will be receiving damage
    /// </summary>
    [DisallowMultipleComponent]
    public class DamageReceiver : DamageEffector
    {
        [SerializeField] private bool isEnabled = true;
        [Space]
        [SerializeField] private UnityEvent<DamageEvent> _onDamageReceived = null;

        /// <summary>
        /// Will be receiving damage or not
        /// </summary>
        public bool IsEnabled
        {
            get => isEnabled;
            set => isEnabled = value;
        }

        public UnityEvent<DamageEvent> OnDamageReceived => _onDamageReceived != null
            ? _onDamageReceived
            : (_onDamageReceived = new UnityEvent<DamageEvent>());

        public void ReceiveDamage(DamageEvent e)
        {
            if (IsEnabled && e.receiver == this)
            {
                OnDamageReceived.Invoke(e);
            }
        }
    }
}