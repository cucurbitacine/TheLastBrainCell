using UnityEngine;

namespace CucuTools.DamageSystem.Impl
{
    /// <inheritdoc />
    [RequireComponent(typeof(Collider2D))]
    public class DamageBox2D : DamageBox
    {
        public Collider2D Collider { get; private set; }
        
        private void Awake()
        {
            if (Source == null) Source = GetComponentInParent<DamageSource>();
            
            Collider = GetComponent<Collider2D>();
        }
    }
}