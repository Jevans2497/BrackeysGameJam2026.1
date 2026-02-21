using System.Collections;
using UnityEngine;

public class TimeDilation : MonoBehaviour
{

    [SerializeField] private ParticleSystem ps;
    [SerializeField] private ColorPalette colorPalette;
    public TimeDilationSpeed speed;
    Vector2 spawnPoint;
    Vector3 originalScale;
    public bool isBeingConsumed = false;

    public AudioClip timeDilationSFX;

    private void Start()
    {
        spawnPoint = transform.position;
        originalScale = ps.transform.localScale;
        LevelManager.Instance.OnResetLevel += ResetTimeDilation;
    }

    private void OnDisable()
    {
        LevelManager.Instance.OnResetLevel -= ResetTimeDilation;
    }

    public void Consume(Player player)
    {
        SFXManager.Instance.PlaySFX(timeDilationSFX, 0.0f, 0.5f, false);
        StartCoroutine(RunConsumeAnimation(player, 0.75f));
        isBeingConsumed = true;
    }

    public IEnumerator RunConsumeAnimation(Player player, float duration)
    {
        Vector3 originalScale = this.ps.transform.localScale;
        Vector3 targetScale = Vector3.zero;
        Vector3 currentPosition = this.transform.position;
        Vector3 targetPosition = player.transform.position;

        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            targetPosition = player.transform.position;
            float t = elapsedTime / duration;
            t = t * t * t;

            this.ps.transform.localScale = Vector3.Lerp(originalScale, targetScale, t);
            this.transform.position = Vector3.Lerp(currentPosition, targetPosition, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        this.ps.transform.localScale = targetScale;
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
                return ps.main.startColor.color;
        }
    }

    public void ResetTimeDilation()
    {
        StopAllCoroutines();
        isBeingConsumed = false;
        this.ps.transform.localScale = originalScale;
        this.transform.position = spawnPoint;
        CircleCollider2D collider = GetComponent<CircleCollider2D>();
        collider.enabled = true;
    }
}
