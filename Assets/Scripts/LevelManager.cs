using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    public Player player;
    // public GameObject levelsParent;
    public List<GameObject> levels;

    int currentLevelIndex = 0;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void AdvanceLevel()
    {
        currentLevelIndex++;
        GameObject currentLevel = levels[currentLevelIndex];
        Instantiate(currentLevel, new Vector3(currentLevelIndex * 20.0f, 0.0f, 0.0f), Quaternion.identity);
        LevelCameraController.Instance.MoveCameraToLevel(currentLevelIndex);
    }

    public void GoBackLevel()
    {
        currentLevelIndex--;
        LevelCameraController.Instance.MoveCameraToLevel(currentLevelIndex);
    }
}
