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
        public PlayerController player;
        
        [Space]
        public ComboController comboController;
        
        [Space]
        public InputAction moveAction;
        public InputAction jumpAction;
        public InputAction attackAction;

        private void MoveHandle(InputAction.CallbackContext ctx)
        {
            var dir = ctx.ReadValue<Vector2>();
            
            player.Move(dir);
            player.View(dir);
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
        
        public override void EnableCharacter(PlayerController character)
        {
            Debug.Log($"{name} enabled");
            
            player = character;

            comboController = player.GetComponentInChildren<ComboController>();
            
            EnableAction(moveAction, MoveHandle);
            EnableAction(jumpAction, JumpHandle);
            EnableAction(attackAction, AttackHandle);
        }

        public override void DisableCharacter(PlayerController character)
        {
            Debug.Log($"{name} disabled");
            
            player = null;
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