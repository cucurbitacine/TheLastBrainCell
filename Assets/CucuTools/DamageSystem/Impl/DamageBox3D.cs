using UnityEngine;

namespace CucuTools.DamageSystem.Impl
{
    /// <inheritdoc />
    [RequireComponent(typeof(Collider))]
    public class DamageBox3D : DamageBox
    {
        public Collider Collider { get; private set; }
        
        private void Awake()
        {
            if (Source == null) Source = GetComponentInParent<DamageSource>();
            
            Collider = GetComponent<Collider>();
        }
    }
}