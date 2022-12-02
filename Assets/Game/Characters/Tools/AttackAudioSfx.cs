using Game.Effects.Audios;
using UnityEngine;

namespace Game.Characters.Tools
{
    public class AttackAudioSfx : MonoBehaviour
    {
        public AudioSfx sfx;
        public CharacterControllerBase character;

        private void OnAttacked()
        {
            sfx.Play();
        }
        
        private void Awake()
        {
            if (sfx == null) sfx = GetComponentInParent<AudioSfx>();
            if (character == null) character = GetComponentInParent<CharacterControllerBase>();
        }

        private void OnEnable()
        {
            character.AttackSetting.onAttacked.AddListener(OnAttacked);
        }

        private void OnDisable()
        {
            character.AttackSetting.onAttacked.RemoveListener(OnAttacked);
        }
    }
}