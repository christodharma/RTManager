using UnityEngine;
using UnityEngine.UI;

public class NPCInteraction : MonoBehaviour
{
    [SerializeField] private Canvas InteractionIndicatorCanvas;
    [SerializeField] private Button talkButton;

    private NPC npc;

    void Awake()
    {
        npc = GetComponentInParent<NPC>();

        talkButton.onClick.AddListener(() =>
        {
            if (!DialogController.Instance.IsDialogOpen)
            {
                npc?.Interact();
            }
        });

        InteractionIndicatorCanvas.gameObject.SetActive(false);
        talkButton.gameObject.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            InteractionIndicatorCanvas.gameObject.SetActive(true);
            talkButton.gameObject.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            InteractionIndicatorCanvas.gameObject.SetActive(false);
            talkButton.gameObject.SetActive(false);
        }
    }
}
