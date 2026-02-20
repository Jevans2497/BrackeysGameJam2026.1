using System.Collections;
using UnityEngine;
using DG.Tweening;
using System;

public class FinalTimeDilation : MonoBehaviour
{
    [SerializeField] private ParticleSystem particleSystem1;
    [SerializeField] private ParticleSystem particleSystem2;
    [SerializeField] private ParticleSystem particleSystem3;
    public bool isBeingConsumed;

    private ParticleConsumeData[] particles;

    public AudioClip finalTimeDilationSFX;

    private int consumptionCounter = 0;

    private void Start()
    {
        particles = new ParticleConsumeData[]
        {
            new ParticleConsumeData(particleSystem1),
            new ParticleConsumeData(particleSystem2),
            new ParticleConsumeData(particleSystem3)
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
        FinalLevelCameraManager.Instance.SetToFinalSceneCamera();
        FinalLevelCameraManager.Instance.ShakeCamera(particles[0].duration, 1.0f, particles[0].duration == 5);
        for (int i = 0; i < particles.Length; i++)
        {
            RunConsumeAnimation(i);
        }
    }

    private void RunConsumeAnimation(int index)
    {
        ParticleConsumeData consumeData = particles[index];
        int pulseCount = consumeData.pulseCount;
        ParticleSystem ps = consumeData.system;
        Vector3 targetScale = new Vector3(0.001f, 0.002f, 0.0f);

        Color originalColor = ps.main.startColor.color;
        Color targetColor = Color.black;

        consumeData.activeTween = consumeData.system.transform
            .DOScale(targetScale, consumeData.duration)
            .SetEase((time, duration, overshoot, period) =>
            {
                float t = time / duration;

                float exponent = duration - 2;
                if (consumeData.pulseCount == 2)
                {
                    exponent += 10;
                }
                float progress = Mathf.Pow(Mathf.Pow(2f, 10f * (t - 1f)), exponent);
                return Mathf.Clamp01(progress);
            })
                    .OnComplete(() =>
            {
                consumeData.pulseCount += 1;
                consumeData.duration += 1.5f;

                if (consumeData.pulseCount >= 3)
                {
                    RunFinalExplosions();
                }
                else
                {
                    ResetTimeDilation(consumeData.system, index);
                }
            });

        if (consumeData.pulseCount < 2)
        {
            DOVirtual.DelayedCall(consumeData.duration - 0.025f, () =>
            {
                if (consumeData.activeTween != null && consumeData.activeTween.IsActive())
                {
                    consumeData.activeTween.Complete();
                }
            });
        }
    }

    private void ResetTimeDilation(ParticleSystem ps, int index)
    {
        float duration = 0.2f;

        Vector3 targetScale = particles[index].originalScale;

        ps.transform
            .DOScale(targetScale, duration)
            .SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                isBeingConsumed = false;
            });
    }

    private void RunFinalExplosions()
    {
        for (int i = 0; i < particles.Length; i++)
        {
            StartCoroutine(RunFinalExplosion(i));
        }
    }

    private IEnumerator RunFinalExplosion(int index)
    {
        yield return new WaitForSeconds(0.5f);
        ParticleSystem ps = particles[index].system;

        float duration = 3.5f;

        Vector3 startScale = ps.transform.localScale;
        Vector3 targetScale = particles[index].originalScale * 20;
        Vector3 startingPos = ps.transform.position;
        Vector3 endPos = ps.transform.position + Vector3.down * 2.0f;

        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;

            ps.transform.position = Vector3.Lerp(startingPos, endPos, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(0.3f);

        duration = 0.2f;
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

[System.Serializable]
class ParticleConsumeData
{
    public ParticleSystem system;
    public Vector3 originalScale;
    public int pulseCount;
    public float duration;
    public Tween activeTween;

    public ParticleConsumeData(ParticleSystem ps)
    {
        system = ps;
        originalScale = ps.transform.localScale;
        pulseCount = 0;
        duration = 3.0f;
    }
}
