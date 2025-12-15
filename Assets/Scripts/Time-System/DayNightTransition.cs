using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DayNightTransition : MonoBehaviour
{
    [Header("References")]
    public Light2D Light;

    [Header("Colors by Phase")]
    public Color pagiColor = new Color(1f, 0.95f, 0.75f, 0.1f);   // soft warm tint
    public Color siangColor = new Color(1f, 1f, 1f, 0f);          // no tint
    public Color soreColor = new Color(1f, 0.75f, 0.5f, 0.15f);   // orange tint
    public Color malamColor = new(0f, 0f, 0f, 0.15f);

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
        Color startColor = Light.color;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * transitionSpeed;
            Light.color = Color.Lerp(startColor, targetColor, t);
            yield return null;
        }
    }

    void OnDestroy()
    {
        if (GameTimeManager.Instance != null)
            GameTimeManager.Instance.OnPhaseChanged -= UpdateOverlayColor;
    }
}
