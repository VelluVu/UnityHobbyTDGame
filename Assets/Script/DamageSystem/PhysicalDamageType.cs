using UnityEngine;

namespace TheTD.DamageSystem
{
    public class PhysicalDamageType : BaseDamageType
    {
        public override string Name => "Physical";
        public override Color Color => Color.yellow;

        public override void CalculateBaseDamage(Damage damage)
        {
           
        }
    }
}