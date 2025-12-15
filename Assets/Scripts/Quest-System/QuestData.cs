using UnityEngine;

[System.Serializable]
public class QuestData
{
    public string questID;
    public string title;
    public string description;

    public int rewardHAM;
    public bool hasCost;
    public int costRupiah;

    public float deadlineHour;

    public QuestState state;

    public QuestObject.Difficulty difficulty;

    public bool completesOnObjectClick;
    public string targetObjectID;

    public QuestDialogueData completionDialogue;

    public bool isMultiStage;
    public QuestStage[] stages;
    public int currentStageIndex;

    public bool IsFinalStage => currentStageIndex >= stages.Length - 1;

    public QuestStage CurrentStage => stages[currentStageIndex];

    public QuestData(QuestObject questObject)
    {
        questID = questObject.questID;
        title = questObject.title;
        description = questObject.description;

        rewardHAM = questObject.rewardHAM;
        hasCost = questObject.hasCost;
        costRupiah = questObject.costRupiah;

        deadlineHour = questObject.deadlineHours;

        difficulty = questObject.difficulty;
        completesOnObjectClick = questObject.completesOnObjectClick;
        targetObjectID = questObject.targetObjectID;

        completionDialogue = questObject.completionDialogue;

        isMultiStage = questObject.isMultiStage;
        stages = questObject.stages;
        currentStageIndex = 0;

        state = QuestState.Active;
    }
}