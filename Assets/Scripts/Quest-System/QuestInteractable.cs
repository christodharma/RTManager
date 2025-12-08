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

            interactButton.gameObject.SetActive(hasQuestToOffer);

            playerInRange = true;

            // >> LOGIKA PENAMBAHAN/PENGHAPUSAN LISTENER
            if (hasQuestToOffer && !isListening)
            {
                // Tambahkan listener HANYA jika ada Quest dan belum ditambahkan
                interactButton.onClick.AddListener(Interact);
                isListening = true;
            }
            else if (!hasQuestToOffer && isListening)
            {
                // Hapus listener jika sudah tidak ada Quest yang ditawarkan (atau dihilangkan oleh logika lain)
                interactButton.onClick.RemoveListener(Interact);
                isListening = false;
            }
        }
        else if (!shouldBeInRange && playerInRange)
        {
            playerInRange = false;
            interactButton.gameObject.SetActive(false);

            // >> PENGHAPUSAN LISTENER KETIKA KELUAR JANGKAUAN
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

        if (interactButton != null)
        {
            interactButton.gameObject.SetActive(false);
        }

        if (quest != null)
        {
            Debug.Log($"[DEBUG INTERACT] NPC {objectID} memicu Quest: {quest.title}");
            dialoguePopup.OpenDialogue(quest, this);
        }
        else
        {
            Debug.Log("No active quest requires this object.");
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