using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [Header("Game Settings")]
    [SerializeField] private int totalStages = 4;

    private bool hasBeenUsed = false;
    private static bool isAnyPortalProcessing = false; 

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasBeenUsed || isAnyPortalProcessing)
        {
            return;
        }

        Player player = collision.GetComponent<Player>();
        if (player == null) return;

        TutorialPlayerHook hook = FindObjectOfType<TutorialPlayerHook>();
        hook?.ReportPortalEnter();

        hasBeenUsed = true;
        isAnyPortalProcessing = true;

        if (GameManager.Instance == null || StageDataManager.Instance == null)
        {
            isAnyPortalProcessing = false;
            return;
        }

        int currentStage = GameManager.Instance.CurrentStage;
        int nextStage = currentStage + 1;
        int maxStage = StageDataManager.Instance.CurrentStageSet.stageDatas.Count;

        if (nextStage > maxStage)
        {
            ShowVictory(player);
        }
        else
        {
            StartCoroutine(LoadNextStageWithDelay(player));
        }
    }

    private IEnumerator TutorialPortalEffect(Player player)
    {
        yield return new WaitForSeconds(0.1f);

        if (player.DotweenManager != null)
        {
            player.DotweenManager.PortalDotween(transform.position, player);
        }

        yield return new WaitForSeconds(0.5f);

        isAnyPortalProcessing = false;

        if (TutorialManager.Instance != null)
        {
            TutorialManager.Instance.CompleteTutorial();
        }
    }

    private IEnumerator LoadNextStageWithDelay(Player player)
    {
        yield return new WaitForSeconds(0.1f);

        player.DotweenManager.PortalDotween(transform.position, player);

        yield return new WaitForSeconds(0.5f);
        isAnyPortalProcessing = false;
    }

    private void ShowVictory(Player player)
    {

        GameManager.Instance.GameClear();

        if (player.GameSceneUI != null)
        {
            player.GameSceneUI.GameClear();
        }
        else
        {
            GameSceneUI gameSceneUI = FindObjectOfType<GameSceneUI>();
            if (gameSceneUI != null)
            {
                gameSceneUI.GameClear();
            }
        }

        gameObject.SetActive(false);
    }

    void OnDestroy()
    {
        if (hasBeenUsed)
        {
            isAnyPortalProcessing = false;
        }
    }
}
