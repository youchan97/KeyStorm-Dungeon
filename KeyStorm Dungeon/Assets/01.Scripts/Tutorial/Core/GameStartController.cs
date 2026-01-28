using UnityEngine;
using UnityEngine.SceneManagement;
using static ConstValue;

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
            LoadingManager.LoadScene(tutorialSceneName);
            return;
        }

        bool tutorialCompleted = PlayerPrefs.GetInt("TutorialCompleted", 0) == 1;

        if (tutorialCompleted)
        {

            LoadingManager.LoadScene(mainGameSceneName);
        }
        else
        {

            LoadingManager.LoadScene(tutorialSceneName);
        }
    }

    public void ResetTutorial()
    {
        PlayerPrefs.SetInt("TutorialCompleted", 0);
        PlayerPrefs.Save();
        Debug.Log("[GameStart] 튜토리얼 리셋됨");
    }
}