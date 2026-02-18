using UnityEngine;

public class FallingBox : MonoBehaviour
{
    [SerializeField] ColorPalette colorPalette;
    private SpriteRenderer sr;

    private float defaultAnimationSpeed = 2f;
    private float currentAnimationSpeed;
    private float initialY = 10f;
    private const float SPEED_MULTIPLIER = 5F;

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

    private void FixedUpdate()
    {
        // if (transform.position.y < -initialY)
        // {
        //     transform.position = new Vector3(transform.position.x, initialY, transform.position.z);
        // }

        transform.position -= new Vector3(0, currentAnimationSpeed * Time.fixedDeltaTime, 0);
    }

    private void HandleTimeDilationChanged(TimeDilationSpeed newSpeed)
    {
        switch (newSpeed)
        {
            case TimeDilationSpeed.normalSpeed:
                currentAnimationSpeed = defaultAnimationSpeed;
                sr.color = colorPalette.timeDilationNormalSpeed;
                break;
            case TimeDilationSpeed.fastSpeed:
                currentAnimationSpeed = defaultAnimationSpeed * SPEED_MULTIPLIER;
                sr.color = colorPalette.timeDilationFastSpeed;
                break;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("DeathLine"))
        {
            transform.position = new Vector3(transform.position.x, initialY, transform.position.z);
        }
    }
}
