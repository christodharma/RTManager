using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Light2D))]
public class EasyQuests_LightFlickering : MonoBehaviour
{
    Light2D light2D;
    Coroutine flickerCoroutine;

    void Awake()
    {
        light2D = GetComponent<Light2D>();
    }

    void Start()
    {
        flickerCoroutine = StartCoroutine(Flicker());
        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.QuestSucceed += FixFlicker;
            QuestManager.Instance.QuestFailed += NoFixFlicker;
        }
    }

    void OnDestroy()
    {
        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.QuestSucceed -= FixFlicker;
            QuestManager.Instance.QuestFailed -= NoFixFlicker;
        }
    }

    public void FixFlicker(QuestData questData)
    {
        if (questData.questID.CompareTo("QE002") != 0) { return; } //don't do anything for other quests

        StopCoroutine(flickerCoroutine);
        StartCoroutine(LightFadeTo(20));
    }

    public void NoFixFlicker(QuestData questData)
    {
        if (questData.questID.CompareTo("QE002") != 0) { return; }

        StopCoroutine(flickerCoroutine);
        StartCoroutine(LightFadeTo(0));
    }

    IEnumerator Flicker()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(0, 2));
            if (Random.value <= 0.5)
            {
                light2D.intensity = Random.Range(0, 20);
            }
        }
    }

    IEnumerator LightFadeTo(float TargetLightIntensity)
    {
        float timeElapsed = 0f;
        float duration = 3f;
        float startIntensity = light2D.intensity;

        while (timeElapsed < duration)
        {
            timeElapsed += Time.deltaTime;
            light2D.intensity = Mathf.Lerp(startIntensity, TargetLightIntensity, timeElapsed / duration);
            yield return null;
        }

        light2D.intensity = TargetLightIntensity;
    }
}
