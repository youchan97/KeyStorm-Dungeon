using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TutorialManager : MonoBehaviour
{
    [Header("데이터")]
    [SerializeField] private TutorialData tutorialData;

    [Header("UI")]
    [SerializeField] private TutorialUI tutorialUI;

    [Header("설정")]
    [SerializeField] private bool showOnStart = true;
    [Tooltip("두 번째 플레이부터는 자동으로 튜토리얼 건너뛰기")]
    [SerializeField] private bool skipIfSeen = true;

    [Header("이벤트")]
    public UnityEvent onTutorialStart;      
    public UnityEvent onTutorialComplete;   

    private int currentPageIndex = 0;
    private bool isFirstTimePlaying;

    private void Start()
    {
        for (int i = 0; i < tutorialData.pages.Count; i++)
        {
            Debug.Log($"Element {i}: {tutorialData.pages[i].title}");
        }

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
        currentPageIndex = 0;

        Time.timeScale = 0f;
        UpdatePage();
        onTutorialStart?.Invoke();
    }

    public void NextPage()
    {
        if (currentPageIndex < tutorialData.pages.Count - 1)
        {
            currentPageIndex++;
            UpdatePage();
        }
    }

    public void PreviousPage()
    {
        if (currentPageIndex > 0)
        {
            currentPageIndex--;
            UpdatePage();
        }
    }

    public void CloseTutorial()
    {
        tutorialUI.HideTutorial();
        Time.timeScale = 1f;

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
        TutorialPage currentPage = tutorialData.pages[currentPageIndex];

        bool hasPrevious = currentPageIndex > 0;
        bool hasNext = currentPageIndex < tutorialData.pages.Count - 1;
        tutorialUI.ShowPage(currentPage, hasPrevious, hasNext);
    }

    private void StartGame()
    {

    }

    public int GetTotalPages()
    {
        return tutorialData.pages.Count;
    }
}
