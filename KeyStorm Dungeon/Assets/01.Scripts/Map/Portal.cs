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


        int currentStage = GameManager.Instance.CurrentStage;
        int nextStage = currentStage + 1;


        if (nextStage > totalStages)
        {
            ShowVictory(player);
        }
        else
        {
            StartCoroutine(LoadNextStageWithDelay(player));
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

        Debug.Log("[Portal] ShowVictory 끝");
    }

    void OnDestroy()
    {
        if (hasBeenUsed)
        {
            isAnyPortalProcessing = false;
        }
    }
}
