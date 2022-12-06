using CucuTools.DamageSystem;
using UnityEngine;

namespace Game.Scores.Handlers
{
    public class DamageScoreHandler : MonoBehaviour
    {
        public ScoreTemplate damageScore;
        
        [Space]
        public DamageReceiver receiver;
        public ScoreManager scoreManager;

        private void OnDamageReceived(DamageEvent de)
        {
            if (de.receiver != receiver) return;

            if (de.damage.amount <= 0) return;
            
            var se = new ScoreEvent
            {
                name = damageScore.scoreName,
                score = damageScore.score
            };

            scoreManager.AddScore(se);
        }

        private void Awake()
        {
            if (receiver == null) receiver = GetComponentInParent<DamageReceiver>();
            if (scoreManager == null) scoreManager = ScoreManager.Instance;
        }

        private void OnEnable()
        {
            receiver.OnDamageReceived.AddListener(OnDamageReceived);
        }

        private void OnDisable()
        {
            receiver.OnDamageReceived.RemoveListener(OnDamageReceived);
        }
    }
}