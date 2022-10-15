using UnityEngine;

namespace CucuTools.IK.Impl
{
    public sealed class CucuIKJointTarget : CucuIKTarget
    {
        public override Vector3 TargetPosition
        {
            get => UseWorldSpace ? transform.position : transform.localPosition;
            set
            {
                if (UseWorldSpace) transform.position = value;
                else transform.localPosition = value;
            }
        }
    }
}