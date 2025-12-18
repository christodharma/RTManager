using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestLogEntryUI : MonoBehaviour
{
    [Header("UI Components")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI rewardText;
    public TextMeshProUGUI deadlineText;
    public TextMeshProUGUI objectiveText;

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

        UpdateObjectiveText();

        if (trackButton != null)
        {
            trackButton.onClick.RemoveAllListeners();
            trackButton.onClick.AddListener(OnTrackClicked);
        }

        UpdateDeadlineText();
    }

    private void UpdateObjectiveText()
    {
        if (objectiveText == null) return;

        QuestObject sourceQuest = QuestManager.Instance.questDatabase.GetQuestByID(quest.questID);

        if (sourceQuest == null)
        {
            objectiveText.text = "Quest details not found.";
            return;
        }

        if (sourceQuest.isMultiStage && sourceQuest.stages != null && sourceQuest.stages.Length > 0)
        {
            int index = Mathf.Clamp(quest.currentStageIndex, 0, sourceQuest.stages.Length - 1);

            string currentObjective = sourceQuest.stages[index].objectiveDescription;

            objectiveText.text = $"<color=yellow>[Stage {index + 1}]</color> {currentObjective}";
        }
        else
        {
            objectiveText.text = sourceQuest.description;
        }
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