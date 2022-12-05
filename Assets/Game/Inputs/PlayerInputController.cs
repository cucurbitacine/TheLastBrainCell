using Game.Characters.Player;
using UnityEngine;

namespace Game.Inputs
{
    public class PlayerInputController : InputController<PlayerController>
    {
        [SerializeField] private InputTemplate<PlayerController> input = null;

        public InputTemplate<PlayerController> Input
        {
            get => input;
            protected set => input = value;
        }
        
        public void SwitchInputTemplate(InputTemplate<PlayerController> inputTemplate)
        {
            Input.DisableCharacter(Character);
            Input = inputTemplate;
            Input.EnableCharacter(Character);
        }
        
        private void OnEnable()
        {
            Input.EnableCharacter(Character);
        }

        private void Update()
        {
            Input.UpdateInput(Time.deltaTime);
        }
        
        private void OnDisable()
        {
            Input.DisableCharacter(Character);
        }
    }
}