using System.Collections;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class PortalExit : MonoBehaviour
{
    void Awake()
    {
        GetComponent<BoxCollider2D>().enabled = false;
    }
    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartCoroutine(FadeInAndClosePortal());
        }
    }

    IEnumerator FadeInAndClosePortal()
    {
        GetComponent<BoxCollider2D>().enabled = false;

        if (FadeTransition.Instance == null) { yield break; }

        yield return new WaitForSeconds(1); // sustaining black screen
        yield return StartCoroutine(FadeTransition.Instance.FadeIn());

    }
}