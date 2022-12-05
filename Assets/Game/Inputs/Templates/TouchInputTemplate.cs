using Game.Characters.Player;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using CallbackContext = UnityEngine.InputSystem.InputAction.CallbackContext;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

namespace Game.Inputs.Templates
{
    [CreateAssetMenu(menuName = "Create TouchInputTemplate", fileName = "TouchInputTemplate", order = 0)]
    public class TouchInputTemplate : InputTemplate<PlayerController>
    {
        public PlayerController player;

        [Space]
        public InputAction touchAction;

        [Space]
        public int lastTouchId;
        public bool moved;
        public Vector2 touchBeganPosition;
        public Vector2 touchPosition;
        
        public Camera MainCamera => Camera.main;
        
        public Vector2 WorldBeganPosition => MainCamera.ScreenToWorldPoint(touchBeganPosition);
        public Vector2 WorldPosition => MainCamera.ScreenToWorldPoint(touchPosition);
        
        public Vector2 Swipe => WorldPosition - WorldBeganPosition;
        
        private void HandleTouch(CallbackContext ctx)
        {
            var touch = ctx.ReadValue<TouchState>();
            
            lastTouchId = touch.touchId;
            
            //if (touch.touchId > 1) return; // ignoring another fingers
            
            moved = touch.phase == TouchPhase.Moved;

            touchPosition = touch.position;
            
            if (touch.phase == TouchPhase.Began)
            {
                touchBeganPosition = touchPosition;
            }
        }
        
        public override void EnableCharacter(PlayerController character)
        {
            Debug.Log($"{name} enabled");
            
            player = character;
            
            EnableAction(touchAction, HandleTouch);
        }

        public override void DisableCharacter(PlayerController character)
        {
            Debug.Log($"{name} disabled");
            
            player = null;
            
            DisableAction(touchAction);
        }

        public override void UpdateInput(float deltaTime)
        {
            if (player == null) return;
            
            if (moved)
            {
                player.Move(Swipe);   
                player.View(Swipe);   
            }
            else
            {
                player.Move(Vector2.zero);   
            }
        }
    }
}