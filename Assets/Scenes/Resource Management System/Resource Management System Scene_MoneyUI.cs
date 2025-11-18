using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class ResourceManagementSystemScene_MoneyUI : MonoBehaviour
{
    TextMeshProUGUI TMPro;
    ResourceManager resource;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (ResourceManager.Instance == null)
        {
            Debug.LogError("[Money UI] No Resource Manager found");
        } else
        {
            resource = ResourceManager.Instance;
        }
        TMPro = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        TMPro.text = ResourceUI.FormatRp(resource.CurrentMoney);
    }
}
