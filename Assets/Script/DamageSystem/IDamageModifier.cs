namespace TheTD.DamageSystem
{
    public interface IDamageModifier
    {
        bool IsApplicableTo(IDamageType damageType);
        bool IsApplicableTo(IOvertimeEffect overtimeEffect);
        int Order { get; }
        int FlatModifyValue { get; set; }
        float PercentualModifyValue { get; set; }
        string DamageTypeName { get; }
        string OvertimeEffectName { get; }
        Damage ModifyDamage(Damage damage);
    }
}