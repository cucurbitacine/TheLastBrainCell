using System;
using CucuTools;
using CucuTools.Attributes;
using Game.Effects;
using UnityEngine;

namespace Game.Characters.Npc
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class ApathyMissile : CucuBehaviour
    {
        public bool active = false;
        public ApathyMissileSettings settings = new ApathyMissileSettings();

        [Space]
        public FxTemplate hitFx;
        public FxController fxController;
        
        public Vector2 direction => transform.TransformDirection(localDirection);
        public Vector2 localDirection => Vector2.up;
        
        private Rigidbody2D _rigidbody;
        
        [CucuButton()]
        public void Fire()
        {
            gameObject.SetActive(true);
            
            active = true;
        }

        public void Fire(ApathyMissileSettings settings)
        {
            this.settings = settings;

            Fire();
        }
        
        [CucuButton()]
        public void Hit()
        {
            active = false;
            
            _rigidbody.velocity = Vector2.zero;
            
            gameObject.SetActive(false);
            
            fxController.Play(hitFx, transform.position, transform.rotation);
        }
        
        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate()
        {
            if (active)
            {
                _rigidbody.velocity = direction * settings.speed;
            }
        }

        private void OnCollisionEnter2D(Collision2D col)
        {
            if (!settings.layers.Contains(col.gameObject.layer)) return;
            
            Hit();
        }
        
        private void OnTriggerEnter2D(Collider2D col)
        {
            if (!settings.layers.Contains(col.gameObject.layer)) return;
            
            Hit();
        }
    }

    [Serializable]
    public class ApathyMissileSettings
    {
        [Min(0f)]
        public float speed = 8f;

        [Space]
        public LayerMask layers = 1;
    }
}
