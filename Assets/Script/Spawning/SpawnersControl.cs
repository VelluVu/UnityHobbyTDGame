using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawnersControl : MonoBehaviour
{
    public static SpawnersControl Instance { get; private set; }

    public List<Enemy> allEnemies = new List<Enemy>();
    public int AliveEnemiesCount { get => allEnemies.Count; }
    public int SpawnsInWave { get => GetSpawnsInWave(); }
    public int EnemiesKilledInWave { get; private set; }
    public int SpawnedEnemiesCount { get; private set; }
    public int EnemiesKilledCount { get; private set; }
    public int SpawnsInLevel { get; private set; }

    private EnemyHolder enemyHolder;
    public EnemyHolder EnemyHolder { get => enemyHolder = enemyHolder != null ? enemyHolder : FindObjectOfType<EnemyHolder>(); }

    private List<Spawner> spawners;
    public List<Spawner> Spawners { get => spawners = spawners != null ? CheckSpawns() : GetSpawns(); }

    public List<Spawner> selectedSpawners = new List<Spawner>();

    public delegate void SpawnersControlDelegate();
    public event SpawnersControlDelegate OnEnemyKill;
    public event SpawnersControlDelegate OnEnemySpawn;
    public event SpawnersControlDelegate OnWaveComplete;
    public event SpawnersControlDelegate OnLevelComplete;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        spawners = GetSpawns();
        if (!spawners.Any()) return;
        spawners.ForEach(o => AddSpawnListeners(o));
        SpawnedEnemiesCount = 0;
        EnemiesKilledCount = 0;
        CalculateTotalSpawnsInLevel();
    }

    private void CalculateTotalSpawnsInLevel()
    {
        SpawnsInLevel = 0;
        Spawners.ForEach(o => SpawnsInLevel += o.TotalSpawns);
    }

    public void AddSpawnListeners(Spawner spawn)
    {
        spawn.OnNewEnemyAdd += OnEnemyAdd;
        spawn.OnEnemyRemove += OnEnemyRemove;
    }

    private void OnEnemyRemove(Enemy enemy)
    {
        if (!allEnemies.Contains(enemy)) return;
        allEnemies.Remove(enemy);
        EnemiesKilledCount++;
        EnemiesKilledInWave++;
        OnEnemyKill?.Invoke();
        if(EnemiesKilledInWave >= SpawnsInWave)
        {
            OnWaveComplete?.Invoke();
        }
        if(EnemiesKilledCount >= SpawnsInLevel)
        {
            OnLevelComplete?.Invoke();
        }
    }

    private void OnEnemyAdd(Enemy enemy)
    {
        if (allEnemies.Contains(enemy)) return;
        allEnemies.Add(enemy);
        SpawnedEnemiesCount++;
        OnEnemySpawn?.Invoke();
    }

    private List<Spawner> CheckSpawns()
    {  
        return HasNewSpawns() ? GetSpawns() : spawners;
    }

    private bool HasNewSpawns()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            var spawn = transform.GetChild(i).GetComponent<Spawner>();
            if (!spawners.Contains(spawn)) return true;
            AddSpawnListeners(spawn);
        }
        return false;
    }

    private List<Spawner> GetSpawns()
    {  
        return GetComponentsInChildren<Spawner>().ToList();
    }

    public void SpawnWithSelectedSpawners(int phase)
    {
        EnemiesKilledInWave = 0;
        foreach (Spawner spawner in selectedSpawners)
        {
            spawner.SpawnSet(phase);
        }
    }

    private int GetSpawnsInWave()
    {
        int spawnsInWave = 0;
        Spawners.ForEach(o => spawnsInWave += o.SpawnsInWave);
        return spawnsInWave;
    }
}
