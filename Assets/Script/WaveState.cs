using System.Collections.Generic;

[System.Serializable]
public class WaveState
{
    public List<Spawner> SpawnersInWave = new List<Spawner>();
    public List<Enemy> enemiesInWave = new List<Enemy>();
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

    public void RemoveEnemy(SpawnersControl control, Enemy enemy, bool reachedEnd = false)
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

    private int GetAmountOFSpawnsInWave()
    {
        int spawnsInWave = 0;
        SpawnersInWave.ForEach(o => spawnsInWave += o.GetSpawnsInWave(WaveNumber));
        return spawnsInWave;
    }
}
