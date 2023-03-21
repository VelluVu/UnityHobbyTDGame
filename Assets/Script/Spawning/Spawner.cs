using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    private const string ENEMIES_HOLDER_IS_NULL_LOG_FORMAT = "Enemies holder is null, assign the reference in inspector to set parent for enemy spawns.";
    private const string ENEMY_PREFAB_PATH = "Prefabs/Enemies/";

    [Header("Testing Parameters")]
    public EnemyType testSingleEnemySpawnEnemyType = EnemyType.Goblin;
    public Spawn testMultipleSpawnSpawn;

    [Header("Spawning Parameters")]
    public int waveToParticipate = 0;
    public SpawnSet spawnSet;
    public List<Enemy> enemies = new List<Enemy>();    

    public int TotalSpawns { get => GetTotalSpawns(); }

    private EnemyHolder enemyHolder;
    public EnemyHolder EnemyHolder { get => enemyHolder = enemyHolder != null ? enemyHolder : FindObjectOfType<EnemyHolder>(); }

    public delegate void SpawnAIDelegate(Vector3 position);
    public static event SpawnAIDelegate OnPathBlocked;

    internal delegate void EnemiesListDelegate(Enemy enemy, int wave);
    internal event EnemiesListDelegate OnNewEnemyAdd;
    internal event EnemiesListDelegate OnEnemyRemove;
    internal event EnemiesListDelegate OnEnemyReachedEnd;

    public int GetSpawnsInWave(int wave)
    {
        int spawnsInWave = 0;
        spawnSet.spawns.ForEach(o => spawnsInWave += o.amount);
        return spawnsInWave;
    }

    virtual protected void Start()
    {
        AddListeners();
        DestroyAllSpawnedEnemies();      
    }

    public int GetTotalSpawns()
    {
        var totalSpawns = 0;
        spawnSet.spawns.ForEach(o => totalSpawns += o.amount);
        return totalSpawns;
    }

    virtual public void DestroyAllSpawnedEnemies()
    {
        enemies.ForEach(o => Destroy(o.gameObject));
        enemies.Clear();
    }

    virtual protected void AddListeners()
    {
        Enemy.OnReachEnd += OnEnemyReachEnd;
        Enemy.OnDeath += OnEnemyDie;
        Enemy.OnPathBlocked += OnEnemyPathBlocked;
    }

    virtual protected void OnEnemyPathBlocked(Enemy enemy)
    {
        OnPathBlocked?.Invoke(enemies.First().transform.position);
    }

    virtual protected void OnEnemyDie(Enemy enemy)
    {
        RemoveEnemy(enemy);
    }

    virtual protected void OnEnemyReachEnd(Enemy enemy)
    {
        if (!enemies.Contains(enemy)) return;
        enemies.Remove(enemy);
        OnEnemyReachedEnd?.Invoke(enemy, waveToParticipate);
    }

    virtual public void SpawnSingleEnemy(EnemyType enemyType)
    {
        var prefab = LoadSpawnPrefab(enemyType);
        var spawnPosition = CalculateSpawnPosition(prefab.transform);
        var enemy = Instantiate(prefab, spawnPosition, transform.rotation).GetComponent<Enemy>();
        AddEnemy(enemy);
        if (!EnemyHolder) Debug.LogFormat(ENEMIES_HOLDER_IS_NULL_LOG_FORMAT);
        else enemy.transform.SetParent(EnemyHolder.transform);
    }

    virtual public void RemoveEnemy(Enemy enemy)
    {
        if (!enemies.Contains(enemy)) return;
        enemies.Remove(enemy);
        OnEnemyRemove?.Invoke(enemy, waveToParticipate);
    }

    virtual public void AddEnemy(Enemy enemy)
    {
        enemies.Add(enemy);
        OnNewEnemyAdd?.Invoke(enemy, waveToParticipate);
    }

    virtual public void SpawnSet()
    {
        StartCoroutine(SpawnSetSpawnsInTurns(spawnSet));
    }

    virtual protected GameObject LoadSpawnPrefab(EnemyType enemyType)
    {
        var loadpath = GetLoadPath(enemyType);
        return Resources.Load<GameObject>(loadpath);
    }

    virtual protected Vector3 CalculateSpawnPosition(Transform prefabTransform)
    {
        return transform.position + Vector3.up * prefabTransform.position.y;
    }

    virtual protected string GetLoadPath(EnemyType enemyType)
    {
        return ENEMY_PREFAB_PATH + enemyType.ToString();
    }

    virtual protected float CalculateSpawningDuration(int amount, float interval, float startDelay)
    {
        return amount * interval + startDelay;
    }

    virtual protected IEnumerator SpawnSetSpawnsInTurns(SpawnSet set)
    {   
        foreach (var spawn in set.spawns)
        {
            SpawnAmountOfEnemies(spawn);
            var spawningDuration = CalculateSpawningDuration(spawn.amount, spawn.interval, spawn.startDelay);
            yield return new WaitForSeconds(spawningDuration);
        }
        yield return null;
    }

    virtual public void SpawnAmountOfEnemies(Spawn spawn)
    {
        StartCoroutine(SpawnEnemies(spawn));
    }

    virtual protected IEnumerator SpawnEnemies(Spawn spawn)
    {
        yield return new WaitForSeconds(spawn.startDelay);

        for (int i = 0; i < spawn.amount; i++)
        {
            SpawnSingleEnemy(spawn.enemyType);
            yield return new WaitForSeconds(spawn.interval);
        }

        yield return null;
    }
}
