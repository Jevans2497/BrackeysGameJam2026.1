using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    public Player player;
    // public GameObject levelsParent;
    public List<Level> levels;
    public Level currentLevel;
    public Level currentLevelInstance;
    public Level lastLevelInstance;

    public float levelSeparationXDistance = 20.0f;

    public Action OnResetLevel;

    int currentLevelIndex = 0;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        currentLevel = levels[currentLevelIndex];
        InstantiateLevel();
    }

    public void AdvanceLevel()
    {
        lastLevelInstance = currentLevelInstance;
        currentLevelIndex++;
        Level nextLevel = levels[currentLevelIndex];
        currentLevel = nextLevel;
        InstantiateLevel();
    }

    private void InstantiateLevel()
    {
        currentLevelInstance = Instantiate(currentLevel, new Vector3(currentLevelIndex * levelSeparationXDistance, 0.0f, 0.0f), Quaternion.identity);
        currentLevelInstance.SetupLevel();
        TimeManager.Instance.SetTimeDilationForLevel(currentLevelInstance.levelSpeed);
        LevelCameraController.Instance.MoveCameraToLevel(currentLevelIndex);

        if (lastLevelInstance != null)
        {
            StartCoroutine(DestroyLastLevel());
        }
    }

    private IEnumerator DestroyLastLevel()
    {
        yield return new WaitForSeconds(2.0f);
        if (lastLevelInstance != null)
            Destroy(lastLevelInstance.gameObject);
    }

    public void GoBackLevel()
    {
        currentLevelIndex--;
        LevelCameraController.Instance.MoveCameraToLevel(currentLevelIndex);
    }

    public Transform GetCurrentSpawnPoint()
    {
        return currentLevel.playerSpawnPoint;
    }

    public void ResetLevel()
    {
        OnResetLevel?.Invoke();
    }
}
