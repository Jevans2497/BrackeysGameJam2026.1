using UnityEngine;

public class LevelBackWall : MonoBehaviour
{
    private bool enteredLeft = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Enetered back wall");
        Player player = collision.GetComponent<Player>();
        enteredLeft = false;
        if (player != null)
        {
            player.ReleaseTimeDilation();

            if (player.isFacingRight)
                enteredLeft = true;
            else
            {
                DisableCollider();
            }


        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("Exit back wall");
        Player player = collision.GetComponent<Player>();
        if (player != null)
        {
            if (player.isFacingRight && enteredLeft)
            {
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
