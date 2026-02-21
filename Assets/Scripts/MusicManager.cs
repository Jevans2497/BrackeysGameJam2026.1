using UnityEngine;
using UnityEngine.Audio;

public class MusicManager : MonoBehaviour
{

    public AudioSource normalSpeedAudioSource;
    public AudioSource fastSpeedAudioSource;

    private const float SPEED_DIFFERENCE = 1.53409f;
    private const float SPEED_DIFFERENCE_RECIPROCAL = 0.65185185185f;

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
}
