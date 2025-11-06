using UnityEngine;
using UnityEngine.UI;

public class DayNightTransition : MonoBehaviour
{
    [Header("References")]
    public Image overlayImage;

    [Header("Colors by Phase")]
    public Color pagiColor = new Color(1f, 0.95f, 0.75f, 0.1f);   // soft warm tint
    public Color siangColor = new Color(1f, 1f, 1f, 0f);          // no tint
    public Color soreColor = new Color(1f, 0.75f, 0.5f, 0.15f);   // orange tint
    public Color malamColor = new Color(0.3f, 0.4f, 0.6f, 0.35f); // dark blue tint

    [Header("Transition Settings")]
    public float transitionSpeed = 1f; // how fast to blend (higher = faster)

    private Color targetColor;
    private Coroutine transitionCoroutine;

    void Start()
    {
        if (GameTimeManager.Instance != null)
        {
            GameTimeManager.Instance.OnPhaseChanged += UpdateOverlayColor;
            UpdateOverlayColor(GameTimeManager.Instance.CurrentPhase);
        }
    }

    void UpdateOverlayColor(DayPhase newPhase)
    {
        switch (newPhase)
        {
            case DayPhase.Pagi:
                targetColor = pagiColor;
                break;
            case DayPhase.Siang:
                targetColor = siangColor;
                break;
            case DayPhase.Sore:
                targetColor = soreColor;
                break;
            case DayPhase.Malam:
                targetColor = malamColor;
                break;
        }

        // Smoothly transition to target color
        if (transitionCoroutine != null) StopCoroutine(transitionCoroutine);
        transitionCoroutine = StartCoroutine(TransitionColor());
    }

    System.Collections.IEnumerator TransitionColor()
    {
        Color startColor = overlayImage.color;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * transitionSpeed;
            overlayImage.color = Color.Lerp(startColor, targetColor, t);
            yield return null;
        }
    }

    void OnDestroy()
    {
        if (GameTimeManager.Instance != null)
            GameTimeManager.Instance.OnPhaseChanged -= UpdateOverlayColor;
    }
}
