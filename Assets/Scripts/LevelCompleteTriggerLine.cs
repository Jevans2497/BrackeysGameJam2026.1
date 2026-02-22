using UnityEngine;

public class LevelCompleteTriggerLine : MonoBehaviour
{

    public AudioClip levelCompleteSFX;

    private bool enteredLeft = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();
        enteredLeft = false;
        if (player != null)
        {
            player.ReleaseTimeDilation();
            SFXManager.Instance.PlaySFX(levelCompleteSFX, 0.0f, 100f, false);
            if (player.isFacingRight)
                enteredLeft = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();
        if (player != null)
        {
            if (player.isFacingRight && enteredLeft)
            {
                LevelManager.Instance.AdvanceLevel();
                DisableCollider();
            }
        }
    }

    private void DisableCollider()
    {
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        collider.isTrigger = false;
    }
}
