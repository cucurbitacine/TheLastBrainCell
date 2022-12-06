using CucuTools.Attributes;
using CucuTools.Scenes;
using TMPro;
using UnityEngine;

namespace Game.Levels
{
    [CucuSceneController("MainMenuScene")]
    public class MainMenuSceneController : CucuSceneController
    {
        [CucuScene]
        public string gameScene;

        [Space]
        public TextMeshProUGUI bestScoreText;
        
        #region Public API

        [CucuButton("Start Game")]
        public void StartGame()
        {
            var menuSceneName = CucuSceneManager.GetActiveScene().name;

            var gameArg = new GameArg() { menuSceneName = menuSceneName };

            var loadingArg = new LoadingSceneArg(gameScene, gameArg);
            
            CucuSceneManager.LoadSingleScene<LoadingSceneController>(loadingArg);
        }

        [CucuButton("Exit Game")]
        public void ExitGame()
        {
            Application.Quit();
        }

        #endregion

        private void UpdateBestScore()
        {
            var key = ScoreInfoArg.BestScoreKeyName;
            var bestScore = PlayerPrefs.HasKey(key) ? PlayerPrefs.GetInt(key) : 0;

            bestScoreText.text = $"Best score: {bestScore}";
        }
        
        private void OnEnable()
        {
            Cursor.visible = true;
#if !UNITY_EDITOR
            Cursor.lockState = CursorLockMode.Confined;
#else
            Cursor.lockState = CursorLockMode.None;
#endif

            UpdateBestScore();
        }
        
        private void OnDisable()
        {
            
#if !UNITY_EDITOR
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
#else
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
#endif
        }
    }
}
