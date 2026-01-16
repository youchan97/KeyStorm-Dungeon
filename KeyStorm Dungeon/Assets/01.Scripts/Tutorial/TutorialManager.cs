using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;

    [Header("Tutorial Steps")]
    [SerializeField] private List<TutorialStep> tutorialSteps = new List<TutorialStep>();

    [Header("UI References")]
    [SerializeField] private GameObject questPanel;
    [SerializeField] private TextMeshProUGUI questText;
    [SerializeField] private GameObject arrowIndicator;

    [Header("Player Reference")]
    [SerializeField] private Player player;

    private int currentStepIndex = 0;
    private TutorialStep currentStep;
    private bool isTutorialActive = true;

    private HashSet<Vector2> movedDirections = new HashSet<Vector2>();
    private int enemiesKilled = 0;
    private int itemsPickedUp = 0;
    private int itemsBought = 0;
    private bool specialAttackUsed = false;
    private bool bombUsed = false;
    private int ammoUsed = 0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (player == null)
        {
            player = FindObjectOfType<Player>();
        }

        StartTutorial();
    }

    void StartTutorial()
    {
        Debug.Log("[TutorialManager] 튜토리얼 시작");

        if (tutorialSteps.Count > 0)
        {
            ShowStep(0);
        }
    }

    void ShowStep(int index)
    {
        if (index >= tutorialSteps.Count)
        {
            CompleteTutorial();
            return;
        }

        currentStepIndex = index;
        currentStep = tutorialSteps[index];

        Debug.Log($"[TutorialManager] 단계 {currentStep.stepNumber}: {currentStep.stepName}");

        if (!string.IsNullOrEmpty(currentStep.dialogueText))
        {
            if (DialogueSystem.Instance != null)
            {
                DialogueSystem.Instance.ShowDialogue(
                    currentStep.dialogueText,
                    "플레이어",
                    currentStep.pauseGameDuringDialogue,
                    OnDialogueComplete
                );
            }
        }
        else
        {
            OnDialogueComplete();
        }
    }

    void OnDialogueComplete()
    {
        if (!string.IsNullOrEmpty(currentStep.questDescription))
        {
            ShowQuestUI(currentStep.questDescription);
        }

        if (currentStep.showArrow && arrowIndicator != null)
        {
            arrowIndicator.SetActive(true);
            arrowIndicator.transform.position = currentStep.arrowTargetPosition;
        }

        if (currentStep.completionCondition == CompletionCondition.Automatic)
        {
            CompleteCurrentStep();
        }
    }

    void ShowQuestUI(string quest)
    {
        if (questPanel != null)
        {
            questPanel.SetActive(true);
        }

        if (questText != null)
        {
            questText.text = quest;
        }
    }

    void HideQuestUI()
    {
        if (questPanel != null)
        {
            questPanel.SetActive(false);
        }

        if (arrowIndicator != null)
        {
            arrowIndicator.SetActive(false);
        }
    }

    void Update()
    {
        if (!isTutorialActive || currentStep == null) return;

        CheckQuestProgress();
    }

    void CheckQuestProgress()
    {
        switch (currentStep.questType)
        {
            case QuestType.Move:
                CheckMovement();
                break;

            case QuestType.MoveDirections:
                CheckDirectionalMovement();
                break;

            case QuestType.Attack:
                CheckAttack();
                break;

            case QuestType.UseAmmo:
                break;

            case QuestType.UseSpecialAttack:
                if (specialAttackUsed && ammoUsed >= currentStep.questTargetCount)
                {
                    CompleteCurrentStep();
                }
                break;
        }
    }

    void CheckMovement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        if (Mathf.Abs(horizontal) > 0.1f || Mathf.Abs(vertical) > 0.1f)
        {
            CompleteCurrentStep();
        }
    }

    void CheckDirectionalMovement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        if (horizontal > 0.5f) movedDirections.Add(Vector2.right);
        if (horizontal < -0.5f) movedDirections.Add(Vector2.left);
        if (vertical > 0.5f) movedDirections.Add(Vector2.up);
        if (vertical < -0.5f) movedDirections.Add(Vector2.down);

        if (movedDirections.Count >= 4)
        {
            CompleteCurrentStep();
        }
    }

    void CheckAttack()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CompleteCurrentStep();
        }
    }

    public void OnAmmoUsed()
    {
        ammoUsed++;
        Debug.Log($"[TutorialManager] 탄약 사용: {ammoUsed}");

        if (currentStep.questType == QuestType.UseAmmo &&
            ammoUsed >= currentStep.questTargetCount)
        {
            CompleteCurrentStep();
        }
    }

    public void OnSpecialAttackUsed()
    {
        specialAttackUsed = true;
        Debug.Log("[TutorialManager] 특수공격 사용");

        if (currentStep.questType == QuestType.UseSpecialAttack)
        {
            // UseAmmo 조건도 확인
            CheckQuestProgress();
        }
    }

    public void OnItemPickedUp()
    {
        itemsPickedUp++;
        Debug.Log($"[TutorialManager] 아이템 획득: {itemsPickedUp}");

        if (currentStep.questType == QuestType.PickupItem &&
            itemsPickedUp >= currentStep.questTargetCount)
        {
            CompleteCurrentStep();
        }
    }

    public void OnEnemyKilled()
    {
        enemiesKilled++;
        Debug.Log($"[TutorialManager] 적 처치: {enemiesKilled}");

        if ((currentStep.questType == QuestType.KillEnemy ||
             currentStep.questType == QuestType.KillAllEnemies) &&
            enemiesKilled >= currentStep.questTargetCount)
        {
            CompleteCurrentStep();
        }
    }

    public void OnItemBought()
    {
        itemsBought++;
        Debug.Log($"[TutorialManager] 아이템 구매: {itemsBought}");

        if (currentStep.questType == QuestType.BuyItem &&
            itemsBought >= currentStep.questTargetCount)
        {
            CompleteCurrentStep();
        }
    }

    public void OnBombBought()
    {
        Debug.Log("[TutorialManager] 폭탄 구매");

        if (currentStep.questType == QuestType.BuyBomb)
        {
            CompleteCurrentStep();
        }
    }

    public void OnBombUsed()
    {
        bombUsed = true;
        Debug.Log("[TutorialManager] 폭탄 사용");

        if (currentStep.questType == QuestType.UseBomb)
        {
            CompleteCurrentStep();
        }
    }

    public void OnBossKilled()
    {
        Debug.Log("[TutorialManager] 보스 처치");

        if (currentStep.questType == QuestType.KillBoss)
        {
            CompleteCurrentStep();
        }
    }

    public void OnPortalEntered()
    {
        Debug.Log("[TutorialManager] 포탈 진입");

        if (currentStep.questType == QuestType.EnterPortal)
        {
            CompleteCurrentStep();
        }
    }

    public void OnRoomEntered()
    {
        Debug.Log("[TutorialManager] 방 진입");

        if (currentStep.questType == QuestType.EnterRoom)
        {
            CompleteCurrentStep();
        }
    }

    void CompleteCurrentStep()
    {
        Debug.Log($"[TutorialManager] 단계 완료: {currentStep.stepName}");

        HideQuestUI();

        // 다음 단계로
        StartCoroutine(MoveToNextStepWithDelay());
    }

    IEnumerator MoveToNextStepWithDelay()
    {
        yield return new WaitForSeconds(0.5f);
        ShowStep(currentStepIndex + 1);
    }

    void CompleteTutorial()
    {
        Debug.Log("[TutorialManager] 튜토리얼 완료!");

        isTutorialActive = false;

        if (DialogueSystem.Instance != null)
        {
            DialogueSystem.Instance.ShowDialogue(
                "튜토리얼을 완료했습니다!\n이제 본 게임을 시작합니다.",
                "시스템",
                true,
                () => SceneManager.LoadScene("GameScene")
            );
        }
    }

    // 스킵 기능
    public void SkipTutorial()
    {
        Debug.Log("[TutorialManager] 튜토리얼 스킵");
        PlayerPrefs.SetInt("TutorialCompleted", 1);
        SceneManager.LoadScene("GameScene");
    }
}
