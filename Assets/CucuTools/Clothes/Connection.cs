using System;
using UnityEngine;

namespace CucuTools.Clothes
{
    [Serializable]
    public class Connection
    {
        public Point a;
        public Point b;
        public float length;

        public Connection(Point a, Point b)
        {
            this.a = a;
            this.b = b;

            length = Vector3.Distance(a.position, b.position);
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