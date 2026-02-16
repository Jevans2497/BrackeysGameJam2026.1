using UnityEngine;

public class LevelCompleteTriggerLine : MonoBehaviour
{

    private bool enteredLeft = false;
    private bool enteredRight = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();
        player.ReleaseTimeDilation();
        enteredLeft = false;
        enteredRight = false;
        if (player != null)
        {
            if (player.isFacingRight)
                enteredLeft = true;
            else
                enteredRight = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();
        if (player != null)
        {
            if (player.isFacingRight && enteredLeft)
                LevelManager.Instance.AdvanceLevel();
            else if (!player.isFacingRight && enteredRight)
                LevelManager.Instance.GoBackLevel();
        }
    }
}
