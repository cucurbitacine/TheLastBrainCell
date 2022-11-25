using Game.Characters;
using UnityEngine;

namespace Game.Audios
{
    public class JumpAudioSFX : MonoBehaviour
    {
        public AudioSFX sfx;
        public CharacterControllerBase character;

        private void OnJumped()
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
            character.JumpSetting.onJumped.AddListener(OnJumped);
        }

        private void OnDisable()
        {
            character.JumpSetting.onJumped.RemoveListener(OnJumped);
        }
    }
}