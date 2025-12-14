using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
public class TutorialUI : MonoBehaviour
{
    [SerializeField] private Button NextButton;
    [SerializeField] private Button PreviousButton;
    [SerializeField] private TextMeshProUGUI TMP;
    private Canvas TutorialCanvas;

    [SerializeField]
    [TextArea(100, 1000)]
    private string TutorialContent;

    void Awake()
    {
        NextButton.onClick.AddListener(() => { TMP.pageToDisplay++; });
        PreviousButton.onClick.AddListener(() => { TMP.pageToDisplay--; });
        TutorialCanvas = GetComponent<Canvas>();
        TutorialCanvas.enabled = false;
    }

    [ContextMenu("Start Tutorial")]
    public void StartTutorial()
    {
        Time.timeScale = 0;
        TutorialCanvas.enabled = true;
    }

    [ContextMenu("Stop Tutorial")]
    public void StopTutorial()
    {
        Time.timeScale = 1;
        TutorialCanvas.enabled = false;
    }

    void Start()
    {
        TMP.text = TutorialContent;
    }

    void Update()
    {
        NextButton.interactable = TMP.pageToDisplay < TMP.textInfo.pageCount;
        PreviousButton.interactable = TMP.pageToDisplay > 1;
    }
}
