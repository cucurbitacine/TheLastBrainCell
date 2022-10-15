using UnityEngine;

namespace CucuTools.Avatar
{
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Rigidbody))]
    public class ConveyorRigid : CucuBehaviour
    {
        public Vector3 Direction = Vector3.forward;
        public float Speed = 1f;
        
        public Rigidbody Rigidbody => GetComponent<Rigidbody>();
        public Vector3 Velocity => Direction.normalized * Speed;

        private void Awake()
        {
            Rigidbody.useGravity = false;
            Rigidbody.isKinematic = true;
            Rigidbody.interpolation = RigidbodyInterpolation.None;
            Rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        }

        private void FixedUpdate()
        {
            Rigidbody.velocity = Velocity;
            var move = Velocity * Time.fixedDeltaTime;
            Rigidbody.position -= move;
            Rigidbody.MovePosition(Rigidbody.position + move);
        }
    }
}