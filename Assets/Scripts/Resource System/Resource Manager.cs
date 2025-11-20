using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    [SerializeField] float _CurrentMoney;
    public float CurrentMoney
    {
        get => _CurrentMoney;
        private set
        {
            _CurrentMoney = value;
        }
    }

    public static ResourceManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Add(float value)
    {
        CurrentMoney += value;
    }

    public void Subtract(float value)
    {
        CurrentMoney -= value;
    }

    public void SetToInitialValue()
    {
        CurrentMoney = 50000;
    }

    public void GetMoneyFromGrade()
    {
        if (HAMGradeManager.Instance == null) { return; }

        HAMGrade grade = HAMGradeManager.Instance.GetGrade();
        switch (grade)
        {
            case HAMGrade.A:
                Add(45000);
                break;
            case HAMGrade.B:
                Add(30000);
                break;
            case HAMGrade.C:
                Add(15000);
                break;
            case HAMGrade.F:
            default:
                Add(5000);
                break;
        }
    }
}
