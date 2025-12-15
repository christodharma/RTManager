using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

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
            SetEnablePlayerMovement(false);
        }
    }

    public void StartTeleport()
    {
        StartCoroutine(FadeOutAndTeleport(player, Target.transform.position));

        SetEnablePlayerMovement(true);
    }

    public void CancelTeleport()
    {
        Destroy(instantiatedPromptGameObject);

        SetEnablePlayerMovement(true);
    }

    private void SetEnablePlayerMovement(bool active)
    {
        if (player.TryGetComponent(out PlayerMovement movement))
        {
            movement.enabled = active;
        }
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
