using System;
using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using static ConstValue;

public enum RoomType
{
    Start,
    Normal,
    Boss,
    Treasure,
    Shop
}

public class RoomNode
{
    public Vector2Int gridPos;
    public RoomType type;
}

public class Room : MonoBehaviour
{
    public static event Action OnGameCleared;
    public RoomType roomType;

    Player player;

    [SerializeField] BoxCollider2D roomCollider;
    [SerializeField] bool isPlayerIn;
    [SerializeField] bool canOpenDoor;
    [SerializeField] GameObject portal;
    [SerializeField] Transform portalTransform;
    [SerializeField] RoomItemSpawner itemSpawner;
    [SerializeField] Door[] doors;

    private float clearDelay = 1.0f;
    private Coroutine clearRoomCoroutine;

    public Transform doorUp;
    public Transform doorDown;
    public Transform doorLeft;
    public Transform doorRight;

    public Tilemap roomGroundTilemap;
    public Tilemap roomWallTilemap;

    public MonsterSpawner monsterSpawner;

    private List<Monster> activeMonsters = new List<Monster>();

    public event Action<Monster> OnBossSpawn;
    private bool hasReportedRoom = false;

    public bool IsPlayerIn { get => isPlayerIn; }
    public bool CanOpenDoor { get => canOpenDoor; }

    public Player Player => player;

    public Transform GetDoor(Vector2Int dir)
    {
        if (dir == Vector2Int.up) return doorUp;
        if (dir == Vector2Int.down) return doorDown;
        if (dir == Vector2Int.left) return doorLeft;
        if (dir == Vector2Int.right) return doorRight;
        return null;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();
        if (player == null) return;

        if (isPlayerIn) return;

        isPlayerIn = true;
        this.player = player;
        hasReportedRoom = false;

        TutorialPlayerHook hook = FindObjectOfType<TutorialPlayerHook>();
        if (hook != null)
        {
            if (roomType == RoomType.Normal || roomType == RoomType.Boss)
            {
                CloseDoors();

                if (monsterSpawner != null)
                    monsterSpawner.SpawnMonsters();
            }
            return;
        }

        if (roomType == RoomType.Boss)
            AudioManager.Instance.PlayBgm(BossBgm);

        GameManager.Instance.InitCurrentRoom(this);

        if (!canOpenDoor)
            player.SetCurrentRoom(this);

        if (monsterSpawner != null)
            monsterSpawner.SpawnMonsters();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (hasReportedRoom) return;

        Player player = collision.GetComponent<Player>();
        if (player == null) return;

        float distance = Vector2.Distance(player.transform.position, transform.position);

        if (distance <= 6f)
        {
            TutorialPlayerHook hook = FindObjectOfType<TutorialPlayerHook>();
            if (hook != null)
            {
                hook.ReportRoomEnter(roomType);
                hasReportedRoom = true;
                Debug.Log($"[Room] {roomType} 퀘스트 보고! 거리: {distance}");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();
        if (player == null) return;

        isPlayerIn = false;
        hasReportedRoom = false;
    }

    public void CloseDoors()
    {
        Door[] roomDoors = GetComponentsInChildren<Door>(true);

        foreach (Door door in roomDoors)
        {
            if (door != null)
            {
                if (door.canUse)
                {
                    door.CloseDoor();
                }
            }
        }
    }

    public Tilemap GetRoomGroundTilemap()
    {
        if (roomGroundTilemap == null)
        {
            Debug.LogError("Room: roomGroundTilemap이 할당되지 않음");
            return null;
        }
        return roomGroundTilemap;
    }

    public Tilemap GetRoomWallTilemap()
    {
        if (roomWallTilemap == null)
        {
            Debug.LogError("Room: roomWallTilemap이 할당되지 않음");
            return null;
        }
        return roomWallTilemap;
    }

    public void RoomClear()
    {
        canOpenDoor = true;
        player.ResetCurrentRoom();
        for (int i = 0; i < doors.Length; i++)
            doors[i].ClearDoor();
        if (player != null && roomCollider != null)
            player.MagnetItems(roomCollider.bounds);
    }

    public void StageClear()
    {
        if (roomType != RoomType.Boss) return;

        RoomClear();

        if (IsFinalStage())
        {
            OnGameCleared?.Invoke();
            return;
        }

        AudioManager.Instance.PlayBgm(StageDataManager.Instance.BgmSetting());

        if (portal != null)
        {
            GameObject go = Instantiate(portal, transform);
            go.transform.position = portalTransform.position;
        }

        itemSpawner.SpawnBossRoomItem();

    }

    bool IsFinalStage()
    {
        StageDataManager manager = StageDataManager.Instance;
        return manager.CurrentStageIndex == manager.CurrentStageSet.stageDatas.Count;
    }

    public void AddMonster(Monster monster)
    {
        if (!activeMonsters.Contains(monster))
        {
            activeMonsters.Add(monster);
            if (monster.MonsterData.tier == MonsterTier.Boss)
            {
                OnBossSpawn?.Invoke(monster);
            }
        }
    }

    public void RemoveMonster(Monster monster)
    {
        if (activeMonsters.Remove(monster))
        {
            CheckRoomClear();
        }
    }

    private void CheckRoomClear()
    {
        if (activeMonsters.Count == 0)
        {
            if (clearRoomCoroutine == null)
            {
                clearRoomCoroutine = StartCoroutine(ClearRoomAfterDelay(clearDelay));
            }
        }
        else
        {
            if (clearRoomCoroutine != null)
            {
                StopCoroutine(clearRoomCoroutine);
                clearRoomCoroutine = null;
            }
        }
    }

    private IEnumerator ClearRoomAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (activeMonsters.Count == 0)
        {
            if (roomType == RoomType.Boss)
                StageClear();
            else
                RoomClear();
        }

        clearRoomCoroutine = null;
    }

    public void ForcePlayerEnter(Player p)
    {
        isPlayerIn = true;
        player = p;

        TutorialPlayerHook hook = FindObjectOfType<TutorialPlayerHook>();
        hook?.ReportRoomEnter(roomType);

        Debug.Log($"[Room] {roomType} 강제 진입 처리됨!");
    }

    public void ForcePlayerEnterWithoutReport(Player p)
    {
        isPlayerIn = true;
        player = p;
        hasReportedRoom = false;  

        Debug.Log($"[Room] {roomType} 진입 처리 (보고 없음)");
    }
}
