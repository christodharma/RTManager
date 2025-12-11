using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestLogEntryUI : MonoBehaviour
{
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI rewardText;
    public TextMeshProUGUI deadlineText;
    public Button trackButton;

    private QuestData quest;
    private float refreshTimer;

    void Update()
    {
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

        if (trackButton != null)
        {
            trackButton.onClick.RemoveAllListeners();
            trackButton.onClick.AddListener(OnTrackClicked);
        }

        UpdateDeadlineText();
    }

    private void OnTrackClicked()
    {
        if (quest == null || string.IsNullOrEmpty(quest.targetObjectID))
        {
            NotificationSystem.Instance.ShowNotification("<color=yellow>Target Quest belum ditentukan.</color>");
            return;
        }

        GameObject targetObject = GameObject.Find(quest.targetObjectID);

        if (targetObject != null)
        {
            QuestTracker.Instance.TrackQuest(targetObject.transform);

            NotificationSystem.Instance.ShowNotification($"Tracking: {quest.title}");
        }
        else
        {
            Debug.LogError($"[Quest Tracker Error] Objek '{quest.targetObjectID}' tidak ditemukan di scene. Pastikan nama GameObject sama persis!");
            NotificationSystem.Instance.ShowNotification($"<color=red>Target NPC tidak ditemukan!</color>");
        }
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