using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TutorialStageManager : MonoBehaviour
{
    [Header("Room Prefabs")]
    public List<Room> roomPrefabs = new List<Room>();  

    [Header("PlayerSpawn")]
    public PlayerSpawner playerSpawner;

    [Header("Corridor Tilemap")]
    public Tilemap corridorTilemap;
    public TileBase horizontalCorridorTile;
    public TileBase verticalCorridorTile;

    [Header("Config")]
    public int roomSpacing = 30;
    public int corridorWidth = 5;

    Dictionary<Vector2Int, Room> spawnedRooms = new Dictionary<Vector2Int, Room>();

    void Start()
    {
        SpawnRooms();
        ConnectRooms();

        StartCoroutine(InitStartRoom());
    }

    IEnumerator InitStartRoom()
    {
        yield return new WaitForSeconds(0.3f);  

        foreach (var room in spawnedRooms.Values)
        {
            if (room.roomType == RoomType.Start)
            {
                Player player = FindObjectOfType<Player>();
                if (player != null)
                {
                    room.ForcePlayerEnter(player);
                    Debug.Log("[TutorialStageManager] 시작방 플레이어 진입 처리!");
                }
                break;
            }
        }
    }

    void SpawnRooms()
    {
        spawnedRooms.Clear();

        for (int i = 0; i < roomPrefabs.Count; i++)
        {
            Vector2Int gridPos = new Vector2Int(i, 0);
            Vector3 worldPos = new Vector3(i * roomSpacing, 0, 0);

            Room room = Instantiate(roomPrefabs[i], worldPos, Quaternion.identity);
            spawnedRooms[gridPos] = room;
        }

        if (playerSpawner != null)
            playerSpawner.SpawnPlayer();
    }

    void ConnectRooms()
    {
        for (int i = 0; i < roomPrefabs.Count - 1; i++)
        {
            Vector2Int pos = new Vector2Int(i, 0);
            Vector2Int nextPos = new Vector2Int(i + 1, 0);

            if (!spawnedRooms.ContainsKey(pos) || !spawnedRooms.ContainsKey(nextPos))
                continue;

            Room room = spawnedRooms[pos];
            Room nextRoom = spawnedRooms[nextPos];

            Transform start = room.doorRight;
            Transform end = nextRoom.doorLeft;

            if (start != null && end != null)
            {
                Door startDoor = start.GetComponent<Door>();
                Door endDoor = end.GetComponent<Door>();

                startDoor?.UseDoor();
                endDoor?.UseDoor();

                DrawCorridor(start.position, end.position);
            }
        }
    }

    void OpenAllDoors()
    {
        foreach (var room in spawnedRooms.Values)
        {
            Door[] doors = room.GetComponentsInChildren<Door>(true);
            foreach (Door door in doors)
            {
                door.canUse = true;

                BoxCollider2D col = door.GetComponent<BoxCollider2D>();
                if (col != null)
                    col.enabled = false;
            }
        }
    }

    void DrawCorridor(Vector3 from, Vector3 to)
    {
        if (corridorTilemap == null) return;

        Vector3Int start = corridorTilemap.WorldToCell(from);
        Vector3Int end = corridorTilemap.WorldToCell(to);

        Vector3Int dir = end - start;
        dir.x = Mathf.Clamp(dir.x, -1, 1);
        dir.y = Mathf.Clamp(dir.y, -1, 1);

        end -= dir;

        int offsetMin = -(corridorWidth - 1) / 2;
        int offsetMax = corridorWidth / 2;

        // 가로 통로
        int minX = Mathf.Min(start.x, end.x);
        int maxX = Mathf.Max(start.x, end.x);

        for (int x = minX; x <= maxX; x++)
        {
            for (int y = offsetMin; y <= offsetMax; y++)
            {
                if (horizontalCorridorTile != null)
                    corridorTilemap.SetTile(new Vector3Int(x, start.y + y, 0), horizontalCorridorTile);
            }
        }
    }
}
