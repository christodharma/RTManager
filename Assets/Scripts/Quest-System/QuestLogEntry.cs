using UnityEngine;
using TMPro;

public class QuestLogEntryUI : MonoBehaviour
{
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI rewardText;
    public TextMeshProUGUI deadlineText;

    private QuestData quest;
    private float refreshTimer;

    void Update()
    {
        // Refresh once per second
        refreshTimer += Time.deltaTime;
        if (refreshTimer >= 1f)
        {
            refreshTimer = 0f;
            if (quest != null && quest.state == QuestState.Active)
                UpdateDeadlineText();
        }
    }

    public void Setup(QuestData data)
    {
        quest = data;
        titleText.text = quest.title;
        rewardText.text = $"Reward: {quest.rewardHAM} HAM Points";
        UpdateDeadlineText();
    }

    private void UpdateDeadlineText()
    {
        if (quest == null) return;

        float currentHours = GameTimeManager.Instance.GetTotalGameHours();
        float remaining = quest.deadlineHour - currentHours;

        if (remaining <= 0f)
        {
            deadlineText.text = "<color=red>Expired</color>";
        }
        else
        {
            int hours = Mathf.FloorToInt(remaining);
            int minutes = Mathf.FloorToInt((remaining - hours) * 60f);
            deadlineText.text = $"Deadline: {hours:D2} h {minutes:D2} m left";
        }
    }
}