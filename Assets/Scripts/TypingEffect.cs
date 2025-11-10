using System.Collections;
using UnityEngine;
using TMPro;

public class TypingEffect : MonoBehaviour
{
    public TMP_Text textMeshPro; // Reference to the TextMeshPro component
    public float typingSpeed = 0.1f; // Speed of typing in seconds

    private string fullText; // The complete text to be typed
    private Coroutine typingCoroutine;

    private void Awake()
    {
        // Store the full text but keep it hidden initially
        fullText = textMeshPro.text;
        textMeshPro.text = string.Empty;
    }
    private void OnEnable()
    {
        StartTyping();
    }


    // This will be triggered from the Timeline
    public void StartTyping()
    {
        // Stop any previous typing if replayed
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        textMeshPro.text = string.Empty;
        typingCoroutine = StartCoroutine(TypeText());
    }

    private IEnumerator TypeText()
    {
        foreach (char letter in fullText)
        {
            textMeshPro.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }
}
