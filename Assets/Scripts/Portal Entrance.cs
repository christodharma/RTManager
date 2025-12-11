using System;
using System.Collections;
using UnityEngine;

public class PortalEntrance : MonoBehaviour
{
    public PortalExit Target;
    public bool PromptFirst = true;
    public GameObject PromptGameObject;
    GameObject instantiated;
    GameObject player;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            player = collision.gameObject;
            if (!PromptFirst)
            {
                StartTeleport(); return;
            }

            instantiated = Instantiate(PromptGameObject);
            instantiated.GetComponent<Canvas>().enabled = true;
            TeleportPromptUI promptUI = instantiated.GetComponent<TeleportPromptUI>();
            promptUI.Entrance = this;

            // TODO pause on prompt?
        }
    }

    public void StartTeleport()
    {
        StartCoroutine(FadeOutAndTeleport(player, Target.transform.position));
    }

    public void CancelTeleport()
    {
        Destroy(instantiated);
    }

    IEnumerator FadeOutAndTeleport(GameObject teleporter, Vector3 destination)
    {
        if (instantiated != null) { Destroy(instantiated); }

        if (FadeTransition.Instance == null) { yield break; }

        yield return StartCoroutine(FadeTransition.Instance.FadeOut());

        teleporter.transform.SetPositionAndRotation(destination, teleporter.transform.rotation);
    }
}
