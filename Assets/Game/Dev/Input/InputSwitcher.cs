using Game.Characters.Player;
using Game.Inputs.Templates;
using Game.UI;
using UnityEngine;

namespace Game.Dev.Input
{
    public class InputSwitcher : MonoBehaviour
    {
        public PlayerInputController playerInput;

        [Space]
        public InputTemplate<PlayerController> template1;
        public InputTemplate<PlayerController> template2;
        public InputTemplate<PlayerController> template3;

        private void Start()
        {
            var touchUi = FindObjectOfType<TouchUIController>();

            if (template3 is TouchInputTemplate touchTemplate)
            {
                touchTemplate.SetupUI(touchUi);
            }
        }

        private void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha1))
            {
                playerInput.SwitchInputTemplate(template1);
            }
            
            if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha2))
            {
                playerInput.SwitchInputTemplate(template2);
            }
            
            if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha3))
            {
                playerInput.SwitchInputTemplate(template3);
            }
        }
    }
}