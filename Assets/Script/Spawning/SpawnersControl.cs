using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawnersControl : MonoBehaviour
{
    public static SpawnersControl Instance { get; private set; }

    public List<WaveState> waveStates = new List<WaveState>();

    public List<Enemy> enemiesInLevel = new List<Enemy>();
    public int AmountOfAliveEnemiesInLevel { get => enemiesInLevel.Count; }
    public int AmountOfSpawnsInLevel { get => GetAmountOfSpawnsInLevel(); }
    public int AmountOfEnemiesDestroyedInLevel { get; private set; }
    public int AmountOFEnemiesReachedEndInLevel { get; private set; }
    public int AmountOfEnemiesSpawnedInLevel { get; private set; }

    private EnemyHolder enemyHolder;
    public EnemyHolder EnemyHolder { get => enemyHolder = enemyHolder != null ? enemyHolder : FindObjectOfType<EnemyHolder>(); }

    private List<Spawner> spawners;
    public List<Spawner> Spawners { get => spawners = spawners != null ? CheckSpawns() : GetSpawns(); }

    public List<Spawner> selectedSpawners = new List<Spawner>();

    public delegate void SpawnersControlDelegate(WaveState waveState, Enemy enemy);
    public event SpawnersControlDelegate OnEnemyRemoved;
    public event SpawnersControlDelegate OnEnemyReachEnd;
    public event SpawnersControlDelegate OnEnemySpawn;

    public delegate void SpawnersControlDelegateTwo(WaveState waveState);
    public event SpawnersControlDelegateTwo OnWaveComplete;
    public event SpawnersControlDelegateTwo OnLevelComplete;

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
    }

    public void AddSpawnListeners(Spawner spawn)
    {
        spawn.OnNewEnemyAdd += OnEnemyAdd;
        spawn.OnEnemyRemove += OnEnemyRemove;
        spawn.OnEnemyReachedEnd += OnEnemyReachedEnd;
    }

    private void OnEnemyReachedEnd(Enemy enemy, int wave)
    {
        if(!enemiesInLevel.Contains(enemy)) return;
        var waveState = FindWaveState(wave);
        if (waveState == null) return;
        waveState.RemoveEnemy(this, enemy, true);
        OnEnemyReachEnd?.Invoke(waveState, enemy);
        AmountOFEnemiesReachedEndInLevel++;
        AmountOfEnemiesDestroyedInLevel++;
        CheckTheGameStateConditions(waveState);
    }

    private void OnEnemyRemove(Enemy enemy, int wave)
    {      
        if (!enemiesInLevel.Contains(enemy)) return;
        var waveState = FindWaveState(wave);
        if (waveState == null) return;       
        waveState.RemoveEnemy(this, enemy);
        enemiesInLevel.Remove(enemy);
        AmountOfEnemiesDestroyedInLevel++;
        OnEnemyRemoved?.Invoke(waveState, enemy);
        CheckTheGameStateConditions(waveState);
    }

    private void OnEnemyAdd(Enemy enemy, int wave)
    {
        if (enemiesInLevel.Contains(enemy)) return;
        var waveState = FindWaveState(wave);
        if (waveState == null) return;
        waveState.AddEnemy(enemy);
        enemiesInLevel.Add(enemy);
        AmountOfEnemiesSpawnedInLevel++;
        OnEnemySpawn?.Invoke(waveState, enemy);
    }

    private void CheckTheGameStateConditions(WaveState waveState)
    {
        if (waveState.AmountOfEnemiesKilledInWave >= waveState.AmountOfSpawnsInWave) OnWaveComplete?.Invoke(waveState);
        if (AmountOfEnemiesDestroyedInLevel >= AmountOfSpawnsInLevel) OnLevelComplete?.Invoke(waveState);
    }

    private WaveState FindWaveState(int wave)
    {
        return waveStates.Find(o => o.WaveNumber == wave);
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
            if (spawners.Contains(spawn)) continue;
            AddSpawnListeners(spawn);
        }
        return false;
    }

    private List<Spawner> GetSpawns()
    {  
        return GetComponentsInChildren<Spawner>().ToList();
    }

    private List<Spawner> GetSpawnersInWave(int wave)
    {
        return Spawners.FindAll(o => o.waveToParticipate == wave);
    }

    public void SpawnWithSelectedSpawners(int wave)
    {
        var spawnersInWave = GetSpawnersInWave(wave);
        WaveState waveState = new WaveState(wave, spawnersInWave);
        waveStates.Add(waveState);
        foreach(var spawn in spawnersInWave)
        {
            spawn.SpawnSet();
        }
    }

    private int GetAmountOfSpawnsInLevel()
    {
        int spawnsInLevel = 0;
        Spawners.ForEach(o => spawnsInLevel += o.TotalSpawns);
        return spawnsInLevel;
    }
}
