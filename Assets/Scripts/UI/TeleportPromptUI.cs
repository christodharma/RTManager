using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TeleportPromptUI : MonoBehaviour {
    [SerializeField] private Button YesButton;
    [SerializeField] private Button NoButton;
    [SerializeField] private TextMeshProUGUI PromptText;
    public PortalEntrance Entrance;

    void Start()
    {
        YesButton.onClick.AddListener(() => Entrance.StartTeleport());
        NoButton.onClick.AddListener(() => Entrance.CancelTeleport());
        PromptText.text = $"Enter {Entrance.gameObject.name}?";
    }
}