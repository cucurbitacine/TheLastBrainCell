using UnityEngine;

namespace CucuTools.Interactables
{
    public class GrabbableBehaviour : GrabbableEntity
    {
        private Rigidbody _rigidbody = default;
        
        [Space]
        [SerializeField] private bool isGrabbed = default;
        
        #region GrabbableEntity

        public override bool IsGrabbed
        {
            get => isGrabbed;
            protected set => isGrabbed = value;
        }
        
        public override bool IsRigid => Rigidbody != null;
        
        public override Rigidbody Rigidbody => _rigidbody != null ? _rigidbody : (_rigidbody = GetComponent<Rigidbody>());

        public override void Grab()
        {
            if (IsGrabbed) return;
            
            //Interactable.Idle();
            Interactable.IsEnabled = false;
            
            IsGrabbed = true;
        }
        
        public override void Free()
        {
            if (!IsGrabbed) return;
            
            IsGrabbed = false;
            
            Interactable.IsEnabled = true;
        }

        #endregion
    }
}