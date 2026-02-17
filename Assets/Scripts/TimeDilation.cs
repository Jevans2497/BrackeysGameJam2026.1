using System.Collections;
using UnityEngine;

public class TimeDilation : MonoBehaviour
{

    [SerializeField] private ParticleSystem particleSystem;
    [SerializeField] private ColorPalette colorPalette;
    public TimeDilationSpeed speed;
    Vector2 spawnPoint;
    Vector3 originalScale;

    private void Start()
    {
        spawnPoint = transform.position;
        originalScale = particleSystem.transform.localScale;
    }

    public void Consume(Transform target, float duration = 1.0f)
    {
        StartCoroutine(RunConsumeAnimation(target, duration));
    }

    public IEnumerator RunConsumeAnimation(Transform target, float duration)
    {
        Vector3 originalScale = this.particleSystem.transform.localScale;
        Vector3 targetScale = Vector3.zero;
        Vector3 currentPosition = this.transform.position;
        Vector3 targetPosition = target.position;

        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            if (t > 0.85) t = 1f;
            this.particleSystem.transform.localScale = Vector3.Lerp(originalScale, targetScale, t);
            this.transform.position = Vector3.Lerp(currentPosition, targetPosition, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        this.particleSystem.transform.localScale = targetScale;
        this.transform.position = targetPosition;
    }

    public Color GetColor()
    {
        switch (speed)
        {
            case TimeDilationSpeed.normalSpeed:
                return colorPalette.timeDilationNormalSpeed;
            default:
                return particleSystem.main.startColor.color;
        }
    }

    public void ResetTimeDilation()
    {
        this.particleSystem.transform.localScale = originalScale;
        this.transform.position = spawnPoint;
    }
}
