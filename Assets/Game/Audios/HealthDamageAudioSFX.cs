using Game.Characters;
using UnityEngine;

namespace Game.Audios
{
    public class HealthDamageAudioSFX : MonoBehaviour
    {
        public AudioSFX sfx;
        public CharacterControllerBase character;

        private void OnHealthDamaged(int damage)
        {
            sfx.PlayOneShot();
        }
        
        private void Awake()
        {
            if (sfx == null) sfx = GetComponentInParent<AudioSFX>();
            if (character == null) character = GetComponentInParent<CharacterControllerBase>();
        }

        private void OnEnable()
        {
            character.Health.Events.OnValueChanged.AddListener(OnHealthDamaged);
        }

        private void OnDisable()
        {
            character.Health.Events.OnValueChanged.RemoveListener(OnHealthDamaged);
        }
    }
}