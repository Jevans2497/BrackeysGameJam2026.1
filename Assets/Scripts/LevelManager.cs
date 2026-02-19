using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    public bool inDebugMode = false;
    public bool isDebuggingFinalLevel = false;
    public TimeDilationSpeed debugDilationSpeed;

    public Action OnResetLevel;
    public Action OnFinalLevelTriggerGammaLasers;
    public Action OnFinalLevelTriggerFallingBoxes;
    public Action OnFinalLevelTriggerCameraShift;
    public Action OnResetGame;

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
        if (inDebugMode)
        {
            TimeManager.Instance.SetTimeDilationForLevel(debugDilationSpeed);
            return;
        }

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

    public Vector3 GetCurrentSpawnPoint()
    {
        return currentLevel.playerSpawnPoint.position + new Vector3(currentLevelIndex * levelSeparationXDistance, 0, 0);
    }

    public void ResetLevel()
    {
        OnResetLevel?.Invoke();
    }

    public bool isFinalLevel()
    {
        return currentLevelIndex == levels.Last().levelNumber || isDebuggingFinalLevel;
    }

    public void TriggerInitialFinalLevelEvent()
    {
        OnFinalLevelTriggerGammaLasers?.Invoke();
        OnFinalLevelTriggerCameraShift?.Invoke();
    }

    public void TriggerFinalLevelFallingBoxes()
    {
        OnFinalLevelTriggerFallingBoxes?.Invoke();
    }
}
