using System;
using Game.Characters.Player;
using Game.Levels;
using Game.Scores;
using Game.Tools;
using TMPro;
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

        [Header("Score")]
        public ScoreManager scoreManager;
        [Space]
        public int totalScore;
        public TextMeshProUGUI scoreText;
        
        [Header("Timer")]
        public TimerController timerController;
        [Space]
        public TextMeshProUGUI timerText;

        private void ChangePauseState(bool paused)
        {
            gameplayPanel.SetActive(!paused);
            pausePanel.SetActive(paused);

            if (paused)
            {
                gameScene.player.GetComponentInChildren<PlayerInputController>().StopInput();
            }
            else
            {
                gameScene.player.GetComponentInChildren<PlayerInputController>().StartInput();
            }
            
#if !UNITY_EDITOR
            Cursor.lockState = CursorLockMode.Confined;
#else
            Cursor.lockState = CursorLockMode.None;
#endif
            Cursor.visible = true;
        }
        
        private void ChangeScore(int score)
        {
            totalScore = score;
            scoreText.text = $"{totalScore}";
        }
        
        public void Awake()
        {
            if (gameScene == null) gameScene = FindObjectOfType<GameSceneController>();
            if (gamePause == null) gamePause = FindObjectOfType<GamePauseController>();
            if (scoreManager == null) scoreManager = ScoreManager.Instance;
        }

        private void Start()
        {
            ChangePauseState(gamePause.paused);

            ChangeScore(scoreManager.totalScore);
        }

        private void OnEnable()
        {
            gamePause.onPauseStateChanged.AddListener(ChangePauseState);
            scoreManager.events.onScoreChanged.AddListener(ChangeScore);
        }

        private void OnDisable()
        {
            gamePause.onPauseStateChanged.RemoveListener(ChangePauseState);
            scoreManager.events.onScoreChanged.RemoveListener(ChangeScore);
        }

        private void Update()
        {
            timerText.text = TimeSpan.FromSeconds(timerController.seconds).ToString("g");
        }
    }
}
