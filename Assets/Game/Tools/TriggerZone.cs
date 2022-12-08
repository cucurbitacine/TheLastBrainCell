using Game.Characters.Player;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Tools
{
    public class TriggerZone : MonoBehaviour
    {
        public bool isActive = true;

        [Space]
        public bool invokeOnce = true;
        
        [Space]
        public UnityEvent onIn;
        public UnityEvent onOut;
        
        private PlayerController _player;

        private bool _invoked;
        
        private void In()
        {
            if (invokeOnce)
            {
                if(_invoked) return;
                _invoked = true;
            }
            
            onIn.Invoke();
        }
        
        private void Out()
        {
            if (invokeOnce && _invoked) return;
            
            onOut.Invoke();
        }

        private void Validate()
        {
            
        }
        
        private void Awake()
        {
            Validate();
            
            _invoked = false;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!isActive) return;
            
            if (_player != null) return;

            _player = other.GetComponent<PlayerController>();

            if (_player == null) return;

            In();
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!isActive) return;
            
            if (_player == null) return;

            if (_player != other.GetComponent<PlayerController>()) return;

            _player = null;

            Out();
        }

        private void OnValidate()
        {
            Validate();
        }
    }
}