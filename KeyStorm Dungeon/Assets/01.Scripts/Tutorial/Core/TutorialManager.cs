using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance { get; private set; }

    [Header("튜토리얼 데이터")]
    [SerializeField] private List<TutorialStep> steps = new List<TutorialStep>();

    [Header("참조 - UI")]
    [SerializeField] private TutorialDialogueUI dialogueUI;
    [SerializeField] private TutorialQuestUI questUI;
    [SerializeField] private TutorialSkipUI skipUI;

    [Header("참조 - 시스템")]
    [SerializeField] private PlayerController playerController;

    [Header("설정")]
    [SerializeField] private string mainGameSceneName = "GameScene";
    [SerializeField] private float stepTransitionDelay = 0.5f;

    private int currentStepIndex = 0;
    private TutorialStep currentStep;
    private bool isQuestActive = false;
    private bool isCompleted = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
    }

    private void Start()
    {
        if (skipUI != null)
            skipUI.OnSkipConfirmed += SkipTutorial;

        if (playerController == null)
        {
            Player player = FindObjectOfType<Player>();
            if (player != null)
                playerController = player.PlayerController;
        }

        currentStepIndex = 0;
        StartCoroutine(RunStep(steps[currentStepIndex]));
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    IEnumerator RunStep(TutorialStep step)
    {
        currentStep = step;

        if (step.resetMoveTrackingOnStart)
        {
            TutorialPlayerHook hook = FindObjectOfType<TutorialPlayerHook>();
            hook?.ResetMoveTracking();
        }

        if (step.preDialogues.Count > 0)
        {
            if (step.blockInputDuringDialogue)
                playerController?.DisableInput();

            yield return StartCoroutine(dialogueUI.ShowDialogues(step.preDialogues));

            if (step.blockInputDuringDialogue)
                playerController?.EnableInput();
        }

        if (step.hasQuest && step.objectives.Count > 0)
        {
            isQuestActive = true;
            step.ResetObjectives();
            questUI.ShowQuest(step.questTitle, step.objectives);

            yield return new WaitUntil(() => step.AreAllObjectivesCompleted());

            isQuestActive = false;
            yield return new WaitForSeconds(0.3f);
            questUI.HideQuest();
        }

        if (step.postDialogues.Count > 0)
        {
            if (step.blockInputDuringDialogue)
                playerController?.DisableInput();

            yield return StartCoroutine(dialogueUI.ShowDialogues(step.postDialogues));

            if (step.blockInputDuringDialogue)
                playerController?.EnableInput();
        }

        yield return new WaitForSeconds(stepTransitionDelay);
        NextStep();
    }

    void NextStep()
    {
        currentStepIndex++;
        if (currentStepIndex >= steps.Count)
        {
            CompleteTutorial();
            return;
        }
        StartCoroutine(RunStep(steps[currentStepIndex]));
    }

    public void ReportQuestProgress(QuestType type, int amount = 1)
    {
        if (!isQuestActive || currentStep == null) return;

        foreach (var obj in currentStep.objectives)
        {
            if (obj.type == type && !obj.IsCompleted)
            {
                obj.AddProgress(amount);
                questUI.UpdateQuest(currentStep.objectives);
                break;
            }
        }
    }

    public void ReportRoomEntered(TutorialRoomType roomType)
    {
        QuestType questType = roomType switch
        {
            TutorialRoomType.Start => QuestType.EnterRoom,
            TutorialRoomType.Treasure => QuestType.EnterTreasureRoom,
            TutorialRoomType.Normal => QuestType.EnterNormalRoom,
            TutorialRoomType.Shop => QuestType.EnterShopRoom,
            TutorialRoomType.Boss => QuestType.EnterBossRoom,
            _ => QuestType.EnterRoom
        };

        ReportQuestProgress(questType);
    }

    public void SkipTutorial()
    {
        if (isCompleted) return;
        StopAllCoroutines();
        dialogueUI?.ForceHide();
        questUI?.HideQuest();
        isCompleted = true;
        PlayerPrefs.SetInt("TutorialCompleted", 1);
        PlayerPrefs.Save();
        SceneManager.LoadScene(mainGameSceneName);
    }

    void CompleteTutorial()
    {
        if (isCompleted) return;
        isCompleted = true;
        PlayerPrefs.SetInt("TutorialCompleted", 1);
        PlayerPrefs.Save();
        SceneManager.LoadScene(mainGameSceneName);
    }

    public static bool IsTutorialCompleted() => PlayerPrefs.GetInt("TutorialCompleted", 0) == 1;
    public static void ResetTutorialProgress()
    {
        PlayerPrefs.SetInt("TutorialCompleted", 0);
        PlayerPrefs.Save();
    }
}