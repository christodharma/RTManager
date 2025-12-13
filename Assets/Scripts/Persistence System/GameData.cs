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
    public float CurrentMoney = 50000;

    [Header("HAM Grade Manager")]
    public int CurrentHAMPoints = 0;
    public int TotalAccumulatedHAM;


    [Header("Quest Manager")]
    public List<QuestData> activeQuests = new();
    public List<QuestData> completedQuests = new();
    public List<QuestData> failedQuests = new();
    public List<QuestData> refusedQuests = new();
    public List<QuestData> availableQuests = new();

    [Header("Game Time Manager")]
    public float secondsElapsedToday;
    public int currentDay  = 1;

    [Header("Movement")]
    public Vector3 Position;
    public Quaternion Rotation;
    public Vector3 LocalScale = Vector3.one;
}