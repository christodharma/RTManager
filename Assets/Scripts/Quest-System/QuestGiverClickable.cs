using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class QuestGiverClickable : MonoBehaviour
{
    public QuestDatabase questDatabase;
    public QuestBoxUI questBoxUI;

    [Header("Filter options")]
    public string[] questIDsToOffer;

    [Header("Interaction Settings")]
    public float interactDistance = 1.5f;
    public Button interactButton;   // Assign via inspector
    public Transform player;        // Assign player transform in inspector

    private List<QuestData> runtimeQuests = new List<QuestData>();

    private bool playerInRange = false;

    void Start()
    {
        foreach (var id in questIDsToOffer)
        {
            var questObj = questDatabase.GetQuestByID(id);
            if (questObj != null)
                runtimeQuests.Add(new QuestData(questObj));
        }

        interactButton.gameObject.SetActive(false);
        interactButton.onClick.AddListener(OpenQuestUI);
    }

    void Update()
    {
        if (player == null) return;

        float dist = Vector2.Distance(player.position, transform.position);

        if (dist <= interactDistance && !playerInRange)
        {
            playerInRange = true;
            interactButton.gameObject.SetActive(true);
        }
        else if (dist > interactDistance && playerInRange)
        {
            playerInRange = false;
            interactButton.gameObject.SetActive(false);
        }
    }

    private void OpenQuestUI()
    {
        questBoxUI.OpenQuestBox(runtimeQuests);
    }

    private void OnDisable()
    {
        interactButton.onClick.RemoveListener(OpenQuestUI);
    }
}