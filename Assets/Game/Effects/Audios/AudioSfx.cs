using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Effects.Audios
{
    public class AudioSfx : SoundFX
    {
        [Min(0)]
        public int indexClip = 0;

        public AudioSFXMode mode;
        
        [Space]
        public AudioClip[] clips;
        
        [Space]
        public AudioSource source;

        #region BaseFx

        public override bool isPlaying => source.isPlaying;
        
        public override void Play()
        {
            if (clips == null) return;

            if (clips.Length == 0) return;

            if (indexClip < 0) indexClip = 0;
            if (indexClip >= clips.Length) indexClip = clips.Length - 1;
            
            switch (mode)
            {
                case AudioSFXMode.Current:
                    Play(clips[indexClip]);
                    break;
                case AudioSFXMode.Queue:
                    Play(clips[indexClip]);
                    indexClip++;
                    if (indexClip >= clips.Length) indexClip = 0;
                    break;
                case AudioSFXMode.Random:
                    indexClip = Random.Range(0, clips.Length);
                    Play(clips[indexClip]);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override void Stop()
        {
            source.Stop();
        }

        #endregion
        
        public void Play(AudioClip clip)
        {
            if (source == null || clip == null) return;
            
            source.PlayOneShot(clip);
        }
        
        private void OnValidate()
        {
            if (clips != null) indexClip = Mathf.Clamp(indexClip, 0, clips.Length);
        }

        public enum AudioSFXMode
        {
            Current,
            Queue,
            Random
        }
    }
}