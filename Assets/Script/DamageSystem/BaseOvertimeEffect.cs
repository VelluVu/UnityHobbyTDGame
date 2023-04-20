using TheTD.StatSystem;
using UnityEngine;

namespace TheTD.DamageSystem
{
    public abstract class BaseOvertimeEffect : IOvertimeEffect
    {
        public virtual Stat Damage { get; }

        public virtual int TickDamage { get; private set; }

        public virtual int NumberOfTicks { get; }

        public virtual float Duration { get; }

        public virtual float Interval { get; }

        public virtual float Chance { get; private set; }

        public virtual string Name { get => "None"; }

        protected DamageType _type;
        public virtual DamageType Type { get => _type; private set => _type = value; }

        public BaseOvertimeEffect(DamageType type, float overallDamage, float duration, int ticks, float chance = 1f)
        {
            Type = type;
            Duration = duration;
            NumberOfTicks = ticks;
            Damage = new Stat(overallDamage, StatFlags.Damage);
            Chance = chance;
            Interval = Duration / NumberOfTicks;
            CalculateTickDamage();
        }

        public BaseOvertimeEffect(DamageType type, int tickDamage, int ticks, float duration, float chance = 1f)
        {
            Type = type;
            Duration = duration;
            NumberOfTicks = ticks;
            TickDamage = tickDamage;
            Chance = chance;
            Interval = Duration / NumberOfTicks;
            Damage = new Stat(TickDamage * NumberOfTicks, StatFlags.Damage);
        }

        private void CalculateTickDamage()
        {
            TickDamage = Mathf.FloorToInt((float)Damage.Value / NumberOfTicks);

            if (TickDamage <= 0)
            {
                TickDamage = 1;
            }
        }
    }
}