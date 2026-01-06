using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialUI : MonoBehaviour
{
    [SerializeField] private GameObject tutorialPanel;

    [Header("좌측 페이지")]
    [SerializeField] private GameObject leftPage;
    [SerializeField] private TextMeshProUGUI leftTitleText;
    [SerializeField] private Image leftExampleImage;
    [SerializeField] private Image leftExampleImage1;
    [SerializeField] private TextMeshProUGUI leftDescriptionText;

    [Header("우측 페이지")]
    [SerializeField] private GameObject rightPage;
    [SerializeField] private TextMeshProUGUI rightTitleText;
    [SerializeField] private Image rightExampleImage;
    [SerializeField] private Image rightExampleImage1;
    [SerializeField] private TextMeshProUGUI rightDescriptionText;

    [Header("버튼")]
    [SerializeField] private Button closeButton;
    [SerializeField] private Button previousButton;
    [SerializeField] private Button nextButton;

    [Header("페이지 번호")]
    [SerializeField] private TextMeshProUGUI pageNumberText;

    private TutorialManager tutorialManager;

    private void Awake()
    {
        tutorialManager = GetComponentInParent<TutorialManager>();
        if (tutorialManager == null)
            tutorialManager = FindObjectOfType<TutorialManager>();

        closeButton.onClick.AddListener(() => tutorialManager.CloseTutorial());
        previousButton.onClick.AddListener(() => tutorialManager.PreviousPage());
        nextButton.onClick.AddListener(() => tutorialManager.NextPage());
    }

    public void ShowPage(TutorialPage leftPageData, TutorialPage rightPageData, bool hasPrevious, bool hasNext)
    {
        tutorialPanel.SetActive(true);

        // 좌측 페이지
        if (leftPageData != null)
        {
            leftPage.SetActive(true);
            leftTitleText.text = leftPageData.title;
            leftDescriptionText.text = leftPageData.description;

            if (leftPageData.exampleImage != null)
            {
                leftExampleImage.sprite = leftPageData.exampleImage;
                leftExampleImage.gameObject.SetActive(true);
            }
            else
            {
                leftExampleImage.gameObject.SetActive(false);
            }
        }
        else
        {
            leftPage.SetActive(false);
        }

        // 우측 페이지
        if (rightPageData != null)
        {
            rightPage.SetActive(true);
            rightTitleText.text = rightPageData.title;
            rightDescriptionText.text = rightPageData.description;

            if (rightPageData.exampleImage != null)
            {
                rightExampleImage.sprite = rightPageData.exampleImage;
                rightExampleImage.gameObject.SetActive(true);
            }
            else
            {
                rightExampleImage.gameObject.SetActive(false);
            }
        }
        else
        {
            rightPage.SetActive(false);
        }

        // 페이지 번호
        if (pageNumberText != null)
        {
            int currentLeft = leftPageData != null ? leftPageData.pageNumber + 1 : 0;
            int currentRight = rightPageData != null ? rightPageData.pageNumber + 1 : 0;
            int total = tutorialManager.GetTotalPages();

            if (rightPageData != null)
                pageNumberText.text = $"{currentLeft}-{currentRight} / {total}";
            else
                pageNumberText.text = $"{currentLeft} / {total}";
        }

        // 버튼
        previousButton.gameObject.SetActive(hasPrevious);
        nextButton.gameObject.SetActive(hasNext);
    }

    public void HideTutorial()
    {
        tutorialPanel.SetActive(false);
    }
}
