using Game.Levels;
using Game.Services;
using UnityEngine;

namespace Game.UI
{
    public class UIGameController : MonoBehaviour
    {
        public GameSceneController gameScene;
        public PauseController pauser;
        
        [Space]
        public GameObject gameplayPanel;
        public GameObject pausePanel;

        private void PauseStateChanged(bool paused)
        {
            gameplayPanel.SetActive(!paused);
            pausePanel.SetActive(paused);

            Cursor.lockState = paused ? CursorLockMode.Confined : CursorLockMode.Locked;
            Cursor.visible = paused;
        }

        public void Awake()
        {
            if (gameScene == null) gameScene = FindObjectOfType<GameSceneController>();
            if (pauser == null) pauser = FindObjectOfType<PauseController>();
        }

        private void Start()
        {
            PauseStateChanged(pauser.paused);
        }

        private void OnEnable()
        {
            pauser.onPauseStateChanged.AddListener(PauseStateChanged);
        }

        private void OnDisable()
        {
            pauser.onPauseStateChanged.RemoveListener(PauseStateChanged);
        }
    }
}
