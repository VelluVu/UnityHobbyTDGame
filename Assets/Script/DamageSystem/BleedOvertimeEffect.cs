namespace TheTD.DamageSystem
{
    public class BleedOvertimeEffect : BaseOvertimeEffect
    {
        public override string Name => "Bleed";

        public BleedOvertimeEffect(DamageType type, float overallDamage, float duration, int ticks, float chance = 1) : base(type, overallDamage, duration, ticks, chance)
        {
        }

        public BleedOvertimeEffect(DamageType type, int tickDamage, int ticks, float duration, float chance = 1) : base(type, tickDamage, ticks, duration, chance)
        {
        }
    }
}