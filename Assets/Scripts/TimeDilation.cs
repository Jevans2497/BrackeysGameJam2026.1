using System.Collections;
using UnityEngine;

public class TimeDilation : MonoBehaviour
{

    [SerializeField] private ParticleSystem particleSystem;
    [SerializeField] private ColorPalette colorPalette;
    public TimeDilationSpeed speed;
    Vector2 spawnPoint;
    Vector3 originalScale;

    public AudioClip timeDilationSFX;

    private void Start()
    {
        spawnPoint = transform.position;
        originalScale = particleSystem.transform.localScale;
        LevelManager.Instance.OnResetLevel += ResetTimeDilation;
    }

    private void OnDisable()
    {
        LevelManager.Instance.OnResetLevel -= ResetTimeDilation;
    }

    public void Consume(Player player)
    {
        SFXManager.Instance.PlaySFX(timeDilationSFX, 0.0f, 1.0f, false);
        StartCoroutine(RunConsumeAnimation(player, 0.75f));
    }

    public IEnumerator RunConsumeAnimation(Player player, float duration)
    {
        Vector3 originalScale = this.particleSystem.transform.localScale;
        Vector3 targetScale = Vector3.zero;
        Vector3 currentPosition = this.transform.position;
        Vector3 targetPosition = player.transform.position;

        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            targetPosition = player.transform.position;
            float t = elapsedTime / duration;
            // if (t > 0.85) t = 1f;
            t = t * t * t;

            this.particleSystem.transform.localScale = Vector3.Lerp(originalScale, targetScale, t);
            this.transform.position = Vector3.Lerp(currentPosition, targetPosition, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        this.particleSystem.transform.localScale = targetScale;
        this.transform.position = targetPosition;
        CircleCollider2D collider = GetComponent<CircleCollider2D>();
        collider.enabled = false;
    }

    public Color GetColor()
    {
        switch (speed)
        {
            case TimeDilationSpeed.normalSpeed:
                return colorPalette.timeDilationNormalSpeed;
            case TimeDilationSpeed.fastSpeed:
                return colorPalette.timeDilationFastSpeed;
            default:
                return particleSystem.main.startColor.color;
        }
    }

    public void ResetTimeDilation()
    {
        StopAllCoroutines();
        this.particleSystem.transform.localScale = originalScale;
        this.transform.position = spawnPoint;
        CircleCollider2D collider = GetComponent<CircleCollider2D>();
        collider.enabled = true;
    }
}
