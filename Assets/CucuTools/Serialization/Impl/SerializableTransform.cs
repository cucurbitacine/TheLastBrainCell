using System;
using UnityEngine;

namespace CucuTools.Serialization.Impl
{
    [DisallowMultipleComponent]
    public sealed class SerializableTransform : SerializableComponent<Transform>
    {
        [SerializeField] private TransformData transformData;
        
        /// <inheritdoc />
        protected override byte[] Serialize()
        {
            if (transformData == null) transformData = new TransformData();
            
            transformData.localPosition = Component.localPosition;
            transformData.localRotation = Component.localRotation;
            transformData.localScale = Component.localScale;
            
            return Serializator.Serialize(transformData);
        }

        /// <inheritdoc />
        protected override void Deserialize(byte[] bytes)
        {
            transformData = Serializator.Deserialize<TransformData>(bytes);
            
            Component.localPosition = transformData.localPosition;
            Component.localRotation = transformData.localRotation;
            Component.localScale = transformData.localScale;
        }

        [Serializable]
        private class TransformData
        {
            public Vector3 localPosition;
            public Quaternion localRotation;
            public Vector3 localScale;
        }
    }
}