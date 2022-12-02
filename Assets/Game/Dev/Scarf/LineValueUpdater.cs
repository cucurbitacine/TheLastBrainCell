using CucuTools.Colors;
using Game.Stats;
using UnityEngine;

namespace Game.Dev.Scarf
{
    public class LineValueUpdater : MonoBehaviour
    {
        [Min(0f)]
        public float damp = 4;
        public GradientMode mode = GradientMode.Blend;
        
        [Space]
        public Color colorEmpty = Color.black;
        [Range(0f, 1f)]
        public float blendEmpty = 0.5f;
        [Range(0f, 1f)]
        public float alphaEmpty = 0.0f;
        
        [Space]
        public Gradient colorGradient;


        [Space]
        public ValueIntBehaviour value;
        public LineRenderer line;

        private Gradient _actualGradient;
        
        private void UpdateLine(float deltaTime)
        {
            _actualGradient = line.colorGradient;
            
            var colorKeys = _actualGradient.colorKeys;
            var alphaKeys = _actualGradient.alphaKeys;

            for (var i = 0; i < colorKeys.Length; i++)
            {
                var t = (float)i / (colorKeys.Length - 1);
                var color = colorGradient.Evaluate(t);

                var valueNormalized = (float)value.Value / value.MaxValue;
                
                if (t > valueNormalized)
                {
                    color = color.LerpTo(colorEmpty, blendEmpty).AlphaTo(alphaEmpty);
                }
                
                colorKeys[i].color = Color.Lerp(colorKeys[i].color, color, deltaTime * damp);
                alphaKeys[i].alpha = Mathf.Lerp(alphaKeys[i].alpha, color.a, deltaTime * damp);
            }

            var tempGradient = line.colorGradient;
            tempGradient.colorKeys = colorKeys;
            tempGradient.alphaKeys = alphaKeys;
            tempGradient.mode = mode;
            line.colorGradient = tempGradient;
        }

        private void Start()
        {
            var count = 8;
            
            _actualGradient = new Gradient();
            var colorKeys = new GradientColorKey[count];
            var alphaKeys = new GradientAlphaKey[count];
            
            for (var i = 0; i < count; i++)
            {
                var t = (float)i / (count - 1);
                var color = colorGradient.Evaluate(t);
                colorKeys[i] = new GradientColorKey(color, t);
                alphaKeys[i] = new GradientAlphaKey(color.a, t);
            }

            _actualGradient.colorKeys = colorKeys;
            _actualGradient.alphaKeys = alphaKeys;

            line.colorGradient = _actualGradient;
        }

        private void Update()
        {
            UpdateLine(Time.deltaTime);
        }
    }
}