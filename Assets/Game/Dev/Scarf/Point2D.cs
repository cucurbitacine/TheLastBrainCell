using System;
using UnityEngine;

namespace Game.Dev.Scarf
{
    [Serializable]
    public class Point2D
    {
        public bool locked;
        public Vector2 position;
        public Vector2 previousPosition;

        public Point2D(Vector2 position, bool locked = false)
        {
            this.locked = locked;
            this.position = position;
            this.previousPosition = position;
        }
    }
    
    [Serializable]
    public class Connection2D
    {
        public Point2D a;
        public Point2D b;
        public float length;

        public Connection2D(Point2D a, Point2D b)
        {
            this.a = a;
            this.b = b;

            length = Vector2.Distance(a.position, b.position);
        }

        public void Simulate()
        {
            var center = (a.position + b.position) * 0.5f;
            var dir = (a.position - b.position).normalized;

            if (!a.locked) a.position = center + dir * (length * 0.5f);
            if (!b.locked) b.position = center - dir * (length * 0.5f);
        }
    }
}
