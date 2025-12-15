using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button NewGameButton;
    [SerializeField] private Button ContinueButton;
    [SerializeField] private Button SettingsButton;
    [SerializeField] private Button CreditsButton;
    [SerializeField] private Button QuitButton;

    void Start()
    {
        NewGameButton.onClick.AddListener(NewGame);
        ContinueButton.onClick.AddListener(ContinueGame);
        QuitButton.onClick.AddListener(QuitGame);

        if (PersistenceManager.Instance.IsNewGame)
        {
            // there is no save file, player can only start new game
            ContinueButton.gameObject.SetActive(false);
        }
    }

    public void NewGame()
    {
        if (PersistenceManager.Instance == null) { return; }

        PersistenceManager.Instance.TriggerNew();
        SceneManager.LoadScene("Game");
    }

    public void ContinueGame()
    {
        if (PersistenceManager.Instance == null) { return; }

        // FIXME this calls save file twice
        PersistenceManager.Instance.TriggerLoad();
        SceneManager.LoadScene("Game");
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }
}
