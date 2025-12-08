using UnityEngine;

[System.Serializable]
public class QuestStage
{
    public string objectiveDescription;
    public bool requiresItem;
    public string requiredItemID;
    public int requiredAmount;
}

[System.Serializable]
public class QuestStageDialogue
{
    [TextArea(2, 5)]
    public string dialogueText;

    public DialogueOption[] options;

    [Header("Outcome Responses")]
    [TextArea(2, 5)] public string successResponse;
    [TextArea(2, 5)] public string failureResponse;
}