using Cinemachine;
using Game.Characters.Player;
using UnityEngine;

namespace Game.Tools
{
    public class CameraZone : MonoBehaviour
    {
        public bool isActive = true;
        
        [Space]
        public CinemachineVirtualCamera vcam;
        public float vcamSize = 0f;
        public Vector2 vcamPosition = Vector2.zero;
        public float vcamDepth = -10f;
        
        [Space]
        public PlayerController player;
        public CinemachineBrain brain;

        public void On()
        {
            vcam.enabled = true;
        }
        
        public void Off()
        {
            vcam.enabled = false;
        }

        private void Validate()
        {
            if (brain == null) brain = FindObjectOfType<CinemachineBrain>();
            if (vcam == null)
            {
                vcam = GetComponentInChildren<CinemachineVirtualCamera>();
                vcamSize = vcam.m_Lens.OrthographicSize;
                vcamPosition = vcam.transform.position;
            }
        }
        
        private void Awake()
        {
            Validate();
            
            vcam.enabled = false;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!isActive) return;
            
            if (player != null) return;

            player = other.GetComponent<PlayerController>();

            if (player == null) return;

            On();
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!isActive) return;
            
            if (player == null) return;

            if (player != other.GetComponent<PlayerController>()) return;

            player = null;

            Off();
        }

        private void OnValidate()
        {
            Validate();

            vcam.m_Lens.OrthographicSize = vcamSize;
            vcam.transform.position = (Vector3)vcamPosition + Vector3.forward * vcamDepth;
        }

        private void OnDrawGizmosSelected()
        {
            if (vcam != null)
            {
                vcam.enabled = true;
                
                var height = 2f * vcam.m_Lens.OrthographicSize;
                var width = height * vcam.m_Lens.Aspect;
                
                var currentResolution = new Vector2(width, height);
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireCube(vcam.transform.position, currentResolution);
                
                vcam.enabled = false;
            }
        }
    }
}