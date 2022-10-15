using UnityEngine;
using UnityEngine.Events;

namespace CucuTools.DamageSystem.Impl
{
    /// <summary>
    /// Hit box which will be contacting with <see cref="DamageBox"/> and transiting damage from sources to receiver.
    /// Linked to the <see cref="DamageReceiver"/>
    /// </summary>
    public abstract class HitBox : MonoBehaviour
    {
        public enum HitMode
        {
            Trigger,
            Collision,
        }
        
        [SerializeField] private bool isEnabled = true;
        [SerializeField] private HitMode mode = HitMode.Trigger;
        
        [Space]
        [SerializeField] private LayerMask hitMask = 1;
        
        [Space]
        [SerializeField] private UnityEvent<DamageEvent> _onDamageReceived = default;
        
        [Space]
        [SerializeField] private DamageReceiver receiver = default;

        /// <summary>
        /// Will be transiting damage or not
        /// </summary>
        public bool IsEnabled
        {
            get => isEnabled;
            set => isEnabled = value;
        }

        /// <summary>
        /// Contact hit mode using Trigger or Collision principle
        /// </summary>
        public HitMode Mode
        {
            get => mode;
            set => mode = value;
        }

        public LayerMask HitMask
        {
            get => hitMask;
            set => hitMask = value;
        }

        public UnityEvent<DamageEvent> OnDamageReceived => _onDamageReceived != null
            ? _onDamageReceived
            : (_onDamageReceived = new UnityEvent<DamageEvent>());
        
        public DamageReceiver Receiver
        {
            get => receiver;
            set => receiver = value;
        }

        /// <summary>
        /// Invoke hit event with damage box
        /// </summary>
        /// <param name="box"></param>
        public void Hit(DamageBox box)
        {
            if (!IsEnabled || !Receiver.IsEnabled) return;

            if (!HitMask.Contains(box.gameObject.layer)) return;
            
            if (box.IsEnabled && box.Source.IsEnabled) ReceiveDamage(GenerateDamageEvent(box.Source));
        }

        /// <summary>
        /// Invoke hit event with source of damage
        /// </summary>
        /// <param name="source"></param>
        public void Hit(DamageSource source)
        {
            if (!IsEnabled || !Receiver.IsEnabled) return;
            
            if (!HitMask.Contains(source.gameObject.layer)) return;
            
            if (source.IsEnabled) ReceiveDamage(GenerateDamageEvent(source));
        }

        /// <summary>
        /// Hit with several damage boxes
        /// </summary>
        /// <param name="boxes"></param>
        public void Hit(params DamageBox[] boxes)
        {
            foreach(var box in boxes)
            {
                Hit(box);
            }
        }

        /// <summary>
        /// Hit with several damage sources
        /// </summary>
        /// <param name="sources"></param>
        public void Hit(params DamageSource[] sources)
        {
            foreach (var source in sources)
            {
                Hit(source);
            }
        }

        /// <summary>
        /// Generate damage event from source of damage
        /// </summary>
        /// <param name="source"></param>
        /// <returns>Damage event</returns>
        private DamageEvent GenerateDamageEvent(DamageSource source)
        {
            // Generate "clear" damage
            var dmg = source.GenerateDamage();
            // Evaluating by source. Some power/weakness buff for example
            dmg = source.EvaluateDamage(dmg);
            // Evaluating by receiver. Some shield/weakness for example
            dmg = Receiver.EvaluateDamage(dmg);

            var e = new DamageEvent(dmg, Receiver, source);

            return e;
        }

        /// <summary>
        /// Transiting damage event to receiver
        /// </summary>
        /// <param name="e"></param>
        private void ReceiveDamage(DamageEvent e)
        {
            OnDamageReceived.Invoke(e);
            
            Receiver.ReceiveDamage(e);
        }
    }
}