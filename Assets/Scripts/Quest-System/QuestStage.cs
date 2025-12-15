using UnityEngine;

[System.Serializable]
public class QuestStage
{
    public string objectiveDescription;
    public bool requiresItem;
    public string requiredItemID;
    public int requiredAmount;
}