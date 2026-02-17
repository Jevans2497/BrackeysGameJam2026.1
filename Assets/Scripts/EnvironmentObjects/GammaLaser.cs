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

    private void Start()
    {
        TimeManager.Instance.OnTimeDilationChange += HandleTimeDilationChanged;
        HandleTimeDilationChanged(TimeManager.Instance.currentSpeed);
    }

    private void OnDisable()
    {
        TimeManager.Instance.OnTimeDilationChange -= HandleTimeDilationChanged;
    }

    private void FixedUpdate()
    {
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
}
