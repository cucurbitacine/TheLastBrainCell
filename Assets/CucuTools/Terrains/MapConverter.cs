using System;
using UnityEngine;

namespace CucuTools.Terrains
{
    public class MapConverter : MonoBehaviour
    {
        public DisplayType DisplayType;

        public TerrainColorsAsset TerrainColors;
        
        public Color[,] Convert(float[,] map, Vector2Int resolution)
        {
            var colorMap = new Color[resolution.x, resolution.y];
            
            if (DisplayType == DisplayType.ColorMap && TerrainColors != null)
            {
                colorMap = new Color[resolution.x,resolution.y];
                for (var i = 0; i < resolution.x; i++)
                {
                    for (var j = 0; j < resolution.y; j++)
                    {
                        colorMap[i, j] = TerrainColors.GetColor(map[i, j]);
                    }
                }
            }

            if (DisplayType == DisplayType.NoiseMap)
            {
                for (var i = 0; i < resolution.x; i++)
                {
                    for (var j = 0; j < resolution.y; j++)
                    {
                        colorMap[i, j] = Color.Lerp(Color.black, Color.white, map[i, j]);
                    }
                }
            }

            return colorMap;
        }
    }
    
    public enum DisplayType
    {
        NoiseMap,
        ColorMap,
    }
}