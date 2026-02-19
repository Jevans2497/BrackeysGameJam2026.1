using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using TMPro;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance;

    [SerializeField] private ColorPalette colorPalette;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask electrifiedPlatformLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.1f;
    [SerializeField] private ParticleSystem loseTimeDilationParticles;

    private List<LayerMask> jumpableLayers = new List<LayerMask>();

    Rigidbody2D rb;
    CapsuleCollider2D col;
    Animator animator;
    SpriteRenderer sr;

    float moveInput;
    float jumpInput;
    bool consumeInput;
    bool resetInput;
    float resetInputHeldCounter;

    PlayerInputActions input;
    public bool isInputEnabled = true;

    //MOVEMENT
    private const float MAX_SPEED = 10f;
    private const float GROUND_ACCELERATION = 80f;
    private const float GROUND_DECELERATION = 50f;
    private const float AIR_ACCELERATION = 50f;
    private const float AIR_DECELERATION = 30f;
    public bool isFacingRight = true;

    //JUMPING
    private const float JUMP_FORCE = 13f;
    private const float JUMP_DELAY = 0.3f;
    private float jumpDelayTimer = 0.0f;
    private const float COYOTE_TIME = 0.10f;
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
    private bool isJumpEnabled = true;
    private bool isFallingEnabled = true;

    private TimeDilation currentTimeDilation;
    private TimeDilation currentlyCollidingTimeDilation;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

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
        jumpableLayers.AddRange(new LayerMask[] { groundLayer });
        LevelManager.Instance.OnResetLevel += Reset;
    }

    private void OnDisable()
    {
        LevelManager.Instance.OnResetLevel -= Reset;
    }

    void Update()
    {
        if (isInputEnabled)
        {
            ReadInput();
        }
    }

    void FixedUpdate()
    {
        HandleMoveInput();
        HandleJumpInput();
        UpdateJumpTimers();
        HandleJumpReleased();
        HandleFalling();
        HandleLandingEffects();
        HandleConsumeInput();
        HandleResetInput();
        HandleElectrifiedPlatform();
        velocityLastFrame = rb.linearVelocity;
        wasGroundedLastFrame = IsGrounded();

        if (IsGrounded())
        {
            isFallingEnabled = true;
            isJumpEnabled = true;
        }
    }

    private void Reset()
    {
        transform.position = LevelManager.Instance.GetCurrentSpawnPoint();
        ReleaseTimeDilation();
    }

    private void ReadInput()
    {
        moveInput = input.Player.Move.ReadValue<Vector2>().x;
        jumpInput = input.Player.Jump.ReadValue<Vector2>().y;
        jumpReleased = jumpInput < 0.1f;
        consumeInput = input.Player.Consume.IsPressed();
        resetInput = input.Player.Reset.IsPressed();
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
        bool canJump = (IsGrounded() || coyoteTimeTimer > 0f) && jumpDelayTimer <= 0f && isJumpEnabled;
        if (userPressedJump && canJump)
        {
            StartCoroutine(SquashAndStretch());
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, JUMP_FORCE);
            jumpDelayTimer = JUMP_DELAY;
        }
    }

    private void HandleConsumeInput()
    {
        if (consumeInput && currentlyCollidingTimeDilation != null && currentTimeDilation == null)
        {
            ConsumeTimeDilation();
            if (LevelManager.Instance.isFinalLevel())
            {
                LevelManager.Instance.TriggerInitialFinalLevelEvent();
            }
        }

        if (!consumeInput && currentTimeDilation != null)
        {
            ReleaseTimeDilation();
        }
    }

    private void HandleResetInput()
    {
        if (resetInput)
        {
            resetInputHeldCounter += Time.fixedDeltaTime;
            if (resetInputHeldCounter >= 0.4f)
            {
                LevelManager.Instance.ResetLevel();
                resetInputHeldCounter = 0.0f;
            }
        }
        else
        {
            resetInputHeldCounter = 0.0f;
        }
    }

    private void ConsumeTimeDilation()
    {
        SetToTimeState(currentlyCollidingTimeDilation.speed);
        currentlyCollidingTimeDilation.Consume(this);
        currentTimeDilation = currentlyCollidingTimeDilation;
        currentlyCollidingTimeDilation = null;
    }

    public void ReleaseTimeDilation()
    {
        ResetTimeState();
        currentTimeDilation = null;

        if (currentlyCollidingTimeDilation != null)
        {
            SetToTimeState(currentlyCollidingTimeDilation.speed);
        }
    }

    private void HandleJumpReleased()
    {
        if (isFallingEnabled == false) return;
        if (jumpReleased && rb.linearVelocity.y > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * JUMP_RELEASED_MULTIPLIER);
        }
    }

    private void HandleFalling()
    {
        if (isFallingEnabled == false) return;
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


    private void HandleElectrifiedPlatform()
    {
        if (IsOnElectrifiedPlatform())
        {
            isFallingEnabled = false;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, JUMP_FORCE * 1.25f);
        }
    }

    private bool IsGrounded()
    {
        return jumpableLayers.Any(layer => Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, layer));
    }

    private bool IsOnElectrifiedPlatform()
    {
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, electrifiedPlatformLayer);
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
        else if (other.gameObject.layer == LayerMask.NameToLayer("GammaLaser") || other.gameObject.layer == LayerMask.NameToLayer("DeathLine"))
        {
            LevelManager.Instance.ResetLevel();
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
            if (currentTimeDilation == null)
            {
                SetToTimeState(timeDilation.speed);
            }
        }
    }

    private void OnExitTimeDilation(Collider2D other)
    {
        TimeDilation timeDilation = other.GetComponent<TimeDilation>();
        if (timeDilation != null && timeDilation == currentlyCollidingTimeDilation)
        {
            currentlyCollidingTimeDilation = null;
            if (currentTimeDilation == null)
            {
                ResetTimeState();
            }
        }
    }

    private void SetToTimeState(TimeDilationSpeed speed)
    {

        Color newColor = speed == TimeDilationSpeed.normalSpeed ? colorPalette.timeDilationNormalSpeed : colorPalette.timeDilationFastSpeed;
        StartCoroutine(RunColorShiftAnimation(newColor, 0.5f));
        TimeManager.Instance.SetTimeDilation(speed);
    }

    private void ResetTimeState()
    {
        loseTimeDilationParticles.Play();
        TimeManager.Instance.ResetTimeToLevelTime();
        StartCoroutine(RunColorShiftAnimation(Color.white, 0.5f));
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

    public Transform GetCurrentTransform()
    {
        return transform;
    }

    public void DisableInput()
    {
        isInputEnabled = false;
        moveInput = 0.0f;
        jumpInput = 0.0f;
        consumeInput = false;
        resetInput = false;
    }

    public void EnableInput()
    {
        isInputEnabled = true;
    }
}