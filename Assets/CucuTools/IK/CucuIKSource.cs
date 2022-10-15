using UnityEngine;

namespace CucuTools.IK
{
    public abstract class CucuIKSource : CucuIKBehaviour
    {
        [Header("Source")]
        [SerializeField] private bool useWorldSpace = true;

        public bool UseWorldSpace
        {
            get => useWorldSpace;
            set => useWorldSpace = value;
        }

        public abstract Vector3[] GetPoints();

        public virtual void SetPoints()
        {
            Brain?.SetPoints(GetPoints(), UseWorldSpace);
        }
        
        protected virtual void Start()
        {
            SetPoints();
        }
    }
}