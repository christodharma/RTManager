using UnityEngine;
using System;

public enum DayPhase { Pagi, Siang, Sore, Malam }

public class GameTimeManager : MonoBehaviour
{
    public static GameTimeManager Instance { get; private set; }

    [Header("Time Settings")]
    [Tooltip("Real seconds for a full in-game day (6 minutes = 360s)")]
    public float realSecondsPerDay = 360f;
    [Tooltip("Start game time hour (0-23)")]
    [Range(0, 23)] public int startHour = 6;
    [Range(0, 59)] public int startMinute = 0;

    [Header("Runtime (read-only)")]
    public int currentHour;
    public int currentMinute;
    public int currentDay = 1;
    [Range(0f, 1f)] public float dayFraction; // 0..1 across the 24h

    public event Action<int, int> OnTimeChanged; // hour, minute
    public event Action<DayPhase> OnPhaseChanged;
    public event Action<float> OnDayFractionChanged; // 0..1

    public DayPhase CurrentPhase { get; private set; }

    private float secondsElapsedToday;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        secondsElapsedToday = (startHour * 3600f + startMinute * 60f) / (24f * 3600f) * realSecondsPerDay;
        ComputeTimeFromSeconds();
    }

    void Update()
    {
        // Normal time progression; multiply by timeScale if need to speed up for testing
        secondsElapsedToday += Time.deltaTime;
        if (secondsElapsedToday >= realSecondsPerDay)
        {
            secondsElapsedToday -= realSecondsPerDay;
            currentDay++;
        }

        ComputeTimeFromSeconds();
    }

    void ComputeTimeFromSeconds()
    {
        dayFraction = Mathf.Clamp01(secondsElapsedToday / realSecondsPerDay);
        float totalGameMinutes = dayFraction * 24f * 60f;
        int hour = Mathf.FloorToInt(totalGameMinutes / 60f) % 24;
        int minute = Mathf.FloorToInt(totalGameMinutes % 60f);

        if (hour != currentHour || minute != currentMinute)
        {
            currentHour = hour;
            currentMinute = minute;
            OnTimeChanged?.Invoke(currentHour, currentMinute);
        }

        OnDayFractionChanged?.Invoke(dayFraction);

        DayPhase phaseNow = CalculatePhase(currentHour);
        if (phaseNow != CurrentPhase)
        {
            CurrentPhase = phaseNow;
            OnPhaseChanged?.Invoke(CurrentPhase);
        }
    }

    DayPhase CalculatePhase(int hour)
    {
        // Customizable thresholds:
        // Pagi: 06:00 - 11:59
        // Siang: 12:00 - 15:59
        // Sore: 16:00 - 18:59  
        // Malam: 19:00 - 05:59
        if (hour >= 6 && hour < 12) return DayPhase.Pagi;
        if (hour >= 12 && hour < 16) return DayPhase.Siang;
        if (hour >= 16 && hour < 19) return DayPhase.Sore;
        return DayPhase.Malam;
    }

    public float GetDayFractionForTime(int hour, int minute)
    {
        float totalMinutes = hour * 60 + minute;
        return totalMinutes / (24f * 60f);
    }

    public void SetTime(int hour, int minute)
    {
        secondsElapsedToday = (hour * 60f + minute) / (24f * 60f) * realSecondsPerDay;
        ComputeTimeFromSeconds();
    }
}