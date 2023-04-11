using UnityEngine;

namespace TheTD.DamageSystem
{
    public abstract class BaseOvertimeEffect : IOvertimeEffect
    {
        public virtual int BaseValue { get; }

        public virtual int OverallDamage { get; private set; }

        public virtual int TickDamage { get; private set; }

        public virtual int NumberOfTicks { get; }

        public virtual float Duration { get; }

        public virtual float Interval { get; }

        public virtual float Chance { get; private set; }

        public virtual string Name { get => "None"; }

        public BaseOvertimeEffect(int overallDamage, float duration, int howManyTicks, float chance = 1f)
        {
            Duration = duration;
            NumberOfTicks = howManyTicks;
            OverallDamage = overallDamage;
            Chance = chance;
            Interval = Duration / NumberOfTicks;
            CalculateTickDamage();
            BaseValue = OverallDamage;
        }

        public BaseOvertimeEffect(int tickDamage, int ticks, float duration, float chance = 1f)
        {
            Duration = duration;
            NumberOfTicks = ticks;
            TickDamage = tickDamage;
            Chance = chance;
            Interval = Duration / NumberOfTicks;
            OverallDamage = TickDamage * NumberOfTicks;
            BaseValue = OverallDamage;
        }

        public virtual void ModifyOvertimeEffect(IDamageModifier modifier)
        {
            if (!modifier.IsApplicableTo(this)) return;
            ModifyCorrectStat(modifier);
            CalculateTickDamage();       
        }

        private void CalculateTickDamage()
        {
            TickDamage = Mathf.RoundToInt((float)OverallDamage / NumberOfTicks);
        }

        private void ModifyCorrectStat(IDamageModifier modifier)
        {
            switch (modifier.ApplicableModifyStatType)
            {
                case ModifyStatType.Damage:
                    ModifyDamage(modifier);
                    break;
                case ModifyStatType.Chance:
                    ModifyChance(modifier);
                    break;
            }
        }

        private void ModifyChance(IDamageModifier modifier)
        {
            Chance += modifier.FlatModifyValue;
            Chance += Chance > 0f ? (Chance * modifier.PercentualModifyValue) : 0f;
        }

        private void ModifyDamage(IDamageModifier modifier)
        {
            OverallDamage = Mathf.RoundToInt(TickDamage * NumberOfTicks);
            OverallDamage += modifier.FlatModifyValue;
            OverallDamage += OverallDamage > 0 ? Mathf.RoundToInt(modifier.PercentualModifyValue * OverallDamage) : 0;
        }
    }
}