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

    private PlayerMovement playerMovementScript;
    private Animator playerAnimator;

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
            playerMovementScript = player.GetComponent<PlayerMovement>();
            if (playerMovementScript != null)
            {
                playerMovementScript.enabled = false;
            }

            playerAnimator = player.GetComponent<Animator>();
            playerAnimator.SetFloat("Velocity", 0f);
            playerAnimator.SetFloat("Vertical", 0f);
            playerAnimator.SetFloat("Horizontal", 0f);

            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;
            }
        }
    }

    public void StartTeleport()
    {
        StartCoroutine(FadeOutAndTeleport(player, Target.transform.position));

        if (playerMovementScript != null)
        {
            playerMovementScript.enabled = true;
        }
    }

    public void CancelTeleport()
    {
        Destroy(instantiatedPromptGameObject);
        
        if (playerMovementScript != null)
        {
            playerMovementScript.enabled = true;
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
