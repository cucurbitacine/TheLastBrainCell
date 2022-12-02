using CucuTools;
using CucuTools.Attributes;

namespace Game.Effects
{
    public abstract class BaseFx : CucuBehaviour
    {
        public const string title = "FX";
        
        public abstract bool isPlaying { get; }

        [CucuButton(group:"FX", colorHex:"39a845")]
        public abstract void Play();
        [CucuButton(group:"FX", colorHex:"9b1b30")]
        public abstract void Stop();
    }
}