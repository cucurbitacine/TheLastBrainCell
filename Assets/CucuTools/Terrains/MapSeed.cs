using System;
using UnityEngine;

namespace CucuTools.Terrains
{
    [Serializable]
    public struct MapSeed
    {
        public static MapSeed Default =>
            new MapSeed
            {
                frequency = 0.125f,
                amplitude = 1,
                lacunarity = 2,
                persistance = 0.5f,
                offset = Vector2.zero,
                octaveOffset = new Vector2[] {Vector2.zero,}
            };
    
        [Min(0f)]
        public float frequency;
        [Min(0f)]
        public float amplitude;
        [Min(0f)]
        public float lacunarity;
        [Min(0f)]
        public float persistance;
        public Vector2 offset;
        public Vector2[] octaveOffset;
    
        public float[,] GetMap(Vector2Int resolution, Vector2 size)
        {
            resolution.x = Mathf.Max(2, resolution.x);
            resolution.y = Mathf.Max(2, resolution.y);

            size.x = Mathf.Max(0f, size.x);
            size.y = Mathf.Max(0f, size.y);

            var map = new float[resolution.x, resolution.y];

            var start = -0.5f * size;
            var step = new Vector2(size.x / (resolution.x - 1), size.y / (resolution.y - 1));
            
            for (var i = 0; i < resolution.x; i++)
            {
                for (var j = 0; j < resolution.y; j++)
                {
                    var point = start + Vector2.Scale(step, new Vector2(i, j));
                    map[i, j] = Noise(point);
                }
            }

            return map;
        }
    
        public float Noise(Vector2 position)
        {
            var height = 0f;
            for (var i = 0; i < octaveOffset.Length; i++)
            {
                var fr = Mathf.Pow(lacunarity, i);
                var am = Mathf.Pow(persistance, i);
                var point = (offset + position + octaveOffset[i]) * (fr * frequency);

                height += am * Mathf.PerlinNoise(point.x, point.y);
            }

            return amplitude * height;
        }
    }
}