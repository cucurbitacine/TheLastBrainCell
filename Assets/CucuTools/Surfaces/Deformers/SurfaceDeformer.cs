using System.Linq;
using UnityEngine;

namespace CucuTools.Surfaces.Deformers
{
    public abstract class SurfaceDeformer : SurfaceBehaviour
    {
        public SurfaceBehaviour Surface
        {
            get
            {
                if (surface != null) return surface;
                var surfaces = GetComponentsInChildren<SurfaceBehaviour>();
                surface = surfaces.FirstOrDefault(s => s != this);
                return surface;
            }
        }

        [SerializeField] private SurfaceBehaviour surface;
    }
}