using Game.Characters.Player;
using UnityEngine;

namespace Game.Inputs
{
    public class InputSwitcher : MonoBehaviour
    {
        public PlayerInputController playerInput;

        [Space]
        public InputTemplate<PlayerController> template1;
        public InputTemplate<PlayerController> template2;

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
        }
    }
}