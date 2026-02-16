using System;
using System.Collections;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class Player : MonoBehaviour
{

    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float checkRadius = 0.1f;
    [SerializeField] private ParticleSystem loseTimeDilationParticles;

    Rigidbody2D rb;
    CapsuleCollider2D col;
    Animator animator;
    SpriteRenderer sr;

    float moveInput;
    float jumpInput;
    bool consumeInput;

    PlayerInputActions input;

    //MOVEMENT
    private const float MAX_SPEED = 15f;
    private const float GROUND_ACCELERATION = 80f;
    private const float GROUND_DECELERATION = 50f;
    private const float AIR_ACCELERATION = 50f;
    private const float AIR_DECELERATION = 30f;
    public bool isFacingRight = true;

    //JUMPING
    private const float JUMP_FORCE = 15f;
    private const float JUMP_DELAY = 0.3f;
    private float jumpDelayTimer = 0.0f;
    private const float COYOTE_TIME = 0.15f;
    private float coyoteTimeTimer;
    private const float JUMP_BUFFER_TIME = 0.1f;
    private float jumpBufferTimer = 0.0f;
    private bool jumpReleased;
    private const float JUMP_RELEASED_MULTIPLIER = 0.5f;
    private const float FALL_MULTIPLIER = 2.5f;
    private const float STRETCH_FACTOR = 1.3f;
    private const float SQUASH_FACTOR = 0.7f;
    private Vector3 originalScale;
    private Vector2 velocityLastFrame;
    private bool wasGroundedLastFrame;

    private TimeDilation currentTimeDilation;
    private TimeDilation currentlyCollidingTimeDilation;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<CapsuleCollider2D>();
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        input = new PlayerInputActions();
        input.Enable();
    }

    private void Start()
    {
        originalScale = transform.localScale;
    }

    void Update()
    {
        ReadInput();
    }

    void FixedUpdate()
    {
        HandleMoveInput();
        HandleJumpInput();
        UpdateJumpTimers();
        HandleJumpReleased();
        HandleFalling();
        HandleLandingEffects();
        velocityLastFrame = rb.linearVelocity;
        wasGroundedLastFrame = IsGrounded();
        HandleConsumeInput();
    }

    private void ReadInput()
    {
        moveInput = input.Player.Move.ReadValue<Vector2>().x;
        jumpInput = input.Player.Jump.ReadValue<Vector2>().y;
        jumpReleased = jumpInput < 0.1f;
        consumeInput = input.Player.Consume.IsPressed();
    }

    private void HandleMoveInput()
    {
        Vector2 velocity = rb.linearVelocity;
        bool isGrounded = IsGrounded();
        float targetSpeed = moveInput * MAX_SPEED;
        float accel = isGrounded ? GROUND_ACCELERATION : AIR_ACCELERATION;
        float decel = isGrounded ? GROUND_DECELERATION : AIR_DECELERATION;

        bool isAccelerating = Mathf.Abs(targetSpeed) > 0.01f;

        if (isAccelerating)
            velocity.x = Mathf.MoveTowards(rb.linearVelocity.x, targetSpeed, accel * Time.fixedDeltaTime);
        else
            velocity.x = Mathf.MoveTowards(rb.linearVelocity.x, 0f, decel * Time.fixedDeltaTime);

        rb.linearVelocity = new Vector2(velocity.x, rb.linearVelocity.y);

        HandleSpriteBehavior(isAccelerating, isGrounded);
    }

    private void HandleSpriteBehavior(bool isAccelerating, bool isGrounded)
    {
        animator.SetBool("isRunning", isAccelerating && isGrounded);
        if (rb.linearVelocity.x > 0.01f)
        {
            sr.flipX = false;   // Facing right
            isFacingRight = true;
        }
        else if (rb.linearVelocity.x < -0.01f)
        {
            sr.flipX = true;    // Facing left
            isFacingRight = false;
        }
    }

    private void HandleJumpInput()
    {
        bool userPressedJump = jumpInput > 0.1f || jumpBufferTimer > 0f;
        bool canJump = (IsGrounded() || coyoteTimeTimer > 0f) && jumpDelayTimer <= 0f;
        if (userPressedJump && canJump)
        {
            StartCoroutine(SquashAndStretch());
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, JUMP_FORCE);
            jumpDelayTimer = JUMP_DELAY;
        }
    }

    private void HandleConsumeInput()
    {
        if (consumeInput && currentlyCollidingTimeDilation != null)
        {
            ConsumeTimeDilation();
        }

        if (!consumeInput && currentTimeDilation != null)
        {
            ReleaseTimeDilation();
        }
    }

    private void ConsumeTimeDilation()
    {
        StartCoroutine(RunColorShiftAnimation(currentlyCollidingTimeDilation.GetColor(), 1.0f));
        currentlyCollidingTimeDilation.Consume(transform);
        currentTimeDilation = currentlyCollidingTimeDilation;
        TimeManager.Instance.SetTimeDilation(currentTimeDilation.speed);
        currentlyCollidingTimeDilation = null;
    }

    public void ReleaseTimeDilation()
    {
        StartCoroutine(RunColorShiftAnimation(Color.white, 1.0f));
        currentTimeDilation = null;
        TimeManager.Instance.SetTimeDilation(TimeDilationSpeed.fastSpeed);
        loseTimeDilationParticles.Play();
    }

    private void HandleJumpReleased()
    {
        if (jumpReleased && rb.linearVelocity.y > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * JUMP_RELEASED_MULTIPLIER);
        }
    }

    private void HandleFalling()
    {
        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * FALL_MULTIPLIER * Time.fixedDeltaTime;
        }
    }

    private void HandleLandingEffects()
    {
        if (IsGrounded() && !wasGroundedLastFrame && velocityLastFrame.y < -0.1f)
        {
            float minVelocity = -20f;
            float maxVelocity = -35f;

            if (velocityLastFrame.y > minVelocity) return;

            float normalized = Mathf.Clamp01((Mathf.Abs(velocityLastFrame.y) - Mathf.Abs(minVelocity)) /
                                             (Mathf.Abs(maxVelocity) - Mathf.Abs(minVelocity)));

            float duration = 0.1f + 0.05f * normalized;
            float magnitude = 0.05f + 0.075f * normalized;

            CameraShake.Instance.Shake(duration, magnitude);
        }
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);
    }

    private void UpdateJumpTimers()
    {
        UpdateCoyoteTime();
        UpdateJumpDelay();
        UpdateJumpBuffer();
    }

    private void UpdateCoyoteTime()
    {
        if (IsGrounded())
            coyoteTimeTimer = COYOTE_TIME;
        else
            coyoteTimeTimer -= Time.fixedDeltaTime;
    }

    private void UpdateJumpDelay()
    {
        if (jumpDelayTimer > 0f)
            jumpDelayTimer -= Time.fixedDeltaTime;
    }

    private void UpdateJumpBuffer()
    {
        if (jumpInput > 0.1f)
            jumpBufferTimer = JUMP_BUFFER_TIME;
        else if (jumpBufferTimer > 0f)
            jumpBufferTimer -= Time.fixedDeltaTime;
    }

    private IEnumerator SquashAndStretch()
    {

        float duration = 0.2f;
        Vector3 squashDownScale = new(originalScale.x * SQUASH_FACTOR, originalScale.y * STRETCH_FACTOR, originalScale.z);
        Vector3 stretchUpScale = new(originalScale.x * STRETCH_FACTOR, originalScale.y * SQUASH_FACTOR, originalScale.z);

        // First third: stretch
        float timer = 0f;
        while (timer < duration * 0.33f)
        {
            timer += Time.deltaTime;
            float t = timer / (duration * 0.33f);
            transform.localScale = Vector3.Lerp(originalScale, squashDownScale, t);
            yield return null;
        }

        // Second third: squash
        timer = 0f;
        while (timer < duration * 0.33f)
        {
            timer += Time.deltaTime;
            float t = timer / (duration * 0.33f);
            transform.localScale = Vector3.Lerp(squashDownScale, stretchUpScale, t);
            yield return null;
        }

        // third third: reset
        timer = 0f;
        while (timer < duration * 0.33f)
        {
            timer += Time.deltaTime;
            float t = timer / (duration * 0.33f);
            transform.localScale = Vector3.Lerp(stretchUpScale, originalScale, t);
            yield return null;
        }

        transform.localScale = originalScale;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("TimeDilation"))
        {
            OnEnteredTimeDilation(other);
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("GammaLaser"))
        {
            Debug.Log("Dead");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("TimeDilation"))
        {
            OnExitTimeDilation(other);
        }
    }



    private void OnEnteredTimeDilation(Collider2D other)
    {
        TimeDilation timeDilation = other.GetComponent<TimeDilation>();
        if (timeDilation != null)
        {
            currentlyCollidingTimeDilation = timeDilation;
        }
    }

    private void OnExitTimeDilation(Collider2D other)
    {
        TimeDilation timeDilation = other.GetComponent<TimeDilation>();
        if (timeDilation != null && timeDilation == currentlyCollidingTimeDilation)
        {
            currentlyCollidingTimeDilation = null;
        }
    }

    private IEnumerator RunColorShiftAnimation(Color targetColor, float duration)
    {
        Color originalColor = sr.color;
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            sr.color = Color.Lerp(originalColor, targetColor, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        sr.color = targetColor;
    }
}