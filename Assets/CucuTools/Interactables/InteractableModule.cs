using UnityEngine;

namespace CucuTools.Interactables
{
    [RequireComponent(typeof(InteractableEntity))]
    public abstract class InteractableModule : CucuBehaviour
    {
        private InteractableEntity _interactable = default;
        
        public InteractableEntity Interactable => _interactable != null
            ? _interactable
            : (_interactable = GetComponent<InteractableEntity>());
    }
}