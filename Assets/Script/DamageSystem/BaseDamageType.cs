namespace TheTD.DamageSystem
{
    public abstract class BaseDamageType : IDamageType
    {
        public abstract string Name { get; }
        public abstract Damage CalculateBaseDamage(Damage damage);      
    }
}