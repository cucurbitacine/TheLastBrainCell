using CucuTools.Attributes;
using UnityEngine;
using UnityEngine.Events;

namespace CucuTools.Blend
{
    public class CucuBlendColor : CucuBlendEntity
    {
        [Header("Color")]
        [CucuReadOnly]
        [SerializeField] private Color color;
        [SerializeField] private Gradient gradient;
        [SerializeField] private UnityEvent<Color> onColorChanged;
        
        public Color Color
        {
            get => color;
            private set => color = value;
        }

        public Gradient Gradient
        {
            get
            {
                if (gradient != null) return gradient;
                gradient = new Gradient();
                gradient.SetKeys(new[] {new GradientColorKey(Color.white, 0f)}, new[] {new GradientAlphaKey(1f, 0f)});
                return gradient;
            }
            set
            {
                Gradient.SetKeys(value.colorKeys, value.alphaKeys);
                Gradient.mode = value.mode;
                
                UpdateEntity();
            }
        }

        public UnityEvent<Color> OnColorChanged => onColorChanged ?? (onColorChanged = new UnityEvent<Color>());

        protected override void UpdateEntityInternal()
        {
            Color = Gradient.Evaluate(Blend);
            
            OnColorChanged.Invoke(Color);
        }

        private void Reset()
        {
            UpdateEntityInternal();
        }
    }
}