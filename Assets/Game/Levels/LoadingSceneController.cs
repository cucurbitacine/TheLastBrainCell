using System;
using System.Threading.Tasks;
using CucuTools.Injects;
using CucuTools.Scenes;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Levels
{
    [CucuSceneController("LoadingScene")]
    public class LoadingSceneController : CucuSceneController
    {
        [CucuArg] public LoadingArg loadingArg;

        public async void LoadScene()
        {
            Debug.Log($"Start Load [{loadingArg.nextSceneName}]");

            await Task.Delay(Random.Range(0, 2000) + 500);
            
            var loadingOperation = CucuSceneManager.LoadSingleSceneAsync(loadingArg.nextSceneName, loadingArg.args);

            loadingOperation.completed += loading =>
            {
                Debug.Log($"[{loadingArg.nextSceneName}] Was Loaded");
            };
        }

        private void Start()
        {
            if (loadingArg.IsDefault)
            {
                Debug.LogWarning("[Loading Scene] was launched manually");
            }
            else
            {
                LoadScene();
            }
        }
    }
    
    [Serializable]
    public class LoadingArg : CucuArg
    {
        public string previousSceneName;
        public string nextSceneName;

        public CucuArg[] args;
    }
}