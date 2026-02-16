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
    public Action<TimeDilationSpeed> OnTimeDilationChange;

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
        currentSpeed = speed;
        OnTimeDilationChange?.Invoke(speed);
    }
}
