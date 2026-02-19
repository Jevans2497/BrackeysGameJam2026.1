using System;
using UnityEngine;

public enum TimeDilationSpeed
{
    normalSpeed, fastSpeed
}

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance;
    public TimeDilationSpeed currentSpeed;
    private TimeDilationSpeed levelSpeed;
    public Action<TimeDilationSpeed> OnTimeDilationChange;

    public AudioClip speedTimeSFX;
    public AudioClip slowTimeSFX;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        SetTimeDilation(TimeDilationSpeed.fastSpeed);
    }

    public void SetTimeDilation(TimeDilationSpeed speed)
    {
        if (currentSpeed != speed)
        {
            currentSpeed = speed;
            PlayTimeDilationSFX();
            OnTimeDilationChange?.Invoke(speed);
        }
    }

    private void PlayTimeDilationSFX()
    {
        switch (currentSpeed)
        {
            case TimeDilationSpeed.normalSpeed:
                SFXManager.Instance.PlaySFX(slowTimeSFX, 0.0f, 1.0f, false);
                break;
            case TimeDilationSpeed.fastSpeed:
                SFXManager.Instance.PlaySFX(speedTimeSFX, 0.0f, 1.0f, false);
                break;
        }
    }

    public void SetTimeDilationForLevel(TimeDilationSpeed speed)
    {
        levelSpeed = speed;
        ResetTimeToLevelTime();
    }

    public void ResetTimeToLevelTime()
    {
        SetTimeDilation(levelSpeed);
    }
}
