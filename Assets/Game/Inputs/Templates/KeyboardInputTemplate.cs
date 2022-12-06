using Game.Characters.Player;
using Game.Inputs.Combos;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Inputs.Templates
{
    
    [CreateAssetMenu(menuName = "Create KeyboardInputTemplate", fileName = "KeyboardInputTemplate", order = 0)]
    public class KeyboardInputTemplate : InputTemplate<PlayerController>
    {
        [Space]
        public ComboController comboController;
        
        [Space]
        public InputAction moveAction;
        public InputAction jumpAction;
        public InputAction attackAction;

        private void MoveHandle(InputAction.CallbackContext ctx)
        {
            var dir = ctx.ReadValue<Vector2>();
            
            character.Move(dir);
            character.View(dir);
        }
        
        private void JumpHandle(InputAction.CallbackContext ctx)
        {
            if (ctx.ReadValueAsButton()) character.Jump();
        }

        private void AttackHandle(InputAction.CallbackContext ctx)
        {
            if (ctx.ReadValueAsButton())
            {
                if (character.Info.isJumping)
                {
                    character.Attack(character.JumpAttackName);
                }
                else
                {
                    if (character.CanAttack())
                    {
                        if (comboController.Attack(out var attackName))
                        {
                            character.Attack(attackName);
                        }
                    }
                }
            }
        }
        
        protected override void StartInput()
        {
            comboController = character.GetComponentInChildren<ComboController>();
            
            EnableAction(moveAction, MoveHandle);
            EnableAction(jumpAction, JumpHandle);
            EnableAction(attackAction, AttackHandle);
        }

        protected override void StopInput()
        {
            comboController = null;
            
            DisableAction(moveAction);
            DisableAction(jumpAction);
            DisableAction(attackAction);
        }

        public override void UpdateInput(float deltaTime)
        {
            //
        }
    }
}