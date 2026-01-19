using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialSkipUI : MonoBehaviour
{
    [Header("스킵 버튼")]
    [SerializeField] private Button skipButton;
    [SerializeField] private TextMeshProUGUI skipButtonText;

    [Header("확인 창")]
    [SerializeField] private GameObject confirmPanel;
    [SerializeField] private TextMeshProUGUI confirmMessageText;
    [SerializeField] private Button confirmYesButton;
    [SerializeField] private Button confirmNoButton;

    public event Action OnSkipConfirmed;

    private void Awake()
    {
        confirmPanel?.SetActive(false);

        skipButton?.onClick.AddListener(() => confirmPanel?.SetActive(true));
        confirmYesButton?.onClick.AddListener(() => {
            confirmPanel?.SetActive(false);
            OnSkipConfirmed?.Invoke();
        });
        confirmNoButton?.onClick.AddListener(() => confirmPanel?.SetActive(false));
    }

    private void Update()
    {
        if (confirmPanel != null && confirmPanel.activeSelf && Input.GetKeyDown(KeyCode.Escape))
            confirmPanel.SetActive(false);
    }
}