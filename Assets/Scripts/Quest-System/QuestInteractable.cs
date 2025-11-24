using UnityEngine;
using UnityEngine.UI;

public class QuestInteractable : MonoBehaviour
{
    [Header("Quest Interaction Settings")]
    public string objectID;
    public DialoguePopup dialoguePopup;

    [Header("Interaction UI")]
    public Button interactButton;     // Assign in inspector
    public Transform player;          // Player reference
    public float interactDistance = 1.5f;

    private bool playerInRange = false;

    private void Start()
    {
        if (interactButton != null)
        {
            interactButton.gameObject.SetActive(false);
            interactButton.onClick.AddListener(Interact);
        }
        else
        {
            Debug.LogWarning($"No Interact Button assigned for {gameObject.name}");
        }
    }

    private void Update()
    {
        if (player == null || interactButton == null) return;

        float distance = Vector2.Distance(player.position, transform.position);

        if (distance <= interactDistance && !playerInRange)
        {
            var quest = QuestManager.Instance.GetQuestRequiringObject(objectID);
            playerInRange = true;
            if (quest != null)
            {
                interactButton.gameObject.SetActive(true);
            }
            else
            {
                interactButton.gameObject.SetActive(false);
            }
            
        }
        else if (distance > interactDistance && playerInRange)
        {
            playerInRange = false;
            interactButton.gameObject.SetActive(false);
        }
    }

    private void Interact()
    {
        var quest = QuestManager.Instance.GetQuestRequiringObject(objectID);

        if (quest != null)
        {
            dialoguePopup.OpenDialogue(quest, this);
        }
        else
        {
            Debug.Log("No active quest requires this object.");
        }
    }

    private void OnDisable()
    {
        if (interactButton != null)
            interactButton.onClick.RemoveListener(Interact);
    }
}