using UnityEngine;

namespace TheTD.DamageSystem
{
    public abstract class BaseDamageModifier : IDamageModifier
    {
        public abstract int Order { get; }
        public abstract int FlatValue { get; set; }
        public abstract float PercentualMultiplier { get; set ; }
        public abstract ModifyStatType ApplicableModifyStatType { get; }

        private string _applicableDamageTypeName = "None";
        public virtual string ApplicableDamageTypeName { get => _applicableDamageTypeName; set => SetApplicableDamageTypeName(value); }

        private string _applicableOVertimeEffectName = "None";
        public virtual string ApplicableOvertimeEffectName { get => _applicableOVertimeEffectName; set => SetApplicableOvertimeEffectName(value); }
      
        public BaseDamageModifier(int flatModifyValue, float percentualModifyValue, string applicableDamageTypeName = "None", string applicableOvertimeEffectName = "None")
        {
            FlatValue = flatModifyValue;
            PercentualMultiplier = percentualModifyValue;
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

        public virtual void Modify(Damage damage) 
        {
            if (!IsApplicableTo(damage.DamageType)) return;
            ModifyCorrectStat(damage);     
        }

        private Damage ModifyCorrectStat(Damage damage)
        {
            switch (ApplicableModifyStatType)
            {
                case ModifyStatType.Damage: return ModifyDamageValue(damage);
                case ModifyStatType.Chance: return ModifyCritChance(damage);
                case ModifyStatType.CritDamage: return ModifyCritDamage(damage);
                default: return ModifyDamageValue(damage);
            }
        }

        private Damage ModifyCritDamage(Damage damage)
        {
            damage.CriticalDamageMultiplier += FlatValue * 0.01f;
            damage.CriticalDamageMultiplier += PercentualMultiplier;
            return damage;
        }

        private Damage ModifyCritChance(Damage damage)
        {
            damage.CriticalChance += FlatValue;
            damage.CriticalChance += damage.CriticalChance > 0 ? Mathf.RoundToInt(damage.CriticalChance * PercentualMultiplier) : 0;
            return damage;
        }

        public virtual Damage ModifyDamageValue(Damage damage)
        {
            damage.Value += FlatValue;
            damage.Value += damage.Value > 0 ? Mathf.RoundToInt(damage.Value * PercentualMultiplier) : 0;
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