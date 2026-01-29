using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Canvas))]
public class DialoguePopup : MonoBehaviour
{
    [Header("UI")]
    public Canvas DialoguePopupCanvas;
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI nameText;
    public Image portraitImage;
    public GameObject buttonPrefab;
    public Transform buttonContainer;
    public GameObject ControllerUI;

    private QuestData currentQuest;
    private List<GameObject> activeButtons = new List<GameObject>();

    private Coroutine typingRoutine;
    public float typingSpeed = 0.02f;

    private List<DialogueNode> currentNodes;
    private QuestStageDialogue currentActiveStageData;

    [Header("Player Settings")]
    [Tooltip("Gambar/potret yang digunakan saat Player menjadi pembicara.")]
    public Sprite playerImage;
    [Tooltip("Nama yang digunakan saat Player menjadi pembicara.")]
    public string playerName = "Player";

    void Awake()
    {
        DialoguePopupCanvas.enabled = false;
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

    void Start()
    {
        if (GameTimeManager.Instance != null)
        {
            GameTimeManager.Instance.OnDayEnded += HandleDayEnded;
        }
    }

    void OnDestroy()
    {
        if (GameTimeManager.Instance != null)
        {
            GameTimeManager.Instance.OnDayEnded -= HandleDayEnded;
        }
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

    public void OpenDefaultDialogue(string name, string message, Sprite portrait)
    {
        ClearDialogueUI();

        nameText.text = name;
        if (portrait != null)
        {
            portraitImage.sprite = portrait;
            portraitImage.gameObject.SetActive(true);
        }
        else portraitImage.gameObject.SetActive(false);

        CreateSimpleCloseButton();
        ShowTextAnimated(message, () => ShowAllButtons());

        DialoguePopupCanvas.enabled = true;
        if (ControllerUI != null) ControllerUI.SetActive(false);
    }

    public void OpenOutcomeDialogue(QuestData quest)
    {
        ClearDialogueUI();

        QuestStageDialogue finalData = quest.completionDialogue.finalCompletionDialogue;

        if (finalData == null && quest.completionDialogue.stageDialogues.Length > 0)
            finalData = quest.completionDialogue.stageDialogues[quest.completionDialogue.stageDialogues.Length - 1];

        if (finalData != null)
        {
            nameText.text = finalData.npcName;
            portraitImage.sprite = finalData.npcImage;
            portraitImage.gameObject.SetActive(finalData.npcImage != null);

            string msg = (quest.state == QuestState.Completed) ? finalData.successResponse : finalData.failureResponse;

            CreateSimpleCloseButton();
            ShowTextAnimated(msg, () => ShowAllButtons());
        }

        DialoguePopupCanvas.enabled = true;
        if (ControllerUI != null) ControllerUI.SetActive(false);
    }

    public void OpenSimpleDialogue(QuestStageDialogue data)
    {
        currentQuest = null;
        currentActiveStageData = data;

        ClearDialogueUI();
        DialoguePopupCanvas.enabled = true;
        if (ControllerUI != null) ControllerUI.SetActive(false);

        if (data.conversationNodes != null && data.conversationNodes.Count > 0)
        {
            currentNodes = data.conversationNodes;
            ShowNode(0);
        }
        else
        {
            nameText.text = data.npcName;
            portraitImage.sprite = data.npcImage;
            portraitImage.gameObject.SetActive(data.npcImage != null);

            ShowTextAnimated(data.dialogueText, () => ShowAllButtons());
        }
    }

    private void ClearDialogueUI()
    {
        foreach (var b in activeButtons) Destroy(b);
        activeButtons.Clear();
    }

    private void CreateSimpleCloseButton()
    {
        GameObject btnObj = Instantiate(buttonPrefab, buttonContainer);
        btnObj.SetActive(false);
        btnObj.GetComponentInChildren<TextMeshProUGUI>().text = "Tutup";
        btnObj.GetComponent<Button>().onClick.AddListener(CloseDialogue);
        activeButtons.Add(btnObj);
    }

    public void OpenDialogue(QuestData quest, QuestInteractable source)
    {
        currentQuest = quest;
        currentActiveStageData = null;

        if (quest.isMultiStage &&
            quest.stages != null &&
            quest.currentStageIndex < quest.stages.Length &&
            quest.completionDialogue.stageDialogues != null &&
            quest.currentStageIndex < quest.completionDialogue.stageDialogues.Length)
        {
            currentActiveStageData = quest.completionDialogue.stageDialogues[quest.currentStageIndex];
        }

        else if (quest.completionDialogue.finalCompletionDialogue != null)
        {
            currentActiveStageData = quest.completionDialogue.finalCompletionDialogue;
        }

        if (currentActiveStageData == null)
        {
            Debug.LogError($"Dialogue data not found for Quest: {quest.title} at stage {quest.currentStageIndex}. Check QuestDialogueData configuration.");
            CloseDialogue();
            return;
        }

        foreach (var b in activeButtons)
            Destroy(b);
        activeButtons.Clear();

        if (currentActiveStageData.conversationNodes != null && currentActiveStageData.conversationNodes.Count > 0)
        {
            currentNodes = currentActiveStageData.conversationNodes;
            ShowNode(0);
        }
        else
        {
            nameText.text = currentActiveStageData.npcName;

            if (currentActiveStageData.npcImage != null)
            {
                portraitImage.sprite = currentActiveStageData.npcImage;
                portraitImage.gameObject.SetActive(true);
            }
            else
            {
                portraitImage.gameObject.SetActive(false);
            }

            if (currentActiveStageData.options != null)
            {
                foreach (var option in currentActiveStageData.options)
                {
                    CreateButton(option, () => FinalizeStageChoice(option, currentActiveStageData));
                }
            }
            ShowTextAnimated(currentActiveStageData.dialogueText, () => ShowAllButtons());
        }

        DialoguePopupCanvas.enabled = true;
        if (ControllerUI != null) ControllerUI.SetActive(false);
    }

    private void ShowNode(int index)
    {
        if (index < 0 || index >= currentNodes.Count)
        {
            Debug.LogError("Dialogue Node Index out of range!");
            CloseDialogue();
            return;
        }

        DialogueNode node = currentNodes[index];

        string speakerName = currentActiveStageData.npcName;
        Sprite speakerImage = currentActiveStageData.npcImage;

        if (node.speaker == Speaker.NPC2)
        {
            speakerName = currentActiveStageData.secondaryNpcName;
            speakerImage = currentActiveStageData.secondaryNpcImage;
        }
        else if (node.speaker == Speaker.Player)
        {
            speakerName = playerName;
            speakerImage = playerImage;
        }
        else if (node.speaker == Speaker.NPC3)
        {
            speakerName = currentActiveStageData.additionalNpcName;
            speakerImage = currentActiveStageData.additionalNpcImage;
        }
        else
        {

        }

        nameText.text = speakerName;

        if (speakerImage != null)
        {
            portraitImage.sprite = speakerImage;
            portraitImage.gameObject.SetActive(true);
        }
        else
        {
            portraitImage.gameObject.SetActive(false);
        }

        foreach (var b in activeButtons) Destroy(b);
        activeButtons.Clear();

        if (node.options != null)
        {
            foreach (var option in node.options)
            {
                CreateButton(option, () => HandleBranchingOption(option));
            }
        }

        ShowTextAnimated(node.dialogueText, () => ShowAllButtons());
    }

    private void CreateButton(DialogueOption option, UnityEngine.Events.UnityAction onClickAction)
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
        button.onClick.AddListener(onClickAction);
    }

    private void HandleBranchingOption(DialogueOption option)
    {
        if (option.costRupiah > 0)
        {
            ResourceManager.Instance.Subtract(option.costRupiah);
        }

        if (option.nextNodeIndex != -1)
        {
            ShowNode(option.nextNodeIndex);
        }
        else
        {
            if (!string.IsNullOrEmpty(option.responseText))
            {
                foreach (var b in activeButtons) Destroy(b);
                activeButtons.Clear();

                GameObject closeBtn = Instantiate(buttonPrefab, buttonContainer);
                closeBtn.SetActive(false);
                closeBtn.GetComponentInChildren<TextMeshProUGUI>().text = "Close";
                closeBtn.GetComponent<Button>().onClick.AddListener(() => FinalizeStageChoice(option, currentActiveStageData));
                activeButtons.Add(closeBtn);

                ShowTextAnimated(option.responseText, () => ShowAllButtons());
            }
            else
            {
                FinalizeStageChoice(option, currentActiveStageData);
            }
        }
    }
    private void FinalizeStageChoice(DialogueOption option, QuestStageDialogue stageData)
    {
        if (currentQuest == null)
        {
            CloseDialogue();
            return;
        }

        if (option.failQuest)
        {
            QuestManager.Instance.TodayReport.npcFeedback.Add(
                $"{stageData.npcName}: \"{stageData.failureResponse}\""
            );

            FailQuest();
            return;
        }

        QuestManager.Instance.TodayReport.npcFeedback.Add(
            $"{stageData.npcName}: \"{stageData.successResponse}\""
        );

        // Check Complete Quest
        if (option.completesQuest || (currentQuest.isMultiStage && currentQuest.currentStageIndex >= currentQuest.stages.Length - 1))
        {
            CompleteQuest();
            return;
        }

        // Next Stage
        currentQuest.currentStageIndex++;

        NotificationSystem.Instance.ShowNotification(
            $"Stage {currentQuest.currentStageIndex}/{currentQuest.stages.Length} complete!"
        );

        CloseDialogue();
    }

    private void CompleteQuest()
    {
        if (currentQuest.state != QuestState.Completed)
        {
            currentQuest.state = QuestState.Completed;
            QuestManager.Instance.CompleteQuest(currentQuest);

            NotificationSystem.Instance.ShowNotification($"Quest Completed: <b>{currentQuest.title}</b>");
        }
        CloseDialogue();
    }

    private void FailQuest()
    {
        if (currentQuest.state != QuestState.Failed)
        {
            currentQuest.state = QuestState.Failed;
            QuestManager.Instance.FailQuest(currentQuest);

            NotificationSystem.Instance.ShowNotification($"Quest Failed: <b>{currentQuest.title}</b>");
        }
        CloseDialogue();
    }

    public void CloseDialogue()
    {
        DialoguePopupCanvas.enabled = false;
        if (ControllerUI != null) ControllerUI.SetActive(true);

        foreach (var b in activeButtons)
            Destroy(b);

        activeButtons.Clear();
    }

    private void HandleDayEnded()
    {
        if (currentQuest != null && currentQuest.state != QuestState.Completed && currentQuest.state != QuestState.Failed)
        {
            Debug.Log($"[DialoguePopup] Auto-failing quest: {currentQuest.title} due to end of day.");

            currentQuest.state = QuestState.Failed;

            QuestManager.Instance.FailQuest(currentQuest);

            if (QuestManager.Instance.TodayReport != null)
            {
                QuestManager.Instance.TodayReport.failedQuests++;
            }
        }

        DialoguePopupCanvas.enabled = false;

        if (typingRoutine != null)
        {
            StopCoroutine(typingRoutine);
            typingRoutine = null;
        }

        foreach (var b in activeButtons)
        {
            Destroy(b);
        }
        activeButtons.Clear();

        currentQuest = null;
        currentActiveStageData = null;
    }
}