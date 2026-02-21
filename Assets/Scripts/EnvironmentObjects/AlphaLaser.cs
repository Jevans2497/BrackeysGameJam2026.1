using UnityEngine;

public class AlphaLaser : MonoBehaviour
{
    private SpriteRenderer sr;
    public AudioClip alphaLaserSFX;
    public int attachedToLevelNumber;
    private Color initialColor;
    private ParticleSystem ps;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        initialColor = sr.color;
        ps = GetComponentInChildren<ParticleSystem>();
        SetParticleSystemShapeBasedOnTransform();
        TimeManager.Instance.OnTimeDilationChange += HandleTimeDilationChanged;
        HandleTimeDilationChanged(TimeManager.Instance.currentSpeed);
    }

    private void OnDisable()
    {
        TimeManager.Instance.OnTimeDilationChange -= HandleTimeDilationChanged;
    }

    private void SetParticleSystemShapeBasedOnTransform()
    {
        var shape = ps.shape;
        Vector3 scale = shape.scale;

        scale.y = transform.localScale.y * 10.0f;
        shape.scale = scale;
    }

    private void HandleTimeDilationChanged(TimeDilationSpeed newSpeed)
    {
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        if (collider == null) return;
        switch (newSpeed)
        {
            case TimeDilationSpeed.normalSpeed:
                collider.enabled = false;
                sr.color = new Color(initialColor.r, initialColor.g, initialColor.b, 0.25f);
                ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                gameObject.layer = LayerMask.NameToLayer("Default");
                StopAlphaLaserSFX();
                break;
            case TimeDilationSpeed.fastSpeed:
                gameObject.layer = LayerMask.NameToLayer("Ground");
                collider.enabled = true;
                sr.color = initialColor;
                ps.Play();
                PlayAlphaLaserSFX();
                break;
        }
    }

    private void PlayAlphaLaserSFX()
    {
        if (LevelManager.Instance.IsInCredits()) return;
        if (LevelManager.Instance.GetCurrentLevelIndex() == attachedToLevelNumber)
            SFXManager.Instance.PlaySFX(alphaLaserSFX, 0.0f, 0.5f, false, true);
    }

    private void StopAlphaLaserSFX()
    {
        SFXManager.Instance.StopClip(alphaLaserSFX);
    }
}
