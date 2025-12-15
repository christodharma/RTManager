using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public enum Speaker
{
    NPC1 = 0, // Pembicara Utama
    NPC2 = 1, // Pembicara Kedua
    Player = 2, // Pemain (Opsional)
    NPC3 = 3 //Pembicara Ketiga
}

[System.Serializable]
public class DialogueOption
{
    public string buttonText = "OK";

    [Header("Branching Logic")]
    [Tooltip("Index dari Dialogue Node selanjutnya. Isi -1 jika ini akhir percakapan.")]
    public int nextNodeIndex = -1;

    [Header("Quest Outcome (Only if ending)")]
    public bool completesQuest = false;
    public bool failQuest = false;
    public int costRupiah = 0;

    [TextArea(2, 5)]
    public string responseText = "";
}

[System.Serializable]
public class DialogueNode
{
    [Tooltip("Tentukan NPC mana yang sedang berbicara untuk baris ini.")]
    public Speaker speaker = Speaker.NPC1;

    [TextArea(3, 6)]
    public string dialogueText;
    public DialogueOption[] options;
}

[System.Serializable]
public class QuestStageDialogue
{
    [Header("NPC 1 Identity (Primary Speaker)")]
    public Sprite npcImage;
    public string npcName;

    [Header("NPC 2 Identity (Secondary Speaker)")]
    public Sprite secondaryNpcImage;
    public string secondaryNpcName;

    [Header("Single-Stage Dialogue / Start Dialogue")]
    [TextArea(3, 6)]
    public string dialogueText = "Default dialogue...";
    public DialogueOption[] options;

    [Header("Outcome Responses")]
    [TextArea(2, 5)] public string successResponse = "Thank you so much! You helped me a lot!";
    [TextArea(2, 5)] public string failureResponse = "Oh... I see. That's disappointing.";

    [Header("Optional Branching Dialogue System")]
    [Tooltip("Jika diisi, ini akan menggantikan Single-Stage Dialogue di atas.")]
    public List<DialogueNode> conversationNodes;
}

[System.Serializable]
public class QuestDialogueData
{
    [Header("Dialogues for Quest Stages")]
    [Tooltip("Dialog untuk tahap quest 1, 2, dst. Stage index 0 akan menggunakan elemen [0].")]
    public QuestStageDialogue[] stageDialogues;

    [Header("Final Quest Completion Dialogue")]
    public QuestStageDialogue finalCompletionDialogue;

}