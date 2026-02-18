using UnityEngine;

public class Level : MonoBehaviour
{
    [SerializeField] private GameObject LevelCompleteTriggerLinePrefab;
    private LevelCompleteTriggerLine levelCompleteTriggerLineInstance;

    [SerializeField] private GameObject levelBackWallPrefab;
    // private 

    public int levelNumber;
    public TimeDilationSpeed levelSpeed;
    public Transform playerSpawnPoint;

    public void SetupLevel()
    {
        transform.position = Vector3.zero + new Vector3(levelNumber * LevelManager.Instance.levelSeparationXDistance, 0, 0);

        float baseXDistance = LevelManager.Instance.levelSeparationXDistance;
        float levelTriggerXPos = baseXDistance * levelNumber + 8.0f;
        GameObject levelTriggerGO = Instantiate(LevelCompleteTriggerLinePrefab, new Vector3(levelTriggerXPos, 0.0f, 0.0f), Quaternion.identity);
        levelCompleteTriggerLineInstance = levelTriggerGO.GetComponent<LevelCompleteTriggerLine>();

        float backWallXPos = baseXDistance * levelNumber - 10.5f;
        GameObject backWallGO = Instantiate(levelBackWallPrefab, new Vector3(backWallXPos, 0.0f, 0.0f), Quaternion.identity);
    }

    private void OnDestroy()
    {
        if (levelCompleteTriggerLineInstance != null)
        {
            ParticleSystem ps = levelCompleteTriggerLineInstance.GetComponentInChildren<ParticleSystem>();
            if (ps != null)
            {
                ps.Stop();
            }
        }
    }
}
