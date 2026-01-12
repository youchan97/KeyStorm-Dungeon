using System.Collections;
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

    [Header("Background Move")]
    [SerializeField] RectTransform backgroundImage;
    [SerializeField] float moveDistance = 200f;

    public static string nextSceneName;

    Vector2 bgStartPos;

    private void Start()
    {
        bgStartPos = backgroundImage.anchoredPosition;
        StartCoroutine(Loading());
    }

    IEnumerator Loading()
    {
        AsyncOperation async = SceneManager.LoadSceneAsync(nextSceneName);
        async.allowSceneActivation = false;

        float timer = 0f;

        while (timer < waitProgressSeconds)
        {
            timer += Time.deltaTime;
            float progress = Mathf.Clamp01(timer / waitProgressSeconds);

            // ===== 로딩 UI =====
            progressText.text = string.Format("{0:F1} %", progress * 100f);
            progressBar.fillAmount = progress;

            // ===== 이미지 내려가는 연출 =====
            float eased = Mathf.SmoothStep(0f, 1f, progress);
            backgroundImage.anchoredPosition =
                bgStartPos + Vector2.down * moveDistance * eased;

            yield return null;
        }

        // 마무리 보정
        progressText.text = "100 %";
        progressBar.fillAmount = 1f;
        backgroundImage.anchoredPosition =
            bgStartPos + Vector2.down * moveDistance;

        async.allowSceneActivation = true;
    }

    public static void LoadScene(string sceneName)
    {
        nextSceneName = sceneName;
        AudioManager.Instance.StopBgm();
        SceneManager.LoadScene(LoadingScene);
    }
}
