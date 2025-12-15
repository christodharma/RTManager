using UnityEngine;
using TMPro;

public class ClockUI : MonoBehaviour
{
    [Header("References")]
    public TextMeshProUGUI clockText;
    public TextMeshProUGUI phaseText;

    void Start()
    {
        if (GameTimeManager.Instance != null)
        {
            GameTimeManager.Instance.OnTimeChanged += UpdateClock;
            GameTimeManager.Instance.OnPhaseChanged += UpdatePhaseText;

            UpdateClock(GameTimeManager.Instance.currentHour, GameTimeManager.Instance.currentMinute);
            UpdatePhaseText(GameTimeManager.Instance.CurrentPhase);
        }
    }

    void UpdateClock(int hour, int minute)
    {
        clockText.text = $"{hour:00}:{minute:00}";
    }

    void UpdatePhaseText(DayPhase newPhase)
    {
        phaseText.text = newPhase.ToString();
    }

    void OnDestroy()
    {
        if (GameTimeManager.Instance != null)
        {
            GameTimeManager.Instance.OnTimeChanged -= UpdateClock;
            GameTimeManager.Instance.OnPhaseChanged -= UpdatePhaseText;
        }
    }
}
