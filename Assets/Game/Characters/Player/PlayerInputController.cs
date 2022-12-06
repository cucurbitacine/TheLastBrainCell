using CucuTools.Attributes;
using Game.Inputs;
using Game.Inputs.Templates;
using UnityEngine;

namespace Game.Characters.Player
{
    public class PlayerInputController : InputController<PlayerController>
    {
        public InputTemplate<PlayerController> input;
        public bool syncWithEnabling = true;
        
        [Space]
        public bool isActive = false;
        
        [CucuButton("Start", colorHex:"39a845", group:"Player input")]
        public void StartInput()
        {
            if (isActive) return;
            isActive = true;

            if (input != null) input.StartInput(Character);
        }
        
        [CucuButton("Stop", colorHex:"9b1b30", group:"Player input")]
        public void StopInput()
        {
            if (!isActive) return;

            isActive = false;
            if (input != null) input.StopInput(Character);
        }
        
        public void SwitchInputTemplate(InputTemplate<PlayerController> inputTemplate)
        {
            if (input != null) input.StopInput(Character);
            input = inputTemplate;

            if (isActive && input != null) input.StartInput(Character);
        }

        private void UpdateInput()
        {
            if (input != null) input.UpdateInput(Time.deltaTime);
        }

        #region MonoBehaviour

        private void OnEnable()
        {
            if (syncWithEnabling) StartInput();
        }

        private void Update()
        {
            if (isActive) UpdateInput();
        }
        
        private void OnDisable()
        {
            if (syncWithEnabling) StopInput();
        }

        #endregion
    }
}