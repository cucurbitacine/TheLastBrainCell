using System;
using UnityEngine;

namespace CucuTools.Blend
{
    public class CucuBlendMove : CucuBlendTransformBase
    {
        [Header("Move")]
        [SerializeField] private bool changeLocal = true;
        [SerializeField] private MoveInfoParam moveInfo;
        
        public bool ChangeLocal
        {
            get => changeLocal;
            set
            {
                changeLocal = value;
                
                UpdateEntity();
            }
        }
        
        public MoveInfoParam MoveInfo
        {
            get => moveInfo ?? (moveInfo = new MoveInfoParam());
            set
            {
                moveInfo = value;
                
                UpdateEntity();
            }
        }

        protected override void UpdateEntityInternal()
        {
            if (ChangeLocal)
            {
                Target.localPosition = MoveInfo.Evaluate(Blend);
            }
            else
            {
                Target.position = MoveInfo.Evaluate(Blend);
            }
        }
    }
    
    [Serializable]
    public class MoveInfoParam : InfoParamBase<Vector3>
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
        
        public MoveInfoParam()
        {
            start = Vector3.zero;
            end = Vector3.zero;
            modificator = AnimationCurve.Constant(0, 1, 1);
        }

        public override Vector3 Evaluate(float t)
        {
            return Modificator.Evaluate(t) * Vector3.Lerp(Start, End, t);
        }
    }
}