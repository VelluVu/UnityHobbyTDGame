using UnityEngine;

namespace TheTD.DamageSystem
{
    public class BleedOvertimeEffect : IOvertimeEffect
    { 
        public int BaseValue { get; set; }
        public int OverAllDamage { get; set; }
        public int DamageTick { get; set; }
        public int NumberOfTicks { get => Mathf.RoundToInt(Duration / Interval); }
        public float Duration { get; set; }
        public float Interval { get; set; }
        public string Name => "Bleed";

        public BleedOvertimeEffect()
        {
            BaseValue = DamageTick * NumberOfTicks;
        }

        public void ModifyOvertimeEffect(IDamageModifier modifier)
        {
            if (!modifier.IsApplicableTo(this)) return;        
            OverAllDamage = Mathf.RoundToInt(DamageTick * NumberOfTicks);  
            OverAllDamage += modifier.FlatModifyValue;
            OverAllDamage += Mathf.RoundToInt(modifier.PercentualModifyValue * DamageTick);
            DamageTick = Mathf.RoundToInt(OverAllDamage / NumberOfTicks);
            //rethink this
        }
    }
}