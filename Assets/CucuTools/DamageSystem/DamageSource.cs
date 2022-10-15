using UnityEngine;

namespace CucuTools.DamageSystem
{
    /// <summary>
    /// Behaviour which will be generating damage
    /// </summary>
    public abstract class DamageSource : DamageEffector
    {
        [SerializeField] private bool isEnabled = true;

        /// <summary>
        /// Will be able to generating damage or not (it is assumed)
        /// </summary>
        public bool IsEnabled
        {
            get => isEnabled;
            set => isEnabled = value;
        }

        public abstract DamageInfo GenerateDamage();
    }
}