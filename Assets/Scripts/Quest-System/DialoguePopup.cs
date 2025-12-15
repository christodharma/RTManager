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
    [SerializeField] private Button NextButton;
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
        NextButton.onClick.AddListener(() => { dialogueText.pageToDisplay++; });
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

    public void OpenSimpleDialogue(string text, string npcName, Sprite npcSprite)
    {
        currentQuest = null;
        currentNodes = null;
        currentActiveStageData = null;

        nameText.text = npcName;

        if (npcSprite != null)
        {
            portraitImage.sprite = npcSprite;
            portraitImage.gameObject.SetActive(true);
        }
        else
        {
            portraitImage.gameObject.SetActive(false);
        }

        foreach (var b in activeButtons) Destroy(b);
        activeButtons.Clear();

        GameObject closeBtn = Instantiate(buttonPrefab, buttonContainer);
        closeBtn.SetActive(false);
        closeBtn.GetComponentInChildren<TextMeshProUGUI>().text = "Close";
        closeBtn.GetComponent<Button>().onClick.AddListener(CloseDialogue);
        activeButtons.Add(closeBtn);

        DialoguePopupCanvas.enabled = true;
        ControllerUI.SetActive(false);

        ShowTextAnimated(text, () => ShowAllButtons());
    }

    public void OpenDialogue(QuestData quest, QuestInteractable source)
    {
        QuestStageDialogue outcomeNpcData = quest.completionDialogue.finalCompletionDialogue;

        if (outcomeNpcData == null && quest.completionDialogue.stageDialogues != null && quest.completionDialogue.stageDialogues.Length > 0)
        {
            outcomeNpcData = quest.completionDialogue.stageDialogues[0];
        }

        string outcomeName = outcomeNpcData != null ? outcomeNpcData.npcName : "NPC";
        Sprite outcomeSprite = outcomeNpcData != null ? outcomeNpcData.npcImage : null;

        if (quest.state == QuestState.Completed)
        {
            OpenSimpleDialogue(quest.completionDialogue.postQuestSuccessDialogue, outcomeName, outcomeSprite);
            return;
        }
        else if (quest.state == QuestState.Failed)
        {
            OpenSimpleDialogue(quest.completionDialogue.postQuestFailureDialogue, outcomeName, outcomeSprite);
            return;
        }

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
        ControllerUI.SetActive(false);
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

        else if (node.speaker == Speaker.NPC3)
        {
            speakerName = currentActiveStageData.additionalNpcName;
            speakerImage= currentActiveStageData.additionalNpcImage;
        }
        else if (node.speaker == Speaker.Player)
        {
            speakerName = playerName;
            speakerImage = playerImage;
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
        ControllerUI.SetActive(true);

        foreach (var b in activeButtons)
            Destroy(b);

        activeButtons.Clear();
    }
}