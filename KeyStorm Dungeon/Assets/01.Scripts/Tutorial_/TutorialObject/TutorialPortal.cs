using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialPortal : MonoBehaviour
{
    [Header("Portal Settings")]
    [SerializeField] private string nextSceneName = "GameScene";
    [SerializeField] private bool isTutorialEnd = true;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("[TutorialPortal] 포탈 진입");

            // TutorialManager에 알림
            if (TutorialManager.Instance != null)
            {
                TutorialManager.Instance.OnPortalEntered();
            }

            if (isTutorialEnd)
            {
                // 튜토리얼 완료 저장
                PlayerPrefs.SetInt("TutorialCompleted", 1);
                PlayerPrefs.Save();

                // 다음 씬으로
                SceneManager.LoadScene(nextSceneName);
            }
        }
    }
}
