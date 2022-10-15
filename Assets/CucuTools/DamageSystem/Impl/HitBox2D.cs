using UnityEngine;

namespace CucuTools.DamageSystem.Impl
{
    /// <inheritdoc />
    [RequireComponent(typeof(Collider2D))]
    public class HitBox2D : HitBox
    {
        public Collider2D Collider { get; private set; }
        
        private void Awake()
        {
            if (Receiver == null) Receiver = GetComponentInParent<DamageReceiver>();
            
            Collider = GetComponent<Collider2D>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (IsEnabled && Mode == HitMode.Trigger) Hit(other.GetComponents<DamageBox>());
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (IsEnabled && Mode == HitMode.Collision) Hit(other.collider.GetComponents<DamageBox>());
        }
    }
}