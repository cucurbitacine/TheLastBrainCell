using Cinemachine;
using Game.Characters.Player;
using UnityEngine;

namespace Game.Dev
{
    public class CameraZone : MonoBehaviour
    {
        public PlayerController player;

        [Space]
        public CinemachineBrain brain;
        public CinemachineVirtualCamera virtualCamera;

        private ICinemachineCamera _previousCamera;
        private int _previousPriority;

        private void Awake()
        {
            if (brain == null) brain = FindObjectOfType<CinemachineBrain>();
            if (virtualCamera == null) virtualCamera = GetComponent<CinemachineVirtualCamera>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (player != null) return;

            player = other.GetComponent<PlayerController>();

            if (player == null) return;

            _previousCamera = brain.ActiveVirtualCamera;
            _previousPriority = _previousCamera.Priority;

            _previousCamera.Priority = 0;
            virtualCamera.Priority = _previousPriority;
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (player == null) return;

            if (player != other.GetComponent<PlayerController>()) return;

            player = null;

            virtualCamera.Priority = 0;

            _previousCamera.Priority = _previousPriority;
        }
    }
}