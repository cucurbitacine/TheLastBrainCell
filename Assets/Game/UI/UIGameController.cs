using Game.Levels;
using Game.Services;
using UnityEngine;

namespace Game.UI
{
    public class UIGameController : MonoBehaviour
    {
        public GameSceneController gameScene;
        public GamePauseController gamePause;
        
        [Space]
        public GameObject gameplayPanel;
        public GameObject pausePanel;

        private void PauseStateChanged(bool paused)
        {
            gameplayPanel.SetActive(!paused);
            pausePanel.SetActive(paused);

            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }

        public void Awake()
        {
            if (gameScene == null) gameScene = FindObjectOfType<GameSceneController>();
            if (gamePause == null) gamePause = FindObjectOfType<GamePauseController>();
        }

        private void Start()
        {
            PauseStateChanged(gamePause.paused);
        }

        private void OnEnable()
        {
            gamePause.onPauseStateChanged.AddListener(PauseStateChanged);
        }

        private void OnDisable()
        {
            gamePause.onPauseStateChanged.RemoveListener(PauseStateChanged);
        }
    }
}
