using Game.Characters;
using UnityEngine;

namespace Game.Audios
{
    public class AttackAudioSFX : MonoBehaviour
    {
        public AudioSFX sfx;
        public CharacterControllerBase character;

        private void OnAttacked()
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
            character.AttackSetting.onAttacked.AddListener(OnAttacked);
        }

        private void OnDisable()
        {
            character.AttackSetting.onAttacked.RemoveListener(OnAttacked);
        }
    }
}
