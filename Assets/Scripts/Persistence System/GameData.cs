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
    public int currentHour;
    public int currentMinute;
    public int currentDay;
    [Range(0f, 1f)] public float dayFraction; // 0..1 across the 24h
}