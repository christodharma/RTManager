using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    public AudioSource bgmSource;

    public AudioSource sfxSource;

    [Header("BGM Clips")]
    public AudioClip dayBGM;
    public AudioClip nightBGM;

    public float bgmFadeDuration = 2.0f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (GameTimeManager.Instance != null)
        {
            GameTimeManager.Instance.OnPhaseChanged += OnPhaseChanged;
        }

        if (GameTimeManager.Instance != null)
        {
            OnPhaseChanged(GameTimeManager.Instance.CurrentPhase);
        }
    }

    private void OnDestroy()
    {
        if (GameTimeManager.Instance != null)
        {
            GameTimeManager.Instance.OnPhaseChanged -= OnPhaseChanged;
        }
    }

    private void OnPhaseChanged(DayPhase newPhase)
    {
        AudioClip targetBGM = null;

        if (newPhase == DayPhase.Malam)
        {
            targetBGM = nightBGM;
        }
        else if (newPhase == DayPhase.Pagi || newPhase == DayPhase.Siang || newPhase == DayPhase.Sore)
        {
            targetBGM = dayBGM;
        }

        if (targetBGM != null && bgmSource.clip != targetBGM)
        {
            StartCoroutine(FadeBGM(targetBGM, bgmFadeDuration));
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        if (sfxSource != null && clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    private System.Collections.IEnumerator FadeBGM(AudioClip newClip, float duration)
    {
        float startVolume = bgmSource.volume;
        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;
            bgmSource.volume = Mathf.Lerp(startVolume, 0f, timer / duration);
            yield return null;
        }

        bgmSource.Stop();
        bgmSource.clip = newClip;
        bgmSource.Play();

        timer = 0f;
        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;
            bgmSource.volume = Mathf.Lerp(0f, startVolume, timer / duration);
            yield return null;
        }
        bgmSource.volume = startVolume;
    }
}