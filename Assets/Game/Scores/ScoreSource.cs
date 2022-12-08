using CucuTools;
using CucuTools.Attributes;
using UnityEngine;

namespace Game.Scores
{
    public abstract class ScoreSource : CucuBehaviour
    {
        [Space]
        public ScoreManager scoreManager;

        public abstract ScoreEvent CreateScore();
        
        [CucuButton()]
        public void Score()
        {
            scoreManager.AddScore(CreateScore());
        }
        
        protected virtual void Awake()
        {
            if (scoreManager == null) scoreManager = ScoreManager.Instance;
        }
    }
}
