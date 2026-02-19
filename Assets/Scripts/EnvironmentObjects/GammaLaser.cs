using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GammaLaser : MonoBehaviour
{
    private float defaultAnimationSpeed = 3f;
    private float currentAnimationSpeed;
    private float initialY = 10f;
    private const float SPEED_MULTIPLIER = 10F;
    public float initialDelay = 0.0f;
    public bool waitingForFinalLevelTrigger = false;

    private Vector3 initialPosition;
    private float initialDelayForReset;
    private bool initialWaitingForFinalLevelTriggerValue = false;

    private void Start()
    {
        initialPosition = this.transform.position;
        initialDelayForReset = initialDelay;
        initialWaitingForFinalLevelTriggerValue = waitingForFinalLevelTrigger;

        TimeManager.Instance.OnTimeDilationChange += HandleTimeDilationChanged;
        LevelManager.Instance.OnFinalLevelTriggerGammaLasers += TriggerFinalLevel;
        LevelManager.Instance.OnResetLevel += Reset;
        HandleTimeDilationChanged(TimeManager.Instance.currentSpeed);
    }

    private void OnDisable()
    {
        TimeManager.Instance.OnTimeDilationChange -= HandleTimeDilationChanged;
        LevelManager.Instance.OnFinalLevelTriggerGammaLasers -= TriggerFinalLevel;
        LevelManager.Instance.OnResetLevel -= Reset;
    }

    private void FixedUpdate()
    {
        if (waitingForFinalLevelTrigger) return;
        if (initialDelay >= 0)
        {
            initialDelay -= Time.fixedDeltaTime;
            return;
        }
        if (transform.position.y < -initialY)
        {
            transform.position = new Vector3(transform.position.x, initialY, transform.position.z);
        }

        transform.position -= new Vector3(0, currentAnimationSpeed * Time.fixedDeltaTime, 0);
    }

    private void HandleTimeDilationChanged(TimeDilationSpeed newSpeed)
    {
        switch (newSpeed)
        {
            case TimeDilationSpeed.normalSpeed:
                currentAnimationSpeed = defaultAnimationSpeed;
                break;
            case TimeDilationSpeed.fastSpeed:
                currentAnimationSpeed = defaultAnimationSpeed * SPEED_MULTIPLIER;
                break;
        }
    }

    private void Reset()
    {
        transform.position = initialPosition;
        initialDelay = initialDelayForReset;
        waitingForFinalLevelTrigger = initialWaitingForFinalLevelTriggerValue;
    }

    private void TriggerFinalLevel()
    {
        waitingForFinalLevelTrigger = false;
    }
}
