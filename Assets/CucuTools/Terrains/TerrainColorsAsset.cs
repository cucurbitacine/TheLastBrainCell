using System;
using System.Linq;
using UnityEngine;

namespace CucuTools.Terrains
{
    [CreateAssetMenu(menuName = "CucuTools/Terrain/" + nameof(TerrainColorsAsset), fileName = nameof(TerrainColorsAsset))]
    public class TerrainColorsAsset : ScriptableObject
    {
        public ColorPalette Palette;

        public Color GetColor(float value)
        {
            var color = Color.black;
            
            var height = value;
            for (var k = 0; k < Palette.colors.Length; k++)
            {
                var array = Array.FindAll(Palette.colors, tt => tt.value <= height);
                if (array.Length > 0)
                {
                    var max = array.Select(a => a.value).Max();
                    var terrain = Array.Find(array, tt => Mathf.Abs(tt.value - max) < float.Epsilon);
                    color = terrain.color;
                }
            }

            return color;
        }
    }

    [Serializable]
    public struct ColorLayer
    {
        public string name;
        public float value;
        public Color color;
    }
    
    [Serializable]
    public struct ColorPalette
    {
        public ColorLayer[] colors;
    }
}