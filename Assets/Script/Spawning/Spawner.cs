using System.Collections;
using System.Collections.Generic;
using TheTD.DamageSystem;
using TheTD.Enemies;
using UnityEngine;

namespace TheTD.Spawning
{
    public class Spawner : MonoBehaviour, IEventListener
    {
        private const string ENEMY_PREFAB_PATH = "Prefabs/Enemies/";
        private const string END_POINT_TAG = "EndPoint";

        [Header("Testing Parameters")]
        public EnemyType testSingleEnemySpawnEnemyType = EnemyType.Goblin;
        public Spawn testMultipleSpawnSpawn;

        [Header("Spawning Parameters")]
        public int waveToParticipate = 0;
        [SerializeField]private Transform _endPointForSpawn;
        public Transform EndTransform { get => _endPointForSpawn = _endPointForSpawn != null ? _endPointForSpawn : GameObject.FindGameObjectWithTag(END_POINT_TAG).transform; }
        public SpawnSet spawnSet;
        public List<Enemy> enemies = new List<Enemy>();

        public int TotalSpawns { get => GetTotalSpawns(); }

        internal delegate void EnemiesListDelegate(Enemy enemy, int wave, Damage damage = null);
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

        virtual public void AddListeners()
        {
            Enemy.OnReachEnd += OnEnemyReachEnd;
            Enemy.OnDeath += OnEnemyDie;
        }

        virtual public void RemoveListeners()
        {
            Enemy.OnReachEnd -= OnEnemyReachEnd;
            Enemy.OnDeath -= OnEnemyDie;
        }

        virtual protected void OnEnemyDie(Enemy enemy, Damage damage)
        {
            RemoveEnemy(enemy, damage);
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
            enemy.FSM.PathControl.Destination = EndTransform.position;
            AddEnemy(enemy);
        }

        virtual public void RemoveEnemy(Enemy enemy, Damage damage)
        {
            if (!enemies.Contains(enemy)) return;
            enemies.Remove(enemy);
            OnEnemyRemove?.Invoke(enemy, waveToParticipate, damage);
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

        virtual protected void OnDestroy()
        {
            RemoveListeners();
        }
    }
}