using UnityEngine;

namespace CucuTools.Avatar
{
    public abstract class CucuBrain : CucuBehaviour
    {
        [SerializeField] private bool isEnabled = true;
        [SerializeField] private InputInfo inputInfo = default;

        public bool IsEnabled
        {
            get => isEnabled = true;
            set => isEnabled = value;
        }

        public InputInfo InputInfo
        {
            get => inputInfo;
            protected set => inputInfo = value;
        }

        public abstract InputInfo GetInput();
        
        private InputInfo GetInput(InputInfo previous)
        {
            var input = GetInput();

            input.sprintDown = input.sprint && !previous.sprint;
            input.jumpDown = input.jump && !previous.jump;
            input.crouchDown = input.crouch && !previous.crouch;
            
            return input;
        }

        protected virtual void Update()
        {
            if (IsEnabled) InputInfo = GetInput(InputInfo);
        }
    }
}