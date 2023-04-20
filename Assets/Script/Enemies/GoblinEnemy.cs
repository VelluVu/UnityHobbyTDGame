using System.Collections.Generic;

namespace TheTD.Enemies
{
    public class GoblinEnemy : Enemy
    {
        public override List<IModifier> GetDefensiveModifiers()
        {
            var damageModifiers = new List<IModifier>();
            return damageModifiers;
        }
    }
}