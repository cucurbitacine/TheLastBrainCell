using Game.Stats.Impl;
using UnityEngine;

namespace Game.Dev.Damage
{
    public class DestroyableBehaviour : MonoBehaviour
    {
        public HealthIntBehaviour Health;

        public void DestroyBox()
        {
            gameObject.SetActive(false);
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
