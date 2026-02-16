using System.Collections;
using UnityEngine;

public class GammaLaser : MonoBehaviour
{
    private float defaultAnimationSpeed = 3f;
    private float currentAnimationSpeed;
    private float initialY = 10f;
    public float initialDelay = 0f;

    private void Start()
    {
        currentAnimationSpeed = defaultAnimationSpeed;
        TimeManager.Instance.OnTimeDilationChange += HandleTimeDilationChanged;
        StartCoroutine(RunAnimation());
    }

    private void HandleTimeDilationChanged(TimeDilationSpeed newSpeed)
    {
        switch (newSpeed)
        {
            case TimeDilationSpeed.normalSpeed:
                currentAnimationSpeed = defaultAnimationSpeed;
                break;
            case TimeDilationSpeed.fastSpeed:
                currentAnimationSpeed = defaultAnimationSpeed * 10f;
                break;
        }
    }

    private IEnumerator RunAnimation()
    {
        yield return new WaitForSeconds(initialDelay * currentAnimationSpeed);
        while (true)
        {
            if (transform.position.y < -initialY)
            {
                transform.position = new Vector3(transform.position.x, initialY, transform.position.z);
            }
            transform.position -= new Vector3(0, currentAnimationSpeed * Time.deltaTime, 0);
            yield return null;
        }
    }
}
