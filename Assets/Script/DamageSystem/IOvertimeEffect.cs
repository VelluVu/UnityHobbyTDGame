using TheTD.StatSystem;

namespace TheTD.DamageSystem
{
    public interface IOvertimeEffect
    {
        Stat Damage { get; }
        DamageType Type { get; }
        int TickDamage { get; }
        int NumberOfTicks { get; }
        float Chance { get; }
        float Duration { get; }
        float Interval { get; }
        string Name { get; }
    }
}