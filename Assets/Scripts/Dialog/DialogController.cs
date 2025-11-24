using System.IO;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogController : MonoBehaviour
{
    public static DialogController Instance { get; private set; }

    [Header("Dialog UI")]
    public GameObject dialogPanel;
    public TMP_Text dialogText;
    public TMP_Text nameText;
    public Image portraitImage;
    public Transform choiceContainer;
    public GameObject choiceButtonPrefab;

    [Header("History UI")]
    public GameObject historyPanel;
    public TMP_Text historyText;
    public ScrollRect historyScrollRect;

    [Header("Settings")]
    public int maxHistoryEntries = 20;
    public bool closeOnFinish = true;

    // runtime state
    public bool IsDialogOpen { get; private set; }

    private DialogData currentData;
    private int currentIndex = 0;
    private Coroutine typingCoroutine;
    private bool isDialogActive = false;
    private bool isTyping = false;
    private bool waitingForInput = false;
    private bool showingChoices = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Update()
    {
        if (!isDialogActive) return;

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0))
        {
            if (showingChoices) return;
            if (isTyping)
            {
                isTyping = false;
            }
            else if (waitingForInput)
            {
                waitingForInput = false;
            }
            else
            {
                NextLine();
            }
        }
    }

    public void StartDialog(string fileName, Sprite portraitOverride = null)
    {
        StopDialogImmediate();
        IsDialogOpen = true;

        currentData = LoadDialog(fileName);
        if (currentData == null)
        {
            Debug.LogError($"DialogController: failed to load dialog '{fileName}'");
            return;
        }

        nameText.text = currentData.npcName ?? "";
        if (portraitOverride != null)
            portraitImage.sprite = portraitOverride;
        else if (!string.IsNullOrEmpty(currentData.npcPortrait))
            portraitImage.sprite = Resources.Load<Sprite>(currentData.npcPortrait);
        else
            portraitImage.sprite = null;

        currentIndex = 0;
        isDialogActive = true;
        dialogPanel.SetActive(true);
        dialogText.text = "";
        ClearChoices();
        StartCoroutine(RunEntry(currentData.dialog[currentIndex]));
    }

    public void StopDialogImmediate()
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        typingCoroutine = null;

        isDialogActive = false;
        isTyping = false;
        waitingForInput = false;
        showingChoices = false;

        ClearChoices();
        dialogText.text = "";

        IsDialogOpen = false;

        if (closeOnFinish && dialogPanel != null)
            dialogPanel.SetActive(false);
    }

    public DialogData LoadDialog(string fileName)
    {
        string path = Path.Combine(Application.streamingAssetsPath, "NPC_Dialogs", fileName + ".json");

        if (!File.Exists(path))
        {
            Debug.LogError("DialogController: dialog file not found: " + path);
            return null;
        }

        string json = File.ReadAllText(path);
        return JsonUtility.FromJson<DialogData>(json);
    }

    void NextLine()
    {
        if (currentData == null) return;

        if (showingChoices) return;

        int nextIndex = currentIndex + 1;
        if (nextIndex >= currentData.dialog.Length)
        {
            EndDialog();
            return;
        }

        currentIndex = nextIndex;
        ClearChoices();
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(RunEntry(currentData.dialog[currentIndex]));
    }

    IEnumerator RunEntry(DialogEntry entry)
    {
        showingChoices = false;

        string mainText =
            !string.IsNullOrEmpty(entry.text) ?
            entry.text :
            (entry.npcReply != null && entry.npcReply.Length > 0 ? entry.npcReply[0] : "");

        yield return StartCoroutine(TypeLine(mainText));
        AddToHistory(currentData.npcName ?? "NPC", mainText);

        if (entry.npcReply != null && entry.npcReply.Length > 1)
        {
            for (int i = 1; i < entry.npcReply.Length; i++)
            {
                waitingForInput = true;
                yield return new WaitUntil(() => !waitingForInput);

                yield return StartCoroutine(TypeLine(entry.npcReply[i]));
                AddToHistory(currentData.npcName ?? "NPC", entry.npcReply[i]);
            }
        }

        if (entry.choices != null && entry.choices.Length > 0)
        {
            showingChoices = true;
            DisplayChoices(entry);
            yield break;
        }

        waitingForInput = true;
        yield return new WaitUntil(() => !waitingForInput);
        showingChoices = false;
    }

    IEnumerator TypeLine(string fullText)
    {
        isTyping = true;
        dialogText.text = "";
        int letterIndex = 0;

        while (letterIndex < fullText.Length)
        {
            dialogText.text = fullText.Substring(0, letterIndex + 1);
            letterIndex++;
            yield return new WaitForSeconds(
                currentData != null ? currentData.typingSpeed : 0.05f
            );

            if (!isTyping)
            {
                dialogText.text = fullText;
                break;
            }
        }

        isTyping = false;
        yield return null;
    }

    void DisplayChoices(DialogEntry entry)
    {
        ClearChoices();
        for (int i = 0; i < entry.choices.Length; i++)
        {
            string choiceText = entry.choices[i];
            GameObject btnGo = Instantiate(choiceButtonPrefab, choiceContainer);
            TMP_Text btnText = btnGo.GetComponentInChildren<TMP_Text>();
            if (btnText != null) btnText.text = choiceText;

            Button btn = btnGo.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.AddListener(() => OnChoiceSelected(choiceText));
            }
        }
    }

    void OnChoiceSelected(string playerChoice)
    {
        AddToHistory("Player", playerChoice);

        int foundIndex = -1;
        for (int i = 0; i < currentData.dialog.Length; i++)
        {
            if (!string.IsNullOrEmpty(currentData.dialog[i].playerChoice) &&
                currentData.dialog[i].playerChoice == playerChoice)
            {
                foundIndex = i;
                break;
            }
        }

        ClearChoices();
        showingChoices = false;

        if (foundIndex != -1)
        {
            currentIndex = foundIndex;
            if (typingCoroutine != null) StopCoroutine(typingCoroutine);
            typingCoroutine = StartCoroutine(RunEntry(currentData.dialog[currentIndex]));
        }
        else
        {
            EndDialog();
        }
    }

    public void AddToHistory(string speaker, string line)
    {
        if (historyText == null) return;

        if (string.IsNullOrEmpty(historyText.text))
            historyText.text = $"<b>{speaker}:</b> {line}";
        else
            historyText.text += $"\n<b>{speaker}:</b> {line}";

        TrimHistory();
        ScrollInstantBottom();
    }

    void TrimHistory()
    {
        if (historyText == null) return;

        string[] lines = historyText.text.Split('\n');
        if (lines.Length <= maxHistoryEntries) return;

        int removeCount = lines.Length - maxHistoryEntries;
        string[] newLines = new string[maxHistoryEntries];
        System.Array.Copy(lines, removeCount, newLines, 0, maxHistoryEntries);
        historyText.text = string.Join("\n", newLines);
    }

    public void ToggleHistory()
    {
        if (historyPanel == null) return;

        historyPanel.SetActive(!historyPanel.activeSelf);
        ScrollInstantBottom();
    }

    void ScrollInstantBottom()
    {
        Canvas.ForceUpdateCanvases();
        if (historyScrollRect != null)
            historyScrollRect.verticalNormalizedPosition = 0f;
        Canvas.ForceUpdateCanvases();
    }

    void ClearChoices()
    {
        if (choiceContainer == null) return;

        for (int i = choiceContainer.childCount - 1; i >= 0; i--)
            Destroy(choiceContainer.GetChild(i).gameObject);
    }

    void EndDialog()
    {
        StopDialogImmediate();
    }
}