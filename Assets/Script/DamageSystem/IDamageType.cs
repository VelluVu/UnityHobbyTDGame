using UnityEngine;

namespace TheTD.DamageSystem
{
    public interface IDamageType
    {
        string Name { get; }
        public Color Color { get; }
        void CalculateBaseDamage(Damage damage);
    }
}