using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Audios
{
    public class AudioSFX : MonoBehaviour
    {
        [Min(0)]
        public int indexClip = 0;

        public AudioSFXMode mode;
        
        [Space]
        public AudioClip[] clips;
        
        [Space]
        public AudioSource source;

        public void PlayOneShot(AudioClip clip)
        {
            if (source == null || clip == null) return;
            
            source.PlayOneShot(clip);
        }
        
        public void PlayOneShot()
        {
            if (clips == null) return;

            if (clips.Length == 0) return;

            if (indexClip < 0) indexClip = 0;
            if (indexClip >= clips.Length) indexClip = clips.Length - 1;
            
            switch (mode)
            {
                case AudioSFXMode.Current:
                    PlayOneShot(clips[indexClip]);
                    break;
                case AudioSFXMode.Queue:
                    PlayOneShot(clips[indexClip]);
                    indexClip++;
                    if (indexClip >= clips.Length) indexClip = 0;
                    break;
                case AudioSFXMode.Random:
                    indexClip = Random.Range(0, clips.Length);
                    PlayOneShot(clips[indexClip]);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
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