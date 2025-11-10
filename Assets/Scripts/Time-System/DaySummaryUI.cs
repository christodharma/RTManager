using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DaySummaryUI : MonoBehaviour
{
    public GameObject panel;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI summaryText;
    public Button nextDayButton;

    void Start()
    {
        panel.SetActive(false);

        if (GameTimeManager.Instance != null)
        {
            GameTimeManager.Instance.OnDayEnded += ShowSummary;
        }

        nextDayButton.onClick.AddListener(OnNextDay);
    }

    void ShowSummary()
    {
        panel.SetActive(true);
        titleText.text = $"Day {GameTimeManager.Instance.currentDay} Ended";
        summaryText.text = "Summary of today’s events will go here."; // Replace with data later
        Time.timeScale = 0f; // optional: pause gameplay systems
    }

    void OnNextDay()
    {
        panel.SetActive(false);
        Time.timeScale = 1f;
        GameTimeManager.Instance.StartNextDay();
    }

    void OnDestroy()
    {
        if (GameTimeManager.Instance != null)
            GameTimeManager.Instance.OnDayEnded -= ShowSummary;
    }
}
