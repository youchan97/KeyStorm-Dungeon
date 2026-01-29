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

    [Header("보스 HP")]
    [SerializeField] private GameObject bossHpBarPrefab; 
    [SerializeField] private Transform bossHpBarLayout;
    private BossHpBar currentBossHpBar;

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
        PlayerPrefs.SetInt("TutorialCompleted", 0);
        if (PlayerPrefs.GetInt("TutorialCompleted", 0) == 1)
        {
            gameObject.SetActive(false);
            return;
        }
        if (skipUI != null)
            skipUI.OnSkipConfirmed += SkipTutorial;

        StartCoroutine(InitAfterDelay());
    }

    IEnumerator InitAfterDelay()
    {
        yield return new WaitForSeconds(0.5f);

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayBgm(ConstValue.EasyBgm);
        }

        SetTutorialStartGold();

        if (playerController == null)
        {
            Player player = FindObjectOfType<Player>();
            if (player != null)
            {
                playerController = player.PlayerController;
            }
        }

        ConnectBossHpEvent();

        if (steps.Count > 0)
        {
            StartCoroutine(RunStep(steps[0]));
        }
    }

    void SetTutorialStartGold()
    {
        Player player = FindObjectOfType<Player>();
        if (player != null && player.Inventory != null)
        {
            player.Inventory.gold = 1100;

            if (player.GameSceneUI != null)
            {
                player.GameSceneUI.UpdateGold();
            }
        }
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

        if (step.waitForRoomEnter)
        {
            yield return new WaitUntil(() => IsPlayerInRoom(step.targetRoomType));
        }

        if (step.preDialogues.Count > 0)
        {
            playerController?.DisableTab();
            playerController?.DisablePause();
            playerController?.DisableInput();

            StopWalkSound();

            yield return StartCoroutine(dialogueUI.ShowDialogues(step.preDialogues));

            playerController?.EnableInput();
            playerController?.EnableTab();
            playerController?.EnablePause();
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

        if (step.wallIndexToOpen >= 0)
        {
            if (TutorialWallManager.Instance != null)
            {
                TutorialWallManager.Instance.OpenWall(step.wallIndexToOpen);
            }
        }

        if (step.postDialogues.Count > 0)
        {
            playerController?.DisableTab();
            playerController?.DisablePause();
            playerController?.DisableInput();

            StopWalkSound();

            yield return StartCoroutine(dialogueUI.ShowDialogues(step.postDialogues));

            playerController?.EnableInput();
            playerController?.EnableTab();
            playerController?.EnablePause();
        }

        if (step.openDoorsAfterQuest)
        {
            OpenCurrentRoomDoors();
        }

        yield return new WaitForSeconds(stepTransitionDelay);
        NextStep();
    }

    void StopWalkSound()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopEffectLoop(ConstValue.PlayerMoveSfx);
        }
    }

    bool IsPlayerInRoom(TutorialRoomType tutorialRoomType)
    {
        Room[] rooms = FindObjectsOfType<Room>();

        RoomType targetType = tutorialRoomType switch
        {
            TutorialRoomType.Start => RoomType.Start,
            TutorialRoomType.Normal => RoomType.Normal,
            TutorialRoomType.Boss => RoomType.Boss,
            TutorialRoomType.Treasure => RoomType.Treasure,
            TutorialRoomType.Shop => RoomType.Shop,
            _ => RoomType.Start
        };

        Player player = FindObjectOfType<Player>();
        if (player == null) return false;

        foreach (Room room in rooms)
        {
            if (room.roomType != targetType) continue;

            if (room.IsPlayerIn) return true;

            float distance = Vector2.Distance(player.transform.position, room.transform.position);
            if (distance <= 14f) return true;
        }

        return false;
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
        playerController?.EnableTab();
        playerController?.EnablePause();
        ResetPlayerData();
        PlayerPrefs.SetInt("TutorialCompleted", 1);
        PlayerPrefs.Save();
        LoadingManager.LoadScene(mainGameSceneName);
    }

    public void CompleteTutorial()
    {
        if (isCompleted) return;
        isCompleted = true;

        playerController?.EnableTab();
        playerController?.EnablePause();

        ResetPlayerData();

        if (GameDataManager.Instance != null)
        {
            GameDataManager.Instance.ResetAllData();
        }

        if (GameTimeManager.Instance != null)
        {
            GameTimeManager.Instance.ResetTimer();
        }

        PlayerPrefs.SetInt("TutorialCompleted", 1);
        PlayerPrefs.Save();

        LoadingManager.LoadScene(mainGameSceneName);
    }

    void ResetPlayerData()
    {
        DG.Tweening.DOTween.KillAll();

        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResetRunDataForTutorial();
        }
    }

    public static bool IsTutorialCompleted() => PlayerPrefs.GetInt("TutorialCompleted", 0) == 1;
    public static void ResetTutorialProgress()
    {
        PlayerPrefs.SetInt("TutorialCompleted", 0);
        PlayerPrefs.Save();
    }

    public void OpenCurrentRoomDoors()
    {
        Room[] rooms = FindObjectsOfType<Room>();

        foreach (Room room in rooms)
        {
            if (room.IsPlayerIn)
            {
                Door[] doors = room.GetComponentsInChildren<Door>(true);
                foreach (Door door in doors)
                {
                    if (door != null && door.canUse)
                    {
                        door.ForceOpen();
                    }
                }
                break;
            }
        }
    }

    void ConnectBossHpEvent()
    {
        Room[] rooms = FindObjectsOfType<Room>();
        foreach (Room room in rooms)
        {
            if (room.roomType == RoomType.Boss)
            {
                room.OnBossSpawn += OnBossSpawned;
            }
        }
    }

    void OnBossSpawned(Monster boss)
    {

        if (bossHpBarPrefab != null && bossHpBarLayout != null)
        {
            GameObject bossHpBarGO = Instantiate(bossHpBarPrefab, bossHpBarLayout);
            currentBossHpBar = bossHpBarGO.GetComponent<BossHpBar>();

            if (currentBossHpBar != null)
            {
                currentBossHpBar.InitBossInfo(boss);
                currentBossHpBar.OnRemoveUI += RemoveBossHpBar;  
            }
        }
    }

    void RemoveBossHpBar(BossHpBar bossHpBar)
    {
        if (bossHpBar == null) return;

        bossHpBar.OnRemoveUI -= RemoveBossHpBar;
        Destroy(bossHpBar.gameObject);
        currentBossHpBar = null;
    }
}