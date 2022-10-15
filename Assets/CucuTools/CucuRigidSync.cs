using System.Collections.Generic;
using CucuTools.Attributes;
using UnityEngine;

namespace CucuTools
{
    [RequireComponent(typeof(Rigidbody))]
    public class CucuRigidSync : MonoBehaviour
    {
        [Header("Main Settings")]
        [SerializeField] private bool isEnabled = true;
        [SerializeField] public bool defaultUseGravityOnDisable = true;
        [SerializeField] private Transform targetSync;

        [Header("Additional Settings")]
        [SerializeField] public bool syncPosition = true;
        [SerializeField] public bool syncRotation = true;
        [Range(0f, 1000f)]
        [SerializeField] private float maxVelocity = 500f;
        [Range(0f, 1f)]
        [SerializeField] private float syncWeight = 1f;
    
        [Header("References")]
        [SerializeField] private Rigidbody rigid;
        [SerializeField] private Collider[] colliders;
        
        private static PhysicMaterial _rigidSyncPhysicMaterial;
    
        public bool IsEnabled
        {
            get => isEnabled;
            set => isEnabled = value;
        }
    
        public Transform TargetSync
        {
            get => targetSync;
            set => targetSync = value;
        }
    
        public Rigidbody Rigidbody
        {
            get => rigid;
            protected set => rigid = value;
        }

        private bool defaultUseGravity;

        private Dictionary<Collider, PhysicMaterial> defaultPhysicMaterials = new Dictionary<Collider, PhysicMaterial>();
        private bool syncing;
        
        public bool IsValid()
        {
            return TargetSync != null && Rigidbody != null;
        }

        private void ValidateRigidbody()
        {
            if (Rigidbody == null) SetupRigidbody();
        
            Rigidbody.isKinematic = false;

            Rigidbody.drag = 0.5f;
            Rigidbody.angularDrag = 0.5f;

            Rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
            Rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

            Rigidbody.maxDepenetrationVelocity = 10f;
        }
    
        private void ValidateColliders()
        {
            if (colliders == null) SetupColliders();

            foreach (var collider in colliders)
                ValidateCollider(collider);
        }

        private void ValidateCollider(Collider collider)
        {
            if (_rigidSyncPhysicMaterial == null)
            {
                _rigidSyncPhysicMaterial = new PhysicMaterial("rigidsync");
                _rigidSyncPhysicMaterial.bounciness = 0f;
                _rigidSyncPhysicMaterial.dynamicFriction = 0f;
                _rigidSyncPhysicMaterial.staticFriction = 0f;
                _rigidSyncPhysicMaterial.frictionCombine = PhysicMaterialCombine.Average;
            }
        
            if (collider == null) return;

            if (collider.sharedMaterial == null) collider.sharedMaterial = _rigidSyncPhysicMaterial;
            collider.isTrigger = false;
        }
    
        [CucuButton("Setup Rigidbody", @group: "Setup")]
        private void SetupRigidbody()
        {
            Rigidbody = GetComponent<Rigidbody>();
            if (Rigidbody == null) Rigidbody = gameObject.AddComponent<Rigidbody>();
            Rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
            Rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        }
    
        [CucuButton("Setup Colliders", @group: "Setup")]
        private void SetupColliders()
        {
            colliders = GetComponentsInChildren<Collider>();
        }
    
        private void Validate()
        {
            ValidateRigidbody();
            ValidateColliders();
        }

        protected virtual void Awake()
        {
            Validate();
        }

        protected void FixedUpdate()
        {
            if (IsEnabled && IsValid())
            {
                if (!syncing)
                {
                    syncing = true;
                    defaultUseGravity = Rigidbody.useGravity;
                    Rigidbody.useGravity = false;
                    foreach (var cld in colliders)
                    {
                        defaultPhysicMaterials[cld] = cld.sharedMaterial;
                        cld.sharedMaterial = _rigidSyncPhysicMaterial;
                    }
                }

                Rigidbody.Sync(TargetSync,
                    syncPosition, syncRotation,
                    maxVelocity, syncWeight,
                    Time.fixedDeltaTime);
            }
            else
            {
                if (syncing)
                {
                    syncing = false;
                    if (defaultUseGravityOnDisable) Rigidbody.useGravity = defaultUseGravity;
                    foreach (var cld in colliders)
                        cld.sharedMaterial = defaultPhysicMaterials[cld];
                }
            }
        }

        protected virtual void OnValidate()
        {
            if (!Application.isPlaying) Validate();
        }
    }
}
