using UnityEngine;

namespace Game.Scores.Impl
{
    public class ScoreMultiplier : ScoreSource
    {
        [Space]
        public string scoreName = "Multiply";
        
        [Space]
        public float multiplyFactor = 2f;
        
        public override ScoreEvent CreateScore()
        {
            var addScore = (int)(scoreManager.totalScore * multiplyFactor) - scoreManager.totalScore;

            return new ScoreEvent()
            {
                name = scoreName,
                score = addScore,
            };
        }

        public void Score(float anotherMultiplyFactor)
        {
            var previous = multiplyFactor;
            multiplyFactor = anotherMultiplyFactor;
            
            Score();
            
            multiplyFactor = previous;
        }

        public void Score(string anotherScoreName)
        {
            var previous = scoreName;
            scoreName = anotherScoreName;
            
            Score();
            
            scoreName = previous;
        }
        
        public void Score(string anotherScoreName, float anotherMultiplyFactor)
        {
            var previousScoreName = scoreName;
            var previousMultiplyFactor = multiplyFactor;
            scoreName = anotherScoreName;
            multiplyFactor = anotherMultiplyFactor;
            
            Score();
            
            scoreName = previousScoreName;
            multiplyFactor = previousMultiplyFactor;
        }
    }
}