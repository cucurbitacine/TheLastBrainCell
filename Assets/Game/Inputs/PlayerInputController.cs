using Game.Characters;
using UnityEngine;
using UnityEngine.InputSystem;
using CallbackContext = UnityEngine.InputSystem.InputAction.CallbackContext;

namespace Game.Inputs
{
    public class PlayerInputController : InputController<PlayerController>
    {
        [Space]
        public InputAction moveAction;
        public InputAction jumpAction;
        public InputAction attackAction;
        public InputAction mouseAction;

        private Vector2 _mousePosition;
        
        private void MoveHandle(CallbackContext ctx)
        {
            var direction = ctx.ReadValue<Vector2>();
            
            Character.Move(direction);
        }
        
        private void JumpHandle(CallbackContext ctx)
        {
            var jump = ctx.ReadValueAsButton();

            if (jump) Character.Jump();
        }
        
        private void AttackHandle(CallbackContext ctx)
        {
            var attack = ctx.ReadValueAsButton();

            if (attack) Character.Attack(Character.AttackMeleeName);
        }

        private void MouseHandle(CallbackContext ctx)
        {
            _mousePosition = ctx.ReadValue<Vector2>();
        }
        
        private void OnEnable()
        {
            EnableAction(moveAction, MoveHandle);
            EnableAction(jumpAction, JumpHandle);
            EnableAction(attackAction, AttackHandle);
            
            EnableAction(mouseAction, MouseHandle);
        }

        private void OnDisable()
        {
            DisableAction(moveAction);
            DisableAction(jumpAction);
            DisableAction(attackAction);
            
            DisableAction(mouseAction);
        }

        private void Update()
        {
           var view =  (Vector2)Camera.main.ScreenToWorldPoint(_mousePosition) - Character.position;
           
           Character.View(view);
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(Camera.main.ScreenToWorldPoint(_mousePosition), 0.5f);
        }
    }
}