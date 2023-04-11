namespace TheTD.DamageSystem
{
    public class PhysicalDamageType : BaseDamageType
    {
        public override string Name => "Physical";

        public override Damage CalculateBaseDamage(Damage damage)
        {
            return damage;
        }
    }
}