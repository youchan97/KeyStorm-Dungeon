using System;
using System.Collections;
using UnityEngine;

public class TutorialPlayerHook : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;

    private bool hasMovedUp, hasMovedDown, hasMovedLeft, hasMovedRight;
    private TutorialManager tutorialManager;

    bool canThrowBomb = false;

    private void Awake()
    {
        if (playerController == null)
            playerController = FindObjectOfType<PlayerController>();
    }

    private void Start()
    {
        tutorialManager = TutorialManager.Instance;

        StartCoroutine(FindPlayerDelayed());
    }

    IEnumerator FindPlayerDelayed()
    {
        yield return new WaitForSeconds(0.5f);

        if (playerController == null)
        {
            playerController = FindObjectOfType<PlayerController>();

            if (playerController != null)
            {
                playerController.OnMove += HandleMove;
                playerController.OnShoot += HandleShootAndBomb;
                playerController.OnBomb += HandleBomb;
                playerController.OnUseActiveItem += HandleSpecialAttack;
                Debug.Log("[TutorialPlayerHook] PlayerController 연결 완료!");
            }
            else
            {
                Debug.LogError("[TutorialPlayerHook] PlayerController를 찾을 수 없음!");
            }
        }
    }

    private void OnEnable()
    {
        if (playerController != null)
        {
            playerController.OnMove += HandleMove;
            playerController.OnShoot += HandleShootAndBomb;
            playerController.OnBomb += HandleBomb;
            playerController.OnUseActiveItem += HandleSpecialAttack;
        }
    }

    private void OnDisable()
    {
        if (playerController == null) return;
        playerController.OnMove -= HandleMove;
        playerController.OnShoot -= HandleShootAndBomb;
        playerController.OnBomb -= HandleBomb;
        playerController.OnUseActiveItem -= HandleSpecialAttack;
    }


    public void ReportRoomEnter(RoomType roomType)
    {
        if (tutorialManager == null) return;

        QuestType questType = roomType switch
        {
            RoomType.Start => QuestType.EnterRoom,
            RoomType.Treasure => QuestType.EnterTreasureRoom,
            RoomType.Normal => QuestType.EnterNormalRoom,
            RoomType.Shop => QuestType.EnterShopRoom,
            RoomType.Boss => QuestType.EnterBossRoom,
            _ => QuestType.EnterRoom
        };

        Debug.Log($"[TutorialPlayerHook] 방 진입: {roomType} → QuestType: {questType}");
        tutorialManager.ReportQuestProgress(questType);
    }

    void HandleMove()
    {
        if (tutorialManager == null) return;
        Vector2 move = playerController.MoveVec;

        if (move.y > 0.5f && !hasMovedUp)
        { hasMovedUp = true; tutorialManager.ReportQuestProgress(QuestType.MoveUp); }
        if (move.y < -0.5f && !hasMovedDown)
        { hasMovedDown = true; tutorialManager.ReportQuestProgress(QuestType.MoveDown); }
        if (move.x < -0.5f && !hasMovedLeft)
        { hasMovedLeft = true; tutorialManager.ReportQuestProgress(QuestType.MoveLeft); }
        if (move.x > 0.5f && !hasMovedRight)
        { hasMovedRight = true; tutorialManager.ReportQuestProgress(QuestType.MoveRight); }
    }

    void HandleShootAndBomb()
    {
        if(canThrowBomb)
        {
            tutorialManager?.ReportQuestProgress(QuestType.UseBomb);
            canThrowBomb = false;
        }
        else
            tutorialManager?.ReportQuestProgress(QuestType.Shoot);
    }
    void HandleBomb()
    {
        Player player = FindFirstObjectByType<Player>();

        if (player.PlayerAttack.CurBomb != null)
            canThrowBomb = true;
        else
        {
            if(player.Inventory.bombCount != 0)
            {
                canThrowBomb = true;
            }
        }         
    }
    void HandleSpecialAttack() => tutorialManager?.ReportQuestProgress(QuestType.SpecialShoot);

    public void ReportItemPickup() => tutorialManager?.ReportQuestProgress(QuestType.PickupItem);
    public void ReportEnemyKill() => tutorialManager?.ReportQuestProgress(QuestType.KillEnemy);
    public void ReportBossKill() => tutorialManager?.ReportQuestProgress(QuestType.KillBoss);
    public void ReportRoomEnter(TutorialRoomType roomType) => tutorialManager?.ReportRoomEntered(roomType);
    public void ReportItemPurchase() => tutorialManager?.ReportQuestProgress(QuestType.BuyItem);
    public void ReportMagazineEmpty() => tutorialManager?.ReportQuestProgress(QuestType.EmptyMagazine);
    public void ReportPortalEnter() => tutorialManager?.ReportQuestProgress(QuestType.EnterPortal);

    public void ResetMoveTracking()
    {
        hasMovedUp = hasMovedDown = hasMovedLeft = hasMovedRight = false;
    }
}
