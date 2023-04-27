using System.Collections.Generic;
using System.Linq;
using TheTD.Core;
using TheTD.DamageSystem;
using TheTD.Enemies;
using UnityEngine;

namespace TheTD.Spawning
{
    public class SpawnersControl : MonoBehaviour
    {
        public static SpawnersControl Instance { get; private set; }
        [Header("Testing Variables")]
        public int spawnWaveForTesting = 0;

        [Header("All spawned waves and enemies")]
        public List<WaveState> waveStates = new List<WaveState>();
        public List<Enemy> enemiesInLevel = new List<Enemy>();

        public int AmountOfAliveEnemiesInLevel { get => enemiesInLevel.Count; }
        public int AmountOfEnemiesDestroyedInLevel { get; private set; }
        public int AmountOFEnemiesReachedEndInLevel { get; private set; }
        public int AmountOfEnemiesSpawnedInLevel { get; private set; }

        protected int _amountOfSpawnsInLevel = -1;
        public int AmountOfSpawnsInLevel { get => _amountOfSpawnsInLevel = _amountOfSpawnsInLevel == -1 ? GetAmountOfSpawnsInLevel() : _amountOfSpawnsInLevel; private set => _amountOfSpawnsInLevel = value; }

        private List<Spawner> _allSpawners;
        public List<Spawner> AllSpawners { get => _allSpawners = _allSpawners != null ? CheckSpawners() : GetSpawners(); }

        public delegate void SpawnersControlDelegate(WaveState waveState, Enemy enemy);
        public event SpawnersControlDelegate OnEnemyKilled;
        public event SpawnersControlDelegate OnEnemyReachEnd;
        public event SpawnersControlDelegate OnEnemySpawn;

        public delegate void SpawnersControlDelegateTwo(WaveState waveState);
        public event SpawnersControlDelegateTwo OnWaveComplete;
        public event SpawnersControlDelegateTwo OnLevelComplete;

        private void Awake()
        {
            CheckSingleton();
        }

        private void CheckSingleton()
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
            _allSpawners = GetSpawners();
            if (!_allSpawners.Any()) return;
            _allSpawners.ForEach(o => AddSpawnListeners(o));
        }

        public void AddSpawnListeners(Spawner spawn)
        {
            spawn.OnNewEnemyAdd += OnEnemyAdd;
            spawn.OnEnemyRemove += OnEnemyRemove;
            spawn.OnEnemyReachedEnd += OnEnemyReachedEnd;
        }

        private void OnEnemyReachedEnd(Enemy enemy, int wave, Damage damage = null)
        {
            HandleEnemyReachEnd(enemy, wave);
        }

        private void HandleEnemyReachEnd(Enemy enemy, int wave)
        {
            if (!enemiesInLevel.Contains(enemy)) return;
            var waveState = FindWaveState(wave);
            if (waveState == null) return;
            waveState.RemoveEnemy(enemy, true);
            enemiesInLevel.Remove(enemy);
            AmountOFEnemiesReachedEndInLevel++;
            AmountOfEnemiesDestroyedInLevel++;
            OnEnemyReachEnd?.Invoke(waveState, enemy);
            CheckTheGameStateConditions(waveState);
        }

        private void OnEnemyRemove(Enemy enemy, int wave, Damage damage = null)
        {
            HandleEnemyRemove(enemy, wave);
        }

        private void HandleEnemyRemove(Enemy enemy, int wave)
        {
            if (!enemiesInLevel.Contains(enemy)) return;
            var waveState = FindWaveState(wave);
            if (waveState == null) return;
            waveState.RemoveEnemy(enemy);
            enemiesInLevel.Remove(enemy);
            AmountOfEnemiesDestroyedInLevel++;
            OnEnemyKilled?.Invoke(waveState, enemy);
            CheckTheGameStateConditions(waveState);
        }

        private void OnEnemyAdd(Enemy enemy, int wave, Damage damage = null)
        {
            HandleEnemyAdd(enemy, wave);
        }

        private void HandleEnemyAdd(Enemy enemy, int wave)
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
            if (AmountOfEnemiesDestroyedInLevel >= AmountOfSpawnsInLevel) OnLevelComplete?.Invoke(waveState);
        }

        private WaveState FindWaveState(int wave)
        {
            return waveStates.Find(o => o.WaveNumber == wave);
        }

        private List<Spawner> CheckSpawners()
        {
            return HasNewSpawners() ? GetSpawners() : _allSpawners;
        }

        private bool HasNewSpawners()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                var spawn = transform.GetChild(i).GetComponent<Spawner>();
                if (_allSpawners.Contains(spawn)) continue;
                AddSpawnListeners(spawn);
            }
            return false;
        }

        private List<Spawner> GetSpawners()
        {
            return GetComponentsInChildren<Spawner>().ToList();
        }

        private List<Spawner> GetSpawnersInWave(int wave)
        {
            return AllSpawners.FindAll(o => o.waveToParticipate == wave);
        }

        public void SpawnWithSelectedSpawners(int wave)
        {
            var spawnersInWave = GetSpawnersInWave(wave);
            var waveState = FindWaveState(wave);

            if (waveState == null)
            {
                waveState = new WaveState(wave, spawnersInWave);
                waveState.OnWaveComplete += WaveCompleted;
                waveStates.Add(waveState);
                GameControl.Instance.SetSpawnWave(wave);
            }
            else
            {
                //AmountOfEnemiesDestroyedInLevel -= waveState.AmountOfEnemiesDestroyedInWave;
                //AmountOfEnemiesSpawnedInLevel -= waveState.AmountOfEnemiesSpawnedInWave;
                //AmountOFEnemiesReachedEndInLevel -= waveState.AmountOFEnemiesReachedEnd;
                var spawners = GetSpawnersInWave(wave);
                spawners.ForEach(o => AmountOfSpawnsInLevel += o.GetSpawnsInWave(wave));
                waveState.ResetWaveState();
                GameControl.Instance.SetSpawnWave(wave);
            }

            foreach (var spawn in spawnersInWave)
            {
                spawn.SpawnSet();
            }
        }

        private void WaveCompleted(WaveState waveState)
        {
            OnWaveComplete?.Invoke(waveState);
        }

        public bool AreOnGoingWavesFinished()
        {
            return waveStates.TrueForAll(o => o.IsComplete == true);
        }

        private int GetAmountOfSpawnsInLevel()
        {
            int spawnsInLevel = 0;
            AllSpawners.ForEach(o => spawnsInLevel += o.TotalSpawns);
            return spawnsInLevel;
        }
    }
}