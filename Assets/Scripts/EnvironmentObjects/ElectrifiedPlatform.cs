using System.Collections;
using UnityEngine;

public class ElectrifiedPlatform : MonoBehaviour
{

    [SerializeField] ColorPalette colorPalette;

    private SpriteRenderer sr;

    private void Start()
    {
        TimeManager.Instance.OnTimeDilationChange += HandleTimeDilationChanged;

        sr = GetComponent<SpriteRenderer>();
        // StartCoroutine(RunAnimation());
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
        sr.color = colorPalette.disabled;
    }

    private void SetToFastSpeedState()
    {
        gameObject.layer = LayerMask.NameToLayer("ElectrifiedPlatform");
        sr.color = colorPalette.timeDilationFastSpeed;
    }
}
