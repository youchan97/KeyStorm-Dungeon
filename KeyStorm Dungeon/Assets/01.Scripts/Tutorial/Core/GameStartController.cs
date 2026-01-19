using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStartController : MonoBehaviour
{
    [SerializeField] private string tutorialSceneName = "TutorialScene";
    [SerializeField] private string mainGameSceneName = "MainScene";
    [SerializeField] private bool checkTutorialOnStart = true;

    private void Start()
    {
        if (checkTutorialOnStart)
        {
            if (TutorialManager.IsTutorialCompleted())
                SceneManager.LoadScene(mainGameSceneName);
            else
                SceneManager.LoadScene(tutorialSceneName);
        }
    }

    [ContextMenu("Reset Tutorial Progress")]
    public void ResetTutorialProgress()
    {
        TutorialManager.ResetTutorialProgress();
    }
}