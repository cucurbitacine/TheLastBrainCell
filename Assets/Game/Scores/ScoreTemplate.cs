using System;
using CucuTools.Attributes;
using UnityEngine;

namespace Game.Scores
{
    [CreateAssetMenu(menuName = "Create ScoreTemplate", fileName = "ScoreTemplate", order = 0)]
    public class ScoreTemplate : ScriptableObject
    {
        [CucuReadOnly]
        public string scoreId;
        [Space]
        public string scoreName = string.Empty;
        [Space]
        public int score = 0;

        [ContextMenu("Refresh Id")]
        private void RefreshId()
        {
            scoreId = Guid.NewGuid().ToString();
        }
        
        private void ValidateId()
        {
            if(!Guid.TryParse(scoreId, out _))
            {
                RefreshId();
            }
        }

        private void OnEnable()
        {
            ValidateId();
        }

        private void Reset()
        {
            ValidateId();
        }
        
        private void OnValidate()
        {
            ValidateId();
        }
    }
}