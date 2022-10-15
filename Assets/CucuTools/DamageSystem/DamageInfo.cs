using System;

namespace CucuTools.DamageSystem
{
    /// <summary>
    /// Basic information about damage. Customizable
    /// </summary>
    [Serializable]
    public struct DamageInfo
    {
        public int amount;
        public DamageType type;
        public bool isCritical;
    }

    public enum DamageType
    {
        Physical,
        Fire,
        Water,
        Earth,
        Air,
        Lightning,
    }
}