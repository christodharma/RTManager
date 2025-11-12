using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestBoxEntryUI : MonoBehaviour
{
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI rewardText;
    public TextMeshProUGUI deadlineText;
    public Button acceptButton;
    public Button refuseButton;

    private QuestData quest;
    private QuestBoxUI parentUI;

    public void Setup(QuestData data, QuestBoxUI ui)
    {
        quest = data;
        parentUI = ui;

        titleText.text = data.title;
        descriptionText.text = data.description;
        rewardText.text = $"Reward: {data.rewardHAM} HAM";
        deadlineText.text = $"Deadline: {data.deadlineHour:0.#}h";

        acceptButton.onClick.RemoveAllListeners();
        refuseButton.onClick.RemoveAllListeners();

        acceptButton.onClick.AddListener(() => parentUI.AcceptQuest(quest));
        refuseButton.onClick.AddListener(() => parentUI.RefuseQuest(quest));
    }
}