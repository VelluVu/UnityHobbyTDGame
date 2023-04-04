using UnityEngine;

namespace TheTD.DamageSystem
{
    public interface IOvertimeEffect
    {
        int BaseValue { get; set; }
        int OverAllDamage { get; set; }
        int DamageTick { get; set; }
        int NumberOfTicks { get; }
        float Duration { get; set; }
        float Interval { get; set; }
        string Name { get; }
        void ModifyOvertimeEffect(IDamageModifier modifier);
    }
}