using UnityEngine;

namespace CucuTools.Terrains
{
    public class MapDisplayer : MonoBehaviour
    {
        public Texture2D Texture = default;
        public Renderer Renderer = default;

        public void DisplayFromColors(Color[] colors, Vector2Int resolution)
        {
            if (Renderer == null) return;
            var material = Renderer.sharedMaterial;

            if (Texture == null) Texture = new Texture2D(resolution.x, resolution.y);
        
            Texture.Reinitialize(resolution.x, resolution.y);
            Texture.SetPixels(colors);
            Texture.filterMode = FilterMode.Point;
            Texture.wrapMode = TextureWrapMode.Clamp;
            Texture.Apply();

            material.mainTexture = Texture;
        }
    
        public void DisplayFromColorMap(Color[,] map, Vector2Int resolution)
        {
            var colors = new Color[resolution.x * resolution.y];
            for (var j = 0; j < resolution.y; j++)
            {
                var i0 = j * resolution.x;
                for (var i = 0; i < resolution.x; i++)
                {
                    colors[i0 + i] = map[i, j];
                }
            }
        
            DisplayFromColors(colors, resolution);
        }
    
        public void DisplayFromNoiseMap(float[,] map, Vector2Int resolution)
        {
            var colorMap = new Color[resolution.x, resolution.y];
            for (var i = 0; i < resolution.x; i++)
            {
                for (var j = 0; j < resolution.y; j++)
                {
                    colorMap[i, j] = Color.Lerp(Color.black, Color.white, map[i, j]);
                }
            }

            DisplayFromColorMap(colorMap, resolution);
        }
    }
}