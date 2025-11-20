using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "QuestDatabase", menuName = "Quests/Database")]
public class QuestDatabase : ScriptableObject
{
    public List<QuestObject> quests = new List<QuestObject>();

    public QuestObject GetQuestByID(string id)
    {
        return quests.Find(q => q.questID == id);
    }
}