using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
public class TutorialUI : MonoBehaviour
{
    [SerializeField] private Button NextButton;
    [SerializeField] private Button PreviousButton;
    [SerializeField] private Button FinishButton;
    [SerializeField] private TextMeshProUGUI TMP;
    private Canvas TutorialCanvas;

    [SerializeField]
    [TextArea]
    private string TutorialContent;

    void Awake()
    {
        NextButton.onClick.AddListener(() => { TMP.pageToDisplay++; });
        PreviousButton.onClick.AddListener(() => { TMP.pageToDisplay--; });
        FinishButton.onClick.AddListener(() => { StopTutorial(); });
        TutorialCanvas = GetComponent<Canvas>();
        TutorialCanvas.enabled = false;
    }

    void OnEnable()
    {
        StartTutorial();
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
        if (TMP.pageToDisplay == TMP.textInfo.pageCount && NextButton.gameObject.activeSelf)
        {
            NextButton.gameObject.SetActive(false);
            FinishButton.gameObject.SetActive(true);
        }
        PreviousButton.interactable = TMP.pageToDisplay > 1;
    }
}
