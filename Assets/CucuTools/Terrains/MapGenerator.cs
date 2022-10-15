using System.IO;
using CucuTools.Attributes;
using UnityEngine;
//using File = UnityEngine.Windows.File;

namespace CucuTools.Terrains
{
    public class MapGenerator : CucuBehaviour
    {
        [Space]
        public Vector2Int Resolution = Vector2Int.one * 32;
        [Min(0f)]
        public Vector2 Size = Vector2.one;

        [Space]
        public bool UpdateAsset;
        public MapSeedAsset Map;
        public MapSeed Seed;
        public ColorPalette Palette;
        
        [Space]
        public bool AutoBuild = false;
        public MapConverter Converter;
        public MapDisplayer Displayer;

        public float[,] Generate()
        {
            return Map.Seed.GetMap(Resolution, Size);
        }

        [CucuButton()]
        public void Build()
        {
            var noiseMap = Generate();
            var colorMap = Converter.Convert(noiseMap, Resolution);
            Displayer.DisplayFromColorMap(colorMap, Resolution);
        }

        [CucuButton(colorHex:"#ff0000")]
        private void SaveTexture()
        {
            Build();
            
            var folder = Application.dataPath;
            var fileName = "map.png";
            var path = Path.Combine(folder, fileName);
            //File.WriteAllBytes(path, Displayer.Texture.EncodeToPNG());
        }
        
        private void OnValidate()
        {
            if (UpdateAsset)
            {
                Map.Seed = Seed;
                Converter.TerrainColors.Palette = Palette;

#if UNITY_EDITOR
                UnityEditor.EditorUtility.SetDirty(Converter.TerrainColors);
#endif
            }
            else
            {
                Seed = Map.Seed;
                Palette = Converter.TerrainColors.Palette;
            }

            
            if (AutoBuild) Build();
        }
    }
}