namespace TheTD.DamageSystem
{
    public interface IDamageModifier
    {
        bool IsApplicableTo(IDamageType damageType);
        bool IsApplicableTo(IOvertimeEffect overtimeEffect);
        int Order { get; }
        int FlatModifyValue { get; }
        float PercentualModifyValue { get; }
        string ApplicableDamageTypeName { get; }
        string ApplicableOvertimeEffectName { get; }
        ModifyStatType ApplicableModifyStatType { get; }
        Damage Modify(Damage damage);
    }
}
