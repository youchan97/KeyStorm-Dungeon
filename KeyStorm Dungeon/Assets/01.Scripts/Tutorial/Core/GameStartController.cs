using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStartController : MonoBehaviour
{
    [Header("씬 이름")]
    [SerializeField] private string tutorialSceneName = "TutorialScene";
    [SerializeField] private string mainGameSceneName = "MainScene";

    [Header("설정")]
    [SerializeField] private bool alwaysShowTutorial = false;

    public void StartGame()
    {
        if (alwaysShowTutorial)
        {
            SceneManager.LoadScene(tutorialSceneName);
            return;
        }

        bool tutorialCompleted = PlayerPrefs.GetInt("TutorialCompleted", 0) == 1;

        if (tutorialCompleted)
        {
            SceneManager.LoadScene(mainGameSceneName);
        }
        else
        {
            SceneManager.LoadScene(tutorialSceneName);
        }
    }

    public void ResetTutorial()
    {
        PlayerPrefs.SetInt("TutorialCompleted", 0);
        PlayerPrefs.Save();
        Debug.Log("[GameStart] 튜토리얼 리셋됨");
    }
}