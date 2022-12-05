using Game.Characters.Player;
using Game.Inputs.Combos;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Inputs
{
    [CreateAssetMenu(menuName = "Create KeyboardAndMouseInputTemplate", fileName = "KeyboardAndMouseInputTemplate", order = 0)]
    public class KeyboardAndMouseInputTemplate : InputTemplate<PlayerController>
    {
        [Space]
        public PlayerController player;
        
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
            player.Move(ctx.ReadValue<Vector2>());
        }
        
        private void JumpHandle(InputAction.CallbackContext ctx)
        {
            if (ctx.ReadValueAsButton()) player.Jump();
        }
        
        private void AttackHandle(InputAction.CallbackContext ctx)
        {
            if (ctx.ReadValueAsButton())
            {
                if (player.Info.isJumping)
                {
                    player.Attack(player.JumpAttackName);
                }
                else
                {
                    if (player.CanAttack())
                    {
                        if (comboController.Attack(out var attackName))
                        {
                            player.Attack(attackName);
                        }
                    }
                }
            }
        }

        private void MouseHandle(InputAction.CallbackContext ctx)
        {
            _mouseScreenPosition = ctx.ReadValue<Vector2>();
        }
        
        public override void EnableCharacter(PlayerController character)
        {
            player = character;

            comboController = player.GetComponentInChildren<ComboController>();
            
            EnableAction(moveAction, MoveHandle);
            EnableAction(jumpAction, JumpHandle);
            EnableAction(attackAction, AttackHandle);
            
            EnableAction(mouseAction, MouseHandle);
        }

        public override void DisableCharacter(PlayerController character)
        {
            player = null;
            comboController = null;
            
            DisableAction(moveAction);
            DisableAction(jumpAction);
            DisableAction(attackAction);
            
            DisableAction(mouseAction);
        }

        public override void UpdateInput(float deltaTime)
        {
            if (player == null) return;
            
            var view = MouseWorldPosition - player.position;
           
            player.View(view);
        }
    }
}