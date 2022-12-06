using CucuTools.Attributes;
using UnityEngine;

namespace Game.Scores.Impl
{
    public class ScoreTemplateSource : ScoreSource
    {
        [CucuReadOnly]
        public string scoreName;
        
        [Space]
        public ScoreTemplate scoreTemplate;
        
        public override ScoreEvent CreateScore()
        {
            return new ScoreEvent()
            {
                name = scoreTemplate.scoreName,
                score = scoreTemplate.score,
            };
        }

        private void OnValidate()
        {
            scoreName = scoreTemplate?.scoreName ?? string.Empty;
        }
    }
}