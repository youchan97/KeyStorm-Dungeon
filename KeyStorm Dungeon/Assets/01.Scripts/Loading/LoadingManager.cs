using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static ConstValue;

public class LoadingManager : MonoBehaviour
{
    [SerializeField] Image progressBar;
    [SerializeField] TextMeshProUGUI progressText;
    [Range(0, 20)]
    [SerializeField] float waitProgressSeconds;

    public static string nextSceneName;

    private void Start()
    {
        StartCoroutine(Loading());
    }

    IEnumerator Loading()
    {
        AsyncOperation async = SceneManager.LoadSceneAsync(nextSceneName);
        async.allowSceneActivation = false;
        float timer = 0f;
        float duration = 0f;
        while (timer < waitProgressSeconds)
        {
            timer += Time.deltaTime;
            float lerp = Mathf.Clamp01(timer / waitProgressSeconds);
            duration = Mathf.Lerp(0f, 1f, lerp);
            progressText.text = string.Format("{0:F1} %", duration * 100f);
            progressBar.fillAmount = duration;
            yield return null;
        }
        progressText.text = "100 %";
        progressBar.fillAmount = 1f;
        async.allowSceneActivation = true;

    }

    public static void LoadScene(string sceneName)
    {
        nextSceneName = sceneName;
        SceneManager.LoadScene(LoadingScene);
    }
}
