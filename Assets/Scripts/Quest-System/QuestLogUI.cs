using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestLogUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject questLogPanel;
    public Transform activeQuestContainer;
    public GameObject questEntryPrefab;
    public TextMeshProUGUI completedCountText;
    public TextMeshProUGUI failedCountText;
    public TextMeshProUGUI refusedCountText;

    [Header("Toggle Button")]
    public Button toggleButton;

    [Header("Show/Hide Controller")]
    public GameObject ControllerUI;

    private bool isOpen = false;

    private void Start()
    {
        if (toggleButton != null)
            toggleButton.onClick.AddListener(ToggleQuestLog);

        // Subscribe to quest updates
        QuestManager.Instance.OnQuestListChanged += HandleQuestListChanged;

        questLogPanel.SetActive(false); // start closed

        if (ControllerUI != null)
            ControllerUI.SetActive(true);
    }

    private void OnDestroy()
    {
        if (QuestManager.Instance != null)
            QuestManager.Instance.OnQuestListChanged -= HandleQuestListChanged;
    }

    private void HandleQuestListChanged()
    {
        if (isOpen)
            RefreshLog();
    }

    public void ToggleQuestLog()
    {
        isOpen = !isOpen;
        questLogPanel.SetActive(isOpen);

        if (ControllerUI != null) 
            ControllerUI.SetActive(!isOpen);

        if (isOpen)
            RefreshLog();
    }

    public void RefreshLog()
    {
        foreach (Transform child in activeQuestContainer)
            Destroy(child.gameObject);

        // Use sorted list
        var sortedQuests = QuestManager.Instance.GetActiveQuestsSorted();

        foreach (var quest in sortedQuests)
        {
            var entry = Instantiate(questEntryPrefab, activeQuestContainer);
            entry.GetComponent<QuestLogEntryUI>().Setup(quest);
        }

        completedCountText.text = $"Completed: {QuestManager.Instance.completedQuests.Count}";
        failedCountText.text = $"Failed: {QuestManager.Instance.failedQuests.Count}";
        refusedCountText.text = $"Refused: {QuestManager.Instance.refusedQuests.Count}";
    }
}