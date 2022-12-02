using Game.Effects.Audios;
using UnityEngine;

namespace Game.Characters.Tools
{
    public class FootstepAudioSfx : MonoBehaviour
    {
        public AudioSfx sfx;
        public CharacterControllerBase character;

        [Space]
        public float timer = 0f;
        public float period = 1f;
        
        private void OnStep()
        {
            sfx.Play();
        }

        private void UpdateStep(float deltaTime)
        {
            if (character.CharacterInfo.isMoving && !character.CharacterInfo.isJumping)
            {
                if (timer <= 0f)
                {
                    OnStep();
                }
                
                timer += deltaTime;

                if (period <= timer)
                {
                    timer = 0f;
                }
            }
        }
        
        private void Awake()
        {
            if (sfx == null) sfx = GetComponentInParent<AudioSfx>();
            if (character == null) character = GetComponentInParent<CharacterControllerBase>();
        }

        private void Update()
        {
            UpdateStep(Time.deltaTime);
        }
    }
}