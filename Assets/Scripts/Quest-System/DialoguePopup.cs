using System.Collections;
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

    private Coroutine typingRoutine;
    public float typingSpeed = 0.02f;

    private void Start()
    {
        DialoguePopupPanel.SetActive(false);
    }

    private IEnumerator TypeText(string fullText, System.Action onFinished = null)
    {
        dialogueText.text = "";

        foreach (char c in fullText)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        typingRoutine = null;
        onFinished?.Invoke();
    }

    private void ShowTextAnimated(string text, System.Action onFinished = null)
    {
        if (typingRoutine != null)
            StopCoroutine(typingRoutine);

        typingRoutine = StartCoroutine(TypeText(text, onFinished));
    }

    private void HideAllButtons()
    {
        foreach (var b in activeButtons)
            b.SetActive(false);
    }

    private void ShowAllButtons()
    {
        foreach (var b in activeButtons)
            b.SetActive(true);
    }

    public void OpenDialogue(QuestData quest, QuestInteractable source)
    {
        currentQuest = quest;

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

            foreach (var b in activeButtons)
                Destroy(b);
            activeButtons.Clear();

            foreach (var option in stageData.options)
            {
                GameObject btnObj = Instantiate(buttonPrefab, buttonContainer);
                btnObj.SetActive(false);
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

            ShowTextAnimated(stageData.dialogueText, () => ShowAllButtons());

            DialoguePopupPanel.SetActive(true);
            return;
        }

        foreach (var b in activeButtons)
            Destroy(b);
        activeButtons.Clear();

        if (quest.completionDialogue.options != null)
        {
            foreach (var option in quest.completionDialogue.options)
            {
                GameObject btnObj = Instantiate(buttonPrefab, buttonContainer);
                btnObj.SetActive(false);
                activeButtons.Add(btnObj);

                var text = btnObj.GetComponentInChildren<TextMeshProUGUI>();
                var button = btnObj.GetComponent<Button>();

                if (option.costRupiah > 0)
                    text.text = $"{option.buttonText} (Rp {option.costRupiah})";
                else
                    text.text = option.buttonText;

                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => HandleOption(option));
            }
        }

        ShowTextAnimated(quest.completionDialogue.dialogueText, () => ShowAllButtons());

        DialoguePopupPanel.SetActive(true);
    }

    private void HandleOption(DialogueOption option)
    {
        if (option.costRupiah > 0)
            ResourceManager.Instance.Subtract(option.costRupiah);

        if (!string.IsNullOrEmpty(option.responseText))
        {
            foreach (var b in activeButtons)
                Destroy(b);
            activeButtons.Clear();

            GameObject closeBtn = Instantiate(buttonPrefab, buttonContainer);
            closeBtn.SetActive(false);
            closeBtn.GetComponentInChildren<TextMeshProUGUI>().text = "Close";
            closeBtn.GetComponent<Button>().onClick.AddListener(() => FinalizeChoice(option));
            activeButtons.Add(closeBtn);

            ShowTextAnimated(option.responseText, () => ShowAllButtons());
            return;
        }

        FinalizeChoice(option);
    }

    private void HandleStageOption(DialogueOption option, QuestStageDialogue stageData)
    {
        if (option.costRupiah > 0)
            ResourceManager.Instance.Subtract(option.costRupiah);

        if (!string.IsNullOrEmpty(option.responseText))
        {
            foreach (var b in activeButtons)
                Destroy(b);
            activeButtons.Clear();

            GameObject closeBtn = Instantiate(buttonPrefab, buttonContainer);
            closeBtn.SetActive(false);
            closeBtn.GetComponentInChildren<TextMeshProUGUI>().text = "Close";
            closeBtn.GetComponent<Button>()
                .onClick.AddListener(() => FinalizeStageChoice(option, stageData));
            activeButtons.Add(closeBtn);

            ShowTextAnimated(option.responseText, () => ShowAllButtons());
            return;
        }

        FinalizeStageChoice(option, stageData);
    }

    private void FinalizeChoice(DialogueOption option)
    {
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
        if (option.failQuest)
        {
            QuestManager.Instance.TodayReport.npcFeedback.Add(
                $"{currentQuest.completionDialogue.npcName}: \"{stageData.failureResponse}\""
            );

            FailQuest();
            return;
        }

        QuestManager.Instance.TodayReport.npcFeedback.Add(
            $"{currentQuest.completionDialogue.npcName}: \"{stageData.successResponse}\""
        );

        if (currentQuest.currentStageIndex >= currentQuest.stages.Length - 1)
        {
            CompleteQuest();
            return;
        }

        currentQuest.currentStageIndex++;

        NotificationSystem.Instance.ShowNotification(
            $"Stage {currentQuest.currentStageIndex}/{currentQuest.stages.Length} complete!"
        );

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