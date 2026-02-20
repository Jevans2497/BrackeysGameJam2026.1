using UnityEngine;

public class FallingBox : MonoBehaviour
{
    [SerializeField] private Transform groundCheck;

    [SerializeField] ColorPalette colorPalette;
    [SerializeField] private AudioClip fallingBoxSFX;
    private SpriteRenderer sr;

    public int attachedToLevelNumber;

    private float defaultAnimationSpeed = 2f;
    private float currentAnimationSpeed;
    private float initialY = 7.5f;
    private const float SPEED_MULTIPLIER = 5F;

    private Collider2D fallingBoxCollider;

    private Vector3 initialPosition;

    public bool waitingForFinalLevelTrigger = false;
    private bool initialWaitingForFinalLevelTriggerValue = false;


    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        fallingBoxCollider = GetComponent<Collider2D>();
        initialWaitingForFinalLevelTriggerValue = waitingForFinalLevelTrigger;

        TimeManager.Instance.OnTimeDilationChange += HandleTimeDilationChanged;
        HandleTimeDilationChanged(TimeManager.Instance.currentSpeed);

        LevelManager.Instance.OnResetLevel += ResetFallingBox;
        LevelManager.Instance.OnFinalLevelTriggerFallingBoxes += TriggerFinalLevelFallingBoxes;

        initialPosition = this.transform.position;
    }

    private void OnDisable()
    {
        TimeManager.Instance.OnTimeDilationChange -= HandleTimeDilationChanged;
        LevelManager.Instance.OnResetLevel -= ResetFallingBox;
        LevelManager.Instance.OnFinalLevelTriggerFallingBoxes -= TriggerFinalLevelFallingBoxes;
    }

    private void FixedUpdate()
    {
        if (waitingForFinalLevelTrigger) return;
        if (!IsGrounded())
        {
            transform.position -= new Vector3(0, currentAnimationSpeed * Time.fixedDeltaTime, 0);
        }
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
            if (LevelManager.Instance.GetCurrentLevelIndex() == attachedToLevelNumber && TimeManager.Instance.currentSpeed == TimeDilationSpeed.fastSpeed)
            {
                SFXManager.Instance.PlaySFX(fallingBoxSFX, 0.0f, 0.1f, true, false);
            }
            transform.position = new Vector3(transform.position.x, initialY, transform.position.z);
        }
    }

    private bool IsGrounded()
    {
        int groundLayerMask = 1 << LayerMask.NameToLayer("Ground");
        Collider2D[] hits = Physics2D.OverlapCircleAll(groundCheck.position, 0.05f, groundLayerMask);

        foreach (var hit in hits)
        {
            if (hit != fallingBoxCollider)
                return true;
        }

        return false;
    }

    private void ResetFallingBox()
    {
        this.transform.position = initialPosition;
        waitingForFinalLevelTrigger = initialWaitingForFinalLevelTriggerValue;
    }

    private void TriggerFinalLevelFallingBoxes()
    {
        waitingForFinalLevelTrigger = false;
    }
}
