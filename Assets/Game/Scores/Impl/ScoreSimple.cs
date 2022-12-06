using UnityEngine;

namespace Game.Scores.Impl
{
    public class ScoreSimple : ScoreSource
    {
        [Space]
        public ScoreEvent scoreEvent;
        
        public override ScoreEvent CreateScore()
        {
            return scoreEvent;
        }
    }
}