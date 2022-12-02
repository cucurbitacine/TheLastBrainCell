using CucuTools.DamageSystem;
using Game.Effects;
using UnityEngine;

namespace Game.Characters.Tools
{
    public class DamageVfx : MonoBehaviour
    {
        public FxTemplate bloodFx;
        public FxTemplate stunFx;
        
        [Space]
        public FxController fxController;
        public DamageReceiver receiver;

        private void OnDamageReceived(DamageEvent e)
        {
            if (bloodFx == null) return;
            
            if (e.receiver != receiver) return;

            if (e.damage.type == DamageType.Physical && e.damage.amount > 0)
            {
                fxController.Play(bloodFx, transform.position, transform.rotation);
            }

            if (e.damage.stun.isOn)
            {
                fxController.Play(stunFx);
            }
        }

        private void Awake()
        {
            if (fxController == null) fxController = GetComponentInParent<FxController>();
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