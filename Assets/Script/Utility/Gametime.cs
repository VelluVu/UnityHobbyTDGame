using System.Collections;
using UnityEngine;

[System.Serializable]
public class Gametime : MonoBehaviour
{
    public static Gametime Instance { get; private set; }

    public float PlayTimeInSeconds { get; private set; }
    public static int Seconds { get; private set; }

    private static int minutes;
    public static int Minutes { get => minutes; private set => SetMinutes(value); }

    private static int hours;
    public static int Hours { get => hours; private set => SetHours(value); }

    public static int Days { get; private set; }

    public delegate void OnTimeChange();
    public static event OnTimeChange OnMinutesChange;
    public static event OnTimeChange OnHoursChange;
    public static event OnTimeChange OnDayChange;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        StartCoroutine(TimeCoroutine());
    }

    IEnumerator TimeCoroutine()
    {
        while (true)
        {
            PlayTimeInSeconds += Time.deltaTime;
            Seconds = Mathf.FloorToInt(PlayTimeInSeconds % 60);
            Minutes = Mathf.FloorToInt(PlayTimeInSeconds / 60) % 60;
            Hours = Mathf.FloorToInt(PlayTimeInSeconds / 3600);
            yield return null;
        }
    }

    private static void SetMinutes(int value)
    {
        if(minutes == value) return;
        minutes = value;
        OnMinutesChange?.Invoke();
    }

    private static void SetHours(int value)
    {
        if(hours == value) return;
        hours = value;
        OnHoursChange?.Invoke();

        if (value >= 24)
        {
            Hours = 0;
            Days++;
            OnDayChange?.Invoke();
        }
    }
}
