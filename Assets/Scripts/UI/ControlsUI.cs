using UnityEngine;
using UnityEngine.UI;

public class ControlsUI : MonoBehaviour {
    [SerializeField] private Button PauseButton;
    [SerializeField] private Button InteractButton;
    [SerializeField] private Button TalkButton;
    [SerializeField] private Button QuestButton;
    [SerializeField] private Button HistoryButton;
    [SerializeField] private GameTimeManager gameTime;
    void Start()
    {
        if (GameTimeManager.Instance != null)
        {
            gameTime = GameTimeManager.Instance;
            PauseButton.onClick.AddListener(gameTime.PauseGame);
        }
    }
}