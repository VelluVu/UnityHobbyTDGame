namespace TheTD.DamageSystem
{
    public interface IDamageModifier
    {
        bool IsApplicableTo(IDamageType damageType);
        bool IsApplicableTo(IOvertimeEffect overtimeEffect);
        int Order { get; }
        int FlatValue { get; }
        float PercentualMultiplier { get; }
        string ApplicableDamageTypeName { get; }
        string ApplicableOvertimeEffectName { get; }
        ModifyStatType ApplicableModifyStatType { get; }
        void Modify(Damage damage);
    }
}
