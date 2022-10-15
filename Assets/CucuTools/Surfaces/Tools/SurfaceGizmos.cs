using System;
using CucuTools.Colors;
using UnityEngine;

namespace CucuTools.Surfaces.Tools
{
    [Serializable]
    public class SurfaceGizmos
    {
        public bool Drawing = true;
        public bool OnlySelected = false;
        
        [Header("Grid")]
        [Range(SurfaceMesh.SizeMin, SurfaceMesh.SizeMax)]
        public int SizeU = SurfaceMesh.SizeDefault;
        [Range(SurfaceMesh.SizeMin, SurfaceMesh.SizeMax)]
        public int SizeV = SurfaceMesh.SizeDefault;

        [Header("Normal")]
        public bool ShowNormal;
        [Min(0)]
        public float NormalHeight = 0.1f;

        [Header("Colors")]
        public Color color00 = CucuColor.Color00;
        public Color color01 = CucuColor.Color01;
        public Color color11 = CucuColor.Color11;
        public Color color10 = CucuColor.Color10;
        
        private float[] gridU;
        private float[] gridV;
        
        public float[] GridU
        {
            get
            {
                if (gridU != null && gridU.Length == SizeU) return gridU;
                gridU = Cucu.LinSpace(SizeU);
                return gridU;
            }
        }

        public float[] GridV
        {
            get
            {
                if (gridV != null && gridV.Length == SizeV) return gridV;
                gridV = Cucu.LinSpace(SizeV);
                return gridV;
            }
        }

        public Vector2Int Size
        {
            get => new Vector2Int(SizeU, SizeV);
            set
            {
                SizeU = value.x;
                SizeV = value.y;
            }
        }

        public void Draw(SurfaceBehaviour surface)
        {
            if (surface == null) return;
            
            if(!Drawing) return;
            
            DrawSurface(surface);

            if (ShowNormal) DrawNormal(surface);

            Gizmos.color = Color.white;
        }

        public void DrawSurface(SurfaceBehaviour surface)
        {
            for (int i = 0; i < GridU.Length - 1; i++)
            {
                var u0 = GridU[i];
                var u1 = GridU[i + 1];
                
                for (int j = 0; j < GridV.Length - 1; j++)
                {
                    var v0 = GridV[j];
                    var v1 = GridV[j + 1];

                    //Gizmos.color = Color.Lerp(color00, color10, (u0 + u1) / 2);
                    Gizmos.color = CucuColor.ColorUV(new Vector2((u0 + u1) / 2, v0), color00, color10, color11, color01);
                    Gizmos.DrawLine(surface.GetPoint(u0, v0), surface.GetPoint(u1, v0));

                    //Gizmos.color = Color.Lerp(color01, color11, (u0 + u1) / 2);
                    Gizmos.color = CucuColor.ColorUV(new Vector2((u0 + u1) / 2, v1), color00, color10, color11, color01);
                    Gizmos.DrawLine(surface.GetPoint(u0, v1), surface.GetPoint(u1, v1));
                    
                    //Gizmos.color = Color.Lerp(color00, color01, (v0 + v1) / 2);
                    Gizmos.color = CucuColor.ColorUV(new Vector2(u0, (v0 + v1) / 2), color00, color10, color11, color01);
                    Gizmos.DrawLine(surface.GetPoint(u0, v0), surface.GetPoint(u0, v1));

                    //Gizmos.color = Color.Lerp(color10, color11, (v0 + v1) / 2);
                    Gizmos.color = CucuColor.ColorUV(new Vector2(u1, (v0 + v1) / 2), color00, color10, color11, color01);
                    Gizmos.DrawLine(surface.GetPoint(u1, v0), surface.GetPoint(u1, v1));
                }
            }
/*
            for (int j = 0; j < GridV.Length - 1; j++)
            {
                var v0 = GridV[j];
                var v1 = GridV[j + 1];
                
                Gizmos.color = Color.Lerp(color00, color01, v0);
                Gizmos.DrawLine(surface.GetPoint(0, v0), surface.GetPoint(0, v1));

                Gizmos.color = Color.Lerp(color10, color11, GridV[j]);
                Gizmos.DrawLine(surface.GetPoint(1, v0), surface.GetPoint(1, v1));
            }*/
        }

        public void DrawNormal(SurfaceBehaviour surface)
        {
            void _DrawNormal(Vector2 uv)
            {
                var point = surface.GetPoint(uv);
                var normal = surface.GetNormal(uv);

                Gizmos.color = GetUVColor(uv);
                Gizmos.DrawLine(point, point + normal * NormalHeight);
                Gizmos.color = Gizmos.color.AlphaTo(0.2f);
                Gizmos.DrawSphere(point, NormalHeight / 10);
            }

            var uv = Vector2.zero;
            for (int i = 0; i < GridU.Length; i++)
            {
                uv.x = GridU[i];
                for (int j = 0; j < GridV.Length; j++)
                {
                    uv.y = GridV[j];

                    _DrawNormal(uv);
                }
            }

            for (int i = 0; i < GridU.Length; i++)
            {
                uv.x = GridU[i];
                uv.y = 0;

                _DrawNormal(uv);

                uv.y = 1;

                _DrawNormal(uv);
            }

            for (int i = 0; i < GridV.Length; i++)
            {
                uv.y = GridV[i];
                uv.x = 0;

                _DrawNormal(uv);

                uv.x = 1;

                _DrawNormal(uv);
            }

            _DrawNormal(new Vector2(0, 0));
            _DrawNormal(new Vector2(0, 1));
            _DrawNormal(new Vector2(1, 1));
            _DrawNormal(new Vector2(1, 0));
        }

        public Color GetUVColor(Vector2 uv)
        {
            return CucuColor.ColorUV(uv, color00, color10, color11, color01);
        }
    }
}