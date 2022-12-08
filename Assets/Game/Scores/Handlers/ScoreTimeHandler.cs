using System;
using CucuTools.Attributes;
using Game.Scores.Impl;
using Game.Tools;
using UnityEngine;

namespace Game.Scores.Handlers
{
    public class ScoreTimeHandler : MonoBehaviour
    {
        public float remainingSeconds;
        public float multiplyFactor;
            
        [Space]
        [CucuReadOnly]
        public string timeView;
        [Min(0f)]
        public float maxSeconds = 60f;
        public float minFactor = 1f;
        public float maxFactor = 3f;
        
        [Space]
        public ScoreMultiplier scoreMultiplier;
        public TimerController timerController;

        public float GetMultiplyFactor()
        {
            UpdateMultiplyFactor();
            
            return multiplyFactor;
        }
        
        public void Score()
        {
            scoreMultiplier.Score("Time", GetMultiplyFactor());
        }

        private void UpdateMultiplyFactor()
        {
            remainingSeconds = Mathf.Clamp(maxSeconds - timerController.seconds, 0f, maxSeconds);

            multiplyFactor = Mathf.Lerp(minFactor, maxFactor, remainingSeconds / maxSeconds);
        }
        
        private void Update()
        {
            UpdateMultiplyFactor();
        }

        private void OnValidate()
        {
            timeView = TimeSpan.FromSeconds(maxSeconds).ToString("g");
        }
    }
}
