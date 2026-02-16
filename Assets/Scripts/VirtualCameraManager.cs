using System.Collections;
using UnityEngine;

public class LevelCameraController : MonoBehaviour
{

    public static LevelCameraController Instance;

    [Header("Level Camera Settings")]
    [SerializeField] private float distancePerLevel = 20f;   // how far camera moves each level (X axis)
    [SerializeField] private float moveDuration = 1.2f;      // time to move between levels
    [SerializeField] private AnimationCurve easeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private Vector3 startPosition;
    private Coroutine moveRoutine;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        startPosition = transform.position;
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
        Vector3 initialPosition = transform.position;
        float time = 0f;

        while (time < moveDuration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / moveDuration);
            float easedT = easeCurve.Evaluate(t);

            transform.position = Vector3.Lerp(initialPosition, targetPosition, easedT);
            yield return null;
        }

        transform.position = targetPosition;
    }
}
