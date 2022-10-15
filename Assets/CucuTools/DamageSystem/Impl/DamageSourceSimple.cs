namespace CucuTools.DamageSystem.Impl
{
    /// <inheritdoc />
    public class DamageSourceSimple : DamageSource
    {
        public DamageInfo DamageInfo;

        /// <inheritdoc />
        public override DamageInfo GenerateDamage()
        {
            return DamageInfo;
        }
    }
}