using System;
using UnityEngine;

namespace CucuTools.Injects
{
    /// <summary>
    /// Some argument
    /// </summary>
    [Serializable]
    public abstract class CucuArg
    {
        public bool IsDefault
        {
            get => isDefault;
            set => isDefault = value;
        }

        [SerializeField] private bool isDefault = true;
    }
}