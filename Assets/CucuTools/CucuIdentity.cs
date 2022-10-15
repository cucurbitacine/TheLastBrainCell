using System;
using CucuTools.Attributes;
using UnityEngine;

namespace CucuTools
{
    [DisallowMultipleComponent]
    public sealed class CucuIdentity : CucuBehaviour
    {
        public static implicit operator Guid(CucuIdentity cuid) => cuid.Guid;
        
        [CucuReadOnly]
        [SerializeField] private string _guid;

        public Guid Guid
        {
            get => Guid.TryParse(_guid, out var guid) ? guid : (Guid = Guid.NewGuid());
            set => _guid = value.ToString();
        }
        
        private void Validate()
        {
            if (Guid == Guid.Empty) Guid = Guid.NewGuid();
        }

        private void Awake()
        {
            Validate();
        }

        private void Reset()
        {
            Validate();
        }
        
        private void OnValidate()
        {
            Validate();
        }

        public static CucuIdentity GetOrAdd(GameObject gameObject)
        {
            if (gameObject == null) return null;
            var cuid = gameObject.GetComponent<CucuIdentity>();
            if (cuid == null) cuid = gameObject.AddComponent<CucuIdentity>();
            return cuid;
        }
    }
}