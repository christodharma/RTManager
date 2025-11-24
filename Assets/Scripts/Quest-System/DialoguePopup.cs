using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialoguePopup : MonoBehaviour
{
    [Header("UI")]
    public GameObject DialoguePopupPanel;
    public TextMeshProUGUI dialogueText;
    public GameObject buttonPrefab;
    public Transform buttonContainer;

    private QuestData currentQuest;
    private List<GameObject> activeButtons = new List<GameObject>();

    private void Start()
    {
        DialoguePopupPanel.SetActive(false);
    }

    public void OpenDialogue(QuestData quest, QuestInteractable source)
    {
        currentQuest = quest;

        // Set text
        dialogueText.text = quest.completionDialogue.dialogueText;

        // Remove old buttons
        foreach (var b in activeButtons)
            Destroy(b);

        activeButtons.Clear();

        // Create dynamic buttons
        foreach (var option in quest.completionDialogue.options)
        {
            GameObject btnObj = Instantiate(buttonPrefab, buttonContainer);
            activeButtons.Add(btnObj);

            var text = btnObj.GetComponentInChildren<TextMeshProUGUI>();
            var button = btnObj.GetComponent<Button>();

            text.text = option.buttonText;

            // Clear previous listeners
            button.onClick.RemoveAllListeners();

            // Assign behavior
            if (option.completesQuest)
                button.onClick.AddListener(CompleteQuest);
            else if (option.failQuest)
                button.onClick.AddListener(FailQuest);
            else
                button.onClick.AddListener(CloseDialogue);
        }

        DialoguePopupPanel.SetActive(true);
    }

    private void CompleteQuest()
    {
        currentQuest.state = QuestState.Completed;
        QuestManager.Instance.CompleteQuest(currentQuest);

        NotificationSystem.Instance.ShowNotification($"Quest Completed: <b>{currentQuest.title}</b>");

        CloseDialogue();
    }

    private void FailQuest()
    {
        currentQuest.state = QuestState.Failed;
        QuestManager.Instance.FailQuest(currentQuest);

        NotificationSystem.Instance.ShowNotification($"Quest Failed: <b>{currentQuest.title}</b>");

        CloseDialogue();
    }

    public void CloseDialogue()
    {
        gameObject.SetActive(false);

        // Optional: clear UI buttons on close to avoid ghost presses
        foreach (var b in activeButtons)
            Destroy(b);

        activeButtons.Clear();
    }
}