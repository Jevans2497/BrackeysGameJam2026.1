using UnityEngine;

public class DialogueSpawner : MonoBehaviour
{
    public TextUIManager textUI;
    private bool hasBeenTriggered = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();

        if (player != null && !hasBeenTriggered)
        {
            textUI.StartTyping();
            hasBeenTriggered = true;
        }
    }

}
