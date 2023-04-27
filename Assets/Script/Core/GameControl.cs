using TheTD.DamageSystem;
using TheTD.Enemies;
using TheTD.Players;
using TheTD.Spawning;
using UnityEngine;

namespace TheTD.Core
{
    public class GameControl : MonoBehaviour
    {
        public static GameControl Instance { get; private set; }

        private const string PLAYER_LOSE_DEBUG_MESSAGE = "Player Lose";
        private const string LEVEL_COMPLETE_DEBUG_MESSAGE = "Level Complete";
        private const string WAVE_COMPLETE_DEBUG_MESSAGE = "Wave Complete";
        private const string ALL_ON_GOING_WAVES_CLEARED_DEBUG_MESSAGE = "All on going waves cleared";

        public GameStyle gameStyle = GameStyle.Normal;
        public int NextSpawnWave { get; private set; }
        public int CurrentSpawnWave { get; private set; }

        public DamageCalculator DamageCalculator { get; private set; }

        [SerializeField]private Player _player;
        public Player Player { get => _player = _player != null ? _player : FindObjectOfType<Player>(); }
        public bool EnableBuilding { get; set; }

        public delegate void GameDelegate(int wave);
        public event GameDelegate OnPlayerLose;
        public event GameDelegate OnStartWave;
        public event GameDelegate OnWaveAndEnemiesClear;

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
            DamageCalculator = new DamageCalculator();
            AddListers();
            EnableBuilding = true;
        }

        public void Ready()
        {
            CurrentSpawnWave = NextSpawnWave;
            NextSpawnWave = CurrentSpawnWave + 1;
            SpawnersControl.Instance.SpawnWithSelectedSpawners(CurrentSpawnWave);
            OnStartWave?.Invoke(CurrentSpawnWave);
        }

        public void SetSpawnWave(int wave)
        {
            if(CurrentSpawnWave == wave) return;    
            CurrentSpawnWave = wave;
            OnStartWave?.Invoke(CurrentSpawnWave);
        }

        private void LoseGame()
        {
            Debug.Log(PLAYER_LOSE_DEBUG_MESSAGE);
            OnPlayerLose?.Invoke(CurrentSpawnWave);
        }

        private void AddListers()
        {
            SpawnersControl.Instance.OnWaveComplete += OnWaveComplete;
            SpawnersControl.Instance.OnLevelComplete += OnLevelComplete;
            SpawnersControl.Instance.OnEnemyReachEnd += OnEnemyReachEnd;
            SpawnersControl.Instance.OnEnemyKilled += OnEnemyKilled;
            SpawnersControl.Instance.OnEnemySpawn += OnEnemySpawn;
            Player.OnDeath += OnPlayerDeath;
        }

        private void OnEnemySpawn(WaveState waveState, Enemy enemy)
        {
            DamageCalculator.AddListener(enemy);
        }

        private void OnEnemyKilled(WaveState waveState, Enemy enemy)
        {
            DamageCalculator.RemoveListener(enemy);
        }

        private void OnEnemyReachEnd(WaveState waveState, Enemy enemy)
        {
            DamageCalculator.RemoveListener(enemy);
        }

        private void OnPlayerDeath(Player player)
        {
            LoseGame();
        }

        private void OnLevelComplete(WaveState waveState)
        {
            Debug.Log(LEVEL_COMPLETE_DEBUG_MESSAGE);
        }

        private void OnWaveComplete(WaveState waveState)
        {
            Debug.Log(WAVE_COMPLETE_DEBUG_MESSAGE);
            if (SpawnersControl.Instance.AreOnGoingWavesFinished())
            {
                Debug.Log(ALL_ON_GOING_WAVES_CLEARED_DEBUG_MESSAGE);
                OnWaveAndEnemiesClear?.Invoke(CurrentSpawnWave);
            }
            CurrentSpawnWave = NextSpawnWave;
        }     
    }
}