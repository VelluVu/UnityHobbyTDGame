using UnityEngine;

namespace TheTD.DamageSystem
{
    public abstract class BaseDamageModifier : IDamageModifier
    {
        public abstract int Order { get; }
        public abstract int FlatModifyValue { get; set; }
        public abstract float PercentualModifyValue { get; set ; }
        public abstract ModifyStatType ApplicableModifyStatType { get; }

        private string _applicableDamageTypeName = "None";
        public virtual string ApplicableDamageTypeName { get => _applicableDamageTypeName; set => SetApplicableDamageTypeName(value); }

        private string _applicableOVertimeEffectName = "None";
        public virtual string ApplicableOvertimeEffectName { get => _applicableOVertimeEffectName; set => SetApplicableOvertimeEffectName(value); }
      
        public BaseDamageModifier(int flatModifyValue, float percentualModifyValue, string applicableDamageTypeName = "None", string applicableOvertimeEffectName = "None")
        {
            FlatModifyValue = flatModifyValue;
            PercentualModifyValue = percentualModifyValue;
            ApplicableDamageTypeName = applicableDamageTypeName;
            ApplicableOvertimeEffectName = applicableOvertimeEffectName;
        }

        public virtual bool IsApplicableTo(IDamageType damageType)
        {
            return damageType.Name == ApplicableDamageTypeName;
        }

        public virtual bool IsApplicableTo(IOvertimeEffect overtimeEffect)
        {
            return overtimeEffect.Name == ApplicableOvertimeEffectName;
        }

        public virtual Damage Modify(Damage damage) 
        {
            if (!IsApplicableTo(damage.DamageType)) return damage;
            ModifyCorrectStat(damage);
            return damage;
        }

        private Damage ModifyCorrectStat(Damage damage)
        {
            switch (ApplicableModifyStatType)
            {
                case ModifyStatType.Damage: return ModifyDamage(damage);
                case ModifyStatType.Chance: return ModifyCritChance(damage);
                case ModifyStatType.CritDamage: return ModifyCritDamage(damage);
                default: return ModifyDamage(damage);
            }
        }

        private Damage ModifyCritDamage(Damage damage)
        {
            damage.CriticalDamagePercent += FlatModifyValue;
            damage.CriticalDamagePercent += damage.CriticalDamagePercent > 0 ? (damage.CriticalDamagePercent * PercentualModifyValue) : 0f;
            return damage;
        }

        private Damage ModifyCritChance(Damage damage)
        {
            damage.CriticalChance += FlatModifyValue;
            damage.CriticalChance += damage.CriticalChance > 0 ? (damage.CriticalChance * PercentualModifyValue) : 0f;
            return damage;
        }

        public virtual Damage ModifyDamage(Damage damage)
        {
            damage.Value += FlatModifyValue;
            damage.Value += damage.Value > 0 ? Mathf.RoundToInt(damage.Value * PercentualModifyValue) : 0;
            return damage;
        }

        private void SetApplicableDamageTypeName(string value)
        {
            ApplicableDamageTypeName = value;
        }

        private void SetApplicableOvertimeEffectName(string value)
        {
            ApplicableOvertimeEffectName = value;
        }
    }
}