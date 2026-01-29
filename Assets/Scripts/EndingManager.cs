using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class EndingManager : MonoBehaviour
{
    [Header("Components")]
    public DialoguePopup dialoguePopup;
    public GameObject mainMenuButton;

    [Header("Ending Content")]
    public QuestStageDialogue endingDialogueData;

    private bool hasStarted = false;

    private IEnumerator Start()
    {
        if (mainMenuButton != null)
            mainMenuButton.SetActive(false);

        yield return new WaitForSeconds(1.0f);

        if (dialoguePopup != null && endingDialogueData != null)
        {
            dialoguePopup.OpenSimpleDialogue(endingDialogueData);
            hasStarted = true;
        }
    }

    void Update()
    {
        if (hasStarted && dialoguePopup != null)
        {
            if (!dialoguePopup.DialoguePopupCanvas.enabled)
            {
                if (mainMenuButton != null && !mainMenuButton.activeSelf)
                {
                    mainMenuButton.SetActive(true);
                }
            }
        }
    }

    public void LoadMainMenu(string menuSceneName)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(menuSceneName);
    }
}