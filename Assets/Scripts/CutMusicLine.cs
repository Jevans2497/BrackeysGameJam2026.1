using Unity.VisualScripting;
using UnityEngine;

public class CutMusicLine : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        MusicManager.Instance.StopMusic();
    }
}
