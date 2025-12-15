using System.Collections;
using TMPro;
using UnityEngine;

/// <summary>
/// Placeholder save indicator
/// </summary>
[RequireComponent(typeof(TextMeshProUGUI))]
public class EarlyBuildTest_LoadSave : MonoBehaviour
{
    private static WaitForSeconds _waitForSeconds0_1 = new(0.1f);
    public float MinimumSavingTime = 2; // the reason why this is placeholder
    Coroutine AnimationCoroutine;
    TextMeshProUGUI TMPro;
    void Awake()
    {
        TMPro = GetComponent<TextMeshProUGUI>();
        TMPro.enabled = false;
    }

    void Start()
    {
        if (PersistenceManager.Instance != null)
        {
            PersistenceManager.Instance.SaveStart += OnIsSaving;
            PersistenceManager.Instance.SaveEnd += OffIsSaving;
        }
        else
        {
            Debug.Log($"[{GetType().Name}] Can't find PersistenceManager!");
        }
    }

    void OnDestroy()
    {
        if (PersistenceManager.Instance != null)
        {
            PersistenceManager.Instance.SaveStart -= OnIsSaving;
            PersistenceManager.Instance.SaveEnd -= OffIsSaving;
        }
    }

    void OnIsSaving()
    {
        TMPro.enabled = true;
        AnimationCoroutine = StartCoroutine(SavingAnimation());
    }

    void OffIsSaving()
    {
        StartCoroutine(StopSavingAnimationWithDelay());
    }

    IEnumerator StopSavingAnimationWithDelay()
    {
        yield return new WaitForSeconds(MinimumSavingTime);

        StopCoroutine(AnimationCoroutine);
        TMPro.enabled = false;
    }

    IEnumerator SavingAnimation()
    {
        TMP_TextInfo textInfo = TMPro.textInfo;
        TMPro.maxVisibleCharacters = textInfo.characterCount - 3;
        while (true)
        {
            for (int i = 0; i < 3; i++)
            {
                TMPro.maxVisibleCharacters++;
                yield return _waitForSeconds0_1;
            }
            TMPro.maxVisibleCharacters = textInfo.characterCount - 3;
            yield return _waitForSeconds0_1;
        }
    }
}