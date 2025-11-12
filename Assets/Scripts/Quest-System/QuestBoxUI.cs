using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
            closeButton.onClick.AddListener(() => gameObject.SetActive(false));
    }

    public void OpenQuestBox(List<QuestData> quests)
    {
        availableQuests = quests;
        questBoxPanel.SetActive(true);

        // Clear old list
        foreach (Transform child in questListContainer)
            Destroy(child.gameObject);

        // Populate available quests
        foreach (var quest in availableQuests)
        {
            var entry = Instantiate(questEntryPrefab, questListContainer);
            var entryUI = entry.GetComponent<QuestBoxEntryUI>();
            entryUI.Setup(quest, this);
        }
    }

    public void AcceptQuest(QuestData quest)
    {
        // Set absolute deadline relative to current game time
        float now = GameTimeManager.Instance.GetTotalGameHours();
        quest.deadlineHour = now + quest.deadlineHour; // treat value as relative duration
        quest.state = QuestState.Active;
        QuestManager.Instance.AddQuest(quest);

        Debug.Log($"Quest accepted: {quest.title}");
        RefreshAvailableList(quest);
    }

    public void RefuseQuest(QuestData quest)
    {
        quest.state = QuestState.Refused;
        QuestManager.Instance.AddQuest(quest);

        Debug.Log($"Quest refused: {quest.title}");
        RefreshAvailableList(quest);
    }

    private void RefreshAvailableList(QuestData handledQuest)
    {
        availableQuests.Remove(handledQuest);

        // Clear and rebuild
        foreach (Transform child in questListContainer)
            Destroy(child.gameObject);

        foreach (var quest in availableQuests)
        {
            var entry = Instantiate(questEntryPrefab, questListContainer);
            entry.GetComponent<QuestBoxEntryUI>().Setup(quest, this);
        }

        // Auto-close if no more quests
        if (availableQuests.Count == 0)
            gameObject.SetActive(false);
    }
}
