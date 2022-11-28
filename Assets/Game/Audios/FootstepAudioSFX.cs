﻿using Game.Characters;
using UnityEngine;

namespace Game.Audios
{
    public class FootstepAudioSFX : MonoBehaviour
    {
        public AudioSFX sfx;
        public CharacterControllerBase character;

        public float timer = 0f;
        public float period = 1f;
        
        private void OnStep()
        {
            sfx.PlayOneShot();
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
            if (sfx == null) sfx = GetComponentInParent<AudioSFX>();
            if (character == null) character = GetComponentInParent<CharacterControllerBase>();
        }

        private void Update()
        {
            UpdateStep(Time.deltaTime);
        }
    }
}