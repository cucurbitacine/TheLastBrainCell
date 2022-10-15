using System.Collections;
using UnityEngine;

namespace CucuTools.Clothes
{
    public class CucuShooter : MonoBehaviour
    {
        public float Radius = 0.1f;
        public float Power = 1000f;
        public float LifeTime = 5f;
        
        public void Shoot()
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.position = ray.origin;
            sphere.transform.localScale = Vector3.one * (Radius * 2);
            
            var rgb = sphere.AddComponent<Rigidbody>();
            rgb.interpolation = RigidbodyInterpolation.Interpolate;
            rgb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            rgb.AddForce(ray.direction * Power);

            StartCoroutine(Die(sphere, LifeTime));
        }
        
        private static IEnumerator Die(GameObject gameObject, float delay)
        {
            yield return new WaitForSeconds(delay);
            Destroy(gameObject);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                Shoot();
            }
        }
    }
}