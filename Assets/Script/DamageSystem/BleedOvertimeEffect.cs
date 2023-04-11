namespace TheTD.DamageSystem
{
    public class BleedOvertimeEffect : BaseOvertimeEffect
    {
        public override string Name => "Bleed";

        public BleedOvertimeEffect(int overallDamage, float duration, int howManyTicks) : base(overallDamage, duration, howManyTicks)
        {
        }

        public BleedOvertimeEffect(int tickDamage, int ticks, float duration) : base(tickDamage, ticks, duration)
        {
        }

    }
}