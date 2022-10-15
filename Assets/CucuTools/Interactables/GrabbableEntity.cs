using UnityEngine;

namespace CucuTools.Interactables
{
    [DisallowMultipleComponent]
    public abstract class GrabbableEntity : InteractableModule
    {
        public abstract bool IsGrabbed { get; protected set; }
        public abstract bool IsRigid { get; }
        public abstract Rigidbody Rigidbody { get; }
        public abstract void Grab();
        public abstract void Free();
    }
}