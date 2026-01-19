using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TutorialStep
{
    [Header("단계 정보")]
    public int stepNumber;
    public string stepName;

    [Header("대화/설명")]
    [TextArea(3, 10)]
    public string dialogueText;
    public bool pauseGameDuringDialogue = true;

    [Header("퀘스트")]
    public QuestType questType;
    public int questTargetCount = 1;

    [Header("UI 표시")]
    public string questDescription;
    public Vector3 arrowTargetPosition;
    public bool showArrow = false;

    [Header("완료 조건")]
    public CompletionCondition completionCondition;
}

public enum QuestType
{
    None,
    Move,                  // 이동
    MoveDirections,        // 상하좌우 이동
    Attack,                // 공격
    UseAmmo,               // 탄창 다 쓰기
    UseSpecialAttack,      // 특수공격 사용
    PickupItem,            // 아이템 획득
    KillEnemy,             // 적 처치
    KillAllEnemies,        // 모든 적 처치
    BuyItem,               // 아이템 구매
    BuyBomb,               // 폭탄 구매
    UseBomb,               // 폭탄 사용
    KillBoss,              // 보스 처치
    EnterPortal,           // 포탈 진입
    EnterRoom              // 방 진입
}

public enum CompletionCondition
{
    Automatic,             // 자동 (대화만 보면 끝)
    QuestComplete,         // 퀘스트 완료
    Timer,                 // 시간 경과
    TriggerEnter           // 특정 트리거 진입
}

