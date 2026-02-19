using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance;

    [Header("SFX Settings")]
    [SerializeField] private AudioSource sfxPrefab;
    [SerializeField] private int poolSize = 10;
    [SerializeField] private float pitchVariance = 0.05f;

    private AudioSource[] pool;
    private int poolIndex = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Create pool
        pool = new AudioSource[poolSize];
        for (int i = 0; i < poolSize; i++)
        {
            pool[i] = Instantiate(sfxPrefab, transform);
        }
    }

    public void PlaySFX(AudioClip clip, float pitchShiftAmount = 0.0f, float volume = 1f, bool isPitchRandomized = true)
    {
        if (clip == null) return;

        AudioSource source = pool[poolIndex];
        poolIndex = (poolIndex + 1) % pool.Length;

        if (isPitchRandomized)
        {
            source.pitch = 1f + Random.Range(-pitchVariance, pitchVariance);
        }

        source.pitch += pitchShiftAmount;

        source.PlayOneShot(clip, volume);
    }
}
