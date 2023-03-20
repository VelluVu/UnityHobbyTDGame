using UnityEngine;

public class GameControl : MonoBehaviour
{
    public static GameControl Instance { get; private set; }

    public GameStyle gameStyle = GameStyle.Normal;
    public int nextWave = 0;
    public int currentWave = 0;

    public void Ready()
    {
        //NEED STATE OF WAVES STORED IN SPAWNERS CONTROL

        SpawnersControl.Instance.SpawnWithSelectedSpawners(currentWave);
        nextWave = currentWave + 1;
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
    }

    private void OnLevelComplete()
    {
        Debug.Log("Level Complete");
        
    }

    private void OnWaveComplete()
    {
        Debug.Log("Wave Complete");
        currentWave = nextWave;
    }

}

public enum GameStyle
{
    Normal,
    Endless,
}