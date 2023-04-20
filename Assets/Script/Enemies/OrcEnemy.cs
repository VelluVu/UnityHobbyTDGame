using System.Collections.Generic;

namespace TheTD.Enemies
{
    public class OrcEnemy : Enemy
    {
        public override List<IModifier> GetDefensiveModifiers()
        {
            var defensiveModifiers = new List<IModifier>
            {
                new Modifier(0.25f, StatModifierType.PercentMultiply, StatFlags.Armor, this)
            };
            return defensiveModifiers;
        }
    }
}