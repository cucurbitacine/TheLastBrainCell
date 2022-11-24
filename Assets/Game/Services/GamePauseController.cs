using Game.Inputs;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using CallbackContext = UnityEngine.InputSystem.InputAction.CallbackContext;

namespace Game.Services
{
    public class GamePauseController : InputController
    {
        public bool paused;

        [Space]
        public UnityEvent<bool> onPauseStateChanged;

        [Space]
        public InputAction pauseAction;

        #region Public API

        /// <summary>
        /// Toggle pause state
        /// </summary>
        public void TogglePause()
        {
            ChangePause(!paused);
        }
        
        /// <summary>
        /// Change pause state on new
        /// </summary>
        /// <param name="pause"></param>
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

        #endregion

        private void PauseHandle(CallbackContext ctx)
        {
            if (ctx.ReadValueAsButton()) TogglePause();
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