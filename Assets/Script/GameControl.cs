using UnityEngine;

namespace TheTD.Core
{
    public class GameControl : MonoBehaviour
    {
        public static GameControl Instance { get; private set; }

        public GameStyle gameStyle = GameStyle.Normal;
        private int nextWave = 0;
        private int spawnWave = 0;

        [SerializeField]private Player _player;
        public Player Player { get => _player = _player != null ? _player : FindObjectOfType<Player>(); }

        public delegate void GameDelegate();
        public event GameDelegate OnPlayerLose;
        public event GameDelegate OnStartWave;
        public event GameDelegate OnWaveClear;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            AddListers();
        }

        public void Ready()
        {
            spawnWave = nextWave;
            SpawnersControl.Instance.SpawnWithSelectedSpawners(spawnWave);
            nextWave = spawnWave + 1;
            OnStartWave?.Invoke();
        }

        private void LoseGame()
        {
            Debug.Log("Player Lose");
            OnPlayerLose?.Invoke();
        }

        private void AddListers()
        {
            SpawnersControl.Instance.OnWaveComplete += OnWaveComplete;
            SpawnersControl.Instance.OnLevelComplete += OnLevelComplete;
            Player.OnDeath += OnPlayerDeath;
        }

        private void OnPlayerDeath(Player player)
        {
            LoseGame();
        }

        private void OnLevelComplete(WaveState waveState)
        {
            Debug.Log("Level Complete");
        }

        private void OnWaveComplete(WaveState waveState)
        {
            Debug.Log("Wave Complete");
            spawnWave = nextWave;
            if (SpawnersControl.Instance.AreOnGoingWavesFinished())
            {
                OnWaveClear?.Invoke();
            }
        }     
    }
}