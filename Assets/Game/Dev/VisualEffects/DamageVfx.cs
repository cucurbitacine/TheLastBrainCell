using System;
using CucuTools.DamageSystem;
using UnityEngine;

namespace Game.Dev.VisualEffects
{
    public class DamageVfx : MonoBehaviour
    {
        public VfxTemplate bloodVfx;
        public VfxTemplate stunVfx;
        
        [Space]
        public VfxController vfxController;
        public DamageReceiver receiver;

        private void OnDamageReceived(DamageEvent e)
        {
            if (bloodVfx == null) return;
            
            if (e.receiver != receiver) return;

            if (e.damage.type == DamageType.Physical && e.damage.amount > 0)
            {
                vfxController.Play(bloodVfx, transform.position, transform.rotation);
            }

            if (e.damage.stun.isOn)
            {
                vfxController.Play(stunVfx);
            }
        }

        private void Awake()
        {
            if (vfxController == null) vfxController = GetComponentInParent<VfxController>();
            if (receiver == null) receiver = GetComponentInParent<DamageReceiver>();
        }

        private void OnEnable()
        {
            receiver.OnDamageReceived.AddListener(OnDamageReceived);
        }

        private void OnDisable()
        {
            receiver.OnDamageReceived.RemoveListener(OnDamageReceived);
        }
    }
}