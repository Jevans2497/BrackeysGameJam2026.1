using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    public Player player;
    public List<Level> levels;
    private Level currentLevel;
    private Level currentLevelInstance;
    private Level lastLevelInstance;
    private Level nextLevelInstance;

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
        LoadCurrentLevel();
    }

    public void AdvanceLevel()
    {
        lastLevelInstance = currentLevelInstance;
        currentLevelIndex++;
        Level nextLevel = levels[currentLevelIndex];
        currentLevel = nextLevel;
        LoadCurrentLevel();
    }

    private void LoadCurrentLevel()
    {
        if (nextLevelInstance)
        {
            currentLevelInstance = nextLevelInstance;
            LoadNextLevelIfNeeded();
        }
        else
        {
            if (currentLevelIndex + 1 < levels.Count)
            {
                currentLevelInstance = Instantiate(currentLevel, new Vector3(currentLevelIndex * levelSeparationXDistance, 0.0f, 0.0f), Quaternion.identity);
                currentLevelInstance.SetupLevel();
                LoadNextLevelIfNeeded();
            }
        }

        TimeManager.Instance.SetTimeDilationForLevel(currentLevelInstance.levelSpeed);
        LevelCameraController.Instance.MoveCameraToLevel(currentLevelIndex);

        if (lastLevelInstance != null)
        {
            StartCoroutine(DestroyLastLevel());
        }
    }

    private void LoadNextLevelIfNeeded()
    {
        if (currentLevelIndex + 1 >= levels.Count) return;
        Level nextLevel = levels[currentLevelIndex + 1];
        nextLevelInstance = Instantiate(nextLevel, new Vector3((currentLevelIndex + 1) * levelSeparationXDistance, 0.0f, 0.0f), Quaternion.identity);
        nextLevelInstance.SetupLevel();
    }

    private IEnumerator DestroyLastLevel()
    {
        yield return new WaitForSeconds(2.0f);
        if (lastLevelInstance != null)
            Destroy(lastLevelInstance.gameObject);
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
