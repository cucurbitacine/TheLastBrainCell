using UnityEngine;

namespace Game.Effects.Audios
{
    public class AudioBg : SoundFX
    {
        public bool playOnAwake = true;
        public bool loop = true;
        
        [Space]
        public AudioClip clip;
        
        [Space]
        public AudioSource source;

        public override bool isPlaying => source;
        
        public override void Play()
        {
            source.clip = clip;
            source.Play();
        }

        public override void Stop()
        {
            source.Stop();
        }

        private void Awake()
        {
            if (playOnAwake) Play();
            source.loop = loop;
        }
    }
}