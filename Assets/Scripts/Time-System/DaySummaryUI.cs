using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class DaySummaryUI : MonoBehaviour
{
    public GameObject panel;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI summaryText;
    public Button nextDayButton;

    public GameObject MainControllerUI;
    public GameObject playerController;
    public GameObject buttonGroup;

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
        var report = QuestManager.Instance.TodayReport;

        panel.SetActive(true);
        MainControllerUI.SetActive(false);
        playerController.SetActive(false);
        buttonGroup.SetActive(false);

        titleText.text = $"Hari {GameTimeManager.Instance.currentDay} Berakhir";

        string feedbackText = report.npcFeedback.Count > 0
            ? string.Join("\n", report.npcFeedback)
            : "Tidak ada feedback hari ini.";

        summaryText.text =
            $"Ringkasan Hari Ini</b>\n" +
            $"Quest Selesai : <b>{report.completedQuests}</b>\n" +
            $"Quest Gagal : <b>{report.failedQuests}</b>\n\n" +

            $"<b>HAM Didapat:</b> {report.hamEarned}\n" +
            $"<b>Grade:</b> {report.grade}\n" +
            $"<b>Total Uang:</b> {CurrencyFormatter.ToRupiah(ResourceManager.Instance.CurrentMoney)}\n\n" +

            $"<b>Feedback Warga:</b>\n{feedbackText}";

        Time.timeScale = 0f;
    }

    void OnNextDay()
    {
        panel.SetActive(false);
        Time.timeScale = 1f;
        MainControllerUI.SetActive(true);
        playerController.SetActive(true);
        buttonGroup.SetActive(true);
        GameTimeManager.Instance.StartNextDay();
    }

    void OnDestroy()
    {
        if (GameTimeManager.Instance != null)
            GameTimeManager.Instance.OnDayEnded -= ShowSummary;
    }
}

public class DailyReport
{
    public int completedQuests;
    public int failedQuests;
    public int hamEarned;
    public HAMGrade grade;
    public float moneyEarned;
    public List<string> npcFeedback = new List<string>();
}
