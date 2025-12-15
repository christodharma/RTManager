using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public enum QuestState
{
    Active,
    Completed,
    Failed,
    Refused
}

public class QuestManager : MonoBehaviour, IPersistable
{
    public static QuestManager Instance { get; private set; }

    public List<QuestData> activeQuests = new List<QuestData>();
    public List<QuestData> completedQuests = new List<QuestData>();
    public List<QuestData> failedQuests = new List<QuestData>();
    public List<QuestData> refusedQuests = new List<QuestData>();
    public List<QuestData> availableQuests = new List<QuestData>();

    public QuestDatabase questDatabase;
    public GameTimeManager timeManager;
    public int CurrentDay => GameTimeManager.Instance.currentDay;
    public int startingMoneyOfDay;
    private List<string> dailyCompletedQuests = new List<string>();

    public DailyReport TodayReport { get; private set; } = new DailyReport();

    // Events (for UI auto-refresh)
    public System.Action OnQuestListChanged;

    public event System.Action<QuestData> QuestFailed;
    public event System.Action<QuestData> QuestSucceed;

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
                q.state = QuestState.Failed;
                failedQuests.Add(q);
                activeQuests.RemoveAt(i);

                NotificationSystem.Instance.ShowNotification($"Quest Failed: <b>{q.title}</b>");
                OnQuestListChanged?.Invoke();
            }
        }
    }

    public QuestData GetQuestRequiringObject(string id)
    {
        foreach (var quest in activeQuests)
        {
            if (quest.isMultiStage)
            {
                var stage = quest.CurrentStage;

                if (stage.requiredItemID == id || quest.targetObjectID == id)
                    return quest;
            }

            if (quest.completesOnObjectClick && quest.targetObjectID == id)
                return quest;
        }
        return null;
    }

    public void CompleteQuest(QuestData quest)
    {
        if (quest.isMultiStage && !quest.IsFinalStage)
        {
            Debug.LogWarning($"Attempting to 'Complete' Quest {quest.questID}...");
            return;
        }

        quest.state = QuestState.Completed;
        activeQuests.Remove(quest);
        completedQuests.Add(quest);

        if (questDatabase.GetQuestByID(quest.questID).isRepeatable)
        {
            if (!dailyCompletedQuests.Contains(quest.questID))
            {
                dailyCompletedQuests.Add(quest.questID);
            }
        }

        TodayReport.completedQuests++;
        TodayReport.hamEarned += quest.rewardHAM;

        HAMGradeManager.Instance.Add(quest.rewardHAM);

        NotificationSystem.Instance.ShowNotification($"Quest Completed: <b>{quest.title}</b>");
        QuestSucceed?.Invoke(quest);

        QuestTracker.Instance.StopTracking();
    }

    public void FailQuest(QuestData quest)
    {
        quest.state = QuestState.Failed;
        activeQuests.Remove(quest);
        failedQuests.Add(quest);

        TodayReport.failedQuests++;

        NotificationSystem.Instance.ShowNotification($"Quest Failed: <b>{quest.title}</b>");
        QuestFailed?.Invoke(quest);
    }

    public List<QuestData> GetActiveQuestsSorted()
    {
        return activeQuests
            .OrderBy(q => q.deadlineHour < 0 ? float.MaxValue : q.deadlineHour)
            .ToList();
    }

    public List<QuestData> GetAvailableQuestsSorted()
    {
        return availableQuests
            .OrderBy(q => q.deadlineHour < 0 ? float.MaxValue : q.deadlineHour)
            .ToList();
    }

    public List<QuestData> GetCompletedQuestsSorted()
    {
        return completedQuests
            .OrderBy(q => q.deadlineHour < 0 ? float.MaxValue : q.deadlineHour)
            .ToList();
    }

    public List<QuestData> GetFailedQuestsSorted()
    {
        return failedQuests
            .OrderBy(q => q.deadlineHour < 0 ? float.MaxValue : q.deadlineHour)
            .ToList();
    }

    private int GetDailyQuestCount()
    {
        return Random.Range(2, 5);
    }

    public void GenerateDailyQuests()
    {
        Debug.Log($"Generating quests for Day {CurrentDay}...");

        List<QuestObject> eligible = GetEligibleQuestsFromDatabase();

        if (eligible.Count == 0)
        {
            Debug.LogWarning("No eligible quests found for today!");
            return;
        }

        int questCount = Mathf.Min(Random.Range(2, 5), eligible.Count);

        availableQuests.Clear();

        for (int i = 0; i < questCount; i++)
        {
            QuestObject chosen = PickWeightedQuest(eligible);

            if (chosen == null)
                break;

            if (chosen.isRepeatable && dailyCompletedQuests.Contains(chosen.questID))
            {
                i--;
                eligible.Remove(chosen);
                continue;
            }

            availableQuests.Add(new QuestData(chosen));

            if (!chosen.isRepeatable)
                eligible.Remove(chosen);
        }

        Debug.Log($"Spawned {availableQuests.Count} quests for Day {CurrentDay}");

        OnQuestListChanged?.Invoke();
    }

    public bool HasQuestBeenCompletedToday(string questID)
    {
        if (dailyCompletedQuests == null) return false;
        return dailyCompletedQuests.Contains(questID);
    }

    private List<QuestObject> GetEligibleQuestsFromDatabase()
    {
        List<QuestObject> result = new List<QuestObject>();

        foreach (var quest in questDatabase.quests)
        {
            // Skip quests outside difficulty range based on day
            if (!IsDifficultyAllowed(quest.difficulty))
                continue;

            // Prevent duplicates in same day
            bool alreadySpawnedToday = availableQuests.Exists(q => q.questID == quest.questID);
            if (alreadySpawnedToday && !quest.isRepeatable)
                continue;

            // Repeatable quests always allowed
            if (quest.isRepeatable)
            {
                result.Add(quest);
                continue;
            }

            // Skip permanent completed or refused quests
            bool alreadyCompleted = completedQuests.Exists(q => q.questID == quest.questID);
            bool alreadyActive = activeQuests.Exists(q => q.questID == quest.questID);
            bool refused = refusedQuests.Exists(q => q.questID == quest.questID);

            if (!alreadyCompleted && !alreadyActive && !refused && quest.canSpawn)
                result.Add(quest);
        }

        return result;
    }

    private QuestObject PickWeightedQuest(List<QuestObject> pool)
    {
        List<QuestObject> weighted = new List<QuestObject>();

        foreach (var q in pool)
        {
            int weight = q.difficulty switch
            {
                QuestObject.Difficulty.Easy => 5,
                QuestObject.Difficulty.Medium => CurrentDay >= 6 ? 3 : 0,
                QuestObject.Difficulty.Hard => CurrentDay >= 11 ? 1 : 0,
                _ => 1
            };

            // Add extra weight for repeatables if allowed
            if (q.isRepeatable && weight > 0)
                weight += 2;

            // Prevent zero-weight quests from being included
            if (weight <= 0)
                continue;

            for (int i = 0; i < weight; i++)
                weighted.Add(q);
        }

        if (weighted.Count == 0)
            return null;

        return weighted[Random.Range(0, weighted.Count)];
    }

    private bool IsDifficultyAllowed(QuestObject.Difficulty difficulty)
    {
        if (CurrentDay <= 5)
            return difficulty == QuestObject.Difficulty.Easy;

        if (CurrentDay <= 10)
            return difficulty == QuestObject.Difficulty.Easy ||
                   difficulty == QuestObject.Difficulty.Medium;

        return difficulty == QuestObject.Difficulty.Easy ||
               difficulty == QuestObject.Difficulty.Medium ||
               difficulty == QuestObject.Difficulty.Hard;
    }

    public void FinalizeDayReport()
    {
        var report = QuestManager.Instance.TodayReport;

        // Apply grade based on HAM points
        report.grade = HAMGradeManager.Instance.GetGrade();

        // Give money according to grade rules
        ResourceManager.Instance.GetMoneyFromGrade();

        // Get final money earned value (difference)
        report.moneyEarned = (int)ResourceManager.Instance.CurrentMoney - startingMoneyOfDay;
    }

    public void ResetDailyReport()
    {
        TodayReport = new DailyReport();
        startingMoneyOfDay = (int)ResourceManager.Instance.CurrentMoney;

        dailyCompletedQuests.Clear();
    }

    public void Save(ref GameData data)
    {
        data.activeQuests = activeQuests;
        data.availableQuests = availableQuests;
        data.completedQuests = completedQuests;
        data.failedQuests = failedQuests;
        data.refusedQuests = refusedQuests;
    }

    public void Load(GameData data)
    {
        activeQuests = data.activeQuests;
        availableQuests = data.availableQuests;
        completedQuests = data.completedQuests;
        failedQuests = data.failedQuests;
        refusedQuests = data.refusedQuests;
    }
}