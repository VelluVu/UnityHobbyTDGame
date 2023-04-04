using UnityEngine;

namespace TheTD.DamageSystem
{
    public class PhysicalDamageModifier : DamageModifier
    {
        public override int Order => 0;
        public override int FlatModifyValue { get; set; }
        public override float PercentualModifyValue { get; set; }
        public override string DamageTypeName => "Physical";
        public override string OvertimeEffectName => "Bleed";

        public override bool IsApplicableTo(IDamageType damageType)
        {
            return damageType.Name == DamageTypeName;
        }

        public override bool IsApplicableTo(IOvertimeEffect overtimeEffect)
        {
            return overtimeEffect.Name == OvertimeEffectName;
        }

        public override Damage ModifyDamage(Damage damage)
        {
            damage.Value += FlatModifyValue;
            damage.Value += Mathf.RoundToInt(damage.Value * PercentualModifyValue);
            return damage;
        }
    }
}