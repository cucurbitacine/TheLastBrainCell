using Game.Characters.Player;
using Game.Scores;
using UnityEngine;

namespace Game.Tools.ScrMsg
{
    public class ScoreScreenMessage : MonoBehaviour
    {
        public Color addScoreColor = Color.white;
        public Color removeScoreColor = Color.gray;
        
        [Space]
        public PlayerController player;
        public ScoreManager scoreManger;

        private void OnScoreAdded(ScoreEvent e)
        {
            var msg = string.Empty;
            var score = e.score;
            var color = Color.black;
            
            if (score > 0)
            {
                color = addScoreColor;
                msg = $"+{score}";
            }
            else if (score < 0)
            {
                color = removeScoreColor;
                msg = $"{score}";
            }

            var pos = player.position + Random.insideUnitCircle;
            
            ScreenMessage.Show(msg, pos, color);
        }
        
        private void Awake()
        {
            if (player == null) player = FindObjectOfType<PlayerController>();
            if (scoreManger == null) scoreManger = ScoreManager.Instance;
        }

        private void OnEnable()
        {
            scoreManger.events.onScoreAdded.AddListener(OnScoreAdded);
        }
        
        private void OnDisable()
        {
            scoreManger.events.onScoreAdded.RemoveListener(OnScoreAdded);
        }
    }
}