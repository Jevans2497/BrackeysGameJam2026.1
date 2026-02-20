using System.Collections;
using UnityEngine;

public class FinalTimeDilation : MonoBehaviour
{
    [SerializeField] private ParticleSystem particleSystem1;
    [SerializeField] private ParticleSystem particleSystem2;
    [SerializeField] private ParticleSystem particleSystem3;
    [SerializeField] private Transform centerPoint;
    public bool isBeingConsumed;

    private ParticleSystem[] particleSystems;
    private Vector3[] originalScales;
    private int[] pulseCounters;

    public AudioClip finalTimeDilationSFX;

    private int consumptionCounter = 0;

    private void Start()
    {
        particleSystems = new ParticleSystem[]
        {
            particleSystem1,
            particleSystem2,
            particleSystem3
        };

        originalScales = new Vector3[particleSystems.Length];

        for (int i = 0; i < particleSystems.Length; i++)
        {
            originalScales[i] = particleSystems[i].transform.localScale;
        }

        pulseCounters = new int[3]
        {
            0, 0, 0
        };
    }

    public void Consume()
    {
        if (isBeingConsumed) return;
        isBeingConsumed = true;
        SFXManager.Instance.PlaySFX(finalTimeDilationSFX, 0.0f, 0.5f, false);
        RunConsumeAnimations();
    }

    private void RunConsumeAnimations()
    {
        for (int i = 0; i < particleSystems.Length; i++)
        {
            StartCoroutine(RunConsumeAnimation(particleSystems[i], i));
        }
    }

    private IEnumerator RunConsumeAnimation(ParticleSystem ps, int index)
    {
        float duration = 5.0f;
        int pulseCount = pulseCounters[index];

        Vector3 targetScale = Vector3.zero;
        Color originalColor = ps.main.startColor.color;
        Color targetColor = Color.black;

        float elapsedTime = 0f;
        while (elapsedTime < duration && elapsedTime < pulseCount + 3.25f)
        {
            float t = elapsedTime / duration;
            t *= t * t;

            ps.transform.localScale = Vector3.Lerp(originalScales[index], targetScale, t);

            if (pulseCounters[index] == 2)
            {
                var main = ps.main;
                main.startColor = Color.Lerp(originalColor, targetColor, t / t * 0.5f);
            }

            if (t / t > 0.6f)
            {
                FinalLevelCameraManager.Instance.ShakeCamera(2.0f, 1.0f);
            }

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        pulseCounters[index] += 1;
        if (pulseCounters[index] >= 3)
        {
            RunFinalExplosions();
        }
        else
        {
            StartCoroutine(ResetTimeDilation(ps, index));
        }
    }

    private IEnumerator ResetTimeDilation(ParticleSystem ps, int index)
    {
        float duration = 0.2f;

        Vector3 startScale = ps.transform.localScale;
        Vector3 targetScale = originalScales[index];

        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            t *= t * t;

            ps.transform.localScale = Vector3.Lerp(startScale, targetScale, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(1.0f);
        isBeingConsumed = false;
    }

    private void RunFinalExplosions()
    {
        for (int i = 0; i < particleSystems.Length; i++)
        {
            StartCoroutine(RunFinalExplosion(particleSystems[i], i));
        }
    }

    private IEnumerator RunFinalExplosion(ParticleSystem ps, int index)
    {
        yield return new WaitForSeconds(0.5f);

        float duration = 3.0f;

        Vector3 startScale = ps.transform.localScale;
        Vector3 targetScale = originalScales[index] + (originalScales[index] * 5);
        Vector3 startingPos = ps.transform.position;
        Vector3 endPos = ps.transform.position + Vector3.down * 1.5f;

        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;

            ps.transform.position = Vector3.Lerp(startingPos, endPos, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(1.0f);

        FinalLevelCameraManager.Instance.ShakeCamera(2.0f, 5.0f);
        duration = 0.5f;
        elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;

            ps.transform.localScale = Vector3.Lerp(startScale, targetScale, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

    }

}