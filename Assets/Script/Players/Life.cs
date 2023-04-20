using TheTD.Enemies;
using TheTD.Spawning;
using UnityEngine;

namespace TheTD.Players
{
    [System.Serializable]
    public class Life
    {
        public int lastValueChange = 0;

        [SerializeField] private int _max = 0;
        public int Max { get => _max; }
        [SerializeField] private int _current = 0;
        public int Current { get => _current; }

        internal delegate void LifeDelegate(Life life);
        internal event LifeDelegate OnZero;
        internal event LifeDelegate OnRemove;
        internal event LifeDelegate OnHeal;
        internal event LifeDelegate OnMaxLife;

        internal void AddListeners()
        {
            SpawnersControl.Instance.OnEnemyReachEnd += OnEnemyReachedEnd;
        }

        private void OnEnemyReachedEnd(WaveState waveState, Enemy enemy)
        {
            Remove(enemy.Stats.Damage.RoundedValue);
        }

        private void Remove(int value)
        {
            lastValueChange = value;
            _current -= value;
            OnRemove?.Invoke(this);
            if (_current <= 0)
            {
                _current = 0;
                OnZero?.Invoke(this);
            }
        }

        private void Heal(int value)
        {
            lastValueChange = value;
            _current += value;
            OnHeal?.Invoke(this);
            if (_current >= _max)
            {
                _current = _max;
                OnMaxLife?.Invoke(this);
            }
        }
    }
}