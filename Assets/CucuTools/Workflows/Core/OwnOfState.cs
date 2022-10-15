using UnityEngine;

namespace CucuTools.Workflows.Core
{
    public abstract class OwnOfState : CucuBehaviour, IOwnOfState
    {
        public StateEntity Owner => GetOwner();
        
        private StateEntity _ownerCache;
        
        [SerializeField] private bool hasOwner;
        
        private StateEntity GetOwner()
        {
            return _ownerCache != null ? _ownerCache : (_ownerCache = GetOwner(transform));
        }
        
        public static StateEntity GetOwner(Transform root)
        {
            if (root == null) return null;

            var state = root.GetComponent<StateEntity>();

            if (state != null) return state;

            return GetOwner(root.parent);
        }
        
        protected virtual void OnValidate()
        {
            hasOwner = Owner != null;
        }
    }
    
    public interface IOwnOfState
    {
        StateEntity Owner { get; }
    }
}