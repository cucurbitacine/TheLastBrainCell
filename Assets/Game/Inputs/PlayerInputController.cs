using Game.Characters;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Inputs
{
    public class PlayerInputController : InputController<PlayerController>
    {
        [Space]
        public InputAction mouseMoveAction;
        public InputAction mouseAttackAction;
        public InputAction moveAction;
        public InputAction jumpAction;
        public InputAction attackAction;

        private bool _mouseMove = false;
        private Vector2 _mousePosition = Vector2.zero;
        private Vector2 _mouseWorldPosition = Vector2.zero;

        private void MouseMoveStartedHandle(InputAction.CallbackContext ctx)
        {
            _mouseMove = true;
        }
        
        private void MouseMoveCanceledHandle(InputAction.CallbackContext ctx)
        {
            _mouseMove = false;
            
            Character.Move(Vector2.zero);
        }
        
        private void MouseAttackHandle(InputAction.CallbackContext ctx)
        {
            var attack = ctx.ReadValueAsButton();

            if (attack) Character.Attack(Character.AttackMeleeName);
        }
        
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

        private void Update()
        {
            if (_mouseMove)
            {
                _mousePosition = Input.mousePosition;

                _mouseWorldPosition = Camera.main.ScreenToWorldPoint(_mousePosition);
                
                var direction = _mouseWorldPosition - Character.position;

                if (direction.sqrMagnitude > 0.001f)
                {
                    Character.Move(direction);
                    Character.View(direction);
                }
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(_mouseWorldPosition, 0.1f);
        }

        private void OnEnable()
        {
            EnableAction(moveAction, MoveHandle);
            EnableAction(jumpAction, JumpHandle);
            EnableAction(attackAction, AttackHandle);
            
            EnableAction(mouseAttackAction, MouseAttackHandle);

            mouseMoveAction.started += MouseMoveStartedHandle;
            mouseMoveAction.canceled += MouseMoveCanceledHandle;
            mouseMoveAction.Enable();
        }

        private void OnDisable()
        {
            DisableAction(moveAction);
            DisableAction(jumpAction);
            DisableAction(attackAction);
            
            DisableAction(mouseAttackAction);
            
            mouseMoveAction.Disable();
            mouseMoveAction.started -= MouseMoveStartedHandle;
            mouseMoveAction.canceled -= MouseMoveCanceledHandle;
        }
    }
}