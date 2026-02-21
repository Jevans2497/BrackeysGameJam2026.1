using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class OpeningAndClosingShotManager : MonoBehaviour
{
    public static OpeningAndClosingShotManager Instance;
    [SerializeField] private Image blackImage;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        if (!LevelManager.Instance.inDebugMode)
        {
            Player.Instance.DisableInput();
            StartCoroutine(FadeIn());
        }
    }

    private IEnumerator FadeIn()
    {
        float elapsedTime = 0.0f;
        float duration = 2.5f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            t = t * t;
            blackImage.color = Color.Lerp(Color.black, new Color(0.0f, 0.0f, 0.0f, 0.0f), t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Player.Instance.EnableInput();
    }

    public void CutToBlack()
    {
        blackImage.color = Color.black;
    }
}
