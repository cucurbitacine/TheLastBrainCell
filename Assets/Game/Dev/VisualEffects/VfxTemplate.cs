using UnityEngine;

namespace Game.Dev.VisualEffects
{
    [CreateAssetMenu(menuName = "Create VfxTemplate", fileName = "VfxTemplate", order = 0)]
    public class VfxTemplate : ScriptableObject
    {
        public BaseVfx vfxPrefab;

        [Space]
        public bool needParent;
    }
}