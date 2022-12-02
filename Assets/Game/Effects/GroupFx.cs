using System.Collections.Generic;
using System.Linq;
using Game.Effects.Visuals;

namespace Game.Effects
{
    public class GroupFx : VisualFX
    {
        public List<BaseFx> vfxList = new List<BaseFx>();

        public override bool isPlaying => vfxList.Any(vfx => vfx.isPlaying);
        
        public override void Play()
        {
            vfxList.ForEach(vfx => vfx.Play());
        }

        public override void Stop()
        {
            vfxList.ForEach(vfx => vfx.Stop());
        }
    }
}