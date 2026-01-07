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
    [SerializeField] private Image leftImage1;
    [SerializeField] private Image leftImage2;
    [SerializeField] private TextMeshProUGUI leftDescriptionText;

    [Header("우측 페이지")]
    [SerializeField] private GameObject rightPage;
    [SerializeField] private TextMeshProUGUI rightTitleText;
    [SerializeField] private Image rightImage1;
    [SerializeField] private Image rightImage2;
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

        if (leftPageData != null)
        {
            leftPage.SetActive(true);
            leftTitleText.text = leftPageData.title;
            leftDescriptionText.text = leftPageData.description;

            SetPageImages(leftImage1, leftImage2, leftPageData.image1, leftPageData.image2);
        }
        else
        {
            leftPage.SetActive(false);
        }

        if (rightPageData != null)
        {
            rightPage.SetActive(true);
            rightTitleText.text = rightPageData.title;
            rightDescriptionText.text = rightPageData.description;

            SetPageImages(rightImage1, rightImage2, rightPageData.image1, rightPageData.image2);
        }
        else
        {
            rightPage.SetActive(false);
        }

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

        previousButton.gameObject.SetActive(hasPrevious);
        nextButton.gameObject.SetActive(hasNext);
    }

    private void SetPageImages(Image img1, Image img2, Sprite sprite1, Sprite sprite2)
    {
        if (sprite1 != null && sprite2 != null)
        {
            img1.gameObject.SetActive(true);
            img2.gameObject.SetActive(true);
            img1.sprite = sprite1;
            img2.sprite = sprite2;
        }
        else if (sprite1 != null)
        {
            img1.gameObject.SetActive(true);
            img2.gameObject.SetActive(false);
            img1.sprite = sprite1;
        }
        else
        {
            img1.gameObject.SetActive(false);
            img2.gameObject.SetActive(false);
        }
    }

    public void HideTutorial()
    {
        tutorialPanel.SetActive(false);
    }
}
