using UnityEngine;

public class SpaceObject : MonoBehaviour
{
    private const float xMin = -10;
    private const float xMax = 303;
    private const float yMin = -1f;
    private const float yMax = 7.5f;

    private const float xTravelMin = 0.001f;
    private const float xTravelMax = 0.03f;
    private const float yTravelMin = 0.0005f;
    private const float yTravelMax = 0.005f;
    private float currentXTravel;
    private float currentYTravel;

    float currentXMin = -10.0f;

    void Start()
    {
        SetRandomPosition();
        SetTravelPath();
    }

    void FixedUpdate()
    {
        TimeDilationSpeed speed = TimeManager.Instance.currentSpeed;
        if (speed == TimeDilationSpeed.normalSpeed)
        {
            transform.position += new Vector3(currentXTravel, currentYTravel, 0.0f);
        }
        else if (speed == TimeDilationSpeed.fastSpeed)
        {
            transform.position += new Vector3(currentXTravel, currentYTravel, 0.0f) * 3f;
        }
        ResetIfOutOfBounds();
    }

    private float GetTravelValue(float minAbs, float maxAbs)
    {
        float value = Random.Range(minAbs, maxAbs);
        return Random.value < 0.5f ? value : -value;
    }

    private void ResetIfOutOfBounds()
    {
        if (transform.position.x > xMax || transform.position.x < xMin || transform.position.y < yMin || transform.position.y > yMax)
        {
            SetRandomPosition();
            SetTravelPath();
        }
    }

    private void SetRandomPosition()
    {
        int levelNum = LevelManager.Instance.GetCurrentLevelIndex();
        currentXMin = levelNum * 20.0f - xMin;

        float randomXPos = Random.Range(currentXMin, xMax);
        float randomYPos = Random.Range(yMin, yMax);

        transform.position = new Vector3(randomXPos, randomYPos, transform.position.z);
    }

    private void SetTravelPath()
    {
        float randomXTravel = Random.Range(xTravelMin, xTravelMax);
        float randomYTravel = Random.Range(yTravelMin, yTravelMin);
        currentXTravel = GetTravelValue(xTravelMin, xTravelMax);
        currentYTravel = GetTravelValue(yTravelMin, yTravelMax);
    }
}
