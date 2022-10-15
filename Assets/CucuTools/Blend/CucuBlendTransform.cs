using System;
using UnityEngine;

namespace CucuTools.Blend
{
    public class CucuBlendTransform : CucuBlendTransformBase
    {
        [Header("Transform")]
        [SerializeField] private bool changeLocal = true;
        [SerializeField] private SyncInfo syncInfo = new SyncInfo(true, true, true);
        [SerializeField] private TransformInfoParam transformInfo;
        
        public bool ChangeLocal
        {
            get => changeLocal;
            set
            {
                changeLocal = value;
                
                UpdateEntity();
            }
        }
        
        public SyncInfo SyncInfo
        {
            get => syncInfo;
            set => syncInfo = value;
        }

        public TransformInfoParam TransformInfo
        {
            get => transformInfo ?? (transformInfo = new TransformInfoParam());
            set
            {
                transformInfo = value;
                
                UpdateEntity();
            }
        }

        private void UpdateTarget(TransformInfo info)
        {
            if (ChangeLocal)
            {
                if (SyncInfo.position) Target.localPosition = info.position;
                if (SyncInfo.rotation) Target.localRotation = info.rotation;
                if (SyncInfo.scale) Target.localScale = info.scale;
            }
            else
            {
                if (SyncInfo.position) Target.position = info.position;
                if (SyncInfo.rotation) Target.rotation = info.rotation;
                if (SyncInfo.scale) Target.localScale = info.scale;
            }
        }
        
        protected override void UpdateEntityInternal()
        {
            UpdateTarget(TransformInfo.Evaluate(Blend));
        }
    }

    [Serializable]
    public class TransformInfoParam : InfoParamBase<TransformInfo>
    {
        public bool UseLocal
        {
            get => useLocal;
            set => useLocal = value;
        }
        
        public Transform Start
        {
            get => start;
            set => start = value;
        }

        public Transform End
        {
            get => end;
            set => end = value;
        }
        
        [SerializeField] private bool useLocal;
        [SerializeField] private Transform start;
        [SerializeField] private Transform end;

        public TransformInfoParam()
        {
            useLocal = true;
            start = null;
            end = null;
        }
        
        public bool IsValid()
        {
            return Start != null && End != null;
        }
        
        public override TransformInfo Evaluate(float t)
        {
            if (IsValid())
            {
                return UseLocal
                    ? new TransformInfo(
                        Vector3.Lerp(Start.localPosition, End.localPosition, t),
                        Quaternion.Lerp(Start.localRotation, End.localRotation, t),
                        Vector3.Lerp(Start.localScale, End.localScale, t))
                    : new TransformInfo(
                        Vector3.Lerp(Start.position, End.position, t),
                        Quaternion.Lerp(Start.rotation, End.rotation, t),
                        Vector3.Lerp(Start.lossyScale, End.lossyScale, t));
            }
            
            return TransformInfo.Zero;
        }
    }

    [Serializable]
    public struct SyncInfo
    {
        public bool position;
        public bool rotation;
        public bool scale;

        public SyncInfo(bool position, bool rotation, bool scale)
        {
            this.position = position;
            this.rotation = rotation;
            this.scale = scale;
        }
    }
    
    [Serializable]
    public struct TransformInfo
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;

        public static TransformInfo Zero => new TransformInfo(Vector3.zero, Quaternion.identity, Vector3.one);
        
        public TransformInfo(Vector3 position, Quaternion rotation, Vector3 scale)
        {
            this.position = position;
            this.rotation = rotation;
            this.scale = scale;
        }

        public void SetTrasform(Transform transform)
        {
            position = transform.position;
            rotation = transform.rotation;
            scale = transform.lossyScale;
        }
        
        public void SetTrasformLocal(Transform transform)
        {
            position = transform.localPosition;
            rotation = transform.localRotation;
            scale = transform.localScale;
        }
    }
}