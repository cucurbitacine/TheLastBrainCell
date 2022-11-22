using System;
using System.Collections;
using UnityEngine;

namespace Game.Dev.Damage
{
    public class BallShooter : MonoBehaviour
    {
        public bool isActive = true;
        public bool isShooting = false;

        public float speed = 1f;
        public float lifetime = 5f;

        public GameObject ballPrefab;

        private Coroutine _shooting;

        public void Shoot()
        {
            if (isShooting) return;

            _shooting = StartCoroutine(Shooting());
        }

        private IEnumerator Shooting()
        {
            isShooting = true;

            var ball = Instantiate(ballPrefab, transform.position, transform.rotation);

            var timer = 0f;
            while (timer < lifetime)
            {
                ball.transform.Translate(Vector2.up * speed * Time.deltaTime);

                timer += Time.deltaTime;
                yield return null;
            }

            Destroy(ball);

            isShooting = false;
        }

        private void Update()
        {
            if (isActive && !isShooting) Shoot();
        }

        private void OnDrawGizmos()
        {
            var pos = transform.position;
            var trg = pos + transform.up * speed * lifetime;
            
            Gizmos.DrawLine(pos,  trg);
        }
    }
}