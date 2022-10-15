using UnityEngine;

namespace CucuTools.IK
{
    public abstract class CucuIKTarget : CucuBehaviour
    {
        [Header("Target")]
        [SerializeField] private bool useWorldSpace = true;

        public bool UseWorldSpace
        {
            get => useWorldSpace;
            set => useWorldSpace = value;
        }
        public abstract Vector3 TargetPosition { get; set; }
    }
}