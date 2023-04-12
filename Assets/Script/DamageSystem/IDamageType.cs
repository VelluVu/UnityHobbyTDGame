namespace TheTD.DamageSystem
{
    public interface IDamageType
    {
        string Name { get; }
        void CalculateBaseDamage(Damage damage);
    }
}