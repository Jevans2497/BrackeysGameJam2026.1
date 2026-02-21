using TMPro;
using UnityEngine;

public class TutorialDeathLine : MonoBehaviour
{

    public static TutorialDeathLine Instance;
    public TMP_Text text;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void ShowTutorialDeathText()
    {
        text.text = "Try absorbing this";
    }
}
