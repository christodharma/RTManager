using UnityEngine;

public class NPCInteractable : MonoBehaviour
{
    [Header("Identity")]
    public string npcName = "Villager";
    public Sprite npcSprite;

    [Header("Default Interaction (No Quest)")]
    [TextArea(3, 5)]
    public string defaultDialogue = "Hello traveler! Nice weather today.";

    [Header("Quest Assignment")]
    public QuestData assignedQuest;

    public void Interact()
    {
        DialoguePopup popup = FindObjectOfType<DialoguePopup>();
        if (popup == null) return;

        if (assignedQuest != null)
        {
            if (assignedQuest.state != QuestState.Inactive)
            {
                popup.OpenDialogue(assignedQuest, null);
            }
            else
            {

                popup.OpenDialogue(assignedQuest, null);
            }
        }
        else
        {
            popup.OpenSimpleDialogue(defaultDialogue, npcName, npcSprite);
        }
    }
}