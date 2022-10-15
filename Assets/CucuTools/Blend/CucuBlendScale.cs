using System;
using UnityEngine;

namespace CucuTools.Blend
{
    public class CucuBlendScale : CucuBlendTransformBase
    {
        [Header("Scale")]
        [SerializeField] private ScaleInfoParam scaleInfo;
        
        public ScaleInfoParam ScaleInfo
        {
            get => scaleInfo ?? (scaleInfo = new ScaleInfoParam());
            set
            {
                scaleInfo = value;
                
                UpdateEntity();
            }
        }

        protected override void UpdateEntityInternal()
        {
            Target.localScale = ScaleInfo.Evaluate(Blend);
        }
    }

    [Serializable]
    public class ScaleInfoParam : InfoParamBase<Vector3>
    {
        public Vector3 Start
        {
            get => start;
            set => start = value;
        }

        public Vector3 End
        {
            get => end;
            set => end = value;
        }
        
        public AnimationCurve Modificator
        {
            get => modificator;
            set => modificator = value;
        }
        
        [SerializeField] private Vector3 start;
        [SerializeField] private Vector3 end;
        [SerializeField] private AnimationCurve modificator;
        
        public ScaleInfoParam()
        {
            start = Vector3.one;
            end = Vector3.one;
            modificator = modificator = AnimationCurve.Constant(0, 1, 1);
        }

        public override Vector3 Evaluate(float t)
        {
            return Modificator.Evaluate(t) * Vector3.Lerp(Start, End, t);
        }
    }
}