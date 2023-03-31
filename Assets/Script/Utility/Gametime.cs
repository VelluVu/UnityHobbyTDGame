using System.Collections;
using TheTD.Core;
using UnityEngine;

[System.Serializable]
public class Gametime : MonoBehaviour
{
    private IEnumerator timeCoroutine;
    private bool isTimeRunning = false;

    public static Gametime Instance { get; private set; }

    public float PlayTimeInSeconds { get; private set; }
    public static int Seconds { get; private set; }

    private static int _minutes;
    public static int Minutes { get => _minutes; private set => SetMinutes(value); }

    private static int _hours;
    public static int Hours { get => _hours; private set => SetHours(value); }

    public static int Days { get; private set; }
    public bool IsPaused { get; private set; }

    public delegate void OnTimeChange();
    public static event OnTimeChange OnMinutesChange;
    public static event OnTimeChange OnHoursChange;
    public static event OnTimeChange OnDayChange;

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
        timeCoroutine = TimeCoroutine();
        GameControl.Instance.OnPlayerLose += OnPlayerLose;
        GameControl.Instance.OnStartWave += OnStartWave;
        GameControl.Instance.OnWaveClear += OnWaveClear;
    }

    private void OnWaveClear(int wave)
    {
        StopTimer();
    }

    private void OnStartWave(int wave)
    {
        StartTimer();
    }

    private void StartTimer()
    {
        if (!isTimeRunning) StartCoroutine(timeCoroutine);
        else IsPaused = false;
    }

    private void StopTimer()
    {
        IsPaused = true;
    }

    private void OnPlayerLose(int wave)
    {
        StopTimer();
    }

    IEnumerator TimeCoroutine()
    {
        isTimeRunning = true;

        while (true)
        {
            if (!IsPaused)
            {
                PlayTimeInSeconds += Time.deltaTime;
                Seconds = Mathf.FloorToInt(PlayTimeInSeconds % 60);
                Minutes = Mathf.FloorToInt(PlayTimeInSeconds / 60) % 60;
                Hours = Mathf.FloorToInt(PlayTimeInSeconds / 3600);
            }
            yield return null;
        }
    }

    private static void SetMinutes(int value)
    {
        if (_minutes == value) return;
        _minutes = value;
        OnMinutesChange?.Invoke();
    }

    private static void SetHours(int value)
    {
        if (_hours == value) return;
        _hours = value;
        OnHoursChange?.Invoke();

        if (value >= 24)
        {
            Hours = 0;
            Days++;
            OnDayChange?.Invoke();
        }
    }
}
