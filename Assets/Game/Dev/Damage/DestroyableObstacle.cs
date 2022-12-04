using Game.Effects;
using Game.Stats.Impl;
using UnityEngine;

namespace Game.Dev.Damage
{
    public class DestroyableObstacle : MonoBehaviour
    {
        public FxTemplate fxTemplate;
        
        [Space]
        public HealthIntBehaviour health;
        public Animator animator;

        private FxController _fxController;
        
        private readonly int _hitTriggerName = Animator.StringToHash("Hit"); 
        
        public void HitObstacle()
        {
            animator.SetTrigger(_hitTriggerName);

            if (_fxController != null && fxTemplate != null)
            {
                _fxController.Play(fxTemplate, transform.position, transform.rotation);
            }
        }
        
        public void DestroyObstacle()
        {
            foreach (var sr in GetComponentsInChildren<SpriteRenderer>())
            {
                sr.enabled = false;
            }

            foreach (var cld in GetComponentsInChildren<Collider2D>())
            {
                cld.enabled = false;
            }
        }

        private void OnHit(int hit)
        {
            HitObstacle();
        }

        private void Awake()
        {
            _fxController = FxController.Instance;
        }

        private void OnEnable()
        {
            health.Events.OnValueChanged.AddListener(OnHit);
            health.Events.OnValueIsEmpty.AddListener(DestroyObstacle);
        }
        
        private void OnDisable()
        {
            health.Events.OnValueChanged.RemoveListener(OnHit);
            health.Events.OnValueIsEmpty.RemoveListener(DestroyObstacle);
        }
    }
}
