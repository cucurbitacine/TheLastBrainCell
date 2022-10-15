using System;
using UnityEngine;
using UnityEngine.Events;

namespace CucuTools.Blend
{
    public abstract class CucuBlendEntity : CucuBehaviour, ICucuBlendable
    {
        [Range(0f, 1f)]
        [SerializeField] private float blend;
        [Tooltip("Tolerance which control setting blend value.\n\nTolerance value is the minimum allowable change in blend value")]
        [SerializeField] private BlendTolerance tolerance;
        [Tooltip("\"OnEntityUpdated\" invoke when entity was updated. Also it invoke before \"OnBlendChanged\"\n\n\"OnBlendChanged\" invoke when blend value was changed")]
        [SerializeField] private BlendEvents events;
        
        public float Blend
        {
            get => GetBlend();
            set => SetBlend(value);
        }

        public BlendTolerance Tolerance => tolerance ?? (tolerance = new BlendTolerance());
        public BlendEvents Events => events ?? (events = new BlendEvents());

        protected virtual float GetBlend()
        {
            return blend;
        }

        protected virtual void SetBlend(float value)
        {
            value = Mathf.Clamp01(value);

            if (AllowedBlendChange(value))
            {
                blend = value;

                UpdateEntity();
                
                Events.OnBlendChanged.Invoke(Blend);
            }
        }

        public virtual void UpdateEntity()
        {
            UpdateEntityInternal();
                
            Events.OnEntityUpdated.Invoke();
        }
        
        protected virtual bool AllowedBlendChange(float value)
        {
            return !Tolerance.Use || Mathf.Abs(Blend - value) >= Tolerance.Tolerance;
        }

        protected abstract void UpdateEntityInternal();

        protected virtual void OnValidate()
        {
            UpdateEntityInternal();
        }
    }

    [Serializable]
    public class BlendTolerance
    {
        public bool Use
        {
            get => use;
            set => use = value;
        }

        public float Tolerance
        {
            get => tolerance;
            set => tolerance = Mathf.Clamp01(value);
        }

        [SerializeField] private bool use;
        [Range(0f, 1f)]
        [SerializeField] private float tolerance;

        public BlendTolerance()
        {
            use = true;
            tolerance = 0.001f;
        }
    }

    [Serializable]
    public class BlendEvents
    {
        public UnityEvent OnEntityUpdated => onEntityUpdated ?? (onEntityUpdated = new UnityEvent());
        public UnityEvent<float> OnBlendChanged => onBlendChanged ?? (onBlendChanged = new UnityEvent<float>());
        
        [SerializeField] private UnityEvent onEntityUpdated;
        [SerializeField] private UnityEvent<float> onBlendChanged;
    }
}