using Game.Characters.Player;
using Game.Inputs.Templates;
using UnityEngine;

namespace Game.Inputs
{
    public class InputSwitcher : MonoBehaviour
    {
        public PlayerInputController playerInput;

        [Space]
        public InputTemplate<PlayerController> template1;
        public InputTemplate<PlayerController> template2;
        public InputTemplate<PlayerController> template3;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                playerInput.SwitchInputTemplate(template1);
            }
            
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                playerInput.SwitchInputTemplate(template2);
            }
            
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                playerInput.SwitchInputTemplate(template3);
            }
        }
    }
}