using System;
using System.Collections.Generic;
using Game.Characters;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Inputs.Templates
{
    public abstract class InputTemplate<TCharacter> : ScriptableObject
        where TCharacter : CharacterControllerBase
    {
        public string inputName;
        public bool isActive;
        
        [Space]
        public TCharacter character;
        
        private readonly Dictionary<InputAction, Action<InputAction.CallbackContext>> _performedBinds = new Dictionary<InputAction, Action<InputAction.CallbackContext>>();
        
        protected void EnableAction(InputAction inputAction, Action<InputAction.CallbackContext> methodAction)
        {
            if (_performedBinds.ContainsKey(inputAction)) return;
            _performedBinds.Add(inputAction, methodAction);
            
            inputAction.performed += _performedBinds[inputAction];
            inputAction.Enable();
        } 
        
        protected void DisableAction(InputAction inputAction)
        {
            if (!_performedBinds.ContainsKey(inputAction)) return;

            inputAction.Disable();
            inputAction.performed -= _performedBinds[inputAction];
            
            _performedBinds.Remove(inputAction);
        }

        public virtual void StartInput(TCharacter character)
        {
            if (isActive) return;
            
            this.character = character;
            isActive = true;
            
            StartInput();
        }

        public virtual void StopInput(TCharacter character)
        {
            if (!isActive || this.character != character) return;
            
            StopInput();
            
            isActive = false;
            this.character = null;
        }
        
        public abstract void UpdateInput(float deltaTime);
        protected abstract void StartInput();
        protected abstract void StopInput();
    }
}