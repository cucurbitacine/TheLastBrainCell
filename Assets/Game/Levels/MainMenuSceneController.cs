using CucuTools.Attributes;
using CucuTools.Injects;
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
            var gameArg =  new GameArg() { menuSceneName = CucuSceneManager.GetActiveScene().name };

            var loadingArg = new LoadingArg();
            loadingArg.previousSceneName = CucuSceneManager.GetActiveScene().name;
            loadingArg.nextSceneName = gameScene;
            loadingArg.args = new CucuArg[] { gameArg };
            
            CucuSceneManager.LoadSingleScene<LoadingSceneController>(loadingArg);
        }

        [CucuButton("Exit Game")]
        public void ExitGame()
        {
            Application.Quit();
        }
    }
}
