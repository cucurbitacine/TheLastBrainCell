using UnityEngine;

namespace Game.Effects.Visuals.Impl
{
    public class ParticleVfx : VisualFX
    {
        public ParticleSystem particle;

        public override bool isPlaying => particle.isPlaying;
        
        public override void Play()
        {
            particle.Play();
        }

        public override void Stop()
        {
            particle.Stop();
        }

        private void Awake()
        {
            if (particle == null) particle = GetComponentInChildren<ParticleSystem>();
        }
    }
}