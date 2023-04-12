using System.Collections.Generic;

namespace TheTD.DamageSystem
{
    public class DamageCalculator
    {
        private SecureRandomNumberGenerator _secureRandomNumberGenerator;
        public SecureRandomNumberGenerator SecureRandomNumberGenerator { get => _secureRandomNumberGenerator = _secureRandomNumberGenerator != null ? _secureRandomNumberGenerator : new SecureRandomNumberGenerator(); }

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
            CalculateDamage(damage, damageable.GetDamageModifiers());            
        }

        private void CalculateDamage(Damage damage, List<IDamageModifier> damageableModifiers)
        {
            damage.CalculateBase();
            damage.ApplyModifiers(damage.Modifiers);      
            damage.ApplyModifiers(damageableModifiers);
            damage.CalculateCritical(SecureRandomNumberGenerator);
            damage.OnDamageCalculated(damage);
        }
    }
}