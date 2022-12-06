using System;

namespace Game.Scores
{
    [Serializable]
    public struct ScoreEvent
    {
        public string name;
        public int score;
    }
}