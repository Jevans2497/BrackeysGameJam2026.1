using UnityEngine;
using Cinemachine;
using System.Collections;
using NUnit.Framework;

public class FinalLevelCameraManager : MonoBehaviour
{
    public static FinalLevelCameraManager Instance;

    [Header("Camera")]
    [SerializeField] private CinemachineVirtualCamera finalLevelVcam;

    private Vector3 expectedOffset;


    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.OnFinalLevelTriggerCameraShift += SwitchToFinalLevelCamera;
            LevelManager.Instance.OnResetGame += ReleaseFinalLevelCamera;
            LevelManager.Instance.OnResetLevel += ReleaseFinalLevelCamera;
        }
    }

    private void OnDestroy()
    {
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.OnFinalLevelTriggerCameraShift -= SwitchToFinalLevelCamera;
            LevelManager.Instance.OnResetGame -= ReleaseFinalLevelCamera;
            LevelManager.Instance.OnResetLevel -= ReleaseFinalLevelCamera;
        }
    }

    private void SwitchToFinalLevelCamera()
    {
        SetCameraPriority(20);
    }

    private void ReleaseFinalLevelCamera()
    {
        if (finalLevelVcam != null)
            finalLevelVcam.Priority = 0;

        // Force Cinemachine to cut to the new camera immediately
        var brain = Camera.main.GetComponent<CinemachineBrain>();
        if (brain != null)
            brain.ManualUpdate(); // this forces the brain to instantly refresh the live camera
    }

    private void SetCameraPriority(int priority)
    {
        if (finalLevelVcam == null)
        {
            Debug.LogWarning("FinalLevelCameraManager: Final Level VCam is not assigned.");
            return;
        }

        finalLevelVcam.Priority = priority;

    }

    public void SetToCreditsState()
    {
        SetFinalCameraYOffset(0.0f);
    }

    private void SetFinalCameraYOffset(float offset)
    {
        if (finalLevelVcam == null)
        {
            Debug.LogWarning("FinalLevelVcam not assigned!");
            return;
        }

        // Get the Framing Transposer from the Virtual Camera
        var transposer = finalLevelVcam.GetCinemachineComponent<CinemachineFramingTransposer>();
        if (transposer != null)
        {
            transposer.m_TrackedObjectOffset.y = offset;
            expectedOffset.y = offset;
        }
        else
        {
            Debug.LogWarning("Framing Transposer not found on the Virtual Camera!");
        }

    }

    public void ResetFinalLevelCamera()
    {
        SetFinalCameraYOffset(3.0f);
        expectedOffset.y = 3.0f;
    }

    public void SetToFinalSceneCamera()
    {
        transposer = finalLevelVcam.GetCinemachineComponent<CinemachineFramingTransposer>();

        if (transposer == null)
        {
            Debug.LogWarning("FramingTransposer not found!");
            return;
        }

        transposer.m_TrackedObjectOffset = new Vector3(0, 3f, 0);
        expectedOffset.y = 3.0f;
    }

    private Coroutine shakeCoroutine;
    private CinemachineFramingTransposer transposer;

    public void ShakeCamera(float duration, float intensity, bool isLast)
    {
        transposer = finalLevelVcam.GetCinemachineComponent<CinemachineFramingTransposer>();

        if (transposer == null)
        {
            Debug.LogWarning("FramingTransposer not found!");
            return;
        }

        if (shakeCoroutine != null)
            StopCoroutine(shakeCoroutine);

        shakeCoroutine = StartCoroutine(ShakeRoutine(duration, intensity, isLast));
    }

    private IEnumerator ShakeRoutine(float duration, float intensity, bool isLast)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float offsetX = Random.Range(-0.1f, 0.1f) * intensity;
            float offsetY = Random.Range(-0.1f, 0.1f) * intensity;

            transposer.m_TrackedObjectOffset =
                expectedOffset + new Vector3(offsetX, offsetY, 0f);

            float intensityAddition = isLast ? 0.03f : 0.035f;
            intensity += intensityAddition;

            yield return null;
        }

        transposer.m_TrackedObjectOffset = expectedOffset;
        shakeCoroutine = null;
    }
}
