using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class ResourceManagementSystemScene_HAMGradeUI : MonoBehaviour
{
    TextMeshProUGUI TMPro;
    HAMGradeManager HAM;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (HAMGradeManager.Instance == null)
        {
            Debug.LogError("[Money UI] No HAM Grade Manager found");
        } else
        {
            HAM = HAMGradeManager.Instance;
        }
        TMPro = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        TMPro.text = HAM.GetGrade().ToString();
    }
}
