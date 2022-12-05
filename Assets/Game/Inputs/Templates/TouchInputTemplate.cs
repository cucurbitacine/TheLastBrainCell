using Game.Characters.Player;
using Game.Inputs.Combos;
using Game.UI;
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
        public ComboController comboController;
        public TouchUIController touchUi; 
        
        [Space]
        public InputAction touchAction;

        [Space]
        public int lastTouchId;
        public bool moved;
        public Vector2 touchBeganPosition;
        public Vector2 touchPosition;
        
        [Space]
        public TouchUIController touchUiPrefab; 
        
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

        private void Attack()
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
        
        private void Jump()
        {
            player.Jump();
        }
        
        public void SetupUI(TouchUIController ui)
        {
            touchUi = ui;
        }
        
        public override void EnableCharacter(PlayerController character)
        {
            Debug.Log($"{name} enabled");
            
            player = character;
            comboController = player.GetComponentInChildren<ComboController>();

            if (touchUiPrefab != null)
            {
                touchUi = Instantiate(touchUiPrefab);
                
                touchUi.attackButton.onClick.AddListener(Attack);
                touchUi.jumpButton.onClick.AddListener(Jump);
                
                touchUi.thumbstickPanel.gameObject.SetActive(false);
            }
            
            EnableAction(touchAction, HandleTouch);
        }

        public override void DisableCharacter(PlayerController character)
        {
            Debug.Log($"{name} disabled");
            
            player = null;
            comboController = null;

            if (touchUi != null)
            {
                touchUi.attackButton.onClick.RemoveListener(Attack);
                touchUi.jumpButton.onClick.RemoveListener(Jump);
                
                Destroy(touchUi.gameObject);
            }
            
            DisableAction(touchAction);
        }

        public override void UpdateInput(float deltaTime)
        {
            if (player == null) return;
            
            if (moved)
            {
                player.Move(Swipe);   
                player.View(Swipe);

                if (touchUi != null)
                {
                    touchUi.thumbstickAnimator.SetFloat("MoveX", Swipe.normalized.x);
                    touchUi.thumbstickAnimator.SetFloat("MoveY", Swipe.normalized.y);
                    touchUi.thumbstickPanel.gameObject.SetActive(true);
                    touchUi.thumbstickPanel.transform.position = touchBeganPosition;
                }
            }
            else
            {
                player.Move(Vector2.zero);
                
                if (touchUi != null)
                {
                    touchUi.thumbstickPanel.gameObject.SetActive(false);
                }
            }
        }
    }
}