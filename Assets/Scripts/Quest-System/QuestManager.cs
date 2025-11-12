using System.Collections.Generic;
using UnityEngine;

public enum QuestState
{
    Active,
    Completed,
    Failed,
    Refused
}

[System.Serializable]
public class QuestData
{
    public string title;
    public string description;
    public int rewardHAM;
    public float deadlineHour;
    public QuestState state;

    public QuestData(string title, string description, int rewardHAM, float deadlineHour, QuestState state)
    {
        this.title = title;
        this.description = description;
        this.rewardHAM = rewardHAM;
        this.deadlineHour = deadlineHour;
        this.state = state;
    }
}

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    public List<QuestData> activeQuests = new List<QuestData>();
    public List<QuestData> completedQuests = new List<QuestData>();
    public List<QuestData> failedQuests = new List<QuestData>();
    public List<QuestData> refusedQuests = new List<QuestData>();

    // Events (for UI auto-refresh)
    public System.Action OnQuestListChanged;

    // Dummy quest for testing
    void Start()
    {
        float now = GameTimeManager.Instance.GetTotalGameHours();

        AddQuest(new QuestData("Konflik parkir", "Mediasi konflik parkir tetangga", 50, now + 1f, QuestState.Active));
        AddQuest(new QuestData("Buat surat keterangan RT", "Failed in time.", 120, 5f, QuestState.Completed));
        AddQuest(new QuestData("Bantuan buku", "Failed in time.", 120, 5f, QuestState.Refused));
    }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Update()
    {
        CheckDeadlines();
    }

    public void AddQuest(QuestData quest)
    {
        switch (quest.state)
        {
            case QuestState.Active:
                activeQuests.Add(quest);
                break;
            case QuestState.Completed:
                completedQuests.Add(quest);
                break;
            case QuestState.Failed:
                failedQuests.Add(quest);
                break;
            case QuestState.Refused:
                refusedQuests.Add(quest);
                break;
        }

        OnQuestListChanged?.Invoke();
    }

    private void CheckDeadlines()
    {
        if (activeQuests.Count == 0) return;

        float currentTime = GameTimeManager.Instance.GetTotalGameHours();

        for (int i = activeQuests.Count - 1; i >= 0; i--)
        {
            QuestData q = activeQuests[i];
            if (currentTime > q.deadlineHour && q.state == QuestState.Active)
            {
                // Mark as failed
                q.state = QuestState.Failed;
                failedQuests.Add(q);
                activeQuests.RemoveAt(i);

                Debug.Log($"Quest failed: {q.title}");
                OnQuestListChanged?.Invoke();
            }
        }
    }
}