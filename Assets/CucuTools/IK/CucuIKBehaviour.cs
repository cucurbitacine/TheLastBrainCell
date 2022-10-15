using UnityEngine;

namespace CucuTools.IK
{
    public abstract class CucuIKBehaviour : CucuBehaviour
    {
        [SerializeField] private CucuIKBrain brain = default;
        
        public CucuIKBrain Brain
        {
            get => brain;
            set => brain = value;
        }
    }
}