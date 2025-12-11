using System.Collections;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class PortalExit : MonoBehaviour
{
    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartCoroutine(FadeInAndClosePortal());
        }
    }

    IEnumerator FadeInAndClosePortal()
    {
        if (FadeTransition.Instance == null) { yield break; }

        yield return StartCoroutine(FadeTransition.Instance.FadeIn());

        GetComponent<BoxCollider2D>().enabled = false;
    }
}