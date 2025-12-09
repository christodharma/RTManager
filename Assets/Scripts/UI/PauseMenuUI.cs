using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
public class PauseMenuUI : MonoBehaviour
{
    [SerializeField] private Canvas pauseMenuCanvas;
    [SerializeField] private Button ResumeButton;
    [SerializeField] private Button QuestLogButton;
    [SerializeField] private Button MapButton;
    [SerializeField] private Button StatsButton;
    [SerializeField] private Button SettingsButton;
    [SerializeField] private Button SaveAndQuitButton;

    void Awake()
    {
        pauseMenuCanvas.enabled = false;
    }

    void Start()
    {
        if (GameTimeManager.Instance != null)
        {
            GameTimeManager.Instance.OnPause += PauseGame;
            GameTimeManager.Instance.OnResume += ResumeGame;
        }

        if (PersistenceManager.Instance != null)
        {
            SaveAndQuitButton.onClick.AddListener(PersistenceManager.Instance.TriggerSave);
        }

        ResumeButton.onClick.AddListener(ResumeGame);
        SaveAndQuitButton.onClick.AddListener(QuitGame);
    }

    void OnDestroy()
    {
        GameTimeManager.Instance.OnPause -= PauseGame;
        GameTimeManager.Instance.OnResume -= ResumeGame;
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
        pauseMenuCanvas.enabled = true;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        pauseMenuCanvas.enabled = false;
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }
}