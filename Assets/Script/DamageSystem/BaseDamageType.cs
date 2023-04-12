namespace TheTD.DamageSystem
{
    public abstract class BaseDamageType : IDamageType
    {
        public abstract string Name { get; }
        public abstract void CalculateBaseDamage(Damage damage);      
    }
}