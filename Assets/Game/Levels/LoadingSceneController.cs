using System;
using System.Collections;
using CucuTools.Async;
using CucuTools.Attributes;
using CucuTools.Injects;
using CucuTools.Scenes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Levels
{
    [CucuSceneController("LoadingScene")]
    public class LoadingSceneController : CucuSceneController
    {
        [CucuArg]
        public LoadingSceneArg loadingArg;
        [CucuArg]
        public ScoreInfoArg scoreInfoArg;
        
        [Space]
        [Min(0f)]
        public float simpleLoadingDuration = 1f;
        public float longLoadingDuration = 3f;
        public Slider loadingSlider;

        [Space]
        public TextMeshProUGUI lastScoreText;
        public TextMeshProUGUI bestScoreText;
        
        public async void LoadScene()
        {
            Debug.Log($"Start Load [{loadingArg.sceneName}]");

            var duration = scoreInfoArg.IsDefault ? simpleLoadingDuration : longLoadingDuration;
            
            await LoadingSimulation(duration).ToTask();
            
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

        private void UpdateScore()
        {
            lastScoreText.enabled = !scoreInfoArg.IsDefault;
            bestScoreText.enabled = true;
            
            var key = ScoreInfoArg.BestScoreKeyName;
            var lastScore = scoreInfoArg.lastScore;
            var bestScore = PlayerPrefs.HasKey(key) ? PlayerPrefs.GetInt(key) : 0;

            lastScoreText.text = $"Last score: {lastScore}";
            bestScoreText.text = $"Best score: {bestScore}";
        }
        
        private void Start()
        {
            UpdateScore();
            
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

    [Serializable]
    public class ScoreInfoArg : CucuArg
    {
        public int lastScore;

        public static string BestScoreKeyName =>
            $"{Application.companyName}-{Application.productName}-{Application.version}-{nameof(BestScoreKeyName)}";
    }
}