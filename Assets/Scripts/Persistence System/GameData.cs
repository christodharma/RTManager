using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class to structure in-game data storage, used to group every save/loadable game components
/// </summary>

[Serializable]
public class GameData
{
    // add data types here, e.g. statistics, level, health, etc
    [Header("Resource Manager")]
    public float CurrentMoney;

    [Header("HAM Grade Manager")]
    public int CurrentHAMPoints;

    [Header("Quest Manager")]
    public List<QuestData> activeQuests;
    public List<QuestData> completedQuests;
    public List<QuestData> failedQuests;
    public List<QuestData> refusedQuests;
    public List<QuestData> availableQuests;

    [Header("Game Time Manager")]
    public float secondsElapsedToday;
    public int currentDay;

    [Header("Movement")]
    public Vector3 Position;
    public Quaternion Rotation;
    public Vector3 LocalScale;
}