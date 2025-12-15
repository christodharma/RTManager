using UnityEngine;

[CreateAssetMenu(fileName = "NewQuest", menuName = "Quests/Quest")]
public class QuestObject : ScriptableObject
{
    [Header("Basic Info")]
    public string questID;
    public string title;
    [TextArea(3, 8)] public string description;

    [Header("Rewards / Cost")]
    public int rewardHAM = 0;
    public bool hasCost = false;
    public int costRupiah = 0;

    [Header("Timing")]
    public float deadlineHours = -1f; // -1 = no deadline

    public enum Difficulty { Easy, Medium, Hard }
    public Difficulty difficulty = Difficulty.Medium;

    [Header("Completion Trigger")]
    public bool completesOnObjectClick = false;
    public string targetObjectID = ""; // Must match world clickable object

    [Header("Dialogue")]
    public QuestDialogueData completionDialogue;

    [Header("Availability")]
    public bool isRepeatable = false;
    public bool canSpawn = true; // default true for normal quests

    [Header("Multi-Stage Settings")]
    public bool isMultiStage = false;
    public QuestStage[] stages;
}