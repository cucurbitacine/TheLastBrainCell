using UnityEngine;

namespace CucuTools.DamageSystem.Impl
{
    /// <inheritdoc />
    [RequireComponent(typeof(Collider))]
    public class HitBox3D : HitBox
    {
        public Collider Collider { get; private set; }
        
        private void Awake()
        {
            if (Receiver == null) Receiver = GetComponentInParent<DamageReceiver>();
            
            Collider = GetComponent<Collider>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (IsEnabled && Mode == HitMode.Trigger) Hit(other.GetComponents<DamageBox>());
        }

        private void OnCollisionEnter(Collision other)
        {
            if (IsEnabled && Mode == HitMode.Collision) Hit(other.collider.GetComponents<DamageBox>());
        }
    }
}