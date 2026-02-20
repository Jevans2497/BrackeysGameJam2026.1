using System.Collections;
using UnityEngine;

public class ElectrifiedPlatform : MonoBehaviour
{

    [SerializeField] ColorPalette colorPalette;
    [SerializeField] Sprite normalSpeedSprite;
    [SerializeField] Sprite fastSpeedSprite;

    private SpriteRenderer sr;
    private Animator animator;

    private void Start()
    {
        TimeManager.Instance.OnTimeDilationChange += HandleTimeDilationChanged;

        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    private void OnDisable()
    {
        TimeManager.Instance.OnTimeDilationChange -= HandleTimeDilationChanged;
    }

    private void HandleTimeDilationChanged(TimeDilationSpeed newSpeed)
    {
        switch (newSpeed)
        {
            case TimeDilationSpeed.normalSpeed:
                SetToNormalSpeedState();
                break;
            case TimeDilationSpeed.fastSpeed:
                SetToFastSpeedState();
                break;
        }
    }

    private void SetToNormalSpeedState()
    {
        gameObject.layer = LayerMask.NameToLayer("Ground");
        animator.SetTrigger("PowerDown");
    }

    private void SetToFastSpeedState()
    {
        gameObject.layer = LayerMask.NameToLayer("ElectrifiedPlatform");
        animator.SetTrigger("SpeedUp");
    }
}
