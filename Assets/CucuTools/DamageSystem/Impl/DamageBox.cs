using UnityEngine;

namespace CucuTools.DamageSystem.Impl
{
    /// <summary>
    /// Damage box which will be contacting with <see cref="HitBox"/>. Linked to the <see cref="DamageSource"/>
    /// </summary>
    public abstract class DamageBox : MonoBehaviour
    {
        [SerializeField] private bool isEnabled = true;
        
        [Space]
        [SerializeField] private DamageSource source = default;

        /// <summary>
        /// Active damage box or not (it is assumed)
        /// </summary>
        public bool IsEnabled
        {
            get => isEnabled;
            set => isEnabled = value;
        }

        public DamageSource Source
        {
            get => source;
            set => source = value;
        }
    }
}