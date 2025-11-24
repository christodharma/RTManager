using UnityEngine;

[System.Serializable]
public class DialogueOption
{
    public string buttonText = "OK";
    public bool completesQuest = false;
    public bool failQuest = false;
}

[System.Serializable]
public class QuestDialogueData
{
    [TextArea(3, 6)]
    public string dialogueText = "Default dialogue...";

    // Supports multiple buttons (ex: Yes/No or 3+ choices later)
    public DialogueOption[] options = new DialogueOption[]
    {
        new DialogueOption(){ buttonText = "Yes", completesQuest = true },
        new DialogueOption(){ buttonText = "No", failQuest = true }
    };
}