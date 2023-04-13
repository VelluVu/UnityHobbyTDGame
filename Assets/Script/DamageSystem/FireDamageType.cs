using UnityEngine;

namespace TheTD.DamageSystem
{
    public class FireDamageType : BaseDamageType
    {
        public override string Name => "Fire";
        public override Color Color => Color.red;

        public override void CalculateBaseDamage(Damage damage)
        {

        }
    }
}