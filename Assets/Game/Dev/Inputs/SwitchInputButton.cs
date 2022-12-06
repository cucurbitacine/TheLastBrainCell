using System;
using Game.Characters.Player;
using Game.Inputs.Templates;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Dev.Inputs
{
    [RequireComponent(typeof(Button))]
    public class SwitchInputButton : MonoBehaviour
    {
        public InputTemplate<PlayerController> inputTemplate;

        [Space]
        public PlayerInputController playerInput;
        public Button button;
        public TextMeshProUGUI text;

        public void Switch()
        {
            playerInput.SwitchInputTemplate(inputTemplate);
            if (text != null) text.text = inputTemplate.inputName;
        }
        
        private void Awake()
        {
            if (button == null) button = GetComponent<Button>();
            if (playerInput == null) playerInput = FindObjectOfType<PlayerInputController>();
            
            button = GetComponent<Button>();
            button.onClick.AddListener(Switch);

            var buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
            buttonText.text = inputTemplate.inputName;
        }

        private void Start()
        {
            if (inputTemplate.isActive)
            {
                if (text != null) text.text = inputTemplate.inputName;
            }
        }
    }
}
