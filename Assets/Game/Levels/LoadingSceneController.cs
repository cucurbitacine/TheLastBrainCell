using System;
using System.Collections;
using CucuTools.Async;
using CucuTools.Attributes;
using CucuTools.Injects;
using CucuTools.Scenes;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Levels
{
    [CucuSceneController("LoadingScene")]
    public class LoadingSceneController : CucuSceneController
    {
        [CucuArg]
        public LoadingSceneArg loadingArg;

        [Space]
        [Min(0f)]
        public float minLoadingDuration = 1f;
        public Slider loadingSlider;
        
        public async void LoadScene()
        {
            Debug.Log($"Start Load [{loadingArg.sceneName}]");

            await LoadingSimulation(minLoadingDuration).ToTask();
            
            var loadingOperation = CucuSceneManager.LoadSingleSceneAsync(loadingArg.sceneName, loadingArg.sceneArgs);

            loadingOperation.completed += loading =>
            {
                Debug.Log($"[{loadingArg.sceneName}] Was Loaded");
            };
        }

        private IEnumerator LoadingSimulation(float duration)
        {
            loadingSlider.value = loadingSlider.minValue;
            
            var timer = 0f;
            while (timer < duration)
            {
                var t = timer / duration;

                loadingSlider.value = Mathf.Lerp(loadingSlider.minValue, loadingSlider.maxValue, t);
                
                timer += Time.deltaTime;
                yield return null;
            }

            loadingSlider.value = loadingSlider.maxValue;
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
    public class LoadingSceneArg : CucuArg
    {
        [CucuScene]
        public string sceneName;
        public CucuArg[] sceneArgs;
        
        public LoadingSceneArg(string sceneName, params CucuArg[] sceneArgs)
        {
            this.sceneName = sceneName;
            this.sceneArgs = sceneArgs;
        }
    }
}