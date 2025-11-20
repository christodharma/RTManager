using System.Collections;
using UnityEngine;
using TMPro;

public class NotificationSystem : MonoBehaviour
{
    public static NotificationSystem Instance;

    [Header("Setup")]
    public GameObject notificationPrefab;
    public Transform notificationParent;

    [Header("Timing")]
    public float displayDuration = 2.5f;
    public float fadeDuration = 0.5f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void ShowNotification(string message)
    {
        GameObject notificationObj = Instantiate(notificationPrefab, notificationParent);
        notificationObj.SetActive(true);

        TextMeshProUGUI text = notificationObj.GetComponentInChildren<TextMeshProUGUI>();
        CanvasGroup canvasGroup = notificationObj.GetComponent<CanvasGroup>();

        text.text = message;
        StartCoroutine(FadeNotification(notificationObj, canvasGroup));
    }

    private IEnumerator FadeNotification(GameObject obj, CanvasGroup canvas)
    {
        canvas.alpha = 1f;

        // Display time
        yield return new WaitForSeconds(displayDuration);

        // Fade out
        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            canvas.alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);
            yield return null;
        }

        Destroy(obj);
    }
}