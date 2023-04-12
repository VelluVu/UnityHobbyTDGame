namespace TheTD.DamageSystem
{
    public class PhysicalDamageModifier : BaseDamageModifier
    {
        public override int Order => 0;
        public override int FlatValue { get; set; }
        public override float PercentualMultiplier { get; set; }
        public override ModifyStatType ApplicableModifyStatType => ModifyStatType.Damage;

        public PhysicalDamageModifier(int flatModifyValue, float percentualModifyValue, string applicableDamageTypeName = "Physical", string applicableOvertimeEffectName = "None") : base(flatModifyValue, percentualModifyValue, applicableDamageTypeName, applicableOvertimeEffectName)
        {
        }
    }
}