using System;

namespace CucuTools.DamageSystem
{
    /// <summary>
    /// Information about damage event
    /// </summary>
    [Serializable]
    public struct DamageEvent
    {
        /// <summary>
        /// Damage which will be applied to receiver
        /// </summary>
        public DamageInfo damage;

        /// <summary>
        /// Receiver of damage
        /// </summary>
        public DamageReceiver receiver;

        /// <summary>
        /// Source of damage
        /// </summary>
        public DamageSource source;

        public DamageEvent(DamageInfo damage, DamageEvent e) : this(damage, e.receiver, e.source)
        {
        }
        
        public DamageEvent(DamageEvent e) : this(e.damage, e.receiver, e.source)
        {
        }
        
        public DamageEvent(DamageInfo damage, DamageReceiver receiver, DamageSource source = null) 
        {
            this.damage = damage;
            this.receiver = receiver;
            this.source = source;
        }
    }
}