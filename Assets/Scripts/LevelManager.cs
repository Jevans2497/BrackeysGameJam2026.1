using UnityEngine;

public class LevelManager : MonoBehaviour
{

    public static LevelManager Instance;
    public Player player;

    int currentLevel = 0;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void AdvanceLevel()
    {
        currentLevel++;
        LevelCameraController.Instance.MoveCameraToLevel(currentLevel);
    }

    public void GoBackLevel()
    {
        currentLevel--;
        LevelCameraController.Instance.MoveCameraToLevel(currentLevel);
    }
}
