namespace CucuTools.DamageSystem.Impl
{
    /// <summary>
    /// Notificating about damage events
    /// </summary>
    public static class DamageEventNotificator
    {
        public delegate void DamageEventHandle(DamageEvent e);
        public static event DamageEventHandle OnDamageEvent;
        
        public static void Notify(DamageEvent e)
        {
            OnDamageEvent?.Invoke(e);
        }
    }
}