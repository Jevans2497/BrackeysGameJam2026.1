using UnityEngine;
using UnityEngine.UIElements;

public class MovingPlatformContainer : MonoBehaviour
{
    public MovingPlatform platform;
    public BoxCollider2D boxCollider;
    public GameObject leftSide;
    public GameObject rightSide;

    private void Start()
    {
        boxCollider.enabled = false;
        TimeManager.Instance.OnTimeDilationChange += HandleTimeDilationChanged;
    }

    private void HandleTimeDilationChanged(TimeDilationSpeed newSpeed)
    {
        if (newSpeed == TimeDilationSpeed.fastSpeed)
        {
            boxCollider.enabled = true;
            leftSide.SetActive(true);
            rightSide.SetActive(true);
        }
        else
        {
            boxCollider.enabled = false;
            leftSide.SetActive(false);
            rightSide.SetActive(false);
        }
    }
}
