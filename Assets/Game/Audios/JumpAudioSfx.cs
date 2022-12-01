using Game.Characters;
using UnityEngine;

namespace Game.Audios
{
    public class JumpAudioSfx : MonoBehaviour
    {
        public AudioSfx sfx;
        public CharacterControllerBase character;

        private void OnJumped()
        {
            sfx.PlayOneShot();
        }
        
        private void Awake()
        {
            if (sfx == null) sfx = GetComponentInParent<AudioSfx>();
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