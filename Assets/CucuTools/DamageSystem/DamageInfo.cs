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
        public CritInfo crit;
        public StunInfo stun;
    }

    [Serializable]
    public struct CritInfo
    {
        public bool isOn;
        public int amount;
    }
    
    [Serializable]
    public struct StunInfo
    {
        public bool isOn;
        public float duration;
        public float speedScale;
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