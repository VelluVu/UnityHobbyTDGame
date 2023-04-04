namespace TheTD.DamageSystem
{
    public interface IDamageType
    {
        string Name { get; }
        Damage CalculateBaseDamage(Damage damage);
    }
}