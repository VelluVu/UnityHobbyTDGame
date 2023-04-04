using System.Collections.Generic;
using System.Linq;

namespace TheTD.DamageSystem
{
    public class DamageCalculator
    {
        public void AddListener(IDamageable damageable)
        {
            damageable.OnTakeDamage += OnTakeDamage;
        }

        public void RemoveListener(IDamageable damageable)
        {
            damageable.OnTakeDamage -= OnTakeDamage;
        }

        private void OnTakeDamage(IDamageable damageable, Damage damage)
        {
            damage = CalculateDamage(damage, damageable.GetDamageModifiers());
            damage.OnDamageCalculated(damage);
        }

        private Damage CalculateDamage(Damage damage, List<IDamageModifier> damageableModifiers)
        {
            damage = damage.DamageType.CalculateBaseDamage(damage);
            damage = HandleDamageModifiers(damage, damage.Modifiers);
            damage = HandleDamageModifiers(damage, damageableModifiers);
            return damage;
        }

        private Damage HandleDamageModifiers(Damage damage, List<IDamageModifier> modifiers)
        {
            if (modifiers == null || !modifiers.Any()) return damage;  
            modifiers.ForEach(o => damage = o.ModifyDamage(damage));

            if (damage.OvertimeEffects == null || damage.OvertimeEffects.Any())
            {
                modifiers.ForEach(o => damage.OvertimeEffects.ForEach(i => i.ModifyOvertimeEffect(o)));
            }

            return damage;
        }
    }
}