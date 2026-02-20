using UnityEngine;

public class FinalLevelReleaseDilationLine : MonoBehaviour
{
    [SerializeField] GameObject deathLine;
    [SerializeField] FinalLevelCameraManager finalLevelCamera;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();
        if (player != null)
        {
            LevelManager.Instance.FinalLevelReleaseDilationLinePassed();
        }
    }
}
