using UnityEngine;

namespace TheTD.Core
{
    public class GameControl : MonoBehaviour
    {
        public static GameControl Instance { get; private set; }

        public GameStyle gameStyle = GameStyle.Normal;
        private int nextWave = 0;
        private int spawnWave = 0;

        [SerializeField] private int _playerLife = 5;
        public int PlayerLife { get => _playerLife; private set => SetPlayerLife(value); }

        [SerializeField] private int _playerGold = 25;
        public int PlayerGold { get => _playerGold; private set => SetPlayerGold(value); }

        public delegate void GameDelegate();
        public event GameDelegate OnPlayerLose;
        public event GameDelegate OnPlayerTakeDamage;
        public event GameDelegate OnStartWave;
        public event GameDelegate OnWaveClear;

        public delegate void GameGoldExchangeDelegate(int goldDifference, int PlayerGoldAfterOperation);
        public event GameGoldExchangeDelegate OnPlayerSpendGold;
        public event GameGoldExchangeDelegate OnPlayerGainGold;

        private void PlayerLose()
        {
            Debug.Log("Player Lose");
            OnPlayerLose?.Invoke();
        }

        public void Ready()
        {
            spawnWave = nextWave;
            SpawnersControl.Instance.SpawnWithSelectedSpawners(spawnWave);
            nextWave = spawnWave + 1;
            OnStartWave?.Invoke();
        }

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            AddListers();
        }

        private void AddListers()
        {
            SpawnersControl.Instance.OnWaveComplete += OnWaveComplete;
            SpawnersControl.Instance.OnLevelComplete += OnLevelComplete;
            SpawnersControl.Instance.OnEnemyReachEnd += OnEnemyReachedEnd;
            SpawnersControl.Instance.OnEnemyKilled += OnEnemyKill;
            BuildArea.OnBuild += OnBuildTower;
        }

        private void OnEnemyKill(WaveState waveState, Enemy enemy)
        {
            AddGold(enemy.GoldValue);
        }

        private void AddGold(int goldValue)
        {
            PlayerGold += goldValue;
        }

        private void OnBuildTower(Building building)
        {
            UseGold(building.Tower.buildCost);
        }

        private void UseGold(int buildCost)
        {
            PlayerGold -= buildCost;
        }

        private void OnEnemyReachedEnd(WaveState waveState, Enemy enemy)
        {
            PlayerTakeDamage(enemy.Damage);
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

        private void PlayerTakeDamage(int value)
        {
            PlayerLife -= value;
            OnPlayerTakeDamage?.Invoke();
        }

        private void SetPlayerLife(int value)
        {
            _playerLife = value;
            if (_playerLife <= 0)
            {
                PlayerLose();
            }
        }

        private void SetPlayerGold(int value)
        {
            if (value < _playerGold)
            {
                OnPlayerSpendGold?.Invoke(_playerGold - value, value);
            }
            if(value > _playerGold)
            {
                OnPlayerGainGold?.Invoke(_playerGold - value, value);
            }
            _playerGold = value;
            if (_playerGold <= 0)
            {
                _playerGold = 0;
                Debug.Log("Player gold is 0");
            }
        }

        public bool HasGoldForTower(Tower tower)
        {
            if (tower.buildCost > PlayerGold) return false;
            return true;
        }
    }
}