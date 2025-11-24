using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class QuestBoxUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject questBoxPanel;
    public Transform questListContainer; // ScrollView → Content
    public GameObject questEntryPrefab;
    public Button closeButton;

    [Header("Runtime")]
    private List<QuestData> availableQuests = new List<QuestData>();

    void Start()
    {
        questBoxPanel.SetActive(false);
        if (closeButton != null)
            closeButton.onClick.AddListener(CloseBox);
    }

    public void CloseBox()
    {
        questBoxPanel.SetActive(false);
    }

    public void OpenQuestBox(List<QuestData> quests)
    {
        gameObject.SetActive(true); // Ensure UI script is active

        availableQuests = quests
            .OrderBy(q => q.deadlineHour < 0 ? float.MaxValue : q.deadlineHour)
            .ToList();

        questBoxPanel.SetActive(true);

        // Refresh UI entries
        foreach (Transform child in questListContainer)
            Destroy(child.gameObject);

        foreach (var quest in availableQuests)
        {
            var entry = Instantiate(questEntryPrefab, questListContainer);
            var entryUI = entry.GetComponent<QuestBoxEntryUI>();
            entryUI.Setup(quest, this);
        }
    }

    public void AcceptQuest(QuestData quest)
    {
        float now = GameTimeManager.Instance.GetTotalGameHours();
        quest.deadlineHour = now + quest.deadlineHour;
        quest.state = QuestState.Active;

        QuestManager.Instance.AddQuest(quest);
        NotificationSystem.Instance.ShowNotification($"Accepted: {quest.title}");
        RefreshAvailableList(quest);
    }

    public void RefuseQuest(QuestData quest)
    {
        quest.state = QuestState.Refused;
        QuestManager.Instance.AddQuest(quest);

        NotificationSystem.Instance.ShowNotification($"Quest Refused: <b>{quest.title}</b>");
        RefreshAvailableList(quest);
    }

    private void RefreshAvailableList(QuestData handledQuest)
    {
        availableQuests.Remove(handledQuest);

        // Re-sort list after removal
        availableQuests = availableQuests
            .OrderBy(q => q.deadlineHour < 0 ? float.MaxValue : q.deadlineHour)
            .ToList();

        foreach (Transform child in questListContainer)
            Destroy(child.gameObject);

        foreach (var quest in availableQuests)
        {
            var entry = Instantiate(questEntryPrefab, questListContainer);
            entry.GetComponent<QuestBoxEntryUI>().Setup(quest, this);
        }

        if (availableQuests.Count == 0)
            gameObject.SetActive(false);
    }
}
