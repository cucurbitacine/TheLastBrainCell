using CucuTools.Attributes;
using CucuTools.Scenes;
using UnityEngine;

namespace Game.Levels
{
    [CucuSceneController("MainMenuScene")]
    public class MainMenuSceneController : CucuSceneController
    {
        [CucuScene]
        public string gameScene;

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

        private void OnEnable()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
        }
        
        private void OnDisable()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
