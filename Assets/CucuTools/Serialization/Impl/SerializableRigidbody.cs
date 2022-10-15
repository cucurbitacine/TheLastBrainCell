using System;
using UnityEngine;

namespace CucuTools.Serialization.Impl
{
    /// <summary>
    /// Serializable Rigidbody
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class SerializableRigidbody : SerializableComponent<Rigidbody, RigidbodyData>
    {
        [SerializeField] private RigidbodyData rigidbodyData;
        
        /// <inheritdoc />
        public override RigidbodyData GetData()
        {
            if (rigidbodyData == null) rigidbodyData = new RigidbodyData();
            
            rigidbodyData.velocity = Component.velocity;
            rigidbodyData.angularVelocity = Component.angularVelocity;

            return rigidbodyData;
        }

        /// <inheritdoc />
        public override void SetData(RigidbodyData data)
        {
            rigidbodyData = data;
            
            Component.velocity = rigidbodyData.velocity;
            Component.angularVelocity = rigidbodyData.angularVelocity;
        }
    }

    [Serializable]
    public class RigidbodyData
    {
        public Vector3 velocity;
        public Vector3 angularVelocity;
    }
}