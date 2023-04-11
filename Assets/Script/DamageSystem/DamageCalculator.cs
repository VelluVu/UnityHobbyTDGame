using System.Collections.Generic;
using System.Linq;

namespace TheTD.DamageSystem
{
    public class DamageCalculator
    {
        public void AddListener(IDamageable damageable)
        {
            damageable.OnTakeRawDamage += OnTakeDamage;
        }

        public void RemoveListener(IDamageable damageable)
        {
            damageable.OnTakeRawDamage -= OnTakeDamage;
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
            damage = HandleOverTimeDamageModifiers(damage, damage.Modifiers);
            damage = HandleDamageModifiers(damage, damageableModifiers);
            damage = HandleOverTimeDamageModifiers(damage, damageableModifiers);
            return damage;
        }

        private Damage HandleOverTimeDamageModifiers(Damage damage, List<IDamageModifier> modifiers)
        {
            if (modifiers == null || !modifiers.Any()) return damage;
            if (damage.OvertimeEffects == null || !damage.OvertimeEffects.Any()) return damage;
            modifiers.ForEach(o => damage.OvertimeEffects.ForEach(i => i.ModifyOvertimeEffect(o)));
            return damage;
        }

        private Damage HandleDamageModifiers(Damage damage, List<IDamageModifier> modifiers)
        {
            if (modifiers == null || !modifiers.Any()) return damage;  
            modifiers.ForEach(o => damage = o.Modify(damage));
            return damage;
        }
    }
}