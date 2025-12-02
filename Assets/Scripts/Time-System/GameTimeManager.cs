using UnityEngine;
using System;
using System.Collections;

public enum DayPhase { Pagi, Siang, Sore, Malam }

public class GameTimeManager : MonoBehaviour, IPersistable
{
    public static GameTimeManager Instance { get; private set; }

    [Header("Day Settings")]
    public float realSecondsPerDay = 360f; // 6 minutes = full day
    public int startHour = 7; // Start of day (Pagi)
    public int startMinute = 0;
    public int endHour = 19;  // End of day (before Malam)

    [Header("Runtime (read-only)")]
    public int currentHour;
    public int currentMinute;
    public int currentDay = 1;
    [Range(0f, 1f)] public float dayFraction; // 0..1 across the 24h

    public event Action<int, int> OnTimeChanged; // hour, minute
    public event Action<int> OnDayChanged;
    public event Action<DayPhase> OnPhaseChanged;
    public event Action<float> OnDayFractionChanged; // 0..1

    public DayPhase CurrentPhase { get; private set; }

    private float secondsElapsedToday;

    public bool isDayPaused = false; // whether time is stopped
    public event Action OnDayEnded;  // event to trigger day summary UI

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        secondsElapsedToday = 0;
        ComputeTimeFromSeconds();
    }

    void Update()
    {
        if (isDayPaused) return;

        secondsElapsedToday += Time.deltaTime;
        ComputeTimeFromSeconds();
    }

    void ComputeTimeFromSeconds()
    {
        dayFraction = Mathf.Clamp01(secondsElapsedToday / realSecondsPerDay);

        // Map real-time 0–1 to in-game startHour–endHour
        float totalGameMinutes = Mathf.Lerp(startHour * 60, endHour * 60, dayFraction);

        int hour = Mathf.FloorToInt(totalGameMinutes / 60f) % 24;
        int minute = Mathf.FloorToInt(totalGameMinutes % 60f);

        if (hour != currentHour || minute != currentMinute)
        {
            currentHour = hour;
            currentMinute = minute;
            OnTimeChanged?.Invoke(currentHour, currentMinute);
        }

        OnDayFractionChanged?.Invoke(dayFraction);

        // Detect if day is still within the active window
        if (dayFraction >= 1f)
        {
            // 🔹 Force Malam once full day is complete
            if (CurrentPhase != DayPhase.Malam)
            {
                CurrentPhase = DayPhase.Malam;
                OnPhaseChanged?.Invoke(CurrentPhase);
                EndDay();
            }
            return;
        }

        // Otherwise, calculate phase normally
        DayPhase phaseNow = CalculatePhase(hour);
        if (phaseNow != CurrentPhase)
        {
            CurrentPhase = phaseNow;
            OnPhaseChanged?.Invoke(CurrentPhase);
        }
    }

    public void EndDay()
    {
        if (isDayPaused) return;

        isDayPaused = true;
        Debug.Log($"Day {currentDay} ended at {currentHour:D2}:{currentMinute:D2}");

        // Begin fade to black, then show summary
        StartCoroutine(HandleEndDayTransition());
    }

    IEnumerator HandleEndDayTransition()
    {
        // Fade screen to black first
        if (FadeTransition.Instance != null)
            yield return FadeTransition.Instance.FadeOut();

        // --- STEP 1: Finalize Daily Progress Before UI Shows ---
        QuestManager.Instance.FinalizeDayReport();

        // --- STEP 2: Trigger UI (DaySummaryUI listens to this) ---
        OnDayEnded?.Invoke();

        // --- STEP 3: Reset report AFTER summary is created ---
        QuestManager.Instance.ResetDailyReport();

        // Gameplay stays paused here — UI now controls "Next Day"
    }

    public void StartNextDay()
    {
        QuestManager.Instance.ResetDailyReport();

        StartCoroutine(HandleStartNextDay());
        QuestManager.Instance.GenerateDailyQuests();
    }

    IEnumerator HandleStartNextDay()
    {
        // Reset before fading back
        secondsElapsedToday = 0f;
        dayFraction = 0f;
        isDayPaused = false;
        currentDay++;

        OnDayChanged?.Invoke(currentDay);

        currentHour = startHour;
        currentMinute = 0;
        OnTimeChanged?.Invoke(currentHour, currentMinute);
        OnDayFractionChanged?.Invoke(dayFraction);
        CurrentPhase = DayPhase.Pagi;
        OnPhaseChanged?.Invoke(CurrentPhase);

        Debug.Log($"Starting Day {currentDay} at {currentHour:D2}:{currentMinute:D2}");

        if (FadeTransition.Instance != null)
            yield return FadeTransition.Instance.FadeIn();

        ComputeTimeFromSeconds();
    }

    DayPhase CalculatePhase(int hour)
    {
        // Malam if beyond endHour
        if (hour >= endHour || hour < startHour)
            return DayPhase.Malam;

        // Calculate relative fraction of current time within day span
        float totalDayHours = endHour - startHour; // e.g., 12 hours (07–19)
        float currentHourFraction = (hour + (currentMinute / 60f) - startHour) / totalDayHours;

        // Divide day into three equal phases
        if (currentHourFraction < 1f / 3f)
            return DayPhase.Pagi;  // 07–10:59
        else if (currentHourFraction < 2f / 3f)
            return DayPhase.Siang; // 11–14:59
        else
            return DayPhase.Sore;  // 15–18:59
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

    public float GetTotalGameMinutes()
    {
        // Convert current in-game day time into total minutes since start.
        float minutesToday = (currentHour * 60f) + currentMinute;
        return (currentDay * 24f * 60f) + minutesToday;
    }

    public float GetTotalGameHours()
    {
        return GetTotalGameMinutes() / 60f;
    }

    public void Save(ref GameData data)
    {
        data.secondsElapsedToday = secondsElapsedToday;
        data.currentDay = currentDay;
    }

    public void Load(GameData data)
    {
        secondsElapsedToday = data.secondsElapsedToday;
        currentDay = data.currentDay;
    }
}