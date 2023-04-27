using System;
using System.Collections.Generic;
using TheTD.Enemies;

namespace TheTD.Spawning
{
    [System.Serializable]
    public class WaveState
    {
        public List<Spawner> SpawnersInWave = new List<Spawner>();
        public List<Enemy> enemiesInWave = new List<Enemy>();
        private bool _isComplete = false;
        public bool IsComplete { get => _isComplete; private set => SetIsComplete(value); }

        public delegate void WaveStateDelegateTwo(WaveState waveState);
        public event WaveStateDelegateTwo OnWaveComplete;

        public int WaveNumber { get; private set; }
        public int AmountOfAliveEnemies { get => enemiesInWave.Count; }
        public int AmountOfSpawnsInWave { get; private set; }
        public int AmountOfEnemiesDestroyedInWave { get; private set; }
        public int AmountOfEnemiesSpawnedInWave { get; private set; }
        public int AmountOFEnemiesReachedEnd { get; private set; }

        public WaveState(int waveNumber, List<Spawner> spawnersInWave)
        {
            WaveNumber = waveNumber;
            SpawnersInWave = spawnersInWave;
            AmountOfSpawnsInWave = GetAmountOfSpawnsInWave();
        }

        public void RemoveEnemy(Enemy enemy, bool reachedEnd = false)
        {
            if (!enemiesInWave.Contains(enemy)) return;
            enemiesInWave.Remove(enemy);
            AmountOfEnemiesDestroyedInWave++;
            if (reachedEnd) AmountOFEnemiesReachedEnd++;
            IsComplete = AmountOfAliveEnemies == 0 && AmountOfEnemiesDestroyedInWave == AmountOfSpawnsInWave;
        }

        public void AddEnemy(Enemy enemy)
        {
            if (enemiesInWave.Contains(enemy)) return;
            enemiesInWave.Add(enemy);
            AmountOfEnemiesSpawnedInWave++;
        }

        public void ResetWaveState()
        {
            AmountOfSpawnsInWave = GetAmountOfSpawnsInWave();
            AmountOfEnemiesDestroyedInWave = IsComplete ? 0 : AmountOfEnemiesDestroyedInWave;
            AmountOfEnemiesSpawnedInWave = IsComplete ? 0 : AmountOfEnemiesSpawnedInWave;
            AmountOFEnemiesReachedEnd = IsComplete ? 0 : AmountOFEnemiesReachedEnd;
            IsComplete = false;
        }

        private int GetAmountOfSpawnsInWave()
        {
            int spawnsInWave = IsComplete ? 0 : AmountOfSpawnsInWave;
            SpawnersInWave.ForEach(o => spawnsInWave += o.GetSpawnsInWave(WaveNumber));
            return spawnsInWave;
        }

        private void SetIsComplete(bool value)
        {
            if (_isComplete == value) return;
            _isComplete = value;
            if (_isComplete)
            {
                OnWaveComplete?.Invoke(this);
            }
        }
    }
}