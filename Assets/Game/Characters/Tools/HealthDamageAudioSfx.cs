using Game.Effects.Audios;
using UnityEngine;

namespace Game.Characters.Tools
{
    public class HealthDamageAudioSfx : MonoBehaviour
    {
        public AudioSfx sfx;
        public CharacterControllerBase character;

        private void OnHealthDamaged(int damage)
        {
            if (damage < 0) sfx.Play();
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