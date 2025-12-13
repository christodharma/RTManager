using System;
using System.Collections;
using UnityEngine;

public class PortalEntrance : MonoBehaviour
{
    public PortalExit Target;
    public bool AskPromptFirst = true;
    public GameObject PromptGameObject;
    GameObject instantiatedPromptGameObject;
    GameObject player;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            player = collision.gameObject;
            if (!AskPromptFirst)
            {
                StartTeleport(); return;
            }

            instantiatedPromptGameObject = Instantiate(PromptGameObject);
            instantiatedPromptGameObject.GetComponent<Canvas>().enabled = true;
            TeleportPromptUI promptUI = instantiatedPromptGameObject.GetComponent<TeleportPromptUI>();
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
        Destroy(instantiatedPromptGameObject);
    }

    IEnumerator FadeOutAndTeleport(GameObject teleported, Vector3 destination)
    {
        if (instantiatedPromptGameObject != null) { Destroy(instantiatedPromptGameObject); }

        if (FadeTransition.Instance == null) { yield break; }

        yield return StartCoroutine(FadeTransition.Instance.FadeOut());

        Target.GetComponent<BoxCollider2D>().enabled = true; // open exit script

        teleported.transform.SetPositionAndRotation(destination, teleported.transform.rotation);
    }
}
