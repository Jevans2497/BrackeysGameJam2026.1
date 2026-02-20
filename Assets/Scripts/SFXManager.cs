using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance;

    [Header("SFX Settings")]
    [SerializeField] private AudioSource sfxPrefab;
    [SerializeField] private float pitchVariance = 0.05f;

    private List<AudioSource> pool = new List<AudioSource>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void PlaySFX(AudioClip clip, float pitchShiftAmount = 0f, float volume = 1f, bool randomizePitch = true, bool isLoop = false)
    {
        if (clip == null) return;

        AudioSource source = GetAvailableSource();

        source.pitch = 1f;
        source.loop = isLoop;
        source.volume = volume;
        source.clip = clip;

        if (randomizePitch)
            source.pitch += Random.Range(-pitchVariance, pitchVariance);

        source.pitch += pitchShiftAmount;

        source.Play();
    }

    private AudioSource GetAvailableSource()
    {
        foreach (var source in pool)
        {
            if (!source.isPlaying)
                return source;
        }

        AudioSource newSource = Instantiate(sfxPrefab, transform);
        pool.Add(newSource);

        return newSource;
    }

    public void StopClip(AudioClip clip)
    {
        foreach (var source in pool)
        {
            if (source.isPlaying && source.clip == clip)
            {
                source.Stop();
            }
        }
    }
}
