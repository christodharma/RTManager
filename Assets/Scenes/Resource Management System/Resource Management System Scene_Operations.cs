using UnityEngine;

public class ResourceManagementSystemScene_Operation : MonoBehaviour
{
    void Start()
    {
        if (ResourceManager.Instance == null)
        {
            Debug.LogError("No ResourceManager found");
        }
        if (HAMGradeManager.Instance == null)
        {
            Debug.LogError("No HAMGradeManager found");
        }
    }
    public void MoneyAdd()
    {
        ResourceManager.Instance.Add(1500);
    }
    public void MoneySubtract()
    {
        ResourceManager.Instance.Subtract(1500);
    }
    public void HAMAdd()
    {
        HAMGradeManager.Instance.Add(15);
    }
    public void HAMReset()
    {
        HAMGradeManager.Instance.SetToInitialValue();
    }
}