using Game.Characters;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Inputs
{
    public class PlayerInputController : InputController<PlayerController>
    {
        [Space]
        public InputAction moveAction;
        public InputAction jumpAction;
        public InputAction attackAction;
        
        private void MoveHandle(InputAction.CallbackContext ctx)
        {
            var direction = ctx.ReadValue<Vector2>();
            
            Character.Move(direction);
            Character.View(direction);
        }
        
        private void JumpHandle(InputAction.CallbackContext ctx)
        {
            var jump = ctx.ReadValueAsButton();

            if (jump) Character.Jump();
        }
        
        private void AttackHandle(InputAction.CallbackContext ctx)
        {
            var attack = ctx.ReadValueAsButton();

            if (attack) Character.Attack(Character.AttackMeleeName);
        }

        private void OnEnable()
        {
            EnableAction(moveAction, MoveHandle);
            EnableAction(jumpAction, JumpHandle);
            EnableAction(attackAction, AttackHandle);
        }

        private void OnDisable()
        {
            DisableAction(moveAction);
            DisableAction(jumpAction);
            DisableAction(attackAction);
        }
    }
}