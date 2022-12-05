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
        
        public abstract void EnableCharacter(TCharacter character);
        public abstract void DisableCharacter(TCharacter character);
        
        public abstract void UpdateInput(float deltaTime);
    }
}