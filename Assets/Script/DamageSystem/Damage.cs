using System;
using System.Collections.Generic;
using System.Linq;
using TheTD.StatSystem;
using UnityEngine;

namespace TheTD.DamageSystem
{
    public class Damage
    {
        public bool IsCritical { get; set; }
        public Stat DamageStat { get; set; }
        public Stat CriticalChance { get; set; }
        public Stat CriticalDamageMultiplier { get; set; }
        public Transform Source { get; set; }

        private List<IOvertimeEffect> _overtimeEffects; 
        public List<IOvertimeEffect> OvertimeEffects { get => _overtimeEffects = _overtimeEffects != null ? _overtimeEffects : new List<IOvertimeEffect>(); }

        private List<DamageType> _damageTypes;
        public List<DamageType> DamageTypes { get => _damageTypes = _damageTypes != null ? _damageTypes : new List<DamageType>(); }

        public Action<Damage> OnDamageCalculated;

        public Damage(Stat damageStat, Stat criticalChange, Stat criticalDamageMultiplier, DamageType damageType, List<IOvertimeEffect> overtimeEffects, Transform source)
        {
            DamageStat = damageStat;
            CriticalChance = criticalChange;
            CriticalDamageMultiplier = criticalDamageMultiplier;
            DamageTypes.Add(damageType);
            AddRangeOfDamageOvertimeEffects(overtimeEffects);
            Source = source;
        }

        public Damage(Stat damageStat, Stat criticalChange, Stat criticalDamageMultiplier, List<DamageType> damageTypes, List<IOvertimeEffect> overtimeEffects, Transform source)
        {
            DamageStat = damageStat;
            CriticalChance = criticalChange;
            CriticalDamageMultiplier = criticalDamageMultiplier;
            AddRangeOfDamageTypes(damageTypes);
            AddRangeOfDamageOvertimeEffects(overtimeEffects);
            Source = source;
        }

        public Damage(float damage, float criticalChance, float criticalDamageMultiplier, List<DamageType> damageTypes, List<IOvertimeEffect> overtimeEffects, Transform source)
        {
            DamageStat = new Stat(damage, StatFlags.Damage);
            CriticalChance = new Stat(criticalChance, StatFlags.CriticalChange);
            CriticalDamageMultiplier = new Stat(criticalDamageMultiplier, StatFlags.CriticalDamageMultiplier);
            AddRangeOfDamageTypes(damageTypes);
            AddRangeOfDamageOvertimeEffects(overtimeEffects);
            Source = source;
        }
        
        public Damage(Damage damage)
        {
            DamageStat = new Stat(damage.DamageStat);
            CriticalChance = new Stat(damage.CriticalChance);
            CriticalDamageMultiplier = new Stat(damage.CriticalDamageMultiplier);
            DamageTypes.AddRange(damage.DamageTypes);
            Source = damage.Source;
            AddRangeOfDamageOvertimeEffects(damage.OvertimeEffects);
        }

        private void AddRangeOfDamageOvertimeEffects(List<IOvertimeEffect> overtimeEffects)
        {
            if (overtimeEffects == null || !overtimeEffects.Any()) return;
            OvertimeEffects.AddRange(overtimeEffects);
        }

        private void AddRangeOfDamageTypes(List<DamageType> damageTypes)
        {
            if (damageTypes == null || !damageTypes.Any()) return;
            DamageTypes.AddRange(damageTypes);       
        }

        public void CalculateCritical(IRandomGenerator randomNumberGenerator)
        {
            IsCritical = randomNumberGenerator.RollPercent(CriticalChance.Value);
            if (!IsCritical) return;
            var criticalDamageIncrement = DamageStat.Value * CriticalDamageMultiplier.Value;
            DamageStat.BaseValue += criticalDamageIncrement >= 0.5f ? criticalDamageIncrement : 1f;
        }

        private void AddDamageTypeIfNotAlreadyInList(DamageType damageType)
        {
            if (DamageTypes.Contains(damageType)) return;
            DamageTypes.Add(damageType);
        }

        public void Add(Damage addedDamage)
        {
            DamageStat.BaseValue += addedDamage.DamageStat.Value;
            CriticalChance.BaseValue += addedDamage.CriticalChance.Value;
            CriticalDamageMultiplier.BaseValue += addedDamage.CriticalDamageMultiplier.Value;
            AddRangeOfDamageOvertimeEffects(addedDamage.OvertimeEffects);
            if (addedDamage.DamageTypes != null || addedDamage.DamageTypes.Any())
            {
                addedDamage.DamageTypes.ForEach(o => AddDamageTypeIfNotAlreadyInList(o));
            }
        }
    }
}