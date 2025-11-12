using UnityEngine;
using TMPro;
using UnityEngine.UI;

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

    private bool isOpen = false;

    private void Start()
    {
        if (toggleButton != null)
            toggleButton.onClick.AddListener(ToggleQuestLog);

        // Subscribe to quest updates
        QuestManager.Instance.OnQuestListChanged += HandleQuestListChanged;

        questLogPanel.SetActive(false); // start closed
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

        if (isOpen)
            RefreshLog();
    }

    public void RefreshLog()
    {
        foreach (Transform child in activeQuestContainer)
            Destroy(child.gameObject);

        foreach (var quest in QuestManager.Instance.activeQuests)
        {
            var entry = Instantiate(questEntryPrefab, activeQuestContainer);
            entry.GetComponent<QuestLogEntryUI>().Setup(quest);
        }

        completedCountText.text = $"Completed: {QuestManager.Instance.completedQuests.Count}";
        failedCountText.text = $"Failed: {QuestManager.Instance.failedQuests.Count}";
        refusedCountText.text = $"Refused: {QuestManager.Instance.refusedQuests.Count}";
    }
}