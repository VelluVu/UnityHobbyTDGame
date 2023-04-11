using System;
using System.Collections.Generic;

namespace TheTD.DamageSystem
{
    public class Damage
    {
        public bool IsCritical { get; set; }
        public int Value { get; set; }
        public float CriticalChance { get; set; }
        public float CriticalDamagePercent { get; set; }
        public IDamageType DamageType { get; set; }
        public List<IOvertimeEffect> OvertimeEffects { get; set; }
        public List<IDamageModifier> Modifiers { get; set; }
        public Action<Damage> OnDamageCalculated;

        public Damage(int value, float criticalChange, float criticalDamagePercent, IDamageType damageType, List<IDamageModifier> modifiers, List<IOvertimeEffect> ovetimeEffects)
        {
            Value = value;
            CriticalChance = criticalChange;
            CriticalDamagePercent = criticalDamagePercent;
            DamageType = damageType;
            OvertimeEffects = ovetimeEffects != null ? OvertimeEffects : new List<IOvertimeEffect>();
            Modifiers = modifiers != null ? modifiers : new List<IDamageModifier>();
        }

        public Damage(DamageProperties properties)
        {
            Value = properties.baseDamage;
            CriticalChance = properties.criticalChange;
            CriticalDamagePercent = properties.criticalDamagePercent;
            DamageType = properties.damageType;
            OvertimeEffects = properties.overtimeEffects;
            Modifiers = properties.damageModifiers;
        }
    }
}