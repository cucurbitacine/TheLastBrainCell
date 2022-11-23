using System;
using CucuTools.Attributes;
using CucuTools.Injects;
using CucuTools.Scenes;
using UnityEngine;

namespace Game.Levels
{
    [CucuSceneController("Dev_Base")]
    public class GameSceneController : CucuSceneController
    {
        [CucuArg] public GameArg gameArg;

        [CucuButton("Return To Menu")]
        public void ReturnToMenu()
        {
            var loadingArg = new LoadingArg();
            loadingArg.previousSceneName = CucuSceneManager.GetActiveScene().name;
            loadingArg.nextSceneName = gameArg.menuSceneName;
            
            CucuSceneManager.LoadSingleScene<LoadingSceneController>(loadingArg);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ReturnToMenu();
            }
        }
    }
    
    [Serializable]
    public class GameArg : CucuArg
    {
        [Space]
        public string menuSceneName;
    }
}