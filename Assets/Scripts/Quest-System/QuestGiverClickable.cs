using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

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

    private bool playerInRange = false;

    void Start()
    {
        if (interactButton != null)
        {
            interactButton.gameObject.SetActive(false);
            interactButton.onClick.AddListener(OpenQuestUI);
        }
    }

    void Update()
    {
        if (player == null || interactButton == null) return;
        float distance = Vector2.Distance(player.position, transform.position);
        bool shouldBeInRange = distance <= interactDistance;

        if (shouldBeInRange)
        {
            List<QuestData> availableQuests = GetAvailableQuestsForGiver();

            bool hasQuestToOffer = availableQuests.Count > 0;

            interactButton.gameObject.SetActive(hasQuestToOffer);

            playerInRange = true;
        }
        else if (!shouldBeInRange && playerInRange)
        {
            playerInRange = false;
            interactButton.gameObject.SetActive(false);
        }
    }

    private List<QuestData> GetAvailableQuestsForGiver()
    {
        List<QuestObject> offeredQuests = questIDsToOffer
            .Select(id => questDatabase.GetQuestByID(id))
            .Where(q => q != null)
            .ToList();

        List<QuestData> availableQuests = new List<QuestData>();

        foreach (var questObj in offeredQuests)
        {
            string qID = questObj.questID;

            if (QuestManager.Instance.activeQuests.Exists(q => q.questID == qID))
                continue;

            bool isRepeatable = questObj.isRepeatable;

            if (isRepeatable)
            {
                if (QuestManager.Instance.HasQuestBeenCompletedToday(qID))
                {
                    continue;
                }
            }
            else
            {
                if (QuestManager.Instance.completedQuests.Exists(q => q.questID == qID))
                    continue;
            }

            availableQuests.Add(new QuestData(questObj));
        }

        return availableQuests;
    }

    private void OpenQuestUI()
    {
        List<QuestData> questsToOffer = GetAvailableQuestsForGiver();

        if (interactButton != null)
        {
            interactButton.gameObject.SetActive(false);
        }

        if (questsToOffer.Count > 0)
        {
            questBoxUI.OpenQuestBox(questsToOffer);
        }
        else
        {
            Debug.Log("Quest Giver has no quests available right now.");
        }
    }

    private void OnDisable()
    {
        if (interactButton != null)
            interactButton.onClick.RemoveListener(OpenQuestUI);
    }
}