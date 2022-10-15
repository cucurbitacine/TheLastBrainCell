using System;
using UnityEngine;

namespace CucuTools.Clothes
{
    [Serializable]
    public class Point
    {
        public bool locked;
        public Vector3 position;
        public Vector3 previousPosition;

        public Point(Vector3 position, bool locked = false)
        {
            this.locked = locked;
            this.position = position;
            this.previousPosition = position;
        }
    }
}