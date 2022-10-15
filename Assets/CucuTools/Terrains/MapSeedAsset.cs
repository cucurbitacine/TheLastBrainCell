using UnityEngine;

namespace CucuTools.Terrains
{
    [CreateAssetMenu(fileName = nameof(MapSeedAsset), menuName = "CucuTools/Terrain/Map Seed")]
    public class MapSeedAsset : ScriptableObject
    {
        public MapSeed Seed = MapSeed.Default;
    }
}