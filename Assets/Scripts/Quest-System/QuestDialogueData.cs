using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class DialogueOption
{
    public string buttonText = "OK";
    public bool completesQuest = false;
    public bool failQuest = false;
    public int costRupiah = 0;

    [TextArea(2, 5)]
    public string responseText = ""; // NEW: optional NPC reply after select
}

[System.Serializable]
public class QuestDialogueData
{
    public Sprite npcImage;
    public string npcName;

    [Header("Single-Stage Dialogue")]
    [TextArea(3, 6)]
    public string dialogueText = "Default dialogue...";
    public DialogueOption[] options = new DialogueOption[]
    {
        new DialogueOption(){ buttonText = "Yes", completesQuest = true },
        new DialogueOption(){ buttonText = "No", failQuest = true }
    };

    [Header("Outcome Responses")]
    [TextArea(2, 5)] public string successResponse = "Thank you so much! You helped me a lot!";
    [TextArea(2, 5)] public string failureResponse = "Oh... I see. That's disappointing.";

    [Header("Multi-Stage Optional")]
    public QuestStageDialogue[] stageDialogues;
}