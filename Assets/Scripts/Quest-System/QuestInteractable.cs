using UnityEngine;

public class QuestInteractable : MonoBehaviour
{
    public string objectID;
    public DialoguePopup dialoguePopup;

    private void OnMouseDown()
    {
        var quest = QuestManager.Instance.GetQuestRequiringObject(objectID);

        if (quest != null)
        {
            dialoguePopup.OpenDialogue(quest, this);
        }
        else
        {
            Debug.Log("No quest requires this object right now.");
        }
    }
}