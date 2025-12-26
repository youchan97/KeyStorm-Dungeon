using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

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
    public GameObject roomGameObject;
}

public class Room : MonoBehaviour
{
    public RoomType roomType;

    Player player;

    [SerializeField] BoxCollider2D roomCollider;
    [SerializeField] bool isPlayerIn;
    [SerializeField] bool canOpenDoor;
    [SerializeField] GameObject portal;
    [SerializeField] Door[] doors;

    public Transform doorUp;
    public Transform doorDown;
    public Transform doorLeft;
    public Transform doorRight;

    public Tilemap roomGroundTilemap;
    public Tilemap roomWallTilemap;

    public MonsterSpawner monsterSpawner;

    public bool IsPlayerIn { get => isPlayerIn;}
    public bool CanOpenDoor { get => canOpenDoor; }

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

        if(player == null) return;

        isPlayerIn = true;

        this.player = player;

        if (monsterSpawner != null)
        {
            monsterSpawner.SpawnMonsters();
        }
        else
        {
            Debug.LogError("monsterSpawner가 없음");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();

        if(player == null) return;

        isPlayerIn = false;
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
        for (int i = 0; i < doors.Length; i++)
            doors[i].ClearDoor();
        player.MagnetItems(roomCollider.bounds);

    }

    public void StageClear(Vector3 pos)
    {
        if (roomType != RoomType.Boss) return;

        RoomClear();

        if (portal != null)
        {
            GameObject go = Instantiate(portal);
            go.transform.position = pos;
        }

    }
}
