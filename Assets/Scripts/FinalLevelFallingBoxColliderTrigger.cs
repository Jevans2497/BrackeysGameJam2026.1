using System;
using UnityEngine;

public class FinalLevelFallingBoxColliderTrigger : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        LevelManager.Instance.TriggerFinalLevelFallingBoxes();
    }
}
