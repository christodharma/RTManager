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

        if (quest.isMultiStage &&
            quest.stages != null &&
            quest.currentStageIndex < quest.stages.Length &&
            quest.completionDialogue.stageDialogues != null &&
            quest.currentStageIndex < quest.completionDialogue.stageDialogues.Length)
        {
            var stageData = quest.completionDialogue.stageDialogues[quest.currentStageIndex];

            dialogueText.text = stageData.dialogueText;

            // Clear previous buttons
            foreach (var b in activeButtons)
                Destroy(b);
            activeButtons.Clear();

            // Create buttons for this stage
            foreach (var option in stageData.options)
            {
                GameObject btnObj = Instantiate(buttonPrefab, buttonContainer);
                activeButtons.Add(btnObj);

                var text = btnObj.GetComponentInChildren<TextMeshProUGUI>();
                var button = btnObj.GetComponent<Button>();

                if (option.costRupiah > 0)
                    text.text = $"{option.buttonText} ({CurrencyFormatter.ToRupiah(option.costRupiah)})";
                else
                    text.text = option.buttonText;

                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => HandleStageOption(option, stageData));
            }

            DialoguePopupPanel.SetActive(true);
            return;
        }

        dialogueText.text = quest.completionDialogue.dialogueText;

        foreach (var b in activeButtons)
            Destroy(b);
        activeButtons.Clear();

        if (quest.completionDialogue.options != null)
        {
            foreach (var option in quest.completionDialogue.options)
            {
                GameObject btnObj = Instantiate(buttonPrefab, buttonContainer);
                activeButtons.Add(btnObj);

                var text = btnObj.GetComponentInChildren<TextMeshProUGUI>();
                var button = btnObj.GetComponent<Button>();

                // Handle text button & cost
                if (option.costRupiah > 0)
                    text.text = $"{option.buttonText} (Rp {option.costRupiah})";
                else
                    text.text = option.buttonText;

                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => HandleOption(option));
            }
        }

        DialoguePopupPanel.SetActive(true);
    }

    private void HandleOption(DialogueOption option)
    {
        // Cost handling
        if (option.costRupiah > 0)
            ResourceManager.Instance.Subtract(option.costRupiah);

        // Immediate short response (optional)
        if (!string.IsNullOrEmpty(option.responseText))
        {
            dialogueText.text = option.responseText;

            // Replace all buttons with a "Close" button
            foreach (var b in activeButtons)
                Destroy(b);
            activeButtons.Clear();

            GameObject closeBtn = Instantiate(buttonPrefab, buttonContainer);
            closeBtn.GetComponentInChildren<TextMeshProUGUI>().text = "Close";
            closeBtn.GetComponent<Button>().onClick.AddListener(() => FinalizeChoice(option));

            activeButtons.Add(closeBtn);
            return;
        }

        // If no responseText → finalize directly
        FinalizeChoice(option);
    }

    private void HandleStageOption(DialogueOption option, QuestStageDialogue stageData)
    {
        // Cost handling
        if (option.costRupiah > 0)
            ResourceManager.Instance.Subtract(option.costRupiah);

        // Optional immediate response
        if (!string.IsNullOrEmpty(option.responseText))
        {
            dialogueText.text = option.responseText;

            foreach (var b in activeButtons)
                Destroy(b);
            activeButtons.Clear();

            GameObject closeBtn = Instantiate(buttonPrefab, buttonContainer);
            closeBtn.GetComponentInChildren<TextMeshProUGUI>().text = "Close";
            closeBtn.GetComponent<Button>().onClick.AddListener(() =>
                FinalizeStageChoice(option, stageData));

            activeButtons.Add(closeBtn);
            return;
        }

        FinalizeStageChoice(option, stageData);
    }

    private void FinalizeChoice(DialogueOption option)
    {
        // Store final reaction for summary, not shown now
        if (option.completesQuest)
        {
            QuestManager.Instance.TodayReport.npcFeedback.Add(
                $"{currentQuest.completionDialogue.npcName}: \"{currentQuest.completionDialogue.successResponse}\""
            );
            CompleteQuest();
        }
        else if (option.failQuest)
        {
            QuestManager.Instance.TodayReport.npcFeedback.Add(
                $"{currentQuest.completionDialogue.npcName}: \"{currentQuest.completionDialogue.failureResponse}\""
            );
            FailQuest();
        }

        CloseDialogue();
    }

    private void FinalizeStageChoice(DialogueOption option, QuestStageDialogue stageData)
    {
        // Fail stage → fail full quest
        if (option.failQuest)
        {
            QuestManager.Instance.TodayReport.npcFeedback.Add(
                $"{currentQuest.completionDialogue.npcName}: \"{stageData.failureResponse}\""
            );

            FailQuest();
            return;
        }

        // Stage completes
        QuestManager.Instance.TodayReport.npcFeedback.Add(
            $"{currentQuest.completionDialogue.npcName}: \"{stageData.successResponse}\""
        );

        // Last stage → complete full quest
        if (currentQuest.currentStageIndex >= currentQuest.stages.Length - 1)
        {
            CompleteQuest();
            return;
        }

        // Advance to next stage
        currentQuest.currentStageIndex++;

        NotificationSystem.Instance.ShowNotification(
            $"Stage {currentQuest.currentStageIndex}/{currentQuest.stages.Length} complete!"
        );

        CloseDialogue(); // close dialog; player re-interacts for next stage
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