using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TheTD.DamageSystem
{
    public class Damage
    {
        public bool IsCritical { get; set; }
        public int Value { get; set; }
        public float CriticalChance { get; set; }
        public float CriticalDamageMultiplier { get; set; }
        public IDamageType DamageType { get; set; }
        public List<IOvertimeEffect> OvertimeEffects { get; set; }
        public List<IDamageModifier> Modifiers { get; set; }
        public Action<Damage> OnDamageCalculated;

        public Damage(int value, float criticalChange, float criticalDamageMultiplier, IDamageType damageType, List<IDamageModifier> modifiers, List<IOvertimeEffect> ovetimeEffects)
        {
            Value = value;
            CriticalChance = criticalChange;
            CriticalDamageMultiplier = criticalDamageMultiplier;
            DamageType = damageType;
            OvertimeEffects = ovetimeEffects != null ? OvertimeEffects : new List<IOvertimeEffect>();
            Modifiers = modifiers != null ? modifiers : new List<IDamageModifier>();
        }

        public Damage(DamageProperties properties)
        {
            Value = properties.baseDamage;
            CriticalChance = properties.criticalChange;
            CriticalDamageMultiplier = properties.criticalDamageMultiplier;
            DamageType = properties.damageType;
            OvertimeEffects = properties.overtimeEffects;
            Modifiers = properties.damageModifiers;
        }
        
        public void CalculateBase()
        {
            DamageType.CalculateBaseDamage(this);
        }

        public void ApplyModifiers(List<IDamageModifier> modifiers)
        {
            if (modifiers == null || !modifiers.Any()) return;
            modifiers.ForEach(o => o.Modify(this));
            if (OvertimeEffects == null || !OvertimeEffects.Any()) return;
            modifiers.ForEach(o => OvertimeEffects.ForEach(i => i.ModifyOvertimeEffect(o)));
        }

        public void CalculateCritical(SecureRandomNumberGenerator randomNumberGenerator)
        {
            IsCritical = randomNumberGenerator.RollPercent(CriticalChance);
            if (!IsCritical) return;
            Value += Mathf.RoundToInt(Value * CriticalDamageMultiplier);
        }
    }
}