using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float parallaxMultiplier = 0.5f;

    private Vector3 previousPlayerPosition;

    private void Start()
    {
        if (playerTransform == null)
            playerTransform = Player.Instance.transform;

        previousPlayerPosition = playerTransform.position;
    }

    private void LateUpdate()
    {
        Vector3 deltaMovement = playerTransform.position - previousPlayerPosition;

        transform.position -= new Vector3(
            deltaMovement.x * parallaxMultiplier,
            0.0f,
            0f);

        previousPlayerPosition = playerTransform.position;
    }
}
