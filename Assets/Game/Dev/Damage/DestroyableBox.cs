using Game.Stats.Impl;
using UnityEngine;

namespace Game.Dev.Damage
{
    public class DestroyableBox : MonoBehaviour
    {
        public HealthIntBehaviour Health;

        public void DestroyBox()
        {
            Destroy(gameObject);
        }

        private void OnEnable()
        {
            Health.Events.OnValueIsEmpty.AddListener(DestroyBox);
        }
        
        private void OnDisable()
        {
            Health.Events.OnValueIsEmpty.RemoveListener(DestroyBox);
        }
    }
}
