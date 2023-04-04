using System;
using System.Collections.Generic;

namespace TheTD.DamageSystem
{
    public interface IDamageable
    {
        event Action<IDamageable, Damage> OnTakeDamage;
        List<IDamageModifier> GetDamageModifiers();
        void TakeDamage(Damage damage);
    }
}