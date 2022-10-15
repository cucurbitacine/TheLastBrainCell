using System;
using UnityEngine;

namespace CucuTools.Avatar
{
    public abstract class CucuBrain2D : MonoBehaviour
    {        
        [SerializeField] private bool isEnabled = true;
        [SerializeField] private InputInfo2D inputInfo2D = default;

        public bool IsEnabled
        {
            get => isEnabled;
            set => isEnabled = value;
        }
    
        public InputInfo2D InputInfo2D
        {
            get => inputInfo2D;
            protected set => inputInfo2D = value;
        }

        protected abstract InputInfo2D GetInput();
    
        protected virtual void Update()
        {
            InputInfo2D = IsEnabled ? GetInput() : default;
        }
    }

    [Serializable]
    public struct InputInfo2D
    {
        public Vector2 view;
        public Vector2 move;
        public bool sprint;
        public bool jump;
        public bool climbDown;
    }
}