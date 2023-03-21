using UnityEngine;

public class GameControl : MonoBehaviour
{
    public static GameControl Instance { get; private set; }

    public GameStyle gameStyle = GameStyle.Normal;
    public int nextWave = 0;
    public int spawnWave = 0;

    [SerializeField] private int playerLife = 100;
    public int PlayerLife { get => playerLife; private set => SetPlayerLife(value); }

    public delegate void GameDelegate();
    public event GameDelegate OnPlayerLose;

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
    }

    private void OnEnemyReachedEnd(WaveState waveState, Enemy enemy)
    {
        PlayerLife -= enemy.Damage;
    }

    private void OnLevelComplete(WaveState waveState)
    {
        Debug.Log("Level Complete");
        
    }

    private void OnWaveComplete(WaveState waveState)
    {
        Debug.Log("Wave Complete");
        spawnWave = nextWave;
    }

    private void SetPlayerLife(int value)
    {
        playerLife = value;
        if (playerLife <= 0)
        {
            PlayerLose();
        }
    }
}
