using UnityEngine;

public class AlphaLaser : MonoBehaviour
{
    [SerializeField] ColorPalette colorPalette;
    private SpriteRenderer sr;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        TimeManager.Instance.OnTimeDilationChange += HandleTimeDilationChanged;
        HandleTimeDilationChanged(TimeManager.Instance.currentSpeed);
    }

    private void OnDisable()
    {
        TimeManager.Instance.OnTimeDilationChange -= HandleTimeDilationChanged;
    }

    private void HandleTimeDilationChanged(TimeDilationSpeed newSpeed)
    {
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        if (collider == null) return;
        switch (newSpeed)
        {
            case TimeDilationSpeed.normalSpeed:
                collider.enabled = false;
                sr.color = colorPalette.alphaLaserTransparent;
                gameObject.layer = LayerMask.NameToLayer("Default");
                break;
            case TimeDilationSpeed.fastSpeed:
                collider.enabled = true;
                sr.color = colorPalette.alphaLaserSolidify;
                gameObject.layer = LayerMask.NameToLayer("Ground");
                break;
        }
    }
}
