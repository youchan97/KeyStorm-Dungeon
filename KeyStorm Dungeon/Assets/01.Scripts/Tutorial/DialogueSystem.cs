using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueSystem : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TextMeshProUGUI speakerNameText;
    [SerializeField] private Button continueButton;

    [Header("Typing Effect")]
    [SerializeField] private float typingSpeed = 0.05f;

    private bool isTyping = false;
    private bool dialogueActive = false;
    private System.Action onDialogueComplete;
    private string fullText = "";

    void Start()
    {
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }

        if (continueButton != null)
        {
            continueButton.onClick.AddListener(OnContinueClicked);
        }
    }

    public void ShowDialogue(string text, string speakerName = "플레이어", bool pauseGame = true, System.Action onComplete = null)
    {
        dialogueActive = true;
        onDialogueComplete = onComplete;
        fullText = text;

        if (pauseGame)
        {
            Time.timeScale = 0f;

            Player player = FindObjectOfType<Player>();
            if (player != null)
            {
                player.SetCanMove(false);
            }
        }

        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(true);
        }

        if (speakerNameText != null)
        {
            speakerNameText.text = speakerName;
        }

        StartCoroutine(TypeText(text));
    }

    IEnumerator TypeText(string text)
    {
        isTyping = true;
        dialogueText.text = "";

        if (continueButton != null)
        {
            continueButton.gameObject.SetActive(false);
        }

        foreach (char c in text)
        {
            dialogueText.text += c;
            yield return new WaitForSecondsRealtime(typingSpeed);
        }

        isTyping = false;

        if (continueButton != null)
        {
            continueButton.gameObject.SetActive(true);
        }
    }

    void OnContinueClicked()
    {
        if (isTyping)
        {
            StopAllCoroutines();
            dialogueText.text = fullText;
            isTyping = false;

            if (continueButton != null)
            {
                continueButton.gameObject.SetActive(true);
            }
        }
        else
        {
            CloseDialogue();
        }
    }

    void CloseDialogue()
    {
        dialogueActive = false;

        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }

        Time.timeScale = 1f;

        Player player = FindObjectOfType<Player>();
        if (player != null)
        {
            player.SetCanMove(true);
        }

        onDialogueComplete?.Invoke();
    }

    void Update()
    {
        if (dialogueActive && (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return)))
        {
            OnContinueClicked();
        }
    }
}
