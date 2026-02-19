using UnityEngine;
using UnityEngine.Audio;

public class MusicManager : MonoBehaviour
{

    public AudioSource normalSpeedAudioSource;
    public AudioSource fastSpeedAudioSource;

    private float syncTimer;

    private const float SPEED_DIFFERENCE = 1.53409f;
    private const float SPEED_DIFFERENCE_RECIPROCAL = 0.65185185185f;

    private bool isNormalSpeed;

    void Start()
    {
        TimeManager.Instance.OnTimeDilationChange += SetMusicSpeed;
        SetMusicToFastSpeed();
    }

    private void OnDestroy()
    {
        TimeManager.Instance.OnTimeDilationChange -= SetMusicSpeed;
    }

    private void FixedUpdate()
    {
        syncTimer += Time.fixedDeltaTime;
    }

    private void SetMusicSpeed(TimeDilationSpeed speed)
    {
        if (speed == TimeDilationSpeed.normalSpeed)
        {
            if (!isNormalSpeed)
            {
                SetMusicToNormalSpeed();
            }
        }
        else
        {
            if (isNormalSpeed)
            {
                SetMusicToFastSpeed();
            }
        }
    }

    private void SetMusicToNormalSpeed()
    {
        normalSpeedAudioSource.time = syncTimer * SPEED_DIFFERENCE;
        syncTimer = syncTimer * SPEED_DIFFERENCE;

        fastSpeedAudioSource.volume = 0.0f;
        normalSpeedAudioSource.volume = 100.0f;
        isNormalSpeed = true;
    }

    private void SetMusicToFastSpeed()
    {
        fastSpeedAudioSource.time = syncTimer * SPEED_DIFFERENCE_RECIPROCAL;
        syncTimer = syncTimer * SPEED_DIFFERENCE_RECIPROCAL;

        normalSpeedAudioSource.volume = 0.0f;
        fastSpeedAudioSource.volume = 100.0f;
        isNormalSpeed = false;
    }
}
