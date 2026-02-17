using UnityEngine;

public class Level : MonoBehaviour
{
    [SerializeField] private GameObject LevelCompleteTriggerLinePrefab;

    private LevelCompleteTriggerLine levelCompleteTriggerLineInstance;
    public int levelNumber;
    public TimeDilationSpeed levelSpeed;
    public float playerSpawnPointX;
    public float playerSpawnPointY;

    public void SetupLevel()
    {
        Debug.Log("Setting up level " + levelNumber);
        float baseXDistance = LevelManager.Instance.levelSeparationXDistance;
        float xPos = baseXDistance * levelNumber + 8.0f;
        GameObject go = Instantiate(LevelCompleteTriggerLinePrefab, new Vector3(xPos, 0.0f, 0.0f), Quaternion.identity);
        levelCompleteTriggerLineInstance = go.GetComponent<LevelCompleteTriggerLine>();
    }

    private void OnDestroy()
    {
        Debug.Log("In OnDestroy");

        if (levelCompleteTriggerLineInstance != null)
        {
            Debug.Log("Instance is not null");

            ParticleSystem ps = levelCompleteTriggerLineInstance.GetComponentInChildren<ParticleSystem>();
            if (ps != null)
            {
                Debug.Log("PS is not null");
                ps.Stop();
            }
        }
    }
}
