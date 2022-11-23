using System.Collections;
using CucuTools.DamageSystem;
using UnityEngine;

namespace Game.Dev.Damage
{
    public class ShakeWhenDamaged : MonoBehaviour
    {
        public DamageReceiver damageReceiver;

        public float duration = 0.5f;
        public float radius = 0.5f;
        public float damp = 4f;

        private Coroutine _shaking = null;
        private Vector2 _initPosition;
        private Vector2 _shakePosition;
        
        public void Shake()
        {
            if (_shaking != null) StopCoroutine(_shaking);
            _shaking = StartCoroutine(Shaking());
        }

        public void ReceiveDamage(DamageEvent e)
        {
            if (e.receiver == damageReceiver && e.damage.amount > 0) Shake();
        }

        private IEnumerator Shaking()
        {
            _initPosition = transform.position;
            
            var timer = 0f;
            while (timer < duration)
            {
                _shakePosition = _initPosition + Random.insideUnitCircle.normalized * radius;
                
                timer += Time.deltaTime;
                yield return null;
            }

            _shakePosition = _initPosition;
        }
        
        private void Awake()
        {
            if (damageReceiver == null) damageReceiver = GetComponent<DamageReceiver>();

            _shakePosition = transform.position;
        }

        private void OnEnable()
        {
            damageReceiver.OnDamageReceived.AddListener(ReceiveDamage);
        }

        private void Update()
        {
            transform.position = Vector2.Lerp(transform.position, _shakePosition, damp * Time.deltaTime);
        }

        private void OnDisable()
        {
            damageReceiver.OnDamageReceived.RemoveListener(ReceiveDamage);
        }
    }
}