using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialDialogueUI : MonoBehaviour
{
    [Header("UI 요소")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private GameObject continueIndicator;
    [SerializeField] private Image backgroundImage;

    [Header("설정")]
    [SerializeField] private float defaultTypingSpeed = 0.03f;
    [SerializeField] private Color backgroundColor = new Color(0, 0, 0, 0.8f);
    [SerializeField] private Color textColor = Color.white;

    private bool isTyping = false;
    private bool skipTyping = false;
    private bool waitingForInput = false;

    private void Awake()
    {
        dialoguePanel?.SetActive(false);
        continueIndicator?.SetActive(false);
        if (backgroundImage != null) backgroundImage.color = backgroundColor;
        if (dialogueText != null) { dialogueText.color = textColor; dialogueText.text = ""; }
    }

    private void Update()
    {
        bool input = Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return);
        if (!input) return;

        if (isTyping) skipTyping = true;
        else if (waitingForInput) waitingForInput = false;
    }

    public IEnumerator ShowDialogues(List<DialogueLine> dialogues)
    {
        if (dialogues == null || dialogues.Count == 0) yield break;

        dialoguePanel.SetActive(true);

        foreach (var line in dialogues)
        {
            yield return StartCoroutine(TypeText(line.text, line.typingSpeed));

            if (line.waitForInput)
            {
                continueIndicator?.SetActive(true);
                waitingForInput = true;
                yield return new WaitUntil(() => !waitingForInput);
                continueIndicator?.SetActive(false);
            }
            else
            {
                yield return new WaitForSeconds(line.autoDelay);
            }
        }

        dialoguePanel.SetActive(false);
    }

    IEnumerator TypeText(string text, float speed)
    {
        isTyping = true;
        skipTyping = false;
        dialogueText.text = "";

        bool inTag = false;
        string currentText = "";

        foreach (char c in text)
        {
            if (skipTyping) { dialogueText.text = text; break; }

            if (c == '<') inTag = true;
            currentText += c;
            if (c == '>') inTag = false;

            if (!inTag)
            {
                dialogueText.text = currentText;
                yield return new WaitForSeconds(speed);
            }
        }

        dialogueText.text = text;
        isTyping = false;
    }

    public void ForceHide()
    {
        StopAllCoroutines();
        isTyping = false;
        waitingForInput = false;
        dialoguePanel?.SetActive(false);
    }
}