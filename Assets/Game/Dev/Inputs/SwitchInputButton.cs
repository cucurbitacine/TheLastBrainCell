using System;
using Game.Characters.Player;
using Game.Inputs.Templates;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Dev.Inputs
{
    [RequireComponent(typeof(Button))]
    public class SwitchInputButton : MonoBehaviour
    {
        public InputTemplate<PlayerController> inputTemplate;
        public PlayerInputController playerInput;

        [Space]
        public Button button;

        public void Switch()
        {
            playerInput.SwitchInputTemplate(inputTemplate);
        }
        
        private void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(Switch);
        }
    }
}
