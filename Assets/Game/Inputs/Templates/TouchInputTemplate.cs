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
            
            moved = touch.phase == TouchPhase.Moved;

            touchPosition = touch.position;
            
            if (touch.phase == TouchPhase.Began)
            {
                touchBeganPosition = touchPosition;
            }
        }

        private void Attack()
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
        
        private void Jump()
        {
            character.Jump();
        }
        
        public void SetupUI(TouchUIController ui)
        {
            touchUi = ui;
        }
        
        protected override void StartInput()
        {
            comboController = character.GetComponentInChildren<ComboController>();

            if (touchUiPrefab != null)
            {
                touchUi = Instantiate(touchUiPrefab);
                
                touchUi.attackButton.onClick.AddListener(Attack);
                touchUi.jumpButton.onClick.AddListener(Jump);
                
                touchUi.thumbstickPanel.gameObject.SetActive(false);
            }
            
            EnableAction(touchAction, HandleTouch);
        }

        protected override void StopInput()
        {
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
            if (character == null) return;
            
            if (moved)
            {
                character.Move(Swipe);   
                character.View(Swipe);

                if (touchUi != null)
                {
                    touchUi.thumbstickAnimator.SetFloat(touchUi.MoveX, Swipe.normalized.x);
                    touchUi.thumbstickAnimator.SetFloat(touchUi.MoveY, Swipe.normalized.y);
                    touchUi.thumbstickPanel.gameObject.SetActive(true);
                    touchUi.thumbstickPanel.transform.position = touchBeganPosition;
                }
            }
            else
            {
                character.Move(Vector2.zero);
                
                if (touchUi != null)
                {
                    touchUi.thumbstickPanel.gameObject.SetActive(false);
                }
            }
        }
    }
}