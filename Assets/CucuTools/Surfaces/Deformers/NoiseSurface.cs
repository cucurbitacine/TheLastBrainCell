using UnityEngine;

namespace CucuTools.Surfaces.Deformers
{
    public class NoiseSurface : SurfaceDeformer
    {
        public float MinValue = -0.1f;
        public float MaxValue = 0.1f;

        public bool UseNoiseMap = false;
        public Texture2D Texture;

        private MeshRenderer MeshRenderer;
        
        public override Vector3 GetLocalPoint(Vector2 uv)
        {
            if (Surface == null) return Vector3.zero;
            
            var point = Surface.GetPoint(uv).ToLocalPoint(Root);
            var normal = GetLocalNormal(uv);

            var t = Random.value;

            if (UseNoiseMap)
            {
                var texture = Texture;
                
                if (texture != null && texture.isReadable)
                {
                    var u = (int)(texture.width * uv.x);
                    var v = (int)(texture.height * uv.y);

                    var pixel = texture.GetPixel(u, v);
                    t = (pixel.r + pixel.g + pixel.b) / 3;
                }
            }
            
            var dist = Mathf.Lerp(MinValue, MaxValue, t);
            return point + normal * dist;
        }

        public override Vector3 GetLocalNormal(Vector2 uv)
        {
            if (Surface == null) return Vector3.zero;

            return Surface.GetNormal(uv).ToLocalDirection(Root);
        }
    }
}