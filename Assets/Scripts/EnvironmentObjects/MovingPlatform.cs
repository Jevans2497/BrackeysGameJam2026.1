using System.Collections;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    private float defaultAnimationSpeed = 3f;
    private float currentAnimationSpeed = 15f;
    private float initialY;
    private float verticalTravelDistance = 1f;

    public BoxCollider2D collider;

    public Vector2 Delta { get; private set; }
    private Vector2 lastPosition;

    private void FixedUpdate()
    {
        Delta = (Vector2)transform.position - lastPosition;
        lastPosition = transform.position;
    }

    private void Awake()
    {
        TimeManager.Instance.OnTimeDilationChange += HandleTimeDilationChanged;
    }

    private void Start()
    {
        initialY = transform.position.y;
        StartCoroutine(RunAnimation());
    }

    private void HandleTimeDilationChanged(TimeDilationSpeed newSpeed)
    {
        switch (newSpeed)
        {
            case TimeDilationSpeed.normalSpeed:
                HandleDefaultSpeed();
                break;
            case TimeDilationSpeed.fastSpeed:
                HandleFastSpeed();
                break;
        }
    }

    private void HandleDefaultSpeed()
    {
        currentAnimationSpeed = defaultAnimationSpeed;
        collider.enabled = true;
    }

    private void HandleFastSpeed()
    {
        currentAnimationSpeed = defaultAnimationSpeed * 5f;
        collider.enabled = false;
    }

    private IEnumerator RunAnimation()
    {
        bool isTravelingUp = true;
        while (true)
        {
            if (transform.position.y < initialY)
            {
                isTravelingUp = true;
            }
            else if (transform.position.y > initialY + verticalTravelDistance)
            {
                isTravelingUp = false;
            }

            transform.position += new Vector3(0, (isTravelingUp ? 1 : -1) * currentAnimationSpeed * Time.deltaTime, 0);
            yield return null;
        }
    }
}
