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

            TutorialManager tutorialManager = FindObjectOfType<TutorialManager>();
            if (tutorialManager != null)
            {
                tutorialManager.OnPortalEntered();
            }

            if (isTutorialEnd)
            {
                PlayerPrefs.SetInt("TutorialCompleted", 1);
                PlayerPrefs.Save();

                SceneManager.LoadScene(nextSceneName);
            }
        }
    }
}
