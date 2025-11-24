using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialoguePopup : MonoBehaviour
{
    [Header("UI")]
    public GameObject DialoguePopupPanel;
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI nameText;
    public Image portraitImage;
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

        // Apply NPC display data
        nameText.text = quest.completionDialogue.npcName;

        if (quest.completionDialogue.npcImage != null)
        {
            portraitImage.sprite = quest.completionDialogue.npcImage;
            portraitImage.gameObject.SetActive(true);
        }
        else
        {
            portraitImage.gameObject.SetActive(false);
        }

        dialogueText.text = quest.completionDialogue.dialogueText;

        // Remove old buttons
        foreach (var b in activeButtons)
            Destroy(b);

        activeButtons.Clear();

        foreach (var option in quest.completionDialogue.options)
        {
            GameObject btnObj = Instantiate(buttonPrefab, buttonContainer);
            activeButtons.Add(btnObj);

            var text = btnObj.GetComponentInChildren<TextMeshProUGUI>();
            var button = btnObj.GetComponent<Button>();

            // Show cost if the option has one
            if (option.costRupiah > 0)
                text.text = $"{option.buttonText} ({CurrencyFormatter.ToRupiah(option.costRupiah)})";
            else
                text.text = option.buttonText;

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => HandleOption(option));
        }

        DialoguePopupPanel.SetActive(true);
    }

    private void HandleOption(DialogueOption option)
    {
        // Debug cost (money system can be added later)
        if (option.costRupiah > 0)
        {
            Debug.Log($"Paid {CurrencyFormatter.ToRupiah(option.costRupiah)}");

            // subtract money from a money manager here later
            // MoneyManager.Instance.Spend(option.costRupiah);
        }

        if (option.completesQuest)
        {
            CompleteQuest();
            return;
        }

        if (option.failQuest)
        {
            FailQuest();
            return;
        }

        CloseDialogue();
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
        DialoguePopupPanel.SetActive(false);

        foreach (var b in activeButtons)
            Destroy(b);

        activeButtons.Clear();
    }
}