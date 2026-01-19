using System;
using UnityEngine;

public class TutorialPlayerHook : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;

    private bool hasMovedUp, hasMovedDown, hasMovedLeft, hasMovedRight;
    private TutorialManager tutorialManager;

    private void Awake()
    {
        if (playerController == null)
            playerController = FindObjectOfType<PlayerController>();
    }

    private void Start()
    {
        tutorialManager = TutorialManager.Instance;
    }

    private void OnEnable()
    {
        if (playerController == null) return;
        playerController.OnMove += HandleMove;
        playerController.OnShoot += HandleShoot;
        playerController.OnBomb += HandleBomb;
        playerController.OnUseActiveItem += HandleSpecialAttack;
    }

    private void OnDisable()
    {
        if (playerController == null) return;
        playerController.OnMove -= HandleMove;
        playerController.OnShoot -= HandleShoot;
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

        tutorialManager.ReportQuestProgress(questType);
        Debug.Log($"[TutorialPlayerHook] 방 진입: {roomType}");
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

    void HandleShoot() => tutorialManager?.ReportQuestProgress(QuestType.Shoot);
    void HandleBomb() => tutorialManager?.ReportQuestProgress(QuestType.UseBomb);
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
