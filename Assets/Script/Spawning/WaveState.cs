using System.Collections.Generic;
using TheTD.Enemies;

namespace TheTD.Spawning
{
    [System.Serializable]
    public class WaveState
    {
        public List<Spawner> SpawnersInWave = new List<Spawner>();
        public List<Enemy> enemiesInWave = new List<Enemy>();
        public bool IsComplete { get => GetIsComplete(); }
        public int WaveNumber { get; private set; }
        public int AmountOfAliveEnemies { get => enemiesInWave.Count; }
        public int AmountOfSpawnsInWave { get => GetAmountOFSpawnsInWave(); }
        public int AmountOfEnemiesKilledInWave { get; private set; }
        public int AmountOfEnemiesSpawnedInWave { get; private set; }
        public int AmountOFEnemiesReachedEnd { get; private set; }

        public WaveState(int waveNumber, List<Spawner> spawnersInWave)
        {
            WaveNumber = waveNumber;
            SpawnersInWave = spawnersInWave;
        }

        public void RemoveEnemy(Enemy enemy, bool reachedEnd = false)
        {
            if (!enemiesInWave.Contains(enemy)) return;
            enemiesInWave.Remove(enemy);
            AmountOfEnemiesKilledInWave++;
            if (reachedEnd) AmountOFEnemiesReachedEnd++;
        }

        public void AddEnemy(Enemy enemy)
        {
            if (enemiesInWave.Contains(enemy)) return;
            enemiesInWave.Add(enemy);
            AmountOfEnemiesSpawnedInWave++;
        }

        public bool GetIsComplete()
        {
            return AmountOfAliveEnemies == 0 && AmountOfEnemiesKilledInWave == AmountOfSpawnsInWave;
        }

        private int GetAmountOFSpawnsInWave()
        {
            int spawnsInWave = 0;
            SpawnersInWave.ForEach(o => spawnsInWave += o.GetSpawnsInWave(WaveNumber));
            return spawnsInWave;
        }
    }
}