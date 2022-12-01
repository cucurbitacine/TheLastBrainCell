using CucuTools;
using CucuTools.Attributes;

namespace Game.Dev.VisualEffects
{
    public abstract class BaseVfx : CucuBehaviour
    {
        public abstract bool isPlaying { get; }

        [CucuButton()]
        public abstract void Play();
        [CucuButton()]
        public abstract void Stop();
    }
}
