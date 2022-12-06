using Game.Characters.Player;
using Game.Inputs.Combos;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Inputs.Templates
{
    [CreateAssetMenu(menuName = "Create KeyboardAndMouseInputTemplate", fileName = "KeyboardAndMouseInputTemplate", order = 0)]
    public class KeyboardAndMouseInputTemplate : InputTemplate<PlayerController>
    {
        [Space]
        public ComboController comboController;
        
        [Space]
        public InputAction moveAction;
        public InputAction jumpAction;
        public InputAction attackAction;
        public InputAction mouseAction;

        public Camera MainCamera => Camera.main;
        public Vector2 MouseWorldPosition => MainCamera.ScreenToWorldPoint(_mouseScreenPosition);
        
        private Vector2 _mouseScreenPosition;

        private void MoveHandle(InputAction.CallbackContext ctx)
        {
            character.Move(ctx.ReadValue<Vector2>());
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

        private void MouseHandle(InputAction.CallbackContext ctx)
        {
            _mouseScreenPosition = ctx.ReadValue<Vector2>();
        }
        
        protected override void StartInput()
        {
            comboController = character.GetComponentInChildren<ComboController>();
            
            EnableAction(moveAction, MoveHandle);
            EnableAction(jumpAction, JumpHandle);
            EnableAction(attackAction, AttackHandle);
            
            EnableAction(mouseAction, MouseHandle);
        }

        protected override void StopInput()
        {
            comboController = null;
            
            DisableAction(moveAction);
            DisableAction(jumpAction);
            DisableAction(attackAction);
            
            DisableAction(mouseAction);
        }

        public override void UpdateInput(float deltaTime)
        {
            if (character == null) return;
            
            var view = MouseWorldPosition - character.position;
           
            character.View(view);
        }
    }
}