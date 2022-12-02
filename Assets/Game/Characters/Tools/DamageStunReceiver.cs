using CucuTools.DamageSystem;
using UnityEngine;

namespace Game.Characters.Tools
{
    public class DamageStunReceiver : MonoBehaviour
    {
        public StunController stunController;
        public DamageReceiver receiver;
        
        private void OnDamageReceived(DamageEvent e)
        {
            if (e.receiver != receiver) return;

            if (e.damage.stun.isOn) stunController.Stun(e.damage.stun.duration, e.damage.stun.speedScale);
        }

        private void Awake()
        {
            if (stunController == null) stunController = GetComponentInParent<StunController>();
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
