using Cinemachine;
using Game.Characters.Player;
using UnityEngine;

namespace Game.Services
{
    public class CameraZone : MonoBehaviour
    {
        public bool isActive = true;
        
        [Space]
        public PlayerController player;

        [Space]
        public CinemachineBrain brain;
        public CinemachineVirtualCamera virtualCamera;

        public void On()
        {
            virtualCamera.enabled = true;
        }
        
        public void Off()
        {
            virtualCamera.enabled = false;
        }
        
        private void Awake()
        {
            if (brain == null) brain = FindObjectOfType<CinemachineBrain>();
            if (virtualCamera == null) virtualCamera = GetComponent<CinemachineVirtualCamera>();

            virtualCamera.enabled = false;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!isActive) return;
            
            if (player != null) return;

            player = other.GetComponent<PlayerController>();

            if (player == null) return;

            virtualCamera.enabled = true;
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!isActive) return;
            
            if (player == null) return;

            if (player != other.GetComponent<PlayerController>()) return;

            player = null;

            virtualCamera.enabled = false;
        }
    }
}