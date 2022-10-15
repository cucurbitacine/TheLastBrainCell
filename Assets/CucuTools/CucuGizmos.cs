using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CucuTools
{
    public static class CucuGizmos
    {
        public static Color color
        {
            get => Gizmos.color;
            set => Gizmos.color = value;
        }
        
        public static void DrawLines(params Vector3[] corners)
        {
            if (corners == null || corners.Length < 2) return;

            for (int i = 0; i < corners.Length -1; i++)
            {
                Gizmos.DrawLine(corners[i], corners[i + 1]);
            }
        }

        public static void DrawLines(IEnumerable<Vector3> corners)
        {
            DrawLines(corners?.ToArray());
        }
        
        public static void DrawLinesLooped(params Vector3[] corners)
        {
            if (corners == null || corners.Length < 2) return;

            if (corners.Length == 2)
            {
                DrawLines(corners);
                return;
            }
            
            DrawLines(corners.Concat(new[] { corners[0] }));
        }

        public static void DrawLinesLooped(IEnumerable<Vector3> corners)
        {
            DrawLinesLooped(corners.ToArray());
        }
        
        public static void DrawWiredCube(Vector3 origin, Vector3 size, Quaternion? rotation = null)
        {
            if (rotation == null) rotation = Quaternion.identity;

            var a = -size * 0.5f;
            var b = a + Vector3.up * size.y;
            var c = b + Vector3.forward * size.z;
            var d = c - Vector3.up * size.y;
            var e = a + Vector3.right * size.x;
            var f = b + Vector3.right * size.x;
            var g = c + Vector3.right * size.x;
            var h = d + Vector3.right * size.x;

            a = rotation.Value * a + origin; 
            b = rotation.Value * b + origin; 
            c = rotation.Value * c + origin; 
            d = rotation.Value * d + origin; 
            e = rotation.Value * e + origin; 
            f = rotation.Value * f + origin; 
            g = rotation.Value * g + origin; 
            h = rotation.Value * h + origin; 
            
            DrawLinesLooped(a, b, c, d);
            DrawLinesLooped(e, f, g, h);
            DrawLines(a, e);
            DrawLines(b, f);
            DrawLines(c, g);
            DrawLines(d, h);
        }

        public static void DrawWiredCube2D(Vector2 origin, Vector2 size, float angle = 0f)
        {
            DrawWiredCube(origin, size, Quaternion.Euler(0f, 0f, angle));
        }
        
        public static void DrawCircle(Vector3 center, Vector3 normal, float radius = 1f, int resolution = 32)
        {
            var t = Cucu.LinSpace(resolution);

            var rot = Quaternion.FromToRotation(Vector3.up, normal);

            var points = new Vector3[t.Length];
            for (int i = 0; i < t.Length; i++)
            {
                var phi = t[i] * Mathf.PI * 2;
                points[i] = center + (rot * new Vector3(Mathf.Cos(phi), 0, Mathf.Sin(phi)) * radius);
            }

            DrawLines(points);
        }

        public static void DrawSpheres(float radius, params Vector3[] centers)
        {
            DrawSpheres(centers, radius);
        }
        
        public static void DrawSpheres(IEnumerable<Vector3> centers, float radius = 1f)
        {
            foreach (var center in centers)
            {
                Gizmos.DrawSphere(center, radius);
            }
        }
        
        public static void DrawWireSpheres(float radius, params Vector3[] centers)
        {
            DrawWireSpheres(centers, radius);
        }
        
        public static void DrawWireSpheres(IEnumerable<Vector3> centers, float radius = 1f)
        {
            foreach (var center in centers)
            {
                Gizmos.DrawWireSphere(center, radius);
            }
        }
    }
}