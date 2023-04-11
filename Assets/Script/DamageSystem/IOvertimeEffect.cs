using UnityEngine;

namespace TheTD.DamageSystem
{
    public interface IOvertimeEffect
    {
        int BaseValue { get; }
        int OverallDamage { get; }
        int TickDamage { get; }
        int NumberOfTicks { get; }
        float Chance { get; }
        float Duration { get; }
        float Interval { get; }
        string Name { get; }
        void ModifyOvertimeEffect(IDamageModifier modifier);
    }
}