using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeTransition : MonoBehaviour
{
    public static FadeTransition Instance { get; private set; }
    public Image fadeImage;
    public float fadeDuration = 3.5f; // seconds

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public IEnumerator FadeOut()
    {
        yield return Fade(0f, 1f); // go black
    }

    public IEnumerator FadeIn()
    {
        yield return Fade(3.5f, 0f); // fade back in
    }

    IEnumerator Fade(float start, float end)
    {
        float t = 0f;
        Color color = fadeImage.color;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime; // unaffected by pause
            float a = Mathf.Lerp(start, end, t / fadeDuration);
            color.a = a;
            fadeImage.color = color;
            yield return null;
        }
        color.a = end;
        fadeImage.color = color;
    }
}