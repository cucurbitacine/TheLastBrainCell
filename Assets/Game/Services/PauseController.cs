using Game.Inputs;
using Game.Levels;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Game.Services
{
    public class PauseController : InputController
    {
        public bool paused;

        [Space]
        public UnityEvent<bool> onPauseStateChanged;

        [Space]
        public InputAction pauseAction;
        
        public void TogglePause()
        {
            ChangePause(!paused);
        }
        
        public void ChangePause(bool pause)
        {
            if (pause) Pause();
            else Unpause();
        }
        
        public void Pause()
        {
            if (paused) return;
            
            paused = true;
            
            Time.timeScale = 0f;
            
            onPauseStateChanged.Invoke(true);
        }
        
        public void Unpause()
        {
            if (!paused) return;
            
            Time.timeScale = 1f;

            paused = false;
            
            onPauseStateChanged.Invoke(false);
        }

        private void PauseHandle(InputAction.CallbackContext ctx)
        {
            var pauseEvent = ctx.ReadValueAsButton();

            if (pauseEvent) TogglePause();
        }
        
        private void OnEnable()
        {
            EnableAction(pauseAction, PauseHandle);
        }

        private void OnDisable()
        {
            DisableAction(pauseAction);

            Time.timeScale = 1f;
        }
    }
}