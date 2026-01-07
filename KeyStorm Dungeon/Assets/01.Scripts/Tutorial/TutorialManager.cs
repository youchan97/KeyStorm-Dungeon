using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private TutorialData tutorialData;
    [SerializeField] private TutorialUI tutorialUI;
    [SerializeField] private bool showOnStart = true;
    [SerializeField] private bool skipIfSeen = true;

    [Header("튜토리얼 표시할 씬")]
    [SerializeField] private string[] tutorialScenes = { "MainScene" }; // 첫 씬 이름

    public UnityEvent onTutorialStart;
    public UnityEvent onTutorialComplete;

    private int currentPageIndex = 0;
    private bool isFirstTimePlaying;
    private bool isTutorialActive = false;
    private PlayerController playerController;

    private const string GAME_STARTED_KEY = "GameStarted";

    private void Start()
    {
        playerController = FindObjectOfType<PlayerController>();

        string currentScene = SceneManager.GetActiveScene().name;
        string hasSeenKey = tutorialData.hasSeenKey;
        int seenValue = PlayerPrefs.GetInt(hasSeenKey, 0);
        int gameStarted = PlayerPrefs.GetInt(GAME_STARTED_KEY, 0);

        Debug.Log($"현재 씬: {currentScene}");
        Debug.Log($"게임 시작됨: {gameStarted}");

        // 게임이 이미 시작됐으면 튜토리얼 스킵
        if (gameStarted == 1)
        {
            Debug.Log("게임 진행 중 - 튜토리얼 건너뛰기");
            tutorialUI.HideTutorial();

            if (playerController != null)
            {
                playerController.AllEnable();
            }

            StartGame();
            return;
        }

        // 이미 봤으면 스킵
        if (seenValue == 1)
        {
            Debug.Log("튜토리얼 이미 봄");
            tutorialUI.HideTutorial();

            if (playerController != null)
            {
                playerController.AllEnable();
            }

            PlayerPrefs.SetInt(GAME_STARTED_KEY, 1);
            PlayerPrefs.Save();

            StartGame();
            return;
        }

        // 처음이면 튜토리얼 시작
        isFirstTimePlaying = true;

        if (showOnStart)
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

        PlayerPrefs.SetInt(tutorialData.hasSeenKey, 1);
        PlayerPrefs.SetInt(GAME_STARTED_KEY, 1);
        PlayerPrefs.Save();

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

    [ContextMenu("튜토리얼 리셋")]
    public void ResetTutorial()
    {
        PlayerPrefs.DeleteKey(tutorialData.hasSeenKey);
        PlayerPrefs.DeleteKey(GAME_STARTED_KEY);
        PlayerPrefs.Save();
        Debug.Log("튜토리얼 리셋 완료!");
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(TutorialManager))]
    public class TutorialManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            TutorialManager manager = (TutorialManager)target;

            EditorGUILayout.Space();

            if (GUILayout.Button(" 튜토리얼 리셋", GUILayout.Height(30)))
            {
                manager.ResetTutorial();
            }
        }
    }
#endif
}
