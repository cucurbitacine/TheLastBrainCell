using Game.Characters;
using UnityEngine;

namespace Game.Audios
{
    public class HealthDamageAudioSfx : MonoBehaviour
    {
        public AudioSfx sfx;
        public CharacterControllerBase character;

        private void OnHealthDamaged(int damage)
        {
            if (damage < 0) sfx.PlayOneShot();
        }
        
        private void Awake()
        {
            if (sfx == null) sfx = GetComponentInParent<AudioSfx>();
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