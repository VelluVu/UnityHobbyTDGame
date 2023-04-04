namespace TheTD.DamageSystem
{
    public abstract class DamageModifier : IDamageModifier
    {
        public abstract int Order { get; }
        public abstract int FlatModifyValue { get; set; }
        public abstract float PercentualModifyValue { get; set ; }
        public abstract string DamageTypeName { get; }
        public abstract string OvertimeEffectName { get; }

        public abstract bool IsApplicableTo(IDamageType damageType);
        public abstract bool IsApplicableTo(IOvertimeEffect overtimeEffect);
        public abstract Damage ModifyDamage(Damage damage);
       
    }
}