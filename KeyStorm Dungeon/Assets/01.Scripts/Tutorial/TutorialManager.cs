using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private TutorialData tutorialData;
    [SerializeField] private TutorialUI tutorialUI;
    [SerializeField] private bool showOnStart = true;
    [SerializeField] private bool skipIfSeen = true;

    public UnityEvent onTutorialStart;
    public UnityEvent onTutorialComplete;

    private int currentPageIndex = 0;
    private bool isFirstTimePlaying;
    private bool isTutorialActive = false;
    private PlayerController playerController;

    private void Start()
    {
        playerController = FindObjectOfType<PlayerController>();

        string hasSeenKey = tutorialData.hasSeenKey;
        isFirstTimePlaying = PlayerPrefs.GetInt(hasSeenKey, 0) == 0;

        if (showOnStart && (!skipIfSeen || isFirstTimePlaying))
        {
            ShowTutorial();
        }
        else
        {
            tutorialUI.HideTutorial();
            StartGame();
        }
    }

    public void ShowTutorial()
    {
        isTutorialActive = true;
        currentPageIndex = 0;
        Time.timeScale = 0f;

        if (playerController != null)
        {
            playerController.AllDisable();
        }

        UpdatePage();
        onTutorialStart?.Invoke();
    }

    public void NextPage()
    {
        if (currentPageIndex < tutorialData.pages.Count - 2)
        {
            currentPageIndex += 2;
            UpdatePage();
        }
    }

    public void PreviousPage()
    {
        if (currentPageIndex >= 2)
        {
            currentPageIndex -= 2;
            UpdatePage();
        }
    }

    public void CloseTutorial()
    {
        isTutorialActive = false;
        tutorialUI.HideTutorial();
        Time.timeScale = 1f;

        if (playerController != null)
        {
            playerController.AllEnable();
        }

        if (isFirstTimePlaying)
        {
            PlayerPrefs.SetInt(tutorialData.hasSeenKey, 1);
            PlayerPrefs.Save();
        }

        onTutorialComplete?.Invoke();
        StartGame();
    }

    private void UpdatePage()
    {
        TutorialPage leftPageData = tutorialData.pages[currentPageIndex];
        TutorialPage rightPageData = null;

        if (currentPageIndex + 1 < tutorialData.pages.Count)
        {
            rightPageData = tutorialData.pages[currentPageIndex + 1];
        }

        bool hasPrevious = currentPageIndex >= 2;
        bool hasNext = currentPageIndex + 2 < tutorialData.pages.Count;

        tutorialUI.ShowPage(leftPageData, rightPageData, hasPrevious, hasNext);
    }

    private void StartGame()
    {

    }

    public int GetTotalPages()
    {
        return tutorialData.pages.Count;
    }

    public bool IsTutorialActive()
    {
        return isTutorialActive;
    }
}
