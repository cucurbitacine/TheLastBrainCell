using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CucuTools.Attributes;
using UnityEngine;

namespace CucuTools.Blend
{
    public class CucuBlendComplex : CucuBlendEntity, IList<CucuBlendEntity>
    {
        [Header("Blends")]
        [SerializeField] private List<CucuBlendEntity> blends;
        
        public List<CucuBlendEntity> Blends => blends ?? (blends = new List<CucuBlendEntity>());

        private const string GroupName = "Complex";
        
        [CucuButton("Get Blends", group: GroupName)]
        private void GetBlends()
        {
            Blends.Clear();
            Blends.AddRange(GetComponents<CucuBlendEntity>().Where(b => b != this));
        }
        
        [CucuButton("Get Blends In Children", group: GroupName)]
        private void GetBlendsInChildren()
        {
            Blends.Clear();
            Blends.AddRange(GetComponentsInChildren<CucuBlendEntity>().Where(b => b != this));
        }
        
        protected override void UpdateEntityInternal()
        {
            foreach (var cucuBlend in Blends)
                if (cucuBlend != null)
                    cucuBlend.Blend = Blend;
        }

        protected override void OnValidate()
        {
            base.OnValidate();

            Blends.RemoveAll(b => b == this);
        }

        #region IList<CucuBlend>

        public int Count => Blends.Count;
        public bool IsReadOnly => false;

        public IEnumerator<CucuBlendEntity> GetEnumerator()
        {
            return Blends.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(CucuBlendEntity item)
        {
            Blends.Add(item);
            
            UpdateEntity();
        }

        public void Clear()
        {
            Blends.Clear();
        }

        public bool Contains(CucuBlendEntity item)
        {
            return Blends.Contains(item);
        }

        public void CopyTo(CucuBlendEntity[] array, int arrayIndex)
        {
            Blends.CopyTo(array, arrayIndex);
            
            UpdateEntity();
        }

        public bool Remove(CucuBlendEntity item)
        {
            if (!Blends.Remove(item)) return false;
            
            UpdateEntity();
            return true;
        }

        public int IndexOf(CucuBlendEntity item)
        {
            return Blends.IndexOf(item);
        }

        public void Insert(int index, CucuBlendEntity item)
        {
            Blends.Insert(index, item);
            
            UpdateEntity();
        }

        public void RemoveAt(int index)
        {
            Blends.RemoveAt(index);
            
            UpdateEntity();
        }

        public CucuBlendEntity this[int index]
        {
            get => Blends[index];
            set
            {
                Blends[index] = value;
                UpdateEntity();
            }
        }

        #endregion
    }
}