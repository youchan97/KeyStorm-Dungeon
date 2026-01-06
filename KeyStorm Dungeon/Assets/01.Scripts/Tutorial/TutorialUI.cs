using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialUI : MonoBehaviour
{
    [Header("UI 패널")]
    [SerializeField] private GameObject tutorialPanel;

    [Header("좌측 페이지 (설명)")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;

    [Header("우측 페이지 (예시 화면)")]
    [SerializeField] private Image exampleImage;

    [Header("버튼")]
    [SerializeField] private Button closeButton;
    [SerializeField] private Button previousButton;
    [SerializeField] private Button nextButton;

    [Header("버튼 텍스트/아이콘")]
    [SerializeField] private TextMeshProUGUI closeButtonText;
    [SerializeField] private TextMeshProUGUI previousButtonText;
    [SerializeField] private TextMeshProUGUI nextButtonText;

    private TutorialManager tutorialManager;

    private void Awake()
    {
        tutorialManager = GetComponentInParent<TutorialManager>();
        if (tutorialManager == null)
        {
            tutorialManager = FindObjectOfType<TutorialManager>();
        }

        if (closeButton != null)
            closeButton.onClick.AddListener(OnCloseClicked);

        if (previousButton != null)
            previousButton.onClick.AddListener(OnPreviousClicked);

        if (nextButton != null)
            nextButton.onClick.AddListener(OnNextClicked);

        if (closeButtonText != null)
            closeButtonText.text = "X";

        if (previousButtonText != null)
            previousButtonText.text = "←";

        if (nextButtonText != null)
            nextButtonText.text = "→";
    }

    public void ShowPage(TutorialPage page, bool hasPrevious, bool hasNext)
    {
        if (page == null)
        {
            Debug.LogError("TutorialPage가 null입니다!");
            return;
        }

        tutorialPanel.SetActive(true);

        if (titleText != null)
            titleText.text = page.title;

        if (descriptionText != null)
            descriptionText.text = page.description;

        if (exampleImage != null && page.exampleImage != null)
        {
            exampleImage.sprite = page.exampleImage;
            exampleImage.gameObject.SetActive(true);
        }
        else if (exampleImage != null)
        {
            exampleImage.gameObject.SetActive(false);
        }

        if (previousButton != null)
            previousButton.gameObject.SetActive(hasPrevious);

        if (nextButton != null)
            nextButton.gameObject.SetActive(hasNext);
    }

    public void HideTutorial()
    {
        tutorialPanel.SetActive(false);
    }

    private void OnCloseClicked()
    {
        if (tutorialManager != null)
            tutorialManager.CloseTutorial();
    }

    private void OnPreviousClicked()
    {
        if (tutorialManager != null)
            tutorialManager.PreviousPage();
    }

    private void OnNextClicked()
    {
        if (tutorialManager != null)
            tutorialManager.NextPage();
    }
}
