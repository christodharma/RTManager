using UnityEngine;
using UnityEngine.UI;

public class QuestInteractable : MonoBehaviour
{
    [Header("Quest Interaction Settings")]
    public string objectID;
    public DialoguePopup dialoguePopup;

    [Header("Default Dialogue Settings")]
    public Sprite npcDefaultPortrait;
    public string npcDisplayName;
    [TextArea(3, 6)]
    public string defaultDialogueText = "Halo! Senang bertemu denganmu.";

    [Header("Interaction UI")]
    public Button interactButton;     // Assign in inspector
    public Transform player;          // Player reference
    public float interactDistance = 1.5f;

    private bool playerInRange = false;
    private bool isListening = false;

    private void Start()
    {
        if (interactButton != null)
        {
            interactButton.gameObject.SetActive(false);
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
        bool shouldBeInRange = distance <= interactDistance;

        if (shouldBeInRange)
        {
            var quest = QuestManager.Instance.GetQuestRequiringObject(objectID);

            bool hasQuestToOffer = (quest != null);

            interactButton.gameObject.SetActive(true);

            playerInRange = true;

            if (!isListening)
            {
                interactButton.onClick.AddListener(Interact);
                isListening = true;
            }
        }
        else if (!shouldBeInRange && playerInRange)
        {
            playerInRange = false;
            interactButton.gameObject.SetActive(false);

            if (isListening)
            {
                interactButton.onClick.RemoveListener(Interact);
                isListening = false;
            }
        }
    }

    private void Interact()
    {
        var quest = QuestManager.Instance.GetQuestRequiringObject(objectID);

        if (interactButton != null) interactButton.gameObject.SetActive(false);

        if (quest != null)
        {
            dialoguePopup.OpenDialogue(quest, this);
        }
        else
        {
            var pastQuest = QuestManager.Instance.GetPastQuestByObjectID(objectID);

            if (pastQuest != null)
            {
                dialoguePopup.OpenOutcomeDialogue(pastQuest);
            }
            else
            {
                dialoguePopup.OpenDefaultDialogue(npcDisplayName, defaultDialogueText, npcDefaultPortrait);
            }
        }
    }

    private void OnDisable()
    {
        if (interactButton != null && isListening)
        {
            interactButton.onClick.RemoveListener(Interact);
            isListening = false;
        }
    }
}