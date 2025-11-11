using UnityEngine;

public class NPCInteraction : MonoBehaviour
{
    [SerializeField] private Canvas InteractionIndicatorCanvas;
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            InteractionIndicatorCanvas.gameObject.SetActive(true);
        }
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            InteractionIndicatorCanvas.gameObject.SetActive(false);
        }
    }
    public void OnClickInteract()
    {
        Debug.Log("Hiya, Player!");
    }
}
