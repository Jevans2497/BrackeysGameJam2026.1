using System.Collections;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class TextUIManager : MonoBehaviour
{

    private TMP_Text dialogueText;
    private string message;
    private float typingSpeed = 0.08f;
    private const float SPEED_UP_TEXT_MULTIPLIER = 0.25F;

    private PlayerInputActions input;
    private bool speedUpTypingInputPressed;

    private void Start()
    {
        dialogueText = GetComponent<TMP_Text>();
        message = dialogueText.text;
        dialogueText.text = "";
    }

    private void OnEnable()
    {
        input = new PlayerInputActions();
        input.Enable();

        input.Player.SpeedUpDialogue.performed += OnSpeedUpPressed;
        input.Player.SpeedUpDialogue.canceled += OnSpeedUpReleased;

    }
    private void OnDisable()
    {
        input.Player.SpeedUpDialogue.performed -= OnSpeedUpPressed;
        input.Disable();
    }
    private void OnSpeedUpPressed(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        speedUpTypingInputPressed = true;
    }

    private void OnSpeedUpReleased(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        speedUpTypingInputPressed = false;
    }

    public void StartTyping()
    {
        //Make it so you have to hit a new input to speed up the text
        speedUpTypingInputPressed = false;
        StopAllCoroutines();
        StartCoroutine(TypeText());
    }

    private IEnumerator TypeText()
    {
        dialogueText.text = "";
        bool isManipulatingTime = false;
        string manipulateTimeString = "";
        float currentTypingSpeed = typingSpeed;

        Player.Instance.DisableInput();

        foreach (char letter in message.ToCharArray())
        {
            if (letter == '<')
            {
                isManipulatingTime = true;
            }

            if (isManipulatingTime)
            {
                manipulateTimeString += letter;
                if (letter == '>')
                {
                    isManipulatingTime = false;
                    if (manipulateTimeString.Contains("r"))
                    { //reset typing speed to default
                        currentTypingSpeed = typingSpeed;
                    }
                    else
                    {
                        currentTypingSpeed = ExtractFloatFromTag(manipulateTimeString);
                    }
                    manipulateTimeString = "";
                }
                yield return null;
            }
            else
            {
                dialogueText.text += letter;
                yield return new WaitForSeconds(currentTypingSpeed * GetCurrentSpeedMultiplier());
            }
        }

        Player.Instance.EnableInput();
        StartCoroutine(RunHideTextAnimation());
    }

    private IEnumerator RunHideTextAnimation()
    {
        yield return new WaitForSeconds(1.0f);

        float duration = 1.0f;
        float elapsedTime = 0.0f;

        Vector3 originalScale = dialogueText.transform.localScale;

        Color startColor = dialogueText.color;
        Color endColor = startColor;
        endColor.a = 0f; // fade to transparent

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;

            dialogueText.transform.localScale =
                Vector3.Lerp(originalScale, originalScale * 1.3f, t);

            dialogueText.color =
                Color.Lerp(startColor, endColor, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        dialogueText.color = endColor;
    }


    private float ExtractFloatFromTag(string tag)
    {
        Match match = Regex.Match(tag, @"<([\d.]+)f>");
        if (match.Success)
        {
            return float.Parse(match.Groups[1].Value);
        }
        else
        {
            return typingSpeed;
        }
    }

    private float GetCurrentSpeedMultiplier()
    {
        float speedMultiplier = speedUpTypingInputPressed ? SPEED_UP_TEXT_MULTIPLIER : 1.0f;
        return speedMultiplier;
    }

}
