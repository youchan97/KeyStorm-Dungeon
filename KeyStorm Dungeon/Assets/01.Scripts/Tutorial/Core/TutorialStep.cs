using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DialogueLine
{
    [TextArea(2, 4)]
    public string text;
    public float typingSpeed = 0.03f;
    public bool waitForInput = true;  
    public float autoDelay = 1.5f;    
}

[Serializable]
public class QuestObjective
{
    public string description;      
    public QuestType type;
    public int targetCount = 1;     

    [HideInInspector]
    public int currentCount = 0;

    public bool IsCompleted => currentCount >= targetCount;

    public void Reset()
    {
        currentCount = 0;
    }

    public void AddProgress(int amount = 1)
    {
        currentCount = Mathf.Min(currentCount + amount, targetCount);
    }
}

public enum QuestType
{
    None,
    // 이동 관련
    MoveUp,
    MoveDown,
    MoveLeft,
    MoveRight,
    // 공격 관련
    Shoot,
    SpecialShoot,
    EmptyMagazine,          // 탄창 비우기
    // 아이템 관련
    PickupItem,
    BuyItem,
    // 전투 관련
    KillEnemy,
    KillBoss,
    // 이동/방 관련
    EnterRoom,
    EnterTreasureRoom,
    EnterNormalRoom,
    EnterShopRoom,
    EnterBossRoom,
    // 기타
    UseBomb,
    EnterPortal
}

public enum TutorialRoomType
{
    Start,
    Normal,
    Boss,
    Treasure,
    Shop
}

[CreateAssetMenu(fileName = "TutorialStep", menuName = "Tutorial/Step")]
public class TutorialStep : ScriptableObject
{
    [Header("기본 정보")]
    public int stepIndex;
    public string stepName;

    [Header("방 진입 대기")]  
    public bool waitForRoomEnter = false;
    public TutorialRoomType targetRoomType;

    [Header("대화 (퀘스트 전)")]
    public List<DialogueLine> preDialogues = new List<DialogueLine>();

    [Header("퀘스트")]
    public bool hasQuest;
    public string questTitle;           
    public List<QuestObjective> objectives = new List<QuestObjective>();

    [Header("대화 (퀘스트 후)")]
    public List<DialogueLine> postDialogues = new List<DialogueLine>();

    [Header("설정")]
    public TutorialRoomType currentRoom;
    public bool openDoorsAfterQuest = false;
    public int wallIndexToOpen = -1;
    public bool blockInputDuringDialogue = true;
    public bool resetMoveTrackingOnStart = false;   

    public void ResetObjectives()
    {
        foreach (var obj in objectives)
        {
            obj.Reset();
        }
    }

    public bool AreAllObjectivesCompleted()
    {
        if (!hasQuest || objectives.Count == 0) return true;

        foreach (var obj in objectives)
        {
            if (!obj.IsCompleted) return false;
        }
        return true;
    }
}

