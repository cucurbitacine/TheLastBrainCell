using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Dev.VisualEffects
{
    public class VfxController : MonoBehaviour
    {
        public Dictionary<BaseVfx, List<BaseVfx>> vfxStorage = new Dictionary<BaseVfx, List<BaseVfx>>();

        public void Play(VfxTemplate template, Vector2? pos = null, Quaternion? rot = null)
        {
            if (template.vfxPrefab == null) return;
            
            if (!vfxStorage.TryGetValue(template.vfxPrefab, out var vfxList))
            {
                vfxList = new List<BaseVfx>();
                vfxStorage.Add(template.vfxPrefab, vfxList);
            }
            
            var vfx = vfxList.FirstOrDefault(v=>!v.isPlaying);

            if (vfx == null)
            {
                if (template.needParent)
                {
                    vfx = Instantiate(template.vfxPrefab, transform);
                }
                else
                {
                    vfx = Instantiate(template.vfxPrefab);
                }
                
                vfxList.Add(vfx);
            }

            if (pos != null) vfx.transform.position = pos.Value;
            if (rot != null) vfx.transform.rotation = rot.Value;
            
            vfx.Play();
        }
    }
}