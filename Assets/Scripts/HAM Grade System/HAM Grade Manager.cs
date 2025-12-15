using System;
using UnityEngine;

public class HAMGradeManager : MonoBehaviour, IPersistable
{
    [SerializeField] private int _TotalAccumulatedHamPoints = 0;
    public int TotalAccumulatedHamPoints => _TotalAccumulatedHamPoints;

    [SerializeField] int _CurrentHAMPoints;
    public int CurrentHamPoints
    {
        get => _CurrentHAMPoints;
        set
        {
            _CurrentHAMPoints = Math.Max(0, Math.Min(100, value));
        }
    }

    public static HAMGradeManager Instance;
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetToInitialValue()
    {
        CurrentHamPoints = 0;
    }

    public void Add(int value)
    {
        CurrentHamPoints += value;
    }

    public void AddToTotal(int value)
    {
        _TotalAccumulatedHamPoints += value;
    }

    public HAMGrade GetGrade()
    {
        if (CurrentHamPoints >= 85 && CurrentHamPoints <= 100)
        {
            return HAMGrade.A;
        }
        else if (CurrentHamPoints >= 65 && CurrentHamPoints <= 84)
        {
            return HAMGrade.B;
        }
        else if (CurrentHamPoints >= 40 && CurrentHamPoints <= 64)
        {
            return HAMGrade.C;
        }
        else if (CurrentHamPoints <= 40)
        {
            return HAMGrade.F;
        }
        else return HAMGrade.F;
    }

    public void Save(ref GameData data)
    {
        data.CurrentHAMPoints = CurrentHamPoints;
        data.TotalAccumulatedHAM = _TotalAccumulatedHamPoints;
    }

    public void Load(GameData data)
    {
        CurrentHamPoints = data.CurrentHAMPoints;
        _TotalAccumulatedHamPoints = data.TotalAccumulatedHAM;
    }
}

public enum HAMGrade
{
    A, B, C, F
}