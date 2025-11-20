using UnityEngine;
using System.Collections.Generic;

public class QuestGiverClickable : MonoBehaviour
{
    public QuestDatabase questDatabase;
    public QuestBoxUI questBoxUI;

    [Header("Filter options")]
    public string[] questIDsToOffer;

    private List<QuestData> runtimeQuests = new List<QuestData>();

    void Start()
    {
        foreach (var id in questIDsToOffer)
        {
            var questObj = questDatabase.GetQuestByID(id);
            if (questObj != null)
                runtimeQuests.Add(new QuestData(questObj));
        }
    }

    void OnMouseDown()
    {
        questBoxUI.OpenQuestBox(runtimeQuests);
    }
}