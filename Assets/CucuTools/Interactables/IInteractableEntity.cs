using System;
using CucuTools.Attributes;
using UnityEngine;

namespace CucuTools.Interactables
{
    /// <summary>
    /// Interactable object
    /// </summary>
    public interface IInteractableEntity
    {
        bool IsEnabled { get; set; }
        
        /// <summary>
        /// Interactable state
        /// </summary>
        InteractInfo InteractInfo { get; }

        /// <summary>
        /// To Idle State
        /// </summary>
        void Idle();
        
        /// <summary>
        /// To Hover State by <paramref name="context"/>
        /// </summary>
        /// <param name="context"></param>
        void Hover(ICucuContext context);
        
        /// <summary>
        /// To Press State by <paramref name="context"/>
        /// </summary>
        /// <param name="context"></param>
        void Press(ICucuContext context);
    }

    /// <summary>
    /// Interactable object info
    /// </summary>
    [Serializable]
    public class InteractInfo
    {
        [CucuReadOnly] [SerializeField] private bool idle;
        [CucuReadOnly] [SerializeField] private bool hover;
        [CucuReadOnly] [SerializeField] private bool press;

        public bool Idle
        {
            get => idle;
            set => idle = value;
        }

        public bool Hover
        {
            get => hover;
            set => hover = value;
        }

        public bool Press
        {
            get => press;
            set => press = value;
        }

        public InteractInfo()
        {
            Idle = true;
            Hover = false;
            Press = false;
        }
    }
}