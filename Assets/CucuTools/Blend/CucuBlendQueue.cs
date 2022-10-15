using System;
using System.Collections;
using System.Collections.Generic;
using CucuTools.Attributes;
using UnityEngine;

namespace CucuTools.Blend
{
    public class CucuBlendQueue : CucuBlendEntity, IList<BlendUnit>
    {
        [Header("Queue")]
        [CucuReadOnly]
        [SerializeField] private float localBlend;
        [SerializeField] private List<BlendUnit> units;
        
        public List<BlendUnit> Units
        {
            get => units ?? (units = new List<BlendUnit>());
            private set => units = value;
        }

        public float LocalBlend
        {
            get => localBlend;
            private set => localBlend = value;
        }

        private float leftBlend;
        private float rightBlend;
        private int left;
        private int right;

        [CucuButton("Sort the List", group: "Queue")]
        public void Sort()
        {
            Units.Sort((a, b) => a.blend.CompareTo(b.blend));
        }

        protected override void UpdateEntityInternal()
        {
            Cucu.IndexesOfBorder(out left, out right, Blend, Units);

            if (0 <= left && left < Units.Count) leftBlend = Units[left].blend;
            else leftBlend = 0f;
            if (0 <= right && right < Units.Count) rightBlend = Units[right].blend;
            else rightBlend = 1f;

            if (Math.Abs(leftBlend - rightBlend) < 0.001f) LocalBlend = 0f;
            else LocalBlend = (Blend - leftBlend) / (rightBlend - leftBlend);

            if (left >= 0)
                if (Units[left].entity != null)
                    Units[left].entity.Blend = LocalBlend;

            for (var i = 0; i < Units.Count; i++)
            {
                if (Units[i].entity != null)
                {
                    var t = i.CompareTo(left);

                    Units[i].entity.Blend = 0.5f * (t * t - t) - LocalBlend * (t * t - 1);
                }
            }
        }

        private void Awake()
        {
            Sort();
        }

        protected override void OnValidate()
        {
            base.OnValidate();

            Units.RemoveAll(u => u.entity == this);

            foreach (var unit in Units)
            {
                unit.displayName = $"[ {unit.blend:F2}";
                unit.displayName += $" : {(unit.entity?.Blend ?? 0):F2} ]";
                unit.displayName += $" {(unit.entity?.name ?? "<empty>")}";
                unit.displayName += $" : {(unit.entity?.GetType().Name ?? "<unknown>")}";
            }
        }

        #region IList<CucuBlend>

        public int Count => Units.Count;
        public bool IsReadOnly => false;

        public IEnumerator<BlendUnit> GetEnumerator()
        {
            return Units.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(BlendUnit item)
        {
            Units.Add(item);
            
            Sort();

            UpdateEntity();
        }

        public void Clear()
        {
            Units.Clear();
        }

        public bool Contains(BlendUnit item)
        {
            return Units.Contains(item);
        }

        public void CopyTo(BlendUnit[] array, int arrayIndex)
        {
            Units.CopyTo(array, arrayIndex);

            Sort();
            
            UpdateEntity();
        }

        public bool Remove(BlendUnit item)
        {
            if (!Units.Remove(item)) return false;
            
            UpdateEntity();
            return true;
        }

        public int IndexOf(BlendUnit item)
        {
            return Units.IndexOf(item);
        }

        public void Insert(int index, BlendUnit item)
        {
            Units.Insert(index, item);
            
            Sort();
            
            UpdateEntity();
        }

        public void RemoveAt(int index)
        {
            Units.RemoveAt(index);
            
            UpdateEntity();
        }

        public BlendUnit this[int index]
        {
            get => Units[index];
            set
            {
                Units[index] = value;
                Sort();
                UpdateEntity();
            }
        }

        #endregion
    }
    
    [Serializable]
    public class BlendUnit : IComparable<float>
    {
        [HideInInspector] public string displayName;
        
        [Range(0f, 1f)]
        public float blend;
        public CucuBlendEntity entity;

        public int CompareTo(float other)
        {
            return blend.CompareTo(other);
        }
    }
}