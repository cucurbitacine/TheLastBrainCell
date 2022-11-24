using System;
using System.Collections.Generic;
using Game.Characters;
using UnityEngine;
using UnityEngine.InputSystem;
using CallbackContext = UnityEngine.InputSystem.InputAction.CallbackContext;

namespace Game.Inputs
{
    public abstract class InputController : MonoBehaviour
    {
        private readonly Dictionary<InputAction, Action<CallbackContext>> _performedBinds = new Dictionary<InputAction, Action<CallbackContext>>();

        protected void EnableAction(InputAction inputAction, Action<CallbackContext> methodAction)
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
    }
    
    public abstract class InputController<T> : InputController where T : CharacterControllerBase
    {
        [SerializeField] private T character = null;

        public T Character
        {
            get => character;
            set => character = value;
        }
    }
}