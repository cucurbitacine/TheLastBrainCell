using CucuTools.DamageSystem;
using Game.Characters;
using UnityEngine;

namespace Game.Scores.Handlers
{
    public class CharacterScoreHandler : MonoBehaviour
    {
        public ScoreTemplate damageScore;
        public ScoreTemplate deathScore;
        
        [Space]
        public CharacterControllerBase character;
        public ScoreManager scoreManager;
        
        private void OnDamageReceived(DamageEvent de)
        {
            if (de.receiver != character.DamageReceiver) return;
            
            if (de.damage.amount <= 0) return;
            
            if (damageScore == null) return;
            
            var se = new ScoreEvent
            {
                name = damageScore.scoreName,
                score = damageScore.score
            };

            scoreManager.AddScore(se);
        }
        
        private void OnDied()
        {
            if (deathScore == null) return;
            
            var e = new ScoreEvent
            {
                name = deathScore.scoreName,
                score = deathScore.score
            };

            scoreManager.AddScore(e);
        }
        
        private void Awake()
        {
            if (character == null) character = GetComponentInParent<CharacterControllerBase>();
            if (scoreManager == null) scoreManager = ScoreManager.Instance;
        }

        private void OnEnable()
        {
            character.DamageReceiver.OnDamageReceived.AddListener(OnDamageReceived);
            character.Health.Events.OnValueIsEmpty.AddListener(OnDied);
        }

        private void OnDisable()
        {
            character.DamageReceiver.OnDamageReceived.RemoveListener(OnDamageReceived);
            character.Health.Events.OnValueIsEmpty.RemoveListener(OnDied);
        }
    }
}
