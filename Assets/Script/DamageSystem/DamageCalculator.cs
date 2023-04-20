using System.Collections.Generic;

namespace TheTD.DamageSystem
{
    public class DamageCalculator
    {
        private IRandomGenerator _secureRandomNumberGenerator;
        public IRandomGenerator SecureRandomNumberGenerator { get => _secureRandomNumberGenerator = _secureRandomNumberGenerator != null ? _secureRandomNumberGenerator : new SecureRandomNumberGenerator(); }

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
            damage.CalculateCritical(SecureRandomNumberGenerator);
            //Reduce dmg with damageable defensive modifiers

            damage.OnDamageCalculated(damage);
          
        }
    }
}