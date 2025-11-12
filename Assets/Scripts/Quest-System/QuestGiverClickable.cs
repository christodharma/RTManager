using UnityEngine;
using System.Collections.Generic;

public class QuestGiverClickable : MonoBehaviour
{
    public QuestBoxUI questBoxUI;
    public List<QuestData> availableQuests = new List<QuestData>();

    void OnMouseDown()
    {
        if (questBoxUI == null)
        {
            Debug.LogWarning($"{name}: QuestBoxUI not assigned!");
            return;
        }

        questBoxUI.OpenQuestBox(availableQuests);
    }
}