namespace TheTD.DamageSystem
{
    public class PhysicalDamageType : BaseDamageType
    {
        public override string Name => throw new System.NotImplementedException();

        public override Damage CalculateBaseDamage(Damage damage)
        {
            return damage;
        }
    }
}