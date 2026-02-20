using System.Collections;
using UnityEngine;

public class LevelCameraController : MonoBehaviour
{
    public static LevelCameraController Instance;

    [Header("Level Camera Settings")]
    [SerializeField] private float distancePerLevel = 20f;
    [SerializeField] private float moveDuration = 1.2f;
    [SerializeField] private AnimationCurve easeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private Vector3 startPosition;
    private Coroutine moveRoutine;

    // Shake variables
    private Coroutine shakeRoutine;
    private Vector3 shakeOffset;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        startPosition = transform.position;
    }

    private void LateUpdate()
    {
        // Always apply shake after movement
        transform.position += shakeOffset;
    }

    public void MoveCameraToLevel(int levelIndex)
    {
        Vector3 targetPosition = startPosition + Vector3.right * (distancePerLevel * levelIndex);

        if (moveRoutine != null)
            StopCoroutine(moveRoutine);

        moveRoutine = StartCoroutine(MoveCameraSmooth(targetPosition));
    }

    private IEnumerator MoveCameraSmooth(Vector3 targetPosition)
    {
        Vector3 initialPosition = transform.position - shakeOffset;
        float time = 0f;

        while (time < moveDuration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / moveDuration);
            float easedT = easeCurve.Evaluate(t);

            Vector3 basePosition = Vector3.Lerp(initialPosition, targetPosition, easedT);
            transform.position = basePosition;

            yield return null;
        }

        transform.position = targetPosition;
    }

}
