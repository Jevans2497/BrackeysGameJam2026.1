using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;
    public AudioSource normalSpeedAudioSource;
    public AudioSource fastSpeedAudioSource;

    private const float SPEED_DIFFERENCE = 1.53409f;
    private const float SPEED_DIFFERENCE_RECIPROCAL = 0.65185185185f;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        TimeManager.Instance.OnTimeDilationChange += SetMusicSpeed;
        SetMusicToFastSpeed();
    }

    private void OnDestroy()
    {
        TimeManager.Instance.OnTimeDilationChange -= SetMusicSpeed;
    }

    private void SetMusicSpeed(TimeDilationSpeed speed)
    {
        if (speed == TimeDilationSpeed.normalSpeed)
        {
            SetMusicToNormalSpeed();
        }
        else
        {
            SetMusicToFastSpeed();
        }
    }

    private void SetMusicToNormalSpeed()
    {
        normalSpeedAudioSource.time = fastSpeedAudioSource.time * SPEED_DIFFERENCE;
        fastSpeedAudioSource.volume = 0.0f;
        normalSpeedAudioSource.volume = 70.0f;
    }

    private void SetMusicToFastSpeed()
    {
        fastSpeedAudioSource.time = normalSpeedAudioSource.time * SPEED_DIFFERENCE_RECIPROCAL;
        normalSpeedAudioSource.volume = 0.0f;
        fastSpeedAudioSource.volume = 70.0f;
    }

    public void StopMusic(float fadeDuration = 2.5f)
    {
        StartCoroutine(FadeOutAndStop(normalSpeedAudioSource, fadeDuration));
        StartCoroutine(FadeOutAndStop(fastSpeedAudioSource, fadeDuration));
    }

    private IEnumerator FadeOutAndStop(AudioSource source, float duration)
    {
        float startVolume = source.volume;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            source.volume = Mathf.Lerp(startVolume, 0f, time / duration);
            yield return null;
        }

        source.volume = 0f;
        source.Stop();
    }
}
