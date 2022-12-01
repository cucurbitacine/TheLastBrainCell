using Game.Characters.Player;
using Game.Inputs.Combos;
using UnityEngine;
using UnityEngine.InputSystem;
using CallbackContext = UnityEngine.InputSystem.InputAction.CallbackContext;

namespace Game.Inputs
{
    public class PlayerInputController : InputController<PlayerController>
    {
        public ComboController comboController;
        
        [Space]
        public InputAction moveAction;
        public InputAction jumpAction;
        public InputAction attackAction;
        public InputAction mouseAction;

        public Camera MainCamera => Camera.main;
        public Vector2 MouseWorldPosition => MainCamera.ScreenToWorldPoint(_mouseScreenPosition);
        
        private Vector2 _mouseScreenPosition;

        private void MoveHandle(CallbackContext ctx)
        {
            Character.Move(ctx.ReadValue<Vector2>());
        }
        
        private void JumpHandle(CallbackContext ctx)
        {
            if (ctx.ReadValueAsButton()) Character.Jump();
        }
        
        private void AttackHandle(CallbackContext ctx)
        {
            if (ctx.ReadValueAsButton())
            {
                if (Character.CanAttack() && comboController.Attack(out var attackName))
                {
                    Character.Attack(attackName);
                }
            }
        }

        private void MouseHandle(CallbackContext ctx)
        {
            _mouseScreenPosition = ctx.ReadValue<Vector2>();
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
           var view = MouseWorldPosition - Character.position;
           
           Character.View(view);
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(MouseWorldPosition, 0.5f);
        }
    }
}