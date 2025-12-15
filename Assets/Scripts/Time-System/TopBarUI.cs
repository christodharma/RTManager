using UnityEngine;
using TMPro;

public class TopBarUI : MonoBehaviour
{
    [Header("References")]
    public TextMeshProUGUI dayText;
    public TextMeshProUGUI clockText;
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI phaseText;

    void Start()
    {
        if (GameTimeManager.Instance != null)
        {
            GameTimeManager.Instance.OnDayChanged += UpdateDay;
            GameTimeManager.Instance.OnTimeChanged += UpdateClock;
            GameTimeManager.Instance.OnPhaseChanged += UpdatePhaseText;

            UpdateDay(GameTimeManager.Instance.currentDay);
            UpdateClock(GameTimeManager.Instance.currentHour, GameTimeManager.Instance.currentMinute);
            UpdatePhaseText(GameTimeManager.Instance.CurrentPhase);
        }

        if (ResourceManager.Instance != null)
        {
            ResourceManager.Instance.OnMoneyChanged += UpdateMoney;
            UpdateMoney(ResourceManager.Instance.CurrentMoney); // initialize UI
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

    void UpdateDay(int day)
    {
        dayText.text = $"Day {day}";
    }

    public void UpdateMoney(float amount)
    {
        moneyText.text = $"Rp {amount:N0}";
    }

    void OnDestroy()
    {
        if (GameTimeManager.Instance != null)
        {
            GameTimeManager.Instance.OnTimeChanged -= UpdateClock;
            GameTimeManager.Instance.OnPhaseChanged -= UpdatePhaseText;
            GameTimeManager.Instance.OnDayChanged -= UpdateDay;
        }

        if (ResourceManager.Instance != null)
        {
            ResourceManager.Instance.OnMoneyChanged -= UpdateMoney;
        }
    }
}
